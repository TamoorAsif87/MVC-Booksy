using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.ApplicationUserId)
               .IsRequired();

        builder.Property(r => r.BookId)
              .IsRequired();

        builder.Property(r => r.Email)
               .IsRequired()
               .HasMaxLength(255);

        builder.Property(r => r.Rating)
               .IsRequired();

        builder.Property(r => r.UserImage)
               .IsRequired(false);

        builder.Property(r => r.ReviewTime)
               .IsRequired();

        builder.HasOne(r => r.User)
               .WithMany() 
               .HasForeignKey(r => r.ApplicationUserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Book)
            .WithMany(b => b.Reviews)
            .HasForeignKey(r => r.BookId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(r => new { r.ApplicationUserId, r.BookId })
       .IsUnique();

    }
}
