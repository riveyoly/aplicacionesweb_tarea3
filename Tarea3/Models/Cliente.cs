using System.ComponentModel.DataAnnotations;
using System.Data;
using CrudClientes.Config;
using Npgsql;
using NpgsqlTypes;

namespace CrudClientes.Models;

/// <summary>
/// MODELO: Cliente
///
/// Representa a un cliente (objeto) y contiene los métodos del CRUD,
/// que llaman a los procedimientos almacenados de PostgreSQL:
///   Crear()       -> sp_clientes_insertar
///   Listar()      -> sp_clientes_listar
///   BuscarPorId() -> sp_clientes_obtener
///   Actualizar()  -> sp_clientes_actualizar
///   Eliminar()    -> sp_clientes_eliminar
/// </summary>
public class Cliente
{
    // =================== ATRIBUTOS (15 campos) ===================

    public int Id { get; set; }

    [Required(ErrorMessage = "La cédula es obligatoria.")]
    [StringLength(13, MinimumLength = 10, ErrorMessage = "La cédula debe tener 10 dígitos (o 13 si es RUC).")]
    [CedulaEcuatoriana]
    [Display(Name = "Cédula / RUC")]
    public string Cedula { get; set; } = string.Empty;

    [Required(ErrorMessage = "Los nombres son obligatorios.")]
    [StringLength(100, ErrorMessage = "Máximo 100 caracteres.")]
    public string Nombres { get; set; } = string.Empty;

    [Required(ErrorMessage = "Los apellidos son obligatorios.")]
    [StringLength(100, ErrorMessage = "Máximo 100 caracteres.")]
    public string Apellidos { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    [Display(Name = "Fecha de nacimiento")]
    public DateTime? FechaNacimiento { get; set; }

    [RegularExpression("^[MFO]$", ErrorMessage = "Seleccione un género válido.")]
    [Display(Name = "Género")]
    public string? Genero { get; set; }

    [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
    [StringLength(150)]
    [Display(Name = "Correo electrónico")]
    public string? Email { get; set; }

    [RegularExpression(@"^\d{7,15}$", ErrorMessage = "Solo números (7 a 15 dígitos).")]
    [Display(Name = "Teléfono")]
    public string? Telefono { get; set; }

    [RegularExpression(@"^09\d{8}$", ErrorMessage = "El celular debe tener 10 dígitos y empezar con 09.")]
    public string? Celular { get; set; }

    [StringLength(250)]
    [Display(Name = "Dirección")]
    public string? Direccion { get; set; }

    [StringLength(80)]
    public string? Ciudad { get; set; }

    [StringLength(80)]
    public string? Provincia { get; set; }

    [Range(0, 9999999999.99, ErrorMessage = "El límite de crédito no puede ser negativo.")]
    [Display(Name = "Límite de crédito")]
    public decimal LimiteCredito { get; set; }

    public bool Activo { get; set; } = true;

    [Display(Name = "Fecha de registro")]
    public DateTime FechaRegistro { get; set; }

    // =================== PROPIEDADES CALCULADAS ===================

    [Display(Name = "Cliente")]
    public string NombreCompleto => $"{Apellidos} {Nombres}".Trim();

    public int? Edad
    {
        get
        {
            if (FechaNacimiento is null)
            {
                return null;
            }

            var hoy = DateTime.Today;
            int edad = hoy.Year - FechaNacimiento.Value.Year;

            if (FechaNacimiento.Value.Date > hoy.AddYears(-edad))
            {
                edad--;
            }

            return edad;
        }
    }

    public string GeneroTexto => Genero switch
    {
        "M" => "Masculino",
        "F" => "Femenino",
        "O" => "Otro",
        _ => "-"
    };

    // =================== MÉTODOS DEL CRUD ===================

    /// <summary>CREATE: inserta el cliente y guarda en Id el código generado.</summary>
    public void Crear()
    {
        using var conexion = Database.Conectar();

        // CommandType.StoredProcedure hace que Npgsql ejecute: CALL sp_clientes_insertar(...)
        using var comando = new NpgsqlCommand("sp_clientes_insertar", conexion)
        {
            CommandType = CommandType.StoredProcedure
        };

        AgregarParametros(comando);

        var parametroId = new NpgsqlParameter("p_id", NpgsqlDbType.Integer)
        {
            Direction = ParameterDirection.InputOutput,
            Value = DBNull.Value
        };
        comando.Parameters.Add(parametroId);

        comando.ExecuteNonQuery();

        Id = Convert.ToInt32(parametroId.Value);
    }

    /// <summary>READ: devuelve la lista de clientes (con búsqueda opcional).</summary>
    public static List<Cliente> Listar(string? buscar = null)
    {
        using var conexion = Database.Conectar();
        using var comando = new NpgsqlCommand("SELECT * FROM sp_clientes_listar(@p_buscar)", conexion);

        comando.Parameters.Add(new NpgsqlParameter("p_buscar", NpgsqlDbType.Varchar)
        {
            Value = string.IsNullOrWhiteSpace(buscar) ? DBNull.Value : buscar.Trim()
        });

        var clientes = new List<Cliente>();

        using var lector = comando.ExecuteReader();

        while (lector.Read())
        {
            clientes.Add(DesdeFila(lector));
        }

        return clientes;
    }

    /// <summary>READ: busca un cliente por su Id. Devuelve null si no existe.</summary>
    public static Cliente? BuscarPorId(int id)
    {
        using var conexion = Database.Conectar();
        using var comando = new NpgsqlCommand("SELECT * FROM sp_clientes_obtener(@p_id)", conexion);
        comando.Parameters.AddWithValue("p_id", id);

        using var lector = comando.ExecuteReader();

        return lector.Read() ? DesdeFila(lector) : null;
    }

    /// <summary>UPDATE: guarda los cambios del cliente.</summary>
    public void Actualizar()
    {
        using var conexion = Database.Conectar();
        using var comando = new NpgsqlCommand("sp_clientes_actualizar", conexion)
        {
            CommandType = CommandType.StoredProcedure
        };

        comando.Parameters.AddWithValue("p_id", Id);
        AgregarParametros(comando);

        comando.ExecuteNonQuery();
    }

    /// <summary>DELETE: elimina el cliente.</summary>
    public void Eliminar()
    {
        using var conexion = Database.Conectar();
        using var comando = new NpgsqlCommand("sp_clientes_eliminar", conexion)
        {
            CommandType = CommandType.StoredProcedure
        };

        comando.Parameters.AddWithValue("p_id", Id);

        comando.ExecuteNonQuery();
    }

    // =================== MÉTODOS DE APOYO ===================

    /// <summary>Parámetros comunes de insertar y actualizar.</summary>
    private void AgregarParametros(NpgsqlCommand comando)
    {
        comando.Parameters.Add(Parametro("p_cedula", NpgsqlDbType.Varchar, Cedula));
        comando.Parameters.Add(Parametro("p_nombres", NpgsqlDbType.Varchar, Nombres));
        comando.Parameters.Add(Parametro("p_apellidos", NpgsqlDbType.Varchar, Apellidos));
        comando.Parameters.Add(Parametro("p_fecha_nacimiento", NpgsqlDbType.Date, FechaNacimiento?.Date));
        comando.Parameters.Add(Parametro("p_genero", NpgsqlDbType.Char, Genero));
        comando.Parameters.Add(Parametro("p_email", NpgsqlDbType.Varchar, Email));
        comando.Parameters.Add(Parametro("p_telefono", NpgsqlDbType.Varchar, Telefono));
        comando.Parameters.Add(Parametro("p_celular", NpgsqlDbType.Varchar, Celular));
        comando.Parameters.Add(Parametro("p_direccion", NpgsqlDbType.Varchar, Direccion));
        comando.Parameters.Add(Parametro("p_ciudad", NpgsqlDbType.Varchar, Ciudad));
        comando.Parameters.Add(Parametro("p_provincia", NpgsqlDbType.Varchar, Provincia));
        comando.Parameters.Add(Parametro("p_limite_credito", NpgsqlDbType.Numeric, LimiteCredito));
        comando.Parameters.Add(Parametro("p_activo", NpgsqlDbType.Boolean, Activo));
    }

    private static NpgsqlParameter Parametro(string nombre, NpgsqlDbType tipo, object? valor)
    {
        if (valor is string texto && string.IsNullOrWhiteSpace(texto))
        {
            valor = null;
        }

        return new NpgsqlParameter(nombre, tipo) { Value = valor ?? DBNull.Value };
    }

    /// <summary>Crea un objeto Cliente a partir de una fila de la base de datos.</summary>
    private static Cliente DesdeFila(NpgsqlDataReader r) => new()
    {
        Id = r.GetInt32(r.GetOrdinal("id")),
        Cedula = r.GetString(r.GetOrdinal("cedula")),
        Nombres = r.GetString(r.GetOrdinal("nombres")),
        Apellidos = r.GetString(r.GetOrdinal("apellidos")),
        FechaNacimiento = r.IsDBNull(r.GetOrdinal("fecha_nacimiento")) ? null : r.GetDateTime(r.GetOrdinal("fecha_nacimiento")),
        Genero = Texto(r, "genero"),
        Email = Texto(r, "email"),
        Telefono = Texto(r, "telefono"),
        Celular = Texto(r, "celular"),
        Direccion = Texto(r, "direccion"),
        Ciudad = Texto(r, "ciudad"),
        Provincia = Texto(r, "provincia"),
        LimiteCredito = r.GetDecimal(r.GetOrdinal("limite_credito")),
        Activo = r.GetBoolean(r.GetOrdinal("activo")),
        FechaRegistro = r.GetDateTime(r.GetOrdinal("fecha_registro"))
    };

    private static string? Texto(NpgsqlDataReader r, string columna)
    {
        int i = r.GetOrdinal(columna);
        return r.IsDBNull(i) ? null : r.GetString(i).Trim();
    }
}
