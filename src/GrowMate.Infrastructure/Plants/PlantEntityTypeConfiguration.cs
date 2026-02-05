using GrowMate.Domain.Plants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GrowMate.Infrastructure.Plants;

public sealed class PlantEntityTypeConfiguration : IEntityTypeConfiguration<Plant>
{
    public void Configure(EntityTypeBuilder<Plant> builder)
    {
        builder.ToTable("Plants");

        builder.HasKey(p => p.Id);
            
        builder.Property(p => p.Id)
            .ValueGeneratedOnAdd();
            
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100)
            .HasComment("Название растения");
            
        builder.Property(p => p.Variety)
            .HasMaxLength(50)
            .HasComment("Сорт");
            
        builder.Property(p => p.Description)
            .HasComment("Описание / Заметки");
            
        builder.Property(p => p.PlantingDate)
            .IsRequired()
            .HasColumnType("date")
            .HasDefaultValueSql("GETDATE()")
            .HasComment("Дата посадки");
            
        builder.Property(p => p.Type)
            .IsRequired()
            .HasConversion(
                v => v.ToString(),
                v => (PlantType)Enum.Parse(typeof(PlantType), v))
            .HasComment("Тип растения");
            
        // Связь с пользователем
        builder.Property(p => p.UserId)
            .IsRequired()
            .HasMaxLength(450);
            
        // Связь с задачами
        builder.HasMany(p => p.Tasks)
            .WithOne(t => t.Plant)
            .HasForeignKey(t => t.PlantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}