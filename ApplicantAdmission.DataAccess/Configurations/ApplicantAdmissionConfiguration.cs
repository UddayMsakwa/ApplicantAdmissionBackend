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
            .HasForeignKey(x => x.ApplicantId);

        builder.HasOne(x => x.AdmissionProgram)
            .WithMany(x => x.ApplicantAdmissions)
            .HasForeignKey(x => x.AdmissionProgramId);

        builder.HasOne(x => x.Manager)
            .WithMany(x => x.ApplicantAdmissions)
            .HasForeignKey(x => x.ManagerId)
            .IsRequired(false);
    }
}
