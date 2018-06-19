
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Molinos.DataAgro.Entities
{
    public class TipoDeNegocioMap : EntityTypeConfiguration<TipoDeNegocio>
    {
        public TipoDeNegocioMap()
        {

            // Properties
            this.Property(t => t.Descripcion)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("TipoDeNegocio");
            this.Property(t => t.TipoDeNegocioId).HasColumnName("TipoDeNegocioId");
            this.Property(t => t.Descripcion).HasColumnName("Descripcion");
        }
    }
}

