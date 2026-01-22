using ApplicantAdmission.DataAccess.Entities;
using ApplicantAdmission.DataAccess;
using ApplicantAdmission.DataAccess.Enums;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;

namespace ApplicantAdmission.DataAccess;

public class ApplicantDbContext : DbContext
{
    public ApplicantDbContext(DbContextOptions<ApplicantDbContext> options)
        : base(options)
    {
    }

    public DbSet<Applicant> Applicants => Set<Applicant>();
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
    public DbSet<EducationDocumentTypeNextLevel> EducationDocumentTypeNextLevels => Set<EducationDocumentTypeNextLevel>();
    public DbSet<DocumentScan> DocumentScans => Set<DocumentScan>();
    


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicantDbContext).Assembly);
        modelBuilder.Entity<Document>().UseTptMappingStrategy();

        modelBuilder.Entity<UserEntity>()
            .HasIndex(x => x.Email)
            .IsUnique();

        modelBuilder.Entity<UserEntity>().HasData(
            new UserEntity
            {
                Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000001"),
                Email = "admin@test.local",
                FullName = "Seeded Admin",
                PasswordHash = HashWithSalt("AdminSeeded!23", "AAAAAAAAAAAAAAAAAAAAAA=="),
                Role = UserRole.Admin,
                IsActive = true
            },
            new UserEntity
            {
                Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000002"),
                Email = "mgr1@test.local",
                FullName = "Seeded Manager 1",
                PasswordHash = HashWithSalt("SeededPwd!23", "AQEBAQEBAQEBAQEBAQEBAQ=="),
                Role = UserRole.Manager,
                IsActive = true
            },
            new UserEntity
            {
                Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000003"),
                Email = "head1@test.local",
                FullName = "Seeded Head 1",
                PasswordHash = HashWithSalt("HeadPwd!23", "AgICAgICAgICAgICAgICAg=="),
                Role = UserRole.HeadManager,
                IsActive = true
            }
        );
    }

    private static string HashWithSalt(string password, string saltB64)
    {
        var salt = Convert.FromBase64String(saltB64);

        var hash = KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100_000,
            numBytesRequested: 32);

        return Convert.ToBase64String(salt) + "." + Convert.ToBase64String(hash);
    }
}
