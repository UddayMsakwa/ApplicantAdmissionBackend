using ApplicantAdmission.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApplicantAdmission.DataAccess.Configurations;

public class ApplicantAdmissionConfiguration : IEntityTypeConfiguration<ApplicantAdmission>
{
    public void Configure(EntityTypeBuilder<ApplicantAdmission> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasOne(x => x.Applicant)
            .WithMany(x => x.Admissions)
            .HasForeignKey(x => x.ApplicantId);

        builder.HasOne(x => x.AdmissionProgram)
            .WithMany(x => x.ApplicantAdmissions)
            .HasForeignKey(x => x.AdmissionProgramId);

        builder.HasOne(x => x.Manager)
            .WithMany(x => x.Admissions)
            .HasForeignKey(x => x.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
