using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(o => o.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(o => o.Address)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(o => o.Phone)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(o => o.City)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(o => o.Country)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(o => o.PostCode)
            .IsRequired();

        builder.Property(o => o.TotalPrice).HasPrecision(10, 2);

        builder.Property(o => o.Status)
            .HasConversion<string>()
            .IsRequired();
        builder.Property(o => o.Paid)
            .HasConversion(
                v => v ? "Y" : "N",
                v => v == "Y"
            );
        
       
    }
}
