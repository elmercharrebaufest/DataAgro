using Molinos.DataAgro.Entities.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Molinos.DataAgro.Entities
{
    public class DescuentosBonificacionesMap : EntityTypeConfiguration<DescuentosBonificaciones>
    {
        public DescuentosBonificacionesMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("DescuentosBonificaciones");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.FechaDesde).HasColumnName("FechaDesde");
            this.Property(t => t.FechaHasta).HasColumnName("FechaHasta");
            this.Property(t => t.Importe).HasColumnName("Importe");
            this.Property(t => t.MonedaId).HasColumnName("MonedaId");
            this.Property(t => t.Porcentaje).HasColumnName("Porcentaje");
            this.Property(t => t.TipoDBId).HasColumnName("TipoDBId");
            this.Property(t => t.TipoPeriodoDBId).HasColumnName("TipoPeriodoDBId");
            this.Property(t => t.ContratoId).HasColumnName("ContratoId");
        }
    }
}


