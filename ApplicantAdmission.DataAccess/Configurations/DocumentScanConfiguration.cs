using ApplicantAdmission.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApplicantAdmission.DataAccess.Configurations;

public class DocumentScanConfiguration : IEntityTypeConfiguration<DocumentScan>
{
    public void Configure(EntityTypeBuilder<DocumentScan> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.FileName).IsRequired();
        builder.Property(x => x.ContentType).IsRequired();

        builder.Property(x => x.CreatedAt).IsRequired();

        builder.HasIndex(x => x.FileId).IsUnique(); 
        builder.ToTable("DocumentScans");
    }
}
