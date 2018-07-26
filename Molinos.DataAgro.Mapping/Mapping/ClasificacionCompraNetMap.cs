using Molinos.DataAgro.Entities.Entities;
using System.Data.Entity.ModelConfiguration;

namespace Molinos.DataAgro.Entities
{
    public class ClasificacionCompraNetMap : EntityTypeConfiguration<ClasificacionCompraNet>
    {
        public ClasificacionCompraNetMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Descripcion)
                .HasMaxLength(40);

            // Table & Column Mappings
            this.ToTable("ClasificacionCompraNet");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Descripcion).HasColumnName("Descripcion");
        }
    }
}


