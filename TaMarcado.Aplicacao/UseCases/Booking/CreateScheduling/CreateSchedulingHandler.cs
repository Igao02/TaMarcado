using TaMarcado.Compartilhado;
using TaMarcado.Dominio.Entities;
using TaMarcado.Dominio.Enum;
using TaMarcado.Dominio.Repositories;

namespace TaMarcado.Aplicacao.UseCases.Booking.CreateScheduling;

public class CreateSchedulingHandler(
    IServiceRepository serviceRepository,
    ISchedulingRepository schedulingRepository,
    IClientRepository clientRepository)
{
    public async Task<Result<CreateSchedulingResponse>> Handle(CreateSchedulingCommand command)
    {
        try
        {
            var service = await serviceRepository.GetByIdAndProfessionalIdAsync(command.ServiceId, command.ProfessionalId);
            if (service is null)
                return Result.Failure<CreateSchedulingResponse>(
                    Error.NotFound("Service.NotFound", "Serviço não encontrado."));

            var initDate = command.Date.ToDateTime(TimeOnly.FromTimeSpan(command.StartTime));
            var endDate = initDate.AddMinutes(service.DurationInMinutes);

            var existingSchedulings = await schedulingRepository.GetByProfessionalIdAndDateAsync(
                command.ProfessionalId, command.Date);

            var isSlotTaken = existingSchedulings.Any(s =>
                initDate < s.EndDate && endDate > s.InitDate);

            if (isSlotTaken)
                return Result.Failure<CreateSchedulingResponse>(
                    Error.Conflict("Scheduling.SlotUnavailable", "Este horário não está mais disponível."));

            var client = await clientRepository.GetByUserIdAndProfessionalIdAsync(
                command.ClientUserId, command.ProfessionalId);

            if (client is null)
            {
                client = new Client(
                    command.ClientUserId,
                    command.ProfessionalId,
                    command.ClientName,
                    command.ClientPhone,
                    command.ClientEmail,
                    observations: null,
                    DateTime.Now);
                await clientRepository.AddAsync(client);
            }

            var scheduling = new Scheduling(
                command.ProfessionalId,
                command.ServiceId,
                client.Id,
                initDate,
                endDate,
                StatusEnum.Pendent,
                service.Price,
                observation: string.Empty,
                DateTime.Now);

            await schedulingRepository.AddAsync(scheduling);

            return Result.Success(new CreateSchedulingResponse(scheduling.Id));
        }
        catch (Exception ex)
        {
            return Result.Failure<CreateSchedulingResponse>(
                Error.Problem("Scheduling.CreateError", ex.Message));
        }
    }
}
