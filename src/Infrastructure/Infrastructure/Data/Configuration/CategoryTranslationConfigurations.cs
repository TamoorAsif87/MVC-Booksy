using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configuration;

class CategoryTranslationConfigurations : IEntityTypeConfiguration<CategoryTranslation>
{
    public void Configure(EntityTypeBuilder<CategoryTranslation> builder)
    {
        // Composite primary key
        builder.HasKey(ct => new { ct.CategoryId, ct.Culture });

        // Property configurations
        builder.Property(ct => ct.Culture)
               .IsRequired()
               .HasMaxLength(10)
               .IsUnicode(false);

        builder.Property(ct => ct.Name)
               .IsRequired()
               .HasMaxLength(256);

        builder.HasOne(ct => ct.Category)
            .WithMany(c => c.CategoryTranslations)
            .HasForeignKey(c => c.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
