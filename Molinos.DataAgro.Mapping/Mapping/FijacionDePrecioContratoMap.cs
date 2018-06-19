
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Molinos.DataAgro.Entities
{
    public class FijacionDePrecioContratoMap : EntityTypeConfiguration<FijacionDePrecioContrato>
    {
        public FijacionDePrecioContratoMap()
        {
            // Primary Key
            this.HasKey(t => t.FijacionDePrecioContratoId);

            // Properties
            // Table & Column Mappings
            this.ToTable("FijacionDePrecioContrato");
            this.Property(t => t.FijacionDePrecioContratoId).HasColumnName("FijacionDePrecioContratoId");
            this.Property(t => t.MaterialId).HasColumnName("MaterialId");
            this.Property(t => t.Precio).HasColumnName("Precio");
            this.Property(t => t.Cantidad).HasColumnName("Cantidad");
            this.Property(t => t.Fecha).HasColumnName("Fecha");
            this.Property(t => t.ProveedorId).HasColumnName("ProveedorId");
            this.Property(t => t.ContratoId).HasColumnName("ContratoId");
            this.Property(t => t.ComercialId).HasColumnName("ComercialId");
            this.Property(t => t.MonedaId).HasColumnName("MonedaId");
            this.Property(t => t.Ampliaciones).HasColumnName("Ampliaciones");
            this.Property(t => t.Estado).HasColumnName(@"Estado").IsRequired();
            this.Property(t => t.Observacion).HasColumnName(@"Observacion").HasColumnType("varchar").IsOptional().IsUnicode(false);
        }
    }
}

