using ApplicantAdmission.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApplicantAdmission.DataAccess.Configurations;

public class ApplicantConfiguration : IEntityTypeConfiguration<Applicant>
{
    public void Configure(EntityTypeBuilder<Applicant> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Email).IsRequired();
        builder.Property(x => x.PasswordHash).IsRequired();
        builder.Property(x => x.FullName).IsRequired();
        builder.Property(x => x.Phone).IsRequired();
        builder.Property(x => x.Citizenship).IsRequired();
        builder.Property(x => x.Gender).IsRequired();

        builder.Property(x => x.DateOfBirth)
    .HasColumnType("date")      
    .IsRequired();


        builder.HasMany(x => x.Documents)
            .WithOne(x => x.Applicant)
            .HasForeignKey(x => x.ApplicantId);

        builder.HasMany(x => x.Admissions)
            .WithOne(x => x.Applicant)
            .HasForeignKey(x => x.ApplicantId);
    }
}
