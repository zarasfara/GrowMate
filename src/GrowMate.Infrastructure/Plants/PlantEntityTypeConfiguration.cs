using System;
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
            .HasDefaultValueSql("NOW()")
            .HasComment("Дата посадки");

        builder.Property(p => p.Type)
            .IsRequired()
            .HasConversion(
                v => v.ToString(),
                v => Enum.Parse<PlantType>(v))
            .HasComment("Тип растения");

        builder.Property(p => p.Quantity)
            .IsRequired()
            .HasDefaultValue(1)
            .HasComment("Количество растений (для групповых посадок)");

        builder.Property(p => p.IsUnique)
            .IsRequired()
            .HasDefaultValue(false)
            .HasComment("Является ли растение уникальным (true) или групповым (false)");

        // Связь с пользователем
        builder.Property(p => p.UserId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(p => p.PlantTemplateId)
            .HasComment("Идентификатор шаблона культуры");

        builder.Property(p => p.GardenBedId)
            .IsRequired()
            .HasComment("Идентификатор грядки");

        // Связь с задачами
        builder.HasMany(p => p.Tasks)
            .WithOne(t => t.Plant)
            .HasForeignKey(t => t.PlantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}