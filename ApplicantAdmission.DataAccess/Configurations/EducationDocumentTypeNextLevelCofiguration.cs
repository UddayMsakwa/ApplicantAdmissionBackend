using ApplicantAdmission.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApplicantAdmission.DataAccess.Configurations;

public class EducationDocumentTypeNextLevelConfiguration : IEntityTypeConfiguration<EducationDocumentTypeNextLevel>
{
    public void Configure(EntityTypeBuilder<EducationDocumentTypeNextLevel> b)
    {
        b.HasKey(x => new { x.DocumentTypeId, x.NextLevelId });

        b.HasOne(x => x.DocumentType)
            .WithMany(dt => dt.NextLevels)
            .HasForeignKey(x => x.DocumentTypeId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.NextLevel)
            .WithMany()
            .HasForeignKey(x => x.NextLevelId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
