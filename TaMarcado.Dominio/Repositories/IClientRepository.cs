using TaMarcado.Dominio.Entities;

namespace TaMarcado.Dominio.Repositories;

public interface IClientRepository
{
    Task<Client> AddAsync(Client client);
    Task<Client?> GetByUserIdAndProfessionalIdAsync(string userId, Guid professionalId);
    Task<List<Client>> GetByProfessionalIdAsync(Guid professionalId);
    Task<Client?> GetByIdAndProfessionalIdAsync(Guid id, Guid professionalId);
    Task UpdateAsync(Client client);
}
