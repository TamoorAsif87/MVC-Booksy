using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configuration;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Title)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(b => b.Author)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(b => b.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(b => b.Price)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(b => b.Discount)
            .IsRequired()
            .HasColumnType("decimal(5,2)");

        builder.Property(b => b.Available)
            .IsRequired();

        builder.Property(b => b.PublishedDate)
            .IsRequired();

        builder.Property(b => b.ISBN)
            .HasMaxLength(20);

        builder.Property(b => b.AverageRating)
            .HasDefaultValue(0);

        builder.Property(b => b.TotalReviews)
            .HasDefaultValue(0);

        builder.Property(p => p.ImageUrls)
            .HasConversion(
            v => string.Join(",", v),
            v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
            ).HasColumnType("nvarchar(max)")
            .Metadata.SetValueComparer(
                new ValueComparer<List<string>>(
                    (c1, c2) => c1!.SequenceEqual(c2!), 
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()
                )
            );
    }
}
