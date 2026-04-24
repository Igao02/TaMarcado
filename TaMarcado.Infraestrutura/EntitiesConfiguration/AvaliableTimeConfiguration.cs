using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaMarcado.Dominio.Entities;

namespace TaMarcado.Infraestrutura.EntitiesConfiguration;

internal class AvaliableTimeConfiguration : IEntityTypeConfiguration<AvaliableTime>
{
    public void Configure(EntityTypeBuilder<AvaliableTime> builder)
    {
        builder
            .HasKey(a => a.Id);

        builder
            .Property(a => a.WeekDay)
            .IsRequired();

        builder
            .Property(a => a.StartTime)
            .IsRequired();

        builder
            .Property(a => a.EndTime)
            .IsRequired();

        builder
            .Property(a => a.Active)
            .IsRequired();

        builder
            .Property(a => a.CreatedAt)
            .IsRequired();
    }
}
