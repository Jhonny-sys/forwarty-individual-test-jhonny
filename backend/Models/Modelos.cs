namespace Forwarty.Api.Models;

public class Cliente
{
    public int Id { get; set; }
    public string Nit { get; set; } = "";
    public string RazonSocial { get; set; } = "";
    public string Ciudad { get; set; } = "";
    public bool Activo { get; set; }

    public List<Operacion> Operaciones { get; set; } = new();
}

public class Operacion
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public string NumeroOperacion { get; set; } = "";
    public string Tipo { get; set; } = "";
    public string Modalidad { get; set; } = "";
    public string Estado { get; set; } = "";
    public DateTime FechaApertura { get; set; }
    public string Moneda { get; set; } = "COP";

    public Cliente? Cliente { get; set; }
    public List<Costo> Costos { get; set; } = new();
}

public class Costo
{
    public long Id { get; set; }
    public int OperacionId { get; set; }
    public string Concepto { get; set; } = "";
    public string Tipo { get; set; } = "";
    public string? Proveedor { get; set; }
    public string Moneda { get; set; } = "COP";
    public decimal Valor { get; set; }
    public decimal ValorCop { get; set; }
    public bool Facturable { get; set; }
    public DateOnly FechaRegistro { get; set; }

    public Operacion? Operacion { get; set; }
}
