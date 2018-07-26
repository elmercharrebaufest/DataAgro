
using Molinos.DataAgro.Entities.Entities;
using System.Data.Entity.ModelConfiguration;

namespace Molinos.DataAgro.Entities
{
    public class AreaInfluenciaMap : EntityTypeConfiguration<AreaInfluencia>
    {
        public AreaInfluenciaMap()
        {
            // Primary Key
            this.HasKey(t => t.AreaInfluenciaId);

            // Properties
            this.Property(t => t.Descripcion)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("AreaInfluencia");
            this.Property(t => t.AreaInfluenciaId).HasColumnName("AreaInfluenciaId");
            this.Property(t => t.Descripcion).HasColumnName("Descripcion");
        }
    }
}




