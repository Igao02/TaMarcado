using Microsoft.EntityFrameworkCore;
using TaMarcado.Dominio.Entities;
using TaMarcado.Dominio.Repositories;
using TaMarcado.Infraestrutura.Data;

namespace TaMarcado.Infraestrutura.Repositories;

public class ClientRepository(ApplicationDbContext context) : IClientRepository
{
    public async Task<Client> AddAsync(Client client)
    {
        context.Client.Add(client);
        await context.SaveChangesAsync();
        return client;
    }

    public Task<Client?> GetByUserIdAndProfessionalIdAsync(string userId, Guid professionalId) =>
        context.Client
            .FirstOrDefaultAsync(c => c.ApplicationUserId == userId && c.ProfessionalId == professionalId);

    public Task<List<Client>> GetByProfessionalIdAsync(Guid professionalId) =>
        context.Client
            .Where(c => c.ProfessionalId == professionalId)
            .OrderBy(c => c.Name)
            .ToListAsync();

    public Task<Client?> GetByIdAndProfessionalIdAsync(Guid id, Guid professionalId) =>
        context.Client
            .FirstOrDefaultAsync(c => c.Id == id && c.ProfessionalId == professionalId);

    public async Task UpdateAsync(Client client)
    {
        context.Client.Update(client);
        await context.SaveChangesAsync();
    }
}
