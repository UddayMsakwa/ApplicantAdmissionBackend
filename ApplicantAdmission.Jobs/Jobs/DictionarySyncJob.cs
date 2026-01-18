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

        
        var levelsJson = await client.GetStringAsync("/api/dictionary/education-levels");
        var facultiesJson = await client.GetStringAsync("/api/dictionary/faculties");
        var programsJson = await client.GetStringAsync("/api/dictionary/programs");

        var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        var levels = JsonSerializer.Deserialize<List<DictionaryEducationLevelDto>>(levelsJson, opts) ?? [];
        var faculties = JsonSerializer.Deserialize<List<DictionaryFacultyDto>>(facultiesJson, opts) ?? [];
        var programs = JsonSerializer.Deserialize<List<DictionaryProgramDto>>(programsJson, opts) ?? [];

        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicantDbContext>();

        
        foreach (var lvl in levels.Where(x => !string.IsNullOrWhiteSpace(x.Name)))
        {
            var existing = await db.EducationLevels.FirstOrDefaultAsync(x => x.Name == lvl.Name);
            if (existing == null)
            {
                db.EducationLevels.Add(new EducationLevel
                {
                    Id = lvl.Id ?? Guid.NewGuid(),
                    Name = lvl.Name.Trim()
                });
            }
        }

        
        foreach (var fac in faculties.Where(x => !string.IsNullOrWhiteSpace(x.Name)))
        {
            var existing = await db.Faculties.FirstOrDefaultAsync(x => x.Name == fac.Name);
            if (existing == null)
            {
                db.Faculties.Add(new Faculty
                {
                    Id = fac.Id ?? Guid.NewGuid(),
                    Name = fac.Name.Trim()
                });
            }
        }

        await db.SaveChangesAsync();

        
        var facultyById = await db.Faculties.ToDictionaryAsync(f => f.Id, f => f);
        var levelById = await db.EducationLevels.ToDictionaryAsync(l => l.Id, l => l);

        
        foreach (var p in programs.Where(x => !string.IsNullOrWhiteSpace(x.Name)))
        {
            var exists = await db.Programs.FirstOrDefaultAsync(x => x.Name == p.Name);
            if (exists != null) continue;

            
            if (p.FacultyId == null || p.LevelId == null) continue;
            if (!facultyById.ContainsKey(p.FacultyId.Value)) continue;
            if (!levelById.ContainsKey(p.LevelId.Value)) continue;

            db.Programs.Add(new ProgramEntity
            {
                Id = p.Id ?? Guid.NewGuid(),
                Name = p.Name.Trim(),
                FacultyId = p.FacultyId.Value,
                LevelId = p.LevelId.Value
            });
        }

        await db.SaveChangesAsync();

        _logger.LogInformation(
            "DictionarySyncJob finished. Levels={levels} Faculties={faculties} Programs={programs}",
            levels.Count, faculties.Count, programs.Count);
    }

    
    private sealed class DictionaryEducationLevelDto
    {
        public Guid? Id { get; set; }
        public string Name { get; set; } = "";
    }

    private sealed class DictionaryFacultyDto
    {
        public Guid? Id { get; set; }
        public string Name { get; set; } = "";
    }

    private sealed class DictionaryProgramDto
    {
        public Guid? Id { get; set; }
        public string Name { get; set; } = "";
        public Guid? FacultyId { get; set; }
        public Guid? LevelId { get; set; }
    }
}
