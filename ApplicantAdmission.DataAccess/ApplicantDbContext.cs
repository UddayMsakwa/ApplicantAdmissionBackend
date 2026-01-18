using ApplicantAdmission.DataAccess.Entities;
using ApplicantAdmission.DataAccess.Enums;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace ApplicantAdmission.DataAccess;

public class ApplicantDbContext : DbContext
{
    public ApplicantDbContext(DbContextOptions<ApplicantDbContext> options)
        : base(options)
    {
    }

    public DbSet<Applicant> Applicants => Set<Applicant>();
    public DbSet<Manager> Managers => Set<Manager>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<EducationDocument> EducationDocuments => Set<EducationDocument>();
    public DbSet<EducationLevel> EducationLevels => Set<EducationLevel>();
    public DbSet<EducationDocumentType> EducationDocumentTypes => Set<EducationDocumentType>();
    public DbSet<Faculty> Faculties => Set<Faculty>();
    public DbSet<ProgramEntity> Programs => Set<ProgramEntity>();
    public DbSet<AdmissionProgram> AdmissionPrograms => Set<AdmissionProgram>();
    public DbSet<ApplicantAdmissionEntity> ApplicantAdmissions => Set<ApplicantAdmissionEntity>();
    public DbSet<UserEntity> Users => Set<UserEntity>();
    public DbSet<NotificationEntity> Notifications => Set<NotificationEntity>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicantDbContext).Assembly);
        modelBuilder.Entity<Document>().UseTptMappingStrategy();

        
        var facultyId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var bachelorLevelId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var programId = Guid.Parse("33333333-3333-3333-3333-333333333333");
        var admissionProgramId = Guid.Parse("44444444-4444-4444-4444-444444444444");
        var managerId = Guid.Parse("55555555-5555-5555-5555-555555555555");

        var diplomaTypeId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var transcriptTypeId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

        
        modelBuilder.Entity<UserEntity>()
            .HasIndex(x => x.Email)
            .IsUnique();

        
        modelBuilder.Entity<Faculty>().HasData(new Faculty
        {
            Id = facultyId,
            Name = "Engineering"
        });

        modelBuilder.Entity<EducationLevel>().HasData(new EducationLevel
        {
            Id = bachelorLevelId,
            Name = "Bachelor"
        });

        modelBuilder.Entity<ProgramEntity>().HasData(new ProgramEntity
        {
            Id = programId,
            Name = "Computer Science",
            FacultyId = facultyId,
            LevelId = bachelorLevelId
        });

        modelBuilder.Entity<AdmissionProgram>().HasData(new AdmissionProgram
        {
            Id = admissionProgramId,
            ProgramId = programId
        });

        modelBuilder.Entity<Manager>().HasData(new Manager
        {
            Id = managerId,
            FullName = "Admin Manager",
            Email = "manager@test.com",
            PasswordHash = Hash("Manager123"),
            Role = "Admin"
        });

        modelBuilder.Entity<UserEntity>().HasData(new UserEntity
        {
            Id = Guid.Parse("99999999-9999-9999-9999-999999999999"),
            Email = "head@admin.com",
            PasswordHash = Hash("Admin123"),
            Role = UserRole.HeadManager,
            IsActive = true
        });


        modelBuilder.Entity<EducationDocumentType>().HasData(
            new EducationDocumentType
            {
                Id = diplomaTypeId,
                Name = "Diploma",
                LevelId = bachelorLevelId,
                NextLevelId = null
            },
            new EducationDocumentType
            {
                Id = transcriptTypeId,
                Name = "Transcript",
                LevelId = bachelorLevelId,
                NextLevelId = null
            }
        );
    }

    
    private static string Hash(string password)
    {
        using var sha = SHA256.Create();
        return Convert.ToBase64String(
            sha.ComputeHash(Encoding.UTF8.GetBytes(password))
        );
    }
}
