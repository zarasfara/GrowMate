using GrowMate.Domain.Plants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GrowMate.Infrastructure.Plants;

public sealed class PlantTemplateEntityTypeConfiguration : IEntityTypeConfiguration<PlantTemplate>
{
    public void Configure(EntityTypeBuilder<PlantTemplate> builder)
    {
        builder.ToTable("PlantTemplates");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .ValueGeneratedOnAdd();

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(100)
            .HasComment("Название культуры (шаблон)");

        builder.Property(t => t.Type)
            .IsRequired()
            .HasConversion(
                v => v.ToString(),
                v => Enum.Parse<PlantType>(v))
            .HasComment("Тип культуры");

        builder.Property(t => t.Description)
            .HasComment("Описание культуры");

        builder.Property(t => t.WateringIntervalDays)
            .HasComment("Рекомендуемый интервал полива в днях");

        builder.Property(t => t.FirstFertilizingAfterDays)
            .HasComment("Через сколько дней после посадки рекомендована первая подкормка");

        builder.Property(t => t.FirstTreatmentAfterDays)
            .HasComment("Через сколько дней после посадки рекомендована первая обработка");

        builder.HasMany(t => t.Plants)
            .WithOne(p => p.Template)
            .HasForeignKey(p => p.PlantTemplateId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}