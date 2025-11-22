using ApplicantAdmission.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApplicantAdmission.DataAccess.Configurations;

public class AdmissionProgramConfiguration : IEntityTypeConfiguration<AdmissionProgram>
{
    public void Configure(EntityTypeBuilder<AdmissionProgram> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Program)
            .WithMany(x => x.AdmissionPrograms)
            .HasForeignKey(x => x.ProgramId);
    }
}
