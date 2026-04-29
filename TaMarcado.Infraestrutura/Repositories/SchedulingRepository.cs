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

    public Task<List<Scheduling>> GetByProfessionalIdWithDetailsAsync(Guid professionalId) =>
        context.Scheduling
            .Include(s => s.Client)
            .Include(s => s.Service)
            .Where(s => s.ProfessionalId == professionalId)
            .OrderByDescending(s => s.InitDate)
            .ToListAsync();

    public Task<Scheduling?> GetByIdAndProfessionalIdAsync(Guid id, Guid professionalId) =>
        context.Scheduling
            .FirstOrDefaultAsync(s => s.Id == id && s.ProfessionalId == professionalId);

    public async Task UpdateAsync(Scheduling scheduling)
    {
        context.Scheduling.Update(scheduling);
        await context.SaveChangesAsync();
    }
}
