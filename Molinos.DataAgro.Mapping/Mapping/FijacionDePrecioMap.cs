
using Molinos.DataAgro.Entities.Entities;
using System.Data.Entity.ModelConfiguration;

namespace Molinos.DataAgro.Entities
{
    public class FijacionDePrecioMap : EntityTypeConfiguration<FijacionDePrecio>
    {
        public FijacionDePrecioMap()
        {
            // Primary Key
            this.HasKey(t => t.FijacionId);

            // Properties
            // Table & Column Mappings
            this.ToTable("FijacionDePrecio");
            this.Property(t => t.FijacionId).HasColumnName("FijacionId");
            this.Property(t => t.MaterialId).HasColumnName("MaterialId");
            this.Property(t => t.Precio).HasColumnName("Precio");
            this.Property(t => t.Fecha).HasColumnName("Fecha");
            this.Property(t => t.ProveedorId).HasColumnName("ProveedorId");
        }
    }
}

