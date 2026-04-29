using TaMarcado.Dominio.Entities;

namespace TaMarcado.Dominio.Repositories;

public interface IClientRepository
{
    Task<Client> AddAsync(Client client);
    Task<Client?> GetByUserIdAndProfessionalIdAsync(string userId, Guid professionalId);
}
