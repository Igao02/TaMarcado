using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaMarcado.Dominio.Entities;

namespace TaMarcado.Infraestrutura.EntitiesConfiguration;

internal class SchedulingConfiguration : IEntityTypeConfiguration<Scheduling>
{
    public void Configure(EntityTypeBuilder<Scheduling> builder)
    {
        builder
            .HasKey(s => s.Id);

        builder
            .Property(s => s.ProfessionalId)
            .IsRequired();

        builder
            .Property(s => s.ServiceId)
            .IsRequired();

        builder
            .Property(s => s.ClientId)
            .IsRequired();

        builder
            .Property(s => s.InitDate)
            .IsRequired();

        builder
            .Property(s => s.EndDate)
            .IsRequired();

        builder
            .Property(s => s.Status)
            .IsRequired();

        builder
            .Property(s => s.Price)
            .IsRequired();

        builder
            .Property(s => s.Observation)
            .IsRequired();

        builder
            .Property(s => s.CreatedAt)
            .IsRequired();

        builder
            .HasOne(s => s.Professional)
            .WithMany()
            .HasForeignKey(s => s.ProfessionalId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(s => s.Service)
            .WithMany()
            .HasForeignKey(s => s.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(s => s.Client)
            .WithMany()
            .HasForeignKey(s => s.ClientId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
