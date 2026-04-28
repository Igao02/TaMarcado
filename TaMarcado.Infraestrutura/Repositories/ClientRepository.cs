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
}
