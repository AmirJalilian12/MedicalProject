using Domain.Entities.IdentityEntities;
using Domain.Entities.IdentityEntities.Medical;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserName)
                .IsRequired(false);

            builder.Property(x => x.Email)
                .IsRequired(false);

            builder.ToTable("Users");

            builder.HasOne(x => x.UserMedical)
                .WithOne(x => x.User)
                .HasForeignKey<UserMedical>(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}