using Microsoft.EntityFrameworkCore;
using TaMarcado.Dominio.Entities;
using TaMarcado.Dominio.Enum;
using TaMarcado.Dominio.Repositories;
using TaMarcado.Infraestrutura.Data;

namespace TaMarcado.Infraestrutura.Repositories;

public class SchedulingRepository(ApplicationDbContext context) : ISchedulingRepository
{
    public async Task<Scheduling> AddAsync(Scheduling scheduling)
    {
        context.Scheduling.Add(scheduling);
        await context.SaveChangesAsync();
        return scheduling;
    }

    public Task<List<Scheduling>> GetByProfessionalIdAndDateAsync(Guid professionalId, DateOnly date) =>
        context.Scheduling
            .Where(s =>
                s.ProfessionalId == professionalId &&
                s.Status != StatusEnum.Canceled &&
                DateOnly.FromDateTime(s.InitDate) == date)
            .ToListAsync();
}
