using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaMarcado.Dominio.Entities;

namespace TaMarcado.Infraestrutura.EntitiesConfiguration;

internal class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
       builder.HasKey(p => p.Id);

        builder
             .Property(p => p.SchedulingId)
             .IsRequired();

        builder
            .Property(p => p.StatusPayment)
            .IsRequired();

        builder
            .Property(p => p.ProofUrl)
            .HasMaxLength(256);

        builder
            .Property(p => p.DatePayment);

        builder
            .Property(p => p.CreatedAt)
            .IsRequired();

        builder
            .HasOne(p => p.Scheduling)
            .WithOne(s => s.Payment)
            .HasForeignKey<Payment>(p => p.SchedulingId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
