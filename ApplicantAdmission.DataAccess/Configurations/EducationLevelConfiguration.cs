using ApplicantAdmission.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApplicantAdmission.DataAccess.Configurations;

public class EducationLevelConfiguration : IEntityTypeConfiguration<EducationLevel>
{
    public void Configure(EntityTypeBuilder<EducationLevel> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired();

        builder.HasMany(x => x.DocumentTypes)
            .WithOne(x => x.Level)
            .HasForeignKey(x => x.LevelId);
    }
}
