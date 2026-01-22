using ApplicantAdmission.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApplicantAdmission.DataAccess.Configurations;

public class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.DocumentKind).IsRequired();

        builder.HasOne(x => x.Applicant)
            .WithMany(x => x.Documents)
            .HasForeignKey(x => x.ApplicantId);

        builder.HasMany(x => x.Scans)
            .WithOne(x => x.Document)
            .HasForeignKey(x => x.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("Documents");
    }
}
