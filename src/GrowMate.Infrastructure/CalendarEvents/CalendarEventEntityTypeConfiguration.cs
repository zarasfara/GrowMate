using GrowMate.Domain.CalendarEvents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GrowMate.Infrastructure.CalendarEvents;

public sealed class CalendarEventEntityTypeConfiguration : IEntityTypeConfiguration<CalendarEvent>
{
    public void Configure(EntityTypeBuilder<CalendarEvent> builder)
    {
        builder.ToTable("CalendarEvents");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.UserId)
            .IsRequired()
            .HasMaxLength(450)
            .HasComment("Идентификатор пользователя-владельца");

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(120)
            .HasComment("Название события");

        builder.Property(e => e.Type)
            .IsRequired()
            .HasMaxLength(40)
            .HasComment("Тип события");

        builder.Property(e => e.ScheduledAt)
            .IsRequired()
            .HasColumnType("timestamp with time zone")
            .HasComment("Дата и время события");

        builder.Property(e => e.Notes)
            .HasComment("Описание события");

        builder.HasIndex(e => new { e.UserId, e.ScheduledAt });
    }
}
