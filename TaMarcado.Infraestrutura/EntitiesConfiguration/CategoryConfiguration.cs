using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaMarcado.Dominio.Entities;

namespace TaMarcado.Infraestrutura.EntitiesConfiguration;

internal class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder
            .HasKey(c => c.Id);

        builder
            .Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasData(
            new Category { Id = Guid.Parse("11111111-0000-0000-0000-000000000001"), Name = "Barbeiro" },
            new Category { Id = Guid.Parse("11111111-0000-0000-0000-000000000002"), Name = "Psicólogo" },
            new Category { Id = Guid.Parse("11111111-0000-0000-0000-000000000003"), Name = "Personal Trainer" },
            new Category { Id = Guid.Parse("11111111-0000-0000-0000-000000000004"), Name = "Dentista" },
            new Category { Id = Guid.Parse("11111111-0000-0000-0000-000000000005"), Name = "Tatuador" },
            new Category { Id = Guid.Parse("11111111-0000-0000-0000-000000000006"), Name = "Manicure / Pedicure" },
            new Category { Id = Guid.Parse("11111111-0000-0000-0000-000000000007"), Name = "Nutricionista" },
            new Category { Id = Guid.Parse("11111111-0000-0000-0000-000000000008"), Name = "Fisioterapeuta" },
            new Category { Id = Guid.Parse("11111111-0000-0000-0000-000000000009"), Name = "Esteticista" },
            new Category { Id = Guid.Parse("11111111-0000-0000-0000-000000000010"), Name = "Outros" }
        );
    }
}
