using ApplicantAdmission.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApplicantAdmission.DataAccess;

public class ApplicantDbContext : DbContext
{
    public ApplicantDbContext(DbContextOptions<ApplicantDbContext> options)
        : base(options)
    {
    }

    
    public DbSet<Applicant> Applicants { get; set; } = null!;
    public DbSet<Manager> Managers { get; set; } = null!;

  
    public DbSet<EducationLevel> EducationLevels { get; set; } = null!;
    public DbSet<EducationDocumentType> EducationDocumentTypes { get; set; } = null!;
    public DbSet<Faculty> Faculties { get; set; } = null!;
    public DbSet<Program> Programs { get; set; } = null!;
    public DbSet<AdmissionProgram> AdmissionPrograms { get; set; } = null!;
    public DbSet<ApplicantAdmission> ApplicantAdmissions { get; set; } = null!;

    
    public DbSet<FileEntity> Files { get; set; } = null!;
    public DbSet<Document> Documents { get; set; } = null!;
    public DbSet<PassportDocument> PassportDocuments { get; set; } = null!;
    public DbSet<EducationDocument> EducationDocuments { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
  
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicantDbContext).Assembly);

        
        modelBuilder.Entity<Document>()
            .HasDiscriminator<string>("DocumentKind")
            .HasValue<PassportDocument>("Passport")
            .HasValue<EducationDocument>("Education");

        base.OnModelCreating(modelBuilder);
    }
}
