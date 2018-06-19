
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Molinos.DataAgro.Entities
{
    public class ProveedorComercialMap : EntityTypeConfiguration<ProveedorComercial>
    {
        public ProveedorComercialMap()
        {
            // Primary Key
            this.HasKey(t => t.ProveedorComercialId);

            // Properties
            // Table & Column Mappings
            this.ToTable("ProveedorComercial");
            this.Property(t => t.ProveedorComercialId).HasColumnName("ProveedorComercialId");
            this.Property(t => t.ProveedorId).HasColumnName("ProveedorId");
            this.Property(t => t.NroItem).HasColumnName("NroItem");
            this.Property(t => t.ComercialId).HasColumnName("ComercialId");
        }
    }
}


