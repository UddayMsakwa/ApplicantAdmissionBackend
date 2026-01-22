
using ApplicantAdmission.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApplicantAdmission.DataAccess.Configurations;

public class ApplicantAdmissionConfiguration : IEntityTypeConfiguration<ApplicantAdmissionEntity>
{
    public void Configure(EntityTypeBuilder<ApplicantAdmissionEntity> builder)
    {
        builder.ToTable("ApplicantAdmissions");

        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Applicant)
     .WithMany(a => a.Admissions)
     .HasForeignKey(x => x.ApplicantId)
     .OnDelete(DeleteBehavior.Cascade);


        builder.HasOne(x => x.ManagerUser)
            .WithMany()
            .HasForeignKey(x => x.ManagerUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(x => x.AdmissionPrograms)
            .WithOne(x => x.ApplicantAdmission)
            .HasForeignKey(x => x.ApplicantAdmissionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.Status).IsRequired();

        builder.Property(x => x.LastModifiedAt).IsRequired();
        builder.HasIndex(x => x.LastModifiedAt);
    }
}
