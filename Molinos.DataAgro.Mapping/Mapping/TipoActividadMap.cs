
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Molinos.DataAgro.Entities
{
    public class TipoActividadMap : EntityTypeConfiguration<TipoActividad>
    {
        public TipoActividadMap()
        {
            // Primary Key
            this.HasKey(t => t.TipoActividadId);

            // Properties
            this.Property(t => t.Descripcion)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("TipoActividad");
            this.Property(t => t.TipoActividadId).HasColumnName("TipoActividadId");
            this.Property(t => t.Descripcion).HasColumnName("Descripcion");
        }
    }
}


