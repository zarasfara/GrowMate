using GrowMate.Domain.Plants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GrowMate.Infrastructure.Plants;

public sealed class PlantImageEntityTypeConfiguration : IEntityTypeConfiguration<PlantImage>
{
    public void Configure(EntityTypeBuilder<PlantImage> builder)
    {
        builder.ToTable("PlantImages");

        builder.HasKey(pi => pi.Id);

        builder.Property(pi => pi.Id)
            .ValueGeneratedOnAdd();

        builder.Property(pi => pi.Path)
            .IsRequired()
            .HasMaxLength(500)
            .HasComment("Путь к изображению посадки");

        builder.Property(pi => pi.CreatedAt)
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("NOW()")
            .HasComment("Дата добавления изображения");

        builder.Property(pi => pi.PlantId)
            .IsRequired()
            .HasComment("Идентификатор растения");

        builder.HasOne(pi => pi.Plant)
            .WithMany(p => p.Images)
            .HasForeignKey(pi => pi.PlantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
