using TaMarcado.Dominio.Entities;

namespace TaMarcado.Dominio.Repositories;

public interface ISchedulingRepository
{
    Task<Scheduling> AddAsync(Scheduling scheduling);
    Task<List<Scheduling>> GetByProfessionalIdAndDateAsync(Guid professionalId, DateOnly date);
    Task<List<Scheduling>> GetByProfessionalIdWithDetailsAsync(Guid professionalId);
    Task<Scheduling?> GetByIdAndProfessionalIdAsync(Guid id, Guid professionalId);
    Task UpdateAsync(Scheduling scheduling);
}
