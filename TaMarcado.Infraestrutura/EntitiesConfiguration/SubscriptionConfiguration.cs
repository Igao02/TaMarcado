using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaMarcado.Dominio.Entities;

namespace TaMarcado.Infraestrutura.EntitiesConfiguration;

internal class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder
            .HasKey(s => s.Id);

        builder
            .Property(s => s.ProfessionalId)
            .IsRequired();

        builder
            .Property(s => s.PlanId)
            .IsRequired();

        builder
            .Property(s => s.Status)
            .IsRequired();

        builder
            .Property(s => s.StartDate)
            .IsRequired();

        builder
            .Property(s => s.EndDate);

        builder
            .Property(s => s.CreatedAt)
            .IsRequired();

        builder
            .HasOne(s => s.Professional)
            .WithMany()
            .HasForeignKey(s => s.ProfessionalId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(s => s.Plan)
            .WithMany()
            .HasForeignKey(s => s.PlanId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
