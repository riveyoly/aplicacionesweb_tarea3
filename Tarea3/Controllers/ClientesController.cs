using CrudClientes.Models;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace CrudClientes.Controllers;

/// <summary>
/// CONTROLADOR: ClientesController
///
/// Recibe las acciones del usuario, usa el modelo Cliente
/// y decide qué vista mostrar.
/// </summary>
public class ClientesController : Controller
{
    private const string CedulaDuplicada = "23505"; // unique_violation de PostgreSQL
    private const string NoEncontrado = "P0002";    // lanzado por los procedimientos

    // GET: /Clientes?buscar=texto   (READ - lista)
    public IActionResult Index(string? buscar)
    {
        ViewData["Buscar"] = buscar;
        return View(Cliente.Listar(buscar));
    }

    // GET: /Clientes/Details/5   (READ - un cliente)
    public IActionResult Details(int id)
    {
        var cliente = Cliente.BuscarPorId(id);
        return cliente is null ? NotFound() : View(cliente);
    }

    // GET: /Clientes/Create   (formulario vacío)
    public IActionResult Create()
    {
        return View(new Cliente());
    }

    // POST: /Clientes/Create   (CREATE)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Cliente cliente)
    {
        if (!ModelState.IsValid)
        {
            return View(cliente);
        }

        try
        {
            cliente.Crear();
            TempData["Mensaje"] = $"Cliente {cliente.NombreCompleto} registrado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (PostgresException ex) when (ex.SqlState == CedulaDuplicada)
        {
            ModelState.AddModelError(nameof(Cliente.Cedula), "Ya existe un cliente con esta cédula.");
            return View(cliente);
        }
    }

    // GET: /Clientes/Edit/5   (formulario con datos)
    public IActionResult Edit(int id)
    {
        var cliente = Cliente.BuscarPorId(id);
        return cliente is null ? NotFound() : View(cliente);
    }

    // POST: /Clientes/Edit/5   (UPDATE)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Cliente cliente)
    {
        if (id != cliente.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(cliente);
        }

        try
        {
            cliente.Actualizar();
            TempData["Mensaje"] = $"Cliente {cliente.NombreCompleto} actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (PostgresException ex) when (ex.SqlState == CedulaDuplicada)
        {
            ModelState.AddModelError(nameof(Cliente.Cedula), "Ya existe otro cliente con esta cédula.");
            return View(cliente);
        }
        catch (PostgresException ex) when (ex.SqlState == NoEncontrado)
        {
            return NotFound();
        }
    }

    // GET: /Clientes/Delete/5   (pantalla de confirmación)
    public IActionResult Delete(int id)
    {
        var cliente = Cliente.BuscarPorId(id);
        return cliente is null ? NotFound() : View(cliente);
    }

    // POST: /Clientes/Delete/5   (DELETE)
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var cliente = Cliente.BuscarPorId(id);

        if (cliente is null)
        {
            TempData["Error"] = "El cliente ya no existe.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            cliente.Eliminar();
            TempData["Mensaje"] = "Cliente eliminado correctamente.";
        }
        catch (PostgresException)
        {
            TempData["Error"] = "No se pudo eliminar el cliente porque tiene información relacionada.";
        }

        return RedirectToAction(nameof(Index));
    }
}
