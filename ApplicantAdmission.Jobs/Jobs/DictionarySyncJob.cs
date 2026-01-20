using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ApplicantAdmission.DataAccess;
using ApplicantAdmission.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Quartz;

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
        _logger.LogInformation("DictionarySyncJob started");

        var username = _config["DictionaryApi:Username"];
        var password = _config["DictionaryApi:Password"];
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            _logger.LogWarning("Dictionary API credentials missing. Skipping sync.");
            return;
        }

        var client = _httpClientFactory.CreateClient("DictionaryApi");
        var authValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authValue);

        var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        
        var levels = await GetAsync<List<EducationLevelModel>>(client, "/api/dictionary/education_levels", opts, context.CancellationToken) ?? [];
        var faculties = await GetAsync<List<FacultyModel>>(client, "/api/dictionary/faculties", opts, context.CancellationToken) ?? [];
        var docTypes = await GetAsync<List<EducationDocumentTypeModel>>(client, "/api/dictionary/document_types", opts, context.CancellationToken) ?? [];

        
        var programs = await LoadAllPrograms(client, opts, context.CancellationToken);

        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicantDbContext>();

        
        foreach (var lvl in levels.Where(x => x.Id > 0 && !string.IsNullOrWhiteSpace(x.Name)))
        {
            var existing = await db.EducationLevels.FirstOrDefaultAsync(x => x.Id == lvl.Id, context.CancellationToken);
            if (existing == null)
            {
                db.EducationLevels.Add(new EducationLevel { Id = lvl.Id, Name = lvl.Name.Trim() });
            }
            else
            {
                existing.Name = lvl.Name.Trim();
            }
        }
        await db.SaveChangesAsync(context.CancellationToken);

        
        foreach (var fac in faculties.Where(x => x.Id != Guid.Empty && !string.IsNullOrWhiteSpace(x.Name)))
        {
            var existing = await db.Faculties.FirstOrDefaultAsync(x => x.Id == fac.Id, context.CancellationToken);
            if (existing == null)
            {
                db.Faculties.Add(new Faculty { Id = fac.Id, Name = fac.Name.Trim() });
            }
            else
            {
                existing.Name = fac.Name.Trim();
            }
        }
        await db.SaveChangesAsync(context.CancellationToken);

        var facultyIds = await db.Faculties.Select(f => f.Id).ToHashSetAsync(context.CancellationToken);
        var levelIds = await db.EducationLevels.Select(l => l.Id).ToHashSetAsync(context.CancellationToken);

        
        foreach (var p in programs.Where(x => x.Id != Guid.Empty && !string.IsNullOrWhiteSpace(x.Name)))
        {
            if (p.Faculty?.Id == Guid.Empty) continue;
            if (p.EducationLevel is null || p.EducationLevel.Id <= 0) continue;

            if (!facultyIds.Contains(p.Faculty.Id)) continue;
            if (!levelIds.Contains(p.EducationLevel.Id)) continue;

            var existing = await db.Programs.FirstOrDefaultAsync(x => x.Id == p.Id, context.CancellationToken);
            if (existing == null)
            {
                db.Programs.Add(new ProgramEntity
                {
                    Id = p.Id,
                    Name = p.Name.Trim(),
                    FacultyId = p.Faculty.Id,
                    LevelId = p.EducationLevel.Id
                });
            }
            else
            {
                existing.Name = p.Name.Trim();
                existing.FacultyId = p.Faculty.Id;
                existing.LevelId = p.EducationLevel.Id;
            }

            
            var apExists = await db.AdmissionPrograms.AnyAsync(x => x.ProgramId == p.Id, context.CancellationToken);
            if (!apExists)
            {
                db.AdmissionPrograms.Add(new AdmissionProgram
                {
                    Id = Guid.NewGuid(),
                    ProgramId = p.Id
                });
            }
        }
        await db.SaveChangesAsync(context.CancellationToken);

        
        foreach (var dt in docTypes.Where(x => x.Id != Guid.Empty && !string.IsNullOrWhiteSpace(x.Name) && x.EducationLevel != null))
        {
            var levelId = dt.EducationLevel.Id;
            if (levelId <= 0 || !levelIds.Contains(levelId)) continue;

            var existing = await db.EducationDocumentTypes
                .Include(x => x.NextLevels)
                .FirstOrDefaultAsync(x => x.Id == dt.Id, context.CancellationToken);

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

        await db.SaveChangesAsync(context.CancellationToken);

        _logger.LogInformation(
            "DictionarySyncJob finished. Levels={levels} Faculties={faculties} Programs={programs} DocTypes={docTypes}",
            levels.Count, faculties.Count, programs.Count, docTypes.Count);
    }

    private static async Task<T?> GetAsync<T>(HttpClient client, string path, JsonSerializerOptions opts, CancellationToken ct)
    {
        using var res = await client.GetAsync(path, ct);
        res.EnsureSuccessStatusCode();
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
            var url = $"/api/dictionary/programs?page={page}&size={size}";
            var chunk = await GetAsync<ProgramPagedListModel>(client, url, opts, ct);

            var programs = chunk?.Programs ?? [];
            if (programs.Count == 0) break;

            all.AddRange(programs);

            
            if (programs.Count < size) break;
            page++;
        }

        return all;
    }

    

    private sealed class EducationLevelModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
    }

    private sealed class FacultyModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
    }

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
        public FacultyModel? Faculty { get; set; }
        public EducationLevelModel? EducationLevel { get; set; }

        
    }

    private sealed class ProgramPagedListModel
    {
        public List<EducationProgramModel>? Programs { get; set; }
        public PageInfoModel? Pagination { get; set; }
    }

    private sealed class PageInfoModel
    {
        public int Size { get; set; }
        public int Count { get; set; }
        public int Current { get; set; }
    }
}
