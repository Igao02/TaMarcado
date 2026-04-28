using TaMarcado.Dominio.Entities;

namespace TaMarcado.Dominio.Repositories;

public interface ISchedulingRepository
{
    Task<Scheduling> AddAsync(Scheduling scheduling);
    Task<List<Scheduling>> GetByProfessionalIdAndDateAsync(Guid professionalId, DateOnly date);
}
