using ApplicantAdmission.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApplicantAdmission.DataAccess.Configurations;

public class ApplicantAdmissionConfiguration : IEntityTypeConfiguration<ApplicantAdmissionEntity>
{
    public void Configure(EntityTypeBuilder<ApplicantAdmissionEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Applicant)
            .WithMany(x => x.Admissions)
            .HasForeignKey(x => x.ApplicantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.AdmissionProgram)
            .WithMany(x => x.ApplicantAdmissions)
            .HasForeignKey(x => x.AdmissionProgramId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ManagerUser)
            .WithMany()
            .HasForeignKey(x => x.ManagerUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.CreatedAt).IsRequired();
    }
}
