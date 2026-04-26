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
builder.Services.AddEndpoints(Assembly.GetExecutingAssembly());

var app = builder.Build();

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