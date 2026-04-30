using TaMarcado.Compartilhado;
using TaMarcado.Dominio.Enum;
using TaMarcado.Dominio.Repositories;

namespace TaMarcado.Aplicacao.UseCases.Payments.ConfirmPayment;

public class ConfirmPaymentHandler(
    IPaymentRepository paymentRepository,
    ISchedulingRepository schedulingRepository)
{
    public async Task<Result<ConfirmPaymentResponse>> Handle(ConfirmPaymentCommand command)
    {
        try
        {
            var scheduling = await schedulingRepository.GetByIdAndProfessionalIdAsync(command.SchedulingId, command.ProfessionalId);
            if (scheduling is null)
                return Result.Failure<ConfirmPaymentResponse>(
                    Error.NotFound("Scheduling.NotFound", "Agendamento não encontrado."));

            var payment = await paymentRepository.GetBySchedulingIdAsync(command.SchedulingId);
            if (payment is null)
                return Result.Failure<ConfirmPaymentResponse>(
                    Error.NotFound("Payment.NotFound", "Pagamento não encontrado."));

            if (payment.StatusPayment == StatusPaymentEnum.Paid)
                return Result.Failure<ConfirmPaymentResponse>(
                    Error.Conflict("Payment.AlreadyPaid", "Este pagamento já foi confirmado."));

            payment.StatusPayment = StatusPaymentEnum.Paid;
            payment.DatePayment = DateTime.Now;
            await paymentRepository.UpdateAsync(payment);

            return Result.Success(new ConfirmPaymentResponse(payment.Id));
        }
        catch (Exception ex)
        {
            return Result.Failure<ConfirmPaymentResponse>(
                Error.Problem("Payment.ConfirmError", ex.Message));
        }
    }
}
