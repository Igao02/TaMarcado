using Microsoft.AspNetCore.Identity;
using TaMarcado.Dominio.Entities;

namespace TaMarcado.Infraestrutura.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public Professional? Professional { get; set; }
}
