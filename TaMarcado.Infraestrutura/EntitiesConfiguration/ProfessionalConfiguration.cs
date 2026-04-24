using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaMarcado.Dominio.Entities;

namespace TaMarcado.Infraestrutura.EntitiesConfiguration;

internal class ProfessionalConfiguration : IEntityTypeConfiguration<Professional>
{
    public void Configure(EntityTypeBuilder<Professional> builder)
    {
        builder
            .HasKey(p => p.Id);

        builder
            .Property(p => p.CategoryId)
            .IsRequired();

        builder
            .Property(p => p.ExibitionName)
            .HasMaxLength(100)
            .IsRequired();

        builder
            .Property(p => p.Slug)
            .HasMaxLength(100)
            .IsRequired();

        builder
            .Property(p => p.WhatsApp)
            .HasMaxLength(20)
            .IsRequired();

        builder
            .Property(p => p.Bio)
            .HasMaxLength(500);

        builder
            .Property(p => p.PhotoUrl)
            .HasMaxLength(200)
            .IsRequired();

        builder
            .Property(p => p.KeyPix)
            .HasMaxLength(100)
            .IsRequired();

        builder
            .Property(p => p.KeyPixType)
            .IsRequired();

        builder
            .Property(p => p.Active)
            .IsRequired();

        builder
            .Property(p => p.CreatedAt)
            .IsRequired();
    }
}
