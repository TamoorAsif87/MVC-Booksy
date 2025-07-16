using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Core.Entities;

namespace Infrastructure.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.BookName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(i => i.Price)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(i => i.Quantity)
            .IsRequired();

        builder.Property(i => i.BookCover)
            .HasMaxLength(300);
    }
}
