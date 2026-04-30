using Microsoft.AspNetCore.Identity;
using QRCoder;
using TaMarcado.Api.Extensions;
using TaMarcado.Api.Infrastructure;
using TaMarcado.Aplicacao.UseCases.Payments.GetPaymentByScheduling;
using TaMarcado.Dominio.Repositories;
using TaMarcado.Infraestrutura.Data;

namespace TaMarcado.Api.Endpoints.Payments;

public class GetPaymentEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/payments/{schedulingId:guid}", async (
            Guid schedulingId,
            string email,
            UserManager<ApplicationUser> userManager,
            IProfessionalRepository professionalRepository,
            GetPaymentBySchedulingHandler handler) =>
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user is null)
                return Results.Unauthorized();

            var professionalId = await professionalRepository.GetIdByUserIdAsync(user.Id);
            if (professionalId is null)
                return Results.NotFound("Perfil profissional não encontrado.");

            var command = new GetPaymentBySchedulingCommand(schedulingId, professionalId.Value);
            var result = await handler.Handle(command);

            return result.Match(
                onSuccess: r =>
                {
                    var qrCodeBase64 = GenerateQrCode(r.PixPayload);
                    return Results.Ok(new
                    {
                        r.PaymentId,
                        r.Status,
                        r.PixPayload,
                        QrCodeBase64 = qrCodeBase64,
                        r.DatePayment
                    });
                },
                onFailure: r => CustomResults.Problem(r));
        });
    }

    private static string GenerateQrCode(string payload)
    {
        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.M);
        using var png = new PngByteQRCode(data);
        return Convert.ToBase64String(png.GetGraphic(6));
    }
}
