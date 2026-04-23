using System.Reflection;

namespace TaMarcado.Infraestrutura;

public static class TaMarcadoAssemblyReference
{
    public static Assembly Assembly => Assembly.GetExecutingAssembly();
}
