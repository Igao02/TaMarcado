using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaMarcado.Dominio.Entities;

namespace TaMarcado.Infraestrutura.EntitiesConfiguration;

internal class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.HasKey(c => c.Id);

        builder
            .Property(c => c.Name)
            .IsRequired();

        builder
            .Property(c => c.Phone)
            .IsRequired();

        builder
            .Property(c => c.Email);

        builder
            .Property(c => c.Observations)
            .HasMaxLength(500);

        builder
            .Property(c => c.CreatedAt)
            .IsRequired();
    }
}
