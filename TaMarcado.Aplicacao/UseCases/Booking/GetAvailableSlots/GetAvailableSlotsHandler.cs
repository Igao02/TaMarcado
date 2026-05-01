using TaMarcado.Compartilhado;
using TaMarcado.Dominio.Enum;
using TaMarcado.Dominio.Repositories;

namespace TaMarcado.Aplicacao.UseCases.Booking.GetAvailableSlots;

public class GetAvailableSlotsHandler(
    IServiceRepository serviceRepository,
    IAvaliableTimeRepository avaliableTimeRepository,
    ISchedulingRepository schedulingRepository)
{
    public async Task<Result<GetAvailableSlotsResponse>> Handle(GetAvailableSlotsCommand command)
    {
        try
        {
            var service = await serviceRepository.GetByIdAndProfessionalIdAsync(command.ServiceId, command.ProfessionalId);
            if (service is null)
                return Result.Failure<GetAvailableSlotsResponse>(
                    Error.NotFound("Service.NotFound", "Serviço não encontrado."));

            var weekDay = MapDayOfWeekToWeekEnum(command.Date.DayOfWeek);
            var allAvaliableTimes = await avaliableTimeRepository.GetByProfessionalIdAsync(command.ProfessionalId);
            var avaliableTimesForDay = allAvaliableTimes.Where(a => a.WeekDay == weekDay).ToList();

            if (avaliableTimesForDay.Count == 0)
                return Result.Success(new GetAvailableSlotsResponse([]));

            var existingSchedulings = await schedulingRepository.GetByProfessionalIdAndDateAsync(
                command.ProfessionalId, command.Date);

            var step = TimeSpan.FromMinutes(30);
            var duration = TimeSpan.FromMinutes(service.DurationInMinutes);
            var slots = new List<TimeSlotItem>();

            foreach (var avaliableTime in avaliableTimesForDay)
            {
                var current = avaliableTime.StartTime;
                while (current + duration <= avaliableTime.EndTime)
                {
                    var slotEnd = current + duration;
                    var baseDate = command.Date.ToDateTime(TimeOnly.MinValue);
                    var slotStartDt = baseDate + current;
                    var slotEndDt = baseDate + slotEnd;

                    var isBooked = existingSchedulings.Any(s =>
                        slotStartDt < s.EndDate && slotEndDt > s.InitDate);

                    if (!isBooked)
                        slots.Add(new TimeSlotItem(
                            current.ToString(@"hh\:mm"),
                            slotEnd.ToString(@"hh\:mm")));

                    current += step;
                }
            }

            return Result.Success(new GetAvailableSlotsResponse(slots));
        }
        catch (Exception ex)
        {
            return Result.Failure<GetAvailableSlotsResponse>(
                Error.Problem("Booking.GetSlotsError", ex.Message));
        }
    }

    private static WeekEnum MapDayOfWeekToWeekEnum(DayOfWeek dayOfWeek) =>
        dayOfWeek switch
        {
            DayOfWeek.Monday => WeekEnum.Monday,
            DayOfWeek.Tuesday => WeekEnum.Tuesday,
            DayOfWeek.Wednesday => WeekEnum.Wednesday,
            DayOfWeek.Thursday => WeekEnum.Thursday,
            DayOfWeek.Friday => WeekEnum.Friday,
            DayOfWeek.Saturday => WeekEnum.Saturday,
            DayOfWeek.Sunday => WeekEnum.Sunday,
            _ => throw new ArgumentOutOfRangeException(nameof(dayOfWeek))
        };
}
