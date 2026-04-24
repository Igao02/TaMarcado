using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaMarcado.Dominio.Entities;

namespace TaMarcado.Infraestrutura.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Category> Categorie { get; set; }
    public DbSet<Professional> Professional { get; set; }
    public DbSet<Service> Service { get; set; }
    public DbSet<AvaliableTime> AvaliableTime { get; set; }
    public DbSet<Blocked> Blocked { get; set; }
    public DbSet<Client> Client { get; set; }
    public DbSet<Scheduling> Scheduling { get; set; }
    public DbSet<Payment> Payment { get; set; }
    public DbSet<NotificationScheduling> NotificationScheduling { get; set; }
    public DbSet<Plan> Plan { get; set; }
    public DbSet<Subscription> Subscription { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(TaMarcadoAssemblyReference.Assembly);
    }
}
