using System.Globalization;
using CrudClientes.Config;
using Microsoft.AspNetCore.Localization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Configuración de la base de datos (cadena "Clientes" de appsettings.json)
Database.Configurar(builder.Configuration.GetConnectionString("Clientes"));

var app = builder.Build();

// Cultura invariante para que los decimales se lean con punto (ej. 1500.50)
var cultura = CultureInfo.InvariantCulture;
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(cultura),
    SupportedCultures = [cultura],
    SupportedUICultures = [cultura]
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Ruta MVC: /Controlador/Acción/Id  (por defecto: Home/Index)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
