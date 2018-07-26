
using Molinos.DataAgro.Entities.Entities;
using System.Data.Entity.ModelConfiguration;

namespace Molinos.DataAgro.Entities
{
    public class TipoNegocioMap : EntityTypeConfiguration<TipoNegocio>
    {
        public TipoNegocioMap()
        {
            // Primary Key
            this.HasKey(t => t.TipoNegocioId);

            // Properties
            this.Property(t => t.Descripcion)
                .HasMaxLength(20);

            this.Property(t => t.Descripcion)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("TipoNegocio");
            this.Property(t => t.TipoNegocioId).HasColumnName("TipoNegocioId");
            this.Property(t => t.Descripcion).HasColumnName("Descripcion");
         

           
        }
    }
}




