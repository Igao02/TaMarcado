using TaMarcado.Dominio.Entities;

namespace TaMarcado.Dominio.Repositories;

public interface IAvaliableTimeRepository
{
    Task AddAsync(AvaliableTime avaliableTime);
    Task<List<AvaliableTime>> GetByProfessionalIdAsync(Guid professionalId);
    Task<AvaliableTime?> GetByIdAndProfessionalIdAsync(Guid id, Guid professionalId);
    Task DeactivateAsync(AvaliableTime avaliableTime);
}
