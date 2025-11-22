using ApplicantAdmission.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApplicantAdmission.DataAccess.Configurations;

public class FileEntityConfiguration : IEntityTypeConfiguration<FileEntity>
{
    public void Configure(EntityTypeBuilder<FileEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.FileName).IsRequired();
        builder.Property(x => x.FilePath).IsRequired();
        builder.Property(x => x.ContentType).IsRequired();
        builder.Property(x => x.Size).IsRequired();

        builder.HasMany(x => x.Documents)
            .WithOne(x => x.File)
            .HasForeignKey(x => x.FileId);
    }
}
