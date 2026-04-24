using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaMarcado.Dominio.Entities;

namespace TaMarcado.Infraestrutura.EntitiesConfiguration;

internal class PlanConfiguration : IEntityTypeConfiguration<Plan>
{
    public void Configure(EntityTypeBuilder<Plan> builder)
    {
        builder
            .HasKey(p => p.Id);

        builder
            .Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder
            .Property(p => p.LimitMonthlySchedulings);

        builder
            .Property(p => p.Price)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder
            .Property(p => p.Active)
            .IsRequired();

        builder.HasData(
            new Plan
            {
                Id = Guid.Parse("22222222-0000-0000-0000-000000000001"),
                Name = "Grátis",
                LimitMonthlySchedulings = 20,
                Price = 0,
                Active = true
            },
            new Plan
            {
                Id = Guid.Parse("22222222-0000-0000-0000-000000000002"),
                Name = "Pro",
                LimitMonthlySchedulings = null,
                Price = 14.99m,
                Active = true
            }
        );
    }
}
