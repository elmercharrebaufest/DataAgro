
using Molinos.DataAgro.Entities.Entities;
using System.Data.Entity.ModelConfiguration;

namespace Molinos.DataAgro.Entities
{
    public class DestinatarioMap : EntityTypeConfiguration<Destinatario>
    {
        public DestinatarioMap()
        {
            // Primary Key
            this.HasKey(t => t.DestinatarioId);

            // Properties
            this.Property(t => t.Descripcion)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Destinatario");
            this.Property(t => t.DestinatarioId).HasColumnName("DestinatarioId");
            this.Property(t => t.Descripcion).HasColumnName("Descripcion");
            this.Property(t => t.Inhabilitado).HasColumnName("Inhabilitado");
        }
    }
}




