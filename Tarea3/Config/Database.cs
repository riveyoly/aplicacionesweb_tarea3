using Npgsql;

namespace CrudClientes.Config;

/// <summary>
/// CONFIGURACIÓN DE LA BASE DE DATOS
///
/// Guarda la cadena de conexión (definida en appsettings.json)
/// y entrega conexiones abiertas a PostgreSQL.
/// </summary>
public static class Database
{
    private static string? _cadenaConexion;

    /// <summary>Se llama una sola vez al iniciar la aplicación (Program.cs).</summary>
    public static void Configurar(string? cadenaConexion)
    {
        if (string.IsNullOrWhiteSpace(cadenaConexion))
        {
            throw new InvalidOperationException(
                "Falta la cadena de conexión 'Clientes' en appsettings.json.");
        }

        _cadenaConexion = cadenaConexion;
    }

    /// <summary>Devuelve una conexión abierta. Debe usarse con "using".</summary>
    public static NpgsqlConnection Conectar()
    {
        var conexion = new NpgsqlConnection(
            _cadenaConexion ?? throw new InvalidOperationException("La base de datos no fue configurada."));

        conexion.Open();
        return conexion;
    }
}
