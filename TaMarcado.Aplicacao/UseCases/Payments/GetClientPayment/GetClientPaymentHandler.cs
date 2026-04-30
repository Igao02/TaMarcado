using TaMarcado.Aplicacao.Services;
using TaMarcado.Compartilhado;
using TaMarcado.Dominio.Repositories;

namespace TaMarcado.Aplicacao.UseCases.Payments.GetClientPayment;

public class GetClientPaymentHandler(
    ISchedulingRepository schedulingRepository,
    IPaymentRepository paymentRepository,
    IProfessionalRepository professionalRepository)
{
    public async Task<Result<GetClientPaymentResponse>> Handle(GetClientPaymentCommand command)
    {
        try
        {
            var scheduling = await schedulingRepository.GetByIdAndClientUserIdAsync(
                command.SchedulingId, command.ApplicationUserId);

            if (scheduling is null)
                return Result.Failure<GetClientPaymentResponse>(
                    Error.NotFound("Scheduling.NotFound", "Agendamento não encontrado."));

            var payment = await paymentRepository.GetBySchedulingIdAsync(command.SchedulingId);
            if (payment is null)
                return Result.Failure<GetClientPaymentResponse>(
                    Error.NotFound("Payment.NotFound", "Pagamento não encontrado."));

            var professional = await professionalRepository.GetByIdAsync(scheduling.ProfessionalId);
            if (professional is null)
                return Result.Failure<GetClientPaymentResponse>(
                    Error.NotFound("Professional.NotFound", "Profissional não encontrado."));

            var pixPayload = PixPayloadBuilder.Build(
                professional.KeyPix ?? string.Empty,
                professional.ExibitionName,
                scheduling.Price,
                scheduling.Id.ToString("N"));

            return Result.Success(new GetClientPaymentResponse(
                payment.Id,
                payment.StatusPayment.ToString(),
                pixPayload,
                payment.DatePayment));
        }
        catch (Exception ex)
        {
            return Result.Failure<GetClientPaymentResponse>(
                Error.Problem("Payment.GetClientError", ex.Message));
        }
    }
}
