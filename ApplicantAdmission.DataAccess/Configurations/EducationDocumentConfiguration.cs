using ApplicantAdmission.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApplicantAdmission.DataAccess.Configurations;

public class EducationDocumentConfiguration : IEntityTypeConfiguration<EducationDocument>
{
    public void Configure(EntityTypeBuilder<EducationDocument> builder)
    {
        
        builder.Property(x => x.InstitutionName).IsRequired();
        builder.Property(x => x.GraduationYear).IsRequired();

        builder.Property(x => x.AverageScore)
            .HasColumnType("numeric(5,2)") 
            .IsRequired();

        builder.HasOne<EducationDocumentType>()
            .WithMany()
            .HasForeignKey(x => x.DocumentTypeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("EducationDocuments");
    }
}
