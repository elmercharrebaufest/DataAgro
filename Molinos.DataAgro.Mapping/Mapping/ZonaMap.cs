
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Molinos.DataAgro.Entities
{
    public class ZonaMap : EntityTypeConfiguration<Zona>
    {
        public ZonaMap()
        {
            // Primary Key
            this.HasKey(t => t.ZonaId);

            // Properties
            this.Property(t => t.Descripcion)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("Zona");
            this.Property(t => t.ZonaId).HasColumnName("ZonaId");
            this.Property(t => t.Descripcion).HasColumnName("Descripcion");
        }
    }
}


