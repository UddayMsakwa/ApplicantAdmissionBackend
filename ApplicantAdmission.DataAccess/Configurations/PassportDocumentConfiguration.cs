using ApplicantAdmission.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApplicantAdmission.DataAccess.Configurations;

public class PassportDocumentConfiguration : IEntityTypeConfiguration<PassportDocument>
{
    public void Configure(EntityTypeBuilder<PassportDocument> builder)
    {
        builder.Property(x => x.Number).IsRequired();
        builder.Property(x => x.Country).IsRequired();
        builder.Property(x => x.IssueDate).IsRequired();

        builder.ToTable("PassportDocuments");
    }
}
