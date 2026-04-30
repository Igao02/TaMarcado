using Microsoft.EntityFrameworkCore;
using TaMarcado.Dominio.Entities;
using TaMarcado.Dominio.Repositories;
using TaMarcado.Infraestrutura.Data;

namespace TaMarcado.Infraestrutura.Repositories;

public class PaymentRepository(ApplicationDbContext context) : IPaymentRepository
{
    public async Task AddAsync(Payment payment)
    {
        context.Payment.Add(payment);
        await context.SaveChangesAsync();
    }

    public Task<Payment?> GetBySchedulingIdAsync(Guid schedulingId) =>
        context.Payment
            .FirstOrDefaultAsync(p => p.SchedulingId == schedulingId);

    public async Task UpdateAsync(Payment payment)
    {
        context.Payment.Update(payment);
        await context.SaveChangesAsync();
    }
}
