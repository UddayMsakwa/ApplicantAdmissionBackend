using ApplicantAdmission.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApplicantAdmission.DataAccess;

public class ApplicantDbContext : DbContext
{
    public ApplicantDbContext(DbContextOptions<ApplicantDbContext> options)
        : base(options)
    {
    }

    
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<EducationDocument> EducationDocuments => Set<EducationDocument>();

    
    public DbSet<Applicant> Applicants => Set<Applicant>();
    public DbSet<Manager> Managers => Set<Manager>();
    public DbSet<EducationLevel> EducationLevels => Set<EducationLevel>();
    public DbSet<EducationDocumentType> EducationDocumentTypes => Set<EducationDocumentType>();
    public DbSet<Faculty> Faculties => Set<Faculty>();
    public DbSet<Program> Programs => Set<Program>();
    public DbSet<AdmissionProgram> AdmissionPrograms => Set<AdmissionProgram>();

    public DbSet<ApplicantAdmission.DataAccess.Entities.ApplicantAdmission> ApplicantAdmissions
        => Set<ApplicantAdmission.DataAccess.Entities.ApplicantAdmission>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Document>().UseTptMappingStrategy();

  
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicantDbContext).Assembly);
    }
}
