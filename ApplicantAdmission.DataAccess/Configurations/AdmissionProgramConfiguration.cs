using ApplicantAdmission.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApplicantAdmission.DataAccess.Configurations;

public class AdmissionProgramConfiguration : IEntityTypeConfiguration<AdmissionProgram>
{
    public void Configure(EntityTypeBuilder<AdmissionProgram> builder)
    {
        builder.ToTable("AdmissionPrograms");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Priority).IsRequired();

        builder.HasOne(x => x.Program)
            .WithMany(p => p.AdmissionPrograms)
            .HasForeignKey(x => x.ProgramId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.ApplicantAdmissionId, x.ProgramId })
            .IsUnique();

        builder.HasIndex(x => new { x.ApplicantAdmissionId, x.Priority })
            .IsUnique();
    }
}
