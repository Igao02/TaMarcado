using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaMarcado.Dominio.Entities;

namespace TaMarcado.Infraestrutura.EntitiesConfiguration;

internal class NotificationSchedulingConfiguration : IEntityTypeConfiguration<NotificationScheduling>
{
    public void Configure(EntityTypeBuilder<NotificationScheduling> builder)
    {
        builder.HasKey(n => n.Id);

        builder.Property(n => n.SchedulingType)
            .IsRequired();

        builder.Property(n => n.StatusNotification)
            .IsRequired();

        builder.Property(n => n.DateScheduling)
            .IsRequired();

        builder.Property(n => n.DateSend);

        builder.Property(n => n.QuantitySend)
            .IsRequired();
         
        builder.Property(n => n.CreatedAt)
            .IsRequired();

        builder.Property(n => n.SchedulingId)
            .IsRequired();

        builder
            .HasOne(n => n.Scheduling)
            .WithMany()
            .HasForeignKey(n => n.SchedulingId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
