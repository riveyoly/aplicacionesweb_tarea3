using System.ComponentModel.DataAnnotations;

namespace CrudClientes.Models;

/// <summary>
/// Valida una cédula ecuatoriana de 10 dígitos (algoritmo módulo 10)
/// o un RUC de 13 dígitos que termine en 001.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class CedulaEcuatorianaAttribute : ValidationAttribute
{
    public CedulaEcuatorianaAttribute()
        : base("La cédula o RUC ingresado no es válido.")
    {
    }

    public override bool IsValid(object? value)
    {
        if (value is not string texto || string.IsNullOrWhiteSpace(texto))
        {
            return true; // [Required] se encarga del campo vacío
        }

        texto = texto.Trim();

        if (texto.Length == 13)
        {
            return texto.EndsWith("001") && EsCedulaValida(texto[..10]);
        }

        return texto.Length == 10 && EsCedulaValida(texto);
    }

    public static bool EsCedulaValida(string cedula)
    {
        if (cedula.Length != 10 || !cedula.All(char.IsDigit))
        {
            return false;
        }

        int provincia = int.Parse(cedula[..2]);
        int tercerDigito = cedula[2] - '0';

        if (provincia is < 1 or > 24 && provincia != 30 || tercerDigito >= 6)
        {
            return false;
        }

        int suma = 0;

        for (int i = 0; i < 9; i++)
        {
            int digito = cedula[i] - '0';

            if (i % 2 == 0)
            {
                digito *= 2;

                if (digito > 9)
                {
                    digito -= 9;
                }
            }

            suma += digito;
        }

        int verificador = (10 - suma % 10) % 10;

        return verificador == cedula[9] - '0';
    }
}
