
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Molinos.DataAgro.Entities
{
    public class ComercialZonaMap : EntityTypeConfiguration<ComercialZona>
    {
        public ComercialZonaMap()
        {
            // Primary Key
            this.HasKey(t => t.ComercialZonaId);

            // Properties
            // Table & Column Mappings
            this.ToTable("ComercialZona");
            this.Property(t => t.ComercialZonaId).HasColumnName("ComercialZonaId");
            this.Property(t => t.ComercialId).HasColumnName("ComercialId");
            this.Property(t => t.NroItem).HasColumnName("NroItem");
            this.Property(t => t.ZonaId).HasColumnName("ZonaId");
        }
    }
}


