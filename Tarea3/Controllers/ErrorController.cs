using System.Diagnostics;
using CrudClientes.Models;
using Microsoft.AspNetCore.Mvc;

namespace CrudClientes.Controllers;

public class ErrorController : Controller
{
    [Route("/Error")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Index()
    {
        return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
