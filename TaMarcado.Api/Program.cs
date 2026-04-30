using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using TaMarcado.Api.Extensions;
using TaMarcado.Aplicacao.UseCases.Professionals.CreateProfessional;
using TaMarcado.Aplicacao.UseCases.Professionals.GetProfessional;
using TaMarcado.Aplicacao.UseCases.Category.GetCategories;
using TaMarcado.Aplicacao.UseCases.Services.CreateService;
using TaMarcado.Aplicacao.UseCases.Services.GetServices;
using TaMarcado.Aplicacao.UseCases.AvaliableTimes.CreateAvaliableTime;
using TaMarcado.Aplicacao.UseCases.AvaliableTimes.GetAvaliableTimes;
using TaMarcado.Aplicacao.UseCases.AvaliableTimes.DeleteAvaliableTime;
using TaMarcado.Dominio.Repositories;
using TaMarcado.Infraestrutura.Repositories;
using TaMarcado.Infraestrutura.Data;
using TaMarcado.Aplicacao.UseCases.Services.DeleteServices;
using TaMarcado.Aplicacao.UseCases.Booking.GetPublicProfile;
using TaMarcado.Aplicacao.UseCases.Booking.GetAvailableSlots;
using TaMarcado.Aplicacao.UseCases.Booking.CreateScheduling;
using TaMarcado.Aplicacao.UseCases.Booking.GetPublicProfessionals;
using TaMarcado.Aplicacao.UseCases.Schedulings.GetSchedulingsByProfessional;
using TaMarcado.Aplicacao.UseCases.Schedulings.ConfirmScheduling;
using TaMarcado.Aplicacao.UseCases.Schedulings.CancelScheduling;
using TaMarcado.Aplicacao.UseCases.Schedulings.ConcludeScheduling;
using TaMarcado.Aplicacao.UseCases.Clients.GetClients;
using TaMarcado.Aplicacao.UseCases.Clients.UpdateClientObservations;
using TaMarcado.Aplicacao.UseCases.Schedulings.GetSchedulingsByClient;
using TaMarcado.Aplicacao.UseCases.Schedulings.CancelSchedulingByClient;
using TaMarcado.Aplicacao.UseCases.Payments.GetPaymentByScheduling;
using TaMarcado.Aplicacao.UseCases.Payments.ConfirmPayment;
using TaMarcado.Aplicacao.UseCases.Payments.GetClientPayment;
using TaMarcado.Infraestrutura.Services;
using TaMarcado.Api.BackgroundServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("ApiWeb", policy =>
    {
        policy.WithOrigins("https://localhost:7137")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddOpenApi();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Banco
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Identity
builder.Services
    .AddIdentityApiEndpoints<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedEmail = false;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthorization();
builder.Services.AddScoped<IProfessionalRepository, ProfessionalRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
builder.Services.AddScoped<IAvaliableTimeRepository, AvaliableTimeRepository>();
builder.Services.AddScoped<CreateAvaliableTimeHandler>();
builder.Services.AddScoped<GetAvaliableTimesHandler>();
builder.Services.AddScoped<DeleteAvaliableTimeHandler>();
builder.Services.AddScoped<CreateProfessionalHandler>();
builder.Services.AddScoped<GetProfessionalHandler>();
builder.Services.AddScoped<GetCategoriesHandler>();
builder.Services.AddScoped<CreateServiceHandler>();
builder.Services.AddScoped<GetServicesHandler>();
builder.Services.AddScoped<DeleteServicesHandler>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<GetClientsHandler>();
builder.Services.AddScoped<UpdateClientObservationsHandler>();
builder.Services.AddScoped<GetSchedulingsByClientHandler>();
builder.Services.AddScoped<CancelSchedulingByClientHandler>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<GetPaymentBySchedulingHandler>();
builder.Services.AddScoped<ConfirmPaymentHandler>();
builder.Services.AddScoped<GetClientPaymentHandler>();
builder.Services.AddScoped<INotificationSchedulingRepository, NotificationSchedulingRepository>();
builder.Services.AddScoped<IEmailService, SmtpEmailService>();
builder.Services.AddHostedService<NotificationBackgroundService>();
builder.Services.AddScoped<ISchedulingRepository, SchedulingRepository>();
builder.Services.AddScoped<GetPublicProfileHandler>();
builder.Services.AddScoped<GetAvailableSlotsHandler>();
builder.Services.AddScoped<CreateSchedulingHandler>();
builder.Services.AddScoped<GetPublicProfessionalsHandler>();
builder.Services.AddScoped<GetSchedulingsByProfessionalHandler>();
builder.Services.AddScoped<ConfirmSchedulingHandler>();
builder.Services.AddScoped<CancelSchedulingHandler>();
builder.Services.AddScoped<ConcludeSchedulingHandler>();
builder.Services.AddEndpoints(Assembly.GetExecutingAssembly());

var app = builder.Build();

// Seed roles
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    foreach (var role in new[] { "Profissional", "Cliente" })
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
}

//CORS
app.UseCors("ApiWeb");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

// Endpoints do Identity
app.MapIdentityApi<ApplicationUser>();

app.MapEndpoints();

app.Run();