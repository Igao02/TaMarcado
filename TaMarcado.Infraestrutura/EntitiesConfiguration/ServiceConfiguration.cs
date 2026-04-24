using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaMarcado.Dominio.Entities;

namespace TaMarcado.Infraestrutura.EntitiesConfiguration;

internal class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder
            .HasKey(s => s.Id);

        builder
            .Property(s => s.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder
            .Property(s => s.Description)
            .HasMaxLength(200)
            .IsRequired();

        builder
            .Property(s => s.DurationInMinutes)
            .IsRequired();

        builder
            .Property(s => s.Price)
            .IsRequired();

        builder
            .Property(s => s.IsActive)
            .IsRequired();

        builder
            .Property(s => s.CreatedAt)
            .IsRequired();
    }
}
