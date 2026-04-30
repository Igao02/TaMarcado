using TaMarcado.Dominio.Entities;

namespace TaMarcado.Dominio.Repositories;

public interface IPaymentRepository
{
    Task AddAsync(Payment payment);
    Task<Payment?> GetBySchedulingIdAsync(Guid schedulingId);
    Task UpdateAsync(Payment payment);
}
