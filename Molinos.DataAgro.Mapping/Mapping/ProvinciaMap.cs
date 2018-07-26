
using Molinos.DataAgro.Entities.Entities;
using System.Data.Entity.ModelConfiguration;

namespace Molinos.DataAgro.Entities
{
    public class ProvinciaMap : EntityTypeConfiguration<Provincia>
    {
        public ProvinciaMap()
        {
            // Primary Key
            this.HasKey(t => t.ProvinciaId);

            // Properties
            this.Property(t => t.Nombre)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Provincia");
            this.Property(t => t.ProvinciaId).HasColumnName("ProvinciaId");
            this.Property(t => t.Nombre).HasColumnName("Nombre");
        }
    }
}




