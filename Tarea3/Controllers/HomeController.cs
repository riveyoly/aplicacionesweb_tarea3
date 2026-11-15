using Microsoft.AspNetCore.Mvc;

namespace CrudClientes.Controllers;

/// <summary>
/// CONTROLADOR: HomeController — página de bienvenida.
/// </summary>
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
