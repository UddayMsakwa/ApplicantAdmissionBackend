using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using ApplicantAdmission.DataAccess;
using ApplicantAdmission.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace ApplicantAdmission.Jobs.Jobs;

public sealed class DictionarySyncJob : IJob
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<DictionarySyncJob> _logger;
    private readonly IConfiguration _config;

    public DictionarySyncJob(
        IServiceScopeFactory scopeFactory,
        IHttpClientFactory httpClientFactory,
        ILogger<DictionarySyncJob> logger,
        IConfiguration config)
    {
        _scopeFactory = scopeFactory;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _config = config;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var ct = context.CancellationToken;
        _logger.LogInformation("DictionarySyncJob started");

        var baseUrl = _config["DictionaryApi:BaseUrl"];
        var username = _config["DictionaryApi:Username"];
        var password = _config["DictionaryApi:Password"];

        if (string.IsNullOrWhiteSpace(baseUrl) ||
            string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(password))
        {
            _logger.LogError("DictionaryApi config missing. BaseUrl/Username/Password must be set.");
            return;
        }

       
        var client = _httpClientFactory.CreateClient("DictionaryApi");
        client.BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/");

        var raw = $"{username}:{password}";
        var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes(raw));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", auth);

        _logger.LogInformation(
            "DictionaryApi runtime config: Base={Base}, User={User}, AuthLen={Len}, HasAuth={HasAuth}",
            client.BaseAddress,
            username,
            auth.Length,
            client.DefaultRequestHeaders.Authorization != null);

        var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        // NOTE: no leading slash
        var levels = await GetAsync<List<EducationLevelModel>>(client, "api/dictionary/education_levels", opts, ct) ?? [];
        var faculties = await GetAsync<List<FacultyModel>>(client, "api/dictionary/faculties", opts, ct) ?? [];
        var docTypes = await GetAsync<List<EducationDocumentTypeModel>>(client, "api/dictionary/document_types", opts, ct) ?? [];
        var programs = await LoadAllPrograms(client, opts, ct);

        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicantDbContext>();

        foreach (var lvl in levels.Where(x => x.Id > 0 && !string.IsNullOrWhiteSpace(x.Name)))
        {
            var existing = await db.EducationLevels.FirstOrDefaultAsync(x => x.Id == lvl.Id, ct);
            if (existing == null)
                db.EducationLevels.Add(new EducationLevel { Id = lvl.Id, Name = lvl.Name.Trim() });
            else
                existing.Name = lvl.Name.Trim();
        }
        await db.SaveChangesAsync(ct);

        foreach (var fac in faculties.Where(x => x.Id != Guid.Empty && !string.IsNullOrWhiteSpace(x.Name)))
        {
            var existing = await db.Faculties.FirstOrDefaultAsync(x => x.Id == fac.Id, ct);
            if (existing == null)
                db.Faculties.Add(new Faculty { Id = fac.Id, Name = fac.Name.Trim() });
            else
                existing.Name = fac.Name.Trim();
        }
        await db.SaveChangesAsync(ct);

        var facultyIds = await db.Faculties.Select(f => f.Id).ToHashSetAsync(ct);
        var levelIds = await db.EducationLevels.Select(l => l.Id).ToHashSetAsync(ct);

        foreach (var p in programs.Where(x => x.Id != Guid.Empty && !string.IsNullOrWhiteSpace(x.Name)))
        {
            if (p.Faculty?.Id == null || p.Faculty.Id == Guid.Empty) continue;
            if (p.EducationLevel is null || p.EducationLevel.Id <= 0) continue;
            if (!facultyIds.Contains(p.Faculty.Id)) continue;
            if (!levelIds.Contains(p.EducationLevel.Id)) continue;

            var existing = await db.Programs.FirstOrDefaultAsync(x => x.Id == p.Id, ct);

            var name = p.Name.Trim();
            var code = (p.Code ?? "").Trim();
            var language = (p.Language ?? "").Trim();
            var studyForm = (p.StudyForm ?? "").Trim();

            if (existing == null)
            {
                db.Programs.Add(new ProgramEntity
                {
                    Id = p.Id,
                    Name = name,
                    Code = code,
                    Language = language,
                    StudyForm = studyForm,
                    FacultyId = p.Faculty.Id,
                    LevelId = p.EducationLevel.Id
                });
            }
            else
            {
                existing.Name = name;
                existing.Code = code;
                existing.Language = language;
                existing.StudyForm = studyForm;
                existing.FacultyId = p.Faculty.Id;
                existing.LevelId = p.EducationLevel.Id;
            }
        }
        await db.SaveChangesAsync(ct);

        foreach (var dt in docTypes.Where(x => x.Id != Guid.Empty && !string.IsNullOrWhiteSpace(x.Name) && x.EducationLevel != null))
        {
            var levelId = dt.EducationLevel!.Id;
            if (levelId <= 0 || !levelIds.Contains(levelId)) continue;

            var existing = await db.EducationDocumentTypes
                .Include(x => x.NextLevels)
                .FirstOrDefaultAsync(x => x.Id == dt.Id, ct);

            if (existing == null)
            {
                existing = new EducationDocumentType
                {
                    Id = dt.Id,
                    Name = dt.Name.Trim(),
                    LevelId = levelId
                };
                db.EducationDocumentTypes.Add(existing);
            }
            else
            {
                existing.Name = dt.Name.Trim();
                existing.LevelId = levelId;
            }

            existing.NextLevels.Clear();

            foreach (var next in dt.NextEducationLevels ?? [])
            {
                if (next.Id <= 0) continue;
                if (!levelIds.Contains(next.Id)) continue;

                existing.NextLevels.Add(new EducationDocumentTypeNextLevel
                {
                    DocumentTypeId = existing.Id,
                    NextLevelId = next.Id
                });
            }
        }

        await db.SaveChangesAsync(ct);

        _logger.LogInformation(
            "DictionarySyncJob finished. Levels={levels} Faculties={faculties} Programs={programs} DocTypes={docTypes}",
            levels.Count, faculties.Count, programs.Count, docTypes.Count);
    }

    private static async Task<T?> GetAsync<T>(HttpClient client, string path, JsonSerializerOptions opts, CancellationToken ct)
    {
        path = path.TrimStart('/');

        using var res = await client.GetAsync(path, ct);

        if (!res.IsSuccessStatusCode)
        {
            var body = await res.Content.ReadAsStringAsync(ct);
            throw new HttpRequestException($"Dictionary API failed: {(int)res.StatusCode} {res.ReasonPhrase}. Body: {body}");
        }

        var json = await res.Content.ReadAsStringAsync(ct);
        return JsonSerializer.Deserialize<T>(json, opts);
    }

    private static async Task<List<EducationProgramModel>> LoadAllPrograms(HttpClient client, JsonSerializerOptions opts, CancellationToken ct)
    {
        var all = new List<EducationProgramModel>();
        var page = 1;
        const int size = 100;

        while (true)
        {
            var url = $"api/dictionary/programs?page={page}&size={size}";
            var chunk = await GetAsync<ProgramPagedListModel>(client, url, opts, ct);

            var programs = chunk?.Programs ?? [];
            if (programs.Count == 0) break;

            all.AddRange(programs);

            if (programs.Count < size) break;
            page++;
        }

        return all;
    }

    private sealed class EducationLevelModel { public int Id { get; set; } public string Name { get; set; } = ""; }
    private sealed class FacultyModel { public Guid Id { get; set; } public string Name { get; set; } = ""; }
    private sealed class EducationDocumentTypeModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public EducationLevelModel? EducationLevel { get; set; }
        public List<EducationLevelModel>? NextEducationLevels { get; set; }
    }
    private sealed class EducationProgramModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public string? Code { get; set; }
        public string? Language { get; set; }
        [JsonPropertyName("educationForm")]
        public string? StudyForm { get; set; }
        public FacultyModel? Faculty { get; set; }
        public EducationLevelModel? EducationLevel { get; set; }
    }
    private sealed class ProgramPagedListModel { public List<EducationProgramModel>? Programs { get; set; } public PageInfoModel? Pagination { get; set; } }
    private sealed class PageInfoModel { public int Size { get; set; } public int Count { get; set; } public int Current { get; set; } }
}
