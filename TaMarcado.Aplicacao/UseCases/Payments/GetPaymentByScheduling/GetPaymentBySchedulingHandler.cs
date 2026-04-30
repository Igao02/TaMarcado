using TaMarcado.Aplicacao.Services;
using TaMarcado.Compartilhado;
using TaMarcado.Dominio.Repositories;

namespace TaMarcado.Aplicacao.UseCases.Payments.GetPaymentByScheduling;

public class GetPaymentBySchedulingHandler(
    IPaymentRepository paymentRepository,
    ISchedulingRepository schedulingRepository,
    IProfessionalRepository professionalRepository)
{
    public async Task<Result<GetPaymentBySchedulingResponse>> Handle(GetPaymentBySchedulingCommand command)
    {
        try
        {
            var scheduling = await schedulingRepository.GetByIdAndProfessionalIdAsync(command.SchedulingId, command.ProfessionalId);
            if (scheduling is null)
                return Result.Failure<GetPaymentBySchedulingResponse>(
                    Error.NotFound("Scheduling.NotFound", "Agendamento não encontrado."));

            var payment = await paymentRepository.GetBySchedulingIdAsync(command.SchedulingId);
            if (payment is null)
                return Result.Failure<GetPaymentBySchedulingResponse>(
                    Error.NotFound("Payment.NotFound", "Pagamento não encontrado."));

            var professional = await professionalRepository.GetByIdAsync(command.ProfessionalId);
            if (professional is null)
                return Result.Failure<GetPaymentBySchedulingResponse>(
                    Error.NotFound("Professional.NotFound", "Profissional não encontrado."));

            var txId = command.SchedulingId.ToString();
            var pixPayload = PixPayloadBuilder.Build(
                professional.KeyPix,
                professional.ExibitionName,
                scheduling.Price,
                txId);

            return Result.Success(new GetPaymentBySchedulingResponse(
                payment.Id,
                payment.StatusPayment.ToString(),
                pixPayload,
                payment.DatePayment));
        }
        catch (Exception ex)
        {
            return Result.Failure<GetPaymentBySchedulingResponse>(
                Error.Problem("Payment.GetError", ex.Message));
        }
    }
}
