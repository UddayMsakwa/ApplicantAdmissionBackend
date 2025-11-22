using ApplicantAdmission.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApplicantAdmission.DataAccess.Configurations;

public class EducationDocumentTypeConfiguration : IEntityTypeConfiguration<EducationDocumentType>
{
    public void Configure(EntityTypeBuilder<EducationDocumentType> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired();

        builder.HasOne(x => x.Level)
            .WithMany(x => x.DocumentTypes)
            .HasForeignKey(x => x.LevelId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.NextLevel)
            .WithMany()
            .HasForeignKey(x => x.NextLevelId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
