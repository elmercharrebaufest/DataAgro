
using Molinos.DataAgro.Entities.Entities;
using System.Data.Entity.ModelConfiguration;

namespace Molinos.DataAgro.Entities
{
    public class ProveedorEstadoMap : EntityTypeConfiguration<ProveedorEstado>
    {
        public ProveedorEstadoMap()
        {
            // Primary Key
            this.HasKey(t => t.ProveedorEstadoId);

            // Properties
            this.Property(t => t.ProveedorId);
            this.Property(t => t.EstadoId);
            this.Property(t => t.ComercialId);



            // Table & Column Mappings
            this.ToTable("ProveedorEstado");
            this.Property(t => t.ProveedorEstadoId).HasColumnName("ProveedorEstadoId");
            this.Property(t => t.ProveedorId).HasColumnName("ProveedorId");
            this.Property(t => t.EstadoId).HasColumnName("EstadoId");
            this.Property(t => t.ComercialId).HasColumnName("ComercialId");
            
        }
    }
}


