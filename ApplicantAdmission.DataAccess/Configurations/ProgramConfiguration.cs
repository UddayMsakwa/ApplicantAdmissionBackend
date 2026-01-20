using ApplicantAdmission.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApplicantAdmission.DataAccess.Configurations;

public class ProgramConfiguration : IEntityTypeConfiguration<ProgramEntity>
{
    public void Configure(EntityTypeBuilder<ProgramEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired();

        builder.HasOne(x => x.Faculty)
            .WithMany(f => f.Programs)
            .HasForeignKey(x => x.FacultyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Level)
            .WithMany(l => l.Programs)
            .HasForeignKey(x => x.LevelId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
