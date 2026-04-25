using Microsoft.EntityFrameworkCore;
using TaMarcado.Dominio.Entities;
using TaMarcado.Dominio.Repositories;
using TaMarcado.Infraestrutura.Data;

namespace TaMarcado.Infraestrutura.Repositories;

public class AvaliableTimeRepository(ApplicationDbContext context) : IAvaliableTimeRepository
{
    public async Task AddAsync(AvaliableTime avaliableTime)
    {
        context.AvaliableTime.Add(avaliableTime);
        await context.SaveChangesAsync();
    }

    public Task<List<AvaliableTime>> GetByProfessionalIdAsync(Guid professionalId) =>
        context.AvaliableTime
            .Where(a => a.ProfessionalId == professionalId && a.Active)
            .OrderBy(a => a.WeekDay)
            .ThenBy(a => a.StartTime)
            .ToListAsync();

    public Task<AvaliableTime?> GetByIdAndProfessionalIdAsync(Guid id, Guid professionalId) =>
        context.AvaliableTime
            .FirstOrDefaultAsync(a => a.Id == id && a.ProfessionalId == professionalId && a.Active);

    public async Task DeactivateAsync(AvaliableTime avaliableTime)
    {
        avaliableTime.Active = false;
        context.AvaliableTime.Update(avaliableTime);
        await context.SaveChangesAsync();
    }
}
