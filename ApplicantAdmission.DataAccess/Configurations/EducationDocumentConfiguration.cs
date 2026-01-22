using ApplicantAdmission.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApplicantAdmission.DataAccess.Configurations;

public class EducationDocumentConfiguration : IEntityTypeConfiguration<EducationDocument>
{
    public void Configure(EntityTypeBuilder<EducationDocument> builder)
    {
        builder.Property(x => x.Name).IsRequired();
        builder.Property(x => x.IssueDate).IsRequired();

        builder.HasOne<EducationDocumentType>()
            .WithMany()
            .HasForeignKey(x => x.DocumentTypeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("EducationDocuments");
    }
}
