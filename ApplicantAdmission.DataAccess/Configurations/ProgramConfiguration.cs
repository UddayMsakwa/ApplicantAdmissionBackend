using ApplicantAdmission.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApplicantAdmission.DataAccess.Configurations;

public class ProgramConfiguration : IEntityTypeConfiguration<Program>
{
    public void Configure(EntityTypeBuilder<Program> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired();

        builder.HasOne(x => x.Faculty)
            .WithMany(x => x.Programs)
            .HasForeignKey(x => x.FacultyId);

        builder.HasOne(x => x.Level)
            .WithMany(x => x.Programs)
            .HasForeignKey(x => x.LevelId);

        builder.HasMany(x => x.AdmissionPrograms)
            .WithOne(x => x.Program)
            .HasForeignKey(x => x.ProgramId);
    }
}
