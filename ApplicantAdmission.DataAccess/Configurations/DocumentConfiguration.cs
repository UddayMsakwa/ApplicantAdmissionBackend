using ApplicantAdmission.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApplicantAdmission.DataAccess.Configurations;

public class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.DocumentKind)
            .IsRequired();

        builder.HasOne(x => x.Applicant)
            .WithMany(x => x.Documents)
            .HasForeignKey(x => x.ApplicantId);

        builder.HasOne(x => x.File)
            .WithMany(x => x.Documents)
            .HasForeignKey(x => x.FileId);

        builder.ToTable("Documents");
    }
}
