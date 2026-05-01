using TaMarcado.Aplicacao.Services;
using TaMarcado.Dominio.Enum;
using TaMarcado.Dominio.Repositories;
using TaMarcado.Infraestrutura.Services;

namespace TaMarcado.Api.BackgroundServices;

public class NotificationBackgroundService(
    IServiceScopeFactory scopeFactory,
    ILogger<NotificationBackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await ProcessPendingNotificationsAsync();
            await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
        }
    }

    private async Task ProcessPendingNotificationsAsync()
    {
        using var scope = scopeFactory.CreateScope();
        var notificationRepo = scope.ServiceProvider.GetRequiredService<INotificationSchedulingRepository>();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

        List<TaMarcado.Dominio.Entities.NotificationScheduling> pending;
        try
        {
            pending = await notificationRepo.GetPendingDueAsync(DateTime.Now);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao buscar notificações pendentes.");
            return;
        }

        foreach (var notification in pending)
        {
            var s = notification.Scheduling;
            var clientEmail = s.Client.Email!;
            var clientName = s.Client.Name;
            var serviceName = s.Service.Name;
            var professionalName = s.Professional.ExibitionName;

            string subject;
            string html;

            try
            {
                (subject, html) = notification.SchedulingType switch
                {
                    SchedulingTypeEnum.Confirmed => (
                        $"Agendamento confirmado — {serviceName} com {professionalName}",
                        EmailTemplates.BookingConfirmation(clientName, serviceName, professionalName, s.InitDate, s.EndDate, s.Price)),

                    SchedulingTypeEnum.TweentyFourHoursReminder => (
                        $"Lembrete: {serviceName} em {s.InitDate:dd/MM} às {s.InitDate:HH:mm}",
                        EmailTemplates.Reminder24h(clientName, serviceName, professionalName, s.InitDate, s.EndDate, s.Price)),

                    SchedulingTypeEnum.OneHourReminder => (
                        $"Lembrete: seu agendamento é em 1 hora — {serviceName}",
                        EmailTemplates.Reminder1h(clientName, serviceName, professionalName, s.InitDate, s.EndDate, s.Price)),

                    _ => (string.Empty, string.Empty)
                };

                if (string.IsNullOrEmpty(subject))
                {
                    logger.LogWarning("Tipo de notificação desconhecido: {Type}", notification.SchedulingType);
                    continue;
                }

                await emailService.SendAsync(clientEmail, subject, html);

                notification.StatusNotification = StatusNotificationScheduling.Sent;
                notification.DateSend = DateTime.Now;
                notification.QuantitySend++;
                logger.LogInformation("E-mail enviado para {Email} ({Type}).", clientEmail, notification.SchedulingType);
            }
            catch (Exception ex)
            {
                notification.StatusNotification = StatusNotificationScheduling.Failed;
                notification.QuantitySend++;
                logger.LogError(ex, "Falha ao enviar e-mail para {Email}.", clientEmail);
            }

            try
            {
                await notificationRepo.UpdateAsync(notification);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Falha ao atualizar status da notificação {Id}.", notification.Id);
            }
        }
    }
}
