
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Molinos.DataAgro.Entities
{
    public class PreslipMap : EntityTypeConfiguration<Preslip>
    {
        public PreslipMap()
        {
            // Primary Key
            this.HasKey(t => t.PreslipId);

            // Properties
            // Table & Column Mappings
            this.ToTable("Preslip");
            this.Property(t => t.PreslipId).HasColumnName("PreslipId");
            this.Property(t => t.TipoDeNegocioId).HasColumnName("TipoDeNegocioId");
            this.Property(t => t.MaterialId).HasColumnName("MaterialId");
            this.Property(t => t.CampañaId).HasColumnName("CampañaId");
            this.Property(t => t.Cantidad).HasColumnName("Cantidad");
            this.Property(t => t.Precio).HasColumnName("Precio");
            this.Property(t => t.FechaDesde).HasColumnName("FechaDesde");
            this.Property(t => t.FechaHasta).HasColumnName("FechaHasta");
            this.Property(t => t.FechaDeEntrega).HasColumnName("FechaDeEntrega");
            this.Property(t => t.ProveedorId).HasColumnName("ProveedorId");
            this.Property(t => t.EstadoPreslipId).HasColumnName("EstadoPreslipId");
        }
    }
}


