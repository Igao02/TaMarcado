using TaMarcado.Dominio.Entities;

namespace TaMarcado.Dominio.Repositories;

public interface IServiceRepository
{
    Task AddAsync(Service service);
    Task<List<Service>> GetByProfessionalIdAsync(Guid professionalId);
    Task<Service?> GetByIdAndProfessionalIdAsync(Guid id, Guid professionalId);
    Task UpdateAsync(Service service);
}
