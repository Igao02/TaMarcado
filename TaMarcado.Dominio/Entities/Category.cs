using TaMarcado.DominioPrincipal.Entities;

namespace TaMarcado.Dominio.Entities;

public class Category : Entity
{
    public Category()
    {
        //ORM Purpose
    }
    public string Name { get; set; } = string.Empty;

    public Category(string name)
    {
        Name = name;
    }
}
