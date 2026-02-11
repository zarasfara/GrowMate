using System;
using GrowMate.Domain.GardenTasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GrowMate.Infrastructure.GardenTasks;

public sealed class GardenTaskEntityTypeConfiguration : IEntityTypeConfiguration<GardenTask>
{
    public void Configure(EntityTypeBuilder<GardenTask> builder)
    {
        builder.ToTable("GardenTasks");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .ValueGeneratedOnAdd();

        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(200)
            .HasComment("Задача");

        builder.Property(t => t.Type)
            .IsRequired()
            .HasConversion(
                v => v.ToString(),
                v => Enum.Parse<TaskType>(v))
            .HasComment("Тип задачи");

        builder.Property(t => t.DueDate)
            .IsRequired()
            .HasColumnType("date")
            .HasDefaultValueSql("GETDATE()")
            .HasComment("Срок выполнения");

        builder.Property(t => t.IsCompleted)
            .IsRequired()
            .HasDefaultValue(false)
            .HasComment("Выполнено");

        builder.Property(t => t.PlantId)
            .IsRequired()
            .HasComment("Идентификатор растения");

        // Индекс для быстрого поиска задач по растению
        builder.HasIndex(t => t.PlantId);

        // Индекс для поиска просроченных задач
        builder.HasIndex(t => new { t.IsCompleted, t.DueDate });
    }
}