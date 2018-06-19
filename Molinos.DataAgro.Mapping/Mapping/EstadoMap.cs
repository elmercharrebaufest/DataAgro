
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Molinos.DataAgro.Entities
{
    public class EstadoMap : EntityTypeConfiguration<Estado>
    {
        public EstadoMap()
        {
            // Primary Key
            this.HasKey(t => t.EstadoId);

            // Properties
            this.Property(t => t.Descripcion)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Estado");
            this.Property(t => t.EstadoId).HasColumnName("EstadoId");
            this.Property(t => t.Descripcion).HasColumnName("Descripcion");
        }
    }
}

