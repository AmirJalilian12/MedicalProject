using Domain.Entities.OrderEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.Property(x => x.SubTotal)
                   .HasPrecision(18, 2);

            builder.OwnsOne(x => x.ShipToAddress, a =>
            {
                a.WithOwner();
            });

            builder.HasMany(x => x.OrderItems)
                   .WithOne()
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.DeliveryMethod)
                   .WithMany()
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.SetNull);

            // رابطه با کاربر ثبت‌کننده (CreatedBy)
            builder.HasOne(x => x.User)
                   .WithMany()
                   .HasForeignKey(x => x.CreatedBy)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
