using ApplicantAdmission.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApplicantAdmission.DataAccess.Configurations;

public class ManagerConfiguration : IEntityTypeConfiguration<Manager>
{
    public void Configure(EntityTypeBuilder<Manager> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Email).IsRequired();
        builder.Property(x => x.PasswordHash).IsRequired();
        builder.Property(x => x.FullName).IsRequired();
        builder.Property(x => x.Role).IsRequired();

        builder.HasMany(x => x.ApplicantAdmissions)
            .WithOne(x => x.Manager)
            .HasForeignKey(x => x.ManagerId)
            .IsRequired(false);
    }
}
