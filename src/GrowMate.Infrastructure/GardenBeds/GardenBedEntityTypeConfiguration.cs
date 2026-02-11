using System;
using GrowMate.Domain.GardenBeds;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GrowMate.Infrastructure.GardenBeds;

public sealed class GardenBedEntityTypeConfiguration : IEntityTypeConfiguration<GardenBed>
{
    public void Configure(EntityTypeBuilder<GardenBed> builder)
    {
        builder.ToTable("GardenBeds");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id)
            .ValueGeneratedOnAdd();

        builder.Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(100)
            .HasComment("Название грядки");

        builder.Property(b => b.Type)
            .IsRequired()
            .HasConversion(
                v => v.ToString(),
                v => Enum.Parse<GardenBedType>(v))
            .HasComment("Тип грядки");

        builder.Property(b => b.UserId)
            .IsRequired()
            .HasMaxLength(450)
            .HasComment("Идентификатор пользователя-владельца");

        // Связь с растениями (посадками)
        builder.HasMany(b => b.Plants)
            .WithOne(p => p.GardenBed)
            .HasForeignKey(p => p.GardenBedId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}