namespace Forwarty.Api.Dtos;

/// <summary>Una fila del listado de operaciones.</summary>
public class OperacionListItemDto
{
    public int Id { get; set; }
    public string NumeroOperacion { get; set; } = "";
    public string ClienteRazonSocial { get; set; } = "";
    public string Tipo { get; set; } = "";
    public string Modalidad { get; set; } = "";
    public string Estado { get; set; } = "";
    public DateTime FechaApertura { get; set; }
    public decimal TotalCop { get; set; }
    public int CantidadCostos { get; set; }
}

/// <summary>
/// Respuesta del listado. El frontend ya consume estos campos, así que puedes
/// agregar los que necesites pero no renombres los que están.
/// </summary>
public class OperacionListResponse
{
    public List<OperacionListItemDto> Items { get; set; } = new();

    /// <summary>Cantidad de operaciones que cumplen los filtros.</summary>
    public int Total { get; set; }
}
