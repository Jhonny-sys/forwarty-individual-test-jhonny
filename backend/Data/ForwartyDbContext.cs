using Microsoft.EntityFrameworkCore;
using Forwarty.Api.Models;

namespace Forwarty.Api.Data;

public class ForwartyDbContext : DbContext
{
    public ForwartyDbContext(DbContextOptions<ForwartyDbContext> options) : base(options) { }

    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Operacion> Operaciones => Set<Operacion>();
    public DbSet<Costo> Costos => Set<Costo>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Cliente>(e =>
        {
            e.ToTable("clientes");
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.Nit).HasColumnName("nit");
            e.Property(x => x.RazonSocial).HasColumnName("razon_social");
            e.Property(x => x.Ciudad).HasColumnName("ciudad");
            e.Property(x => x.Activo).HasColumnName("activo");
        });

        b.Entity<Operacion>(e =>
        {
            e.ToTable("operaciones");
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.ClienteId).HasColumnName("cliente_id");
            e.Property(x => x.NumeroOperacion).HasColumnName("numero_operacion");
            e.Property(x => x.Tipo).HasColumnName("tipo");
            e.Property(x => x.Modalidad).HasColumnName("modalidad");
            e.Property(x => x.Estado).HasColumnName("estado");
            e.Property(x => x.FechaApertura).HasColumnName("fecha_apertura");
            e.Property(x => x.Moneda).HasColumnName("moneda");
            e.HasOne(x => x.Cliente).WithMany(c => c.Operaciones).HasForeignKey(x => x.ClienteId);
        });

        b.Entity<Costo>(e =>
        {
            e.ToTable("costos");
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.OperacionId).HasColumnName("operacion_id");
            e.Property(x => x.Concepto).HasColumnName("concepto");
            e.Property(x => x.Tipo).HasColumnName("tipo");
            e.Property(x => x.Proveedor).HasColumnName("proveedor");
            e.Property(x => x.Moneda).HasColumnName("moneda");
            e.Property(x => x.Valor).HasColumnName("valor").HasPrecision(18, 2);
            e.Property(x => x.ValorCop).HasColumnName("valor_cop").HasPrecision(18, 2);
            e.Property(x => x.Facturable).HasColumnName("facturable");
            e.Property(x => x.FechaRegistro).HasColumnName("fecha_registro");
            e.HasOne(x => x.Operacion).WithMany(o => o.Costos).HasForeignKey(x => x.OperacionId);
        });
    }
}
