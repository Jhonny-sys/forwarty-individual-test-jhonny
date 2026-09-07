using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Forwarty.Api.Data;
using Forwarty.Api.Dtos;

namespace Forwarty.Api.Controllers;

[ApiController]
[Route("api/operaciones")]
public class OperacionesController : ControllerBase
{
    private readonly ForwartyDbContext _db;

    public OperacionesController(ForwartyDbContext db) => _db = db;

    /// <summary>
    /// Listado de operaciones con el total de costos de cada una.
    /// Alimenta la pantalla principal del ERP.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<OperacionListResponse>> Get(
        [FromQuery] DateTime? desde,
        [FromQuery] DateTime? hasta,
        CancellationToken ct = default)
    {
        var consulta = _db.Operaciones.AsNoTracking().AsQueryable();

        if (desde.HasValue)
            consulta = consulta.Where(o => o.FechaApertura >= desde.Value);

        if (hasta.HasValue)
            consulta = consulta.Where(o => o.FechaApertura <= hasta.Value);

        var items = await consulta
            .OrderByDescending(o => o.FechaApertura)
            .Select(o => new OperacionListItemDto
            {
                Id = o.Id,
                NumeroOperacion = o.NumeroOperacion,
                ClienteRazonSocial = o.Cliente!.RazonSocial,
                Tipo = o.Tipo,
                Modalidad = o.Modalidad,
                Estado = o.Estado,
                FechaApertura = o.FechaApertura,
                TotalCop = o.Costos.Sum(c => (decimal?)c.ValorCop) ?? 0m,
                CantidadCostos = o.Costos.Count()
            })
            .ToListAsync(ct);

        return Ok(new OperacionListResponse
        {
            Items = items,
            Total = items.Count
        });
    }

    /// <summary>Estados posibles, para armar el filtro en el frontend.</summary>
    [HttpGet("estados")]
    public async Task<ActionResult<List<string>>> Estados(CancellationToken ct = default)
    {
        var estados = await _db.Operaciones
            .AsNoTracking()
            .Select(o => o.Estado)
            .Distinct()
            .OrderBy(e => e)
            .ToListAsync(ct);

        return Ok(estados);
    }
}
