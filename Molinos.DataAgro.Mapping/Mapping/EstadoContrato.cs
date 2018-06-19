
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Molinos.DataAgro.Entities
{
    public class EstadoContratoMap : EntityTypeConfiguration<EstadoContrato>
    {
        public EstadoContratoMap()
        {
            // Primary Key
            this.HasKey(t => t.EstadoContratoId);

            // Properties
            this.Property(t => t.Descripcion)
                .HasMaxLength(50);
            this.Property(t => t.Orden);

            // Table & Column Mappings
            this.ToTable("EstadoContrato");
            this.Property(t => t.EstadoContratoId).HasColumnName("EstadoContratoId");
            this.Property(t => t.Descripcion).HasColumnName("Descripcion");
            this.Property(t => t.Orden).HasColumnName("Orden");

        }
    }
}

