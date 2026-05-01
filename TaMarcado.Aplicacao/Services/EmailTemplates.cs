using System.Globalization;

namespace TaMarcado.Aplicacao.Services;

public static class EmailTemplates
{
    public static string BookingConfirmation(
        string clientName, string serviceName, string professionalName,
        DateTime initDate, DateTime endDate, decimal price) =>
        Build(
            clientName,
            "Agendamento confirmado! ✅",
            $"Seu agendamento foi confirmado. Estamos te esperando!",
            serviceName, professionalName, initDate, endDate, price,
            "#22c55e");

    public static string Reminder24h(
        string clientName, string serviceName, string professionalName,
        DateTime initDate, DateTime endDate, decimal price) =>
        Build(
            clientName,
            "Lembrete do seu agendamento 📅",
            $"Passando para lembrar que você tem um agendamento no dia {initDate:dd/MM} às {initDate:HH:mm}. Até lá!",
            serviceName, professionalName, initDate, endDate, price,
            "#f59e0b");

    public static string Reminder1h(
        string clientName, string serviceName, string professionalName,
        DateTime initDate, DateTime endDate, decimal price) =>
        Build(
            clientName,
            "Lembrete: seu agendamento é em 1 hora ⏰",
            "Seu agendamento começa em breve. Não se esqueça!",
            serviceName, professionalName, initDate, endDate, price,
            "#ef4444");

    private static string Build(
        string clientName, string title, string message,
        string serviceName, string professionalName,
        DateTime initDate, DateTime endDate, decimal price,
        string badgeColor)
    {
        var date = initDate.ToString("dddd, dd 'de' MMMM 'de' yyyy", new CultureInfo("pt-BR"));
        var time = $"{initDate:HH:mm} – {endDate:HH:mm}";
        var priceFormatted = price.ToString("C2", new CultureInfo("pt-BR"));

        return $"""
        <!DOCTYPE html>
        <html lang="pt-BR">
        <head>
          <meta charset="UTF-8" />
          <meta name="viewport" content="width=device-width, initial-scale=1.0" />
          <title>{title}</title>
        </head>
        <body style="margin:0;padding:0;background-color:#f3f4f6;font-family:Arial,Helvetica,sans-serif;">
          <table width="100%" cellpadding="0" cellspacing="0" style="background-color:#f3f4f6;padding:32px 0;">
            <tr>
              <td align="center">
                <table width="600" cellpadding="0" cellspacing="0" style="max-width:600px;width:100%;background-color:#ffffff;border-radius:12px;overflow:hidden;box-shadow:0 4px 6px rgba(0,0,0,0.07);">

                  <!-- Header -->
                  <tr>
                    <td style="background:linear-gradient(135deg,#7c3aed,#4f46e5);padding:36px 40px;text-align:center;">
                      <p style="margin:0;font-size:28px;font-weight:bold;color:#ffffff;letter-spacing:-0.5px;">📋 TaMarcado</p>
                      <p style="margin:8px 0 0;font-size:14px;color:#e0d9ff;">Agendamento online simplificado</p>
                    </td>
                  </tr>

                  <!-- Badge -->
                  <tr>
                    <td style="padding:28px 40px 0;text-align:center;">
                      <span style="display:inline-block;background-color:{badgeColor};color:#fff;font-size:13px;font-weight:bold;padding:6px 18px;border-radius:999px;">{title}</span>
                    </td>
                  </tr>

                  <!-- Greeting -->
                  <tr>
                    <td style="padding:24px 40px 0;">
                      <p style="margin:0;font-size:20px;font-weight:bold;color:#111827;">Olá, {clientName}! 👋</p>
                      <p style="margin:8px 0 0;font-size:15px;color:#6b7280;line-height:1.6;">{message}</p>
                    </td>
                  </tr>

                  <!-- Details Card -->
                  <tr>
                    <td style="padding:24px 40px;">
                      <table width="100%" cellpadding="0" cellspacing="0" style="background-color:#f8f7ff;border:1px solid #e9d5ff;border-radius:10px;overflow:hidden;">
                        <tr>
                          <td style="padding:20px 24px;border-bottom:1px solid #e9d5ff;">
                            <p style="margin:0;font-size:11px;font-weight:bold;color:#7c3aed;text-transform:uppercase;letter-spacing:1px;">Detalhes do Agendamento</p>
                          </td>
                        </tr>
                        <tr>
                          <td style="padding:0 24px;">
                            <table width="100%" cellpadding="0" cellspacing="0">
                              {DetailRow("📅", "Data", date)}
                              {DetailRow("🕐", "Horário", time)}
                              {DetailRow("✂️", "Serviço", serviceName)}
                              {DetailRow("👤", "Profissional", professionalName)}
                              {DetailRow("💰", "Valor", priceFormatted, last: true)}
                            </table>
                          </td>
                        </tr>
                      </table>
                    </td>
                  </tr>

                  <!-- Footer -->
                  <tr>
                    <td style="background-color:#f9fafb;padding:24px 40px;text-align:center;border-top:1px solid #e5e7eb;">
                      <p style="margin:0;font-size:13px;color:#9ca3af;">Este e-mail foi enviado automaticamente pelo <strong>TaMarcado</strong>.</p>
                      <p style="margin:6px 0 0;font-size:12px;color:#d1d5db;">Agendamento online para profissionais autônomos brasileiros.</p>
                    </td>
                  </tr>

                </table>
              </td>
            </tr>
          </table>
        </body>
        </html>
        """;
    }

    private static string DetailRow(string icon, string label, string value, bool last = false)
    {
        var border = last ? "" : "border-bottom:1px solid #e9d5ff;";
        return $"""
        <tr>
          <td style="padding:14px 0;{border}width:32px;vertical-align:top;">
            <span style="font-size:18px;">{icon}</span>
          </td>
          <td style="padding:14px 8px;{border}vertical-align:top;">
            <p style="margin:0;font-size:11px;color:#9ca3af;text-transform:uppercase;letter-spacing:0.5px;">{label}</p>
            <p style="margin:3px 0 0;font-size:15px;font-weight:600;color:#1f2937;">{value}</p>
          </td>
        </tr>
        """;
    }
}
