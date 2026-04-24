using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaMarcado.Dominio.Entities;

namespace TaMarcado.Infraestrutura.EntitiesConfiguration;

internal class BlockedConfiguration : IEntityTypeConfiguration<Blocked>
{
    public void Configure(EntityTypeBuilder<Blocked> builder)
    {
        builder
            .HasKey(b => b.Id);

        builder
            .Property(b => b.ProfessionalId)
            .IsRequired();

        builder
            .Property(b => b.InitDate)
            .IsRequired();

        builder
            .Property(b => b.EndDate)
            .IsRequired();

        builder
            .Property(b => b.Reason)
            .HasMaxLength(300);

        builder
            .Property(b => b.CreatedAt)
            .IsRequired(); 
    }
}
