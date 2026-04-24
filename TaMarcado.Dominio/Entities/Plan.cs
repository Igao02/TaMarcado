using TaMarcado.DominioPrincipal.Entities;

namespace TaMarcado.Dominio.Entities;

public class Plan : Entity
{
    public Plan()
    {
        //ORM Purpose
    }

    public string Name { get; set; } = string.Empty;
    public int? LimitMonthlySchedulings { get; set; }
    public decimal Price { get; set; }
    public bool Active { get; set; }

    public Plan(string name, int? limitMonthlySchedulings, decimal price, bool active)
    {
        Name = name;
        LimitMonthlySchedulings = limitMonthlySchedulings;
        Price = price;
        Active = active;
    }
}
