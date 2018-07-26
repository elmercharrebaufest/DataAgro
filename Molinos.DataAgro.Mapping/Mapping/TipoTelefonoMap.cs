
using Molinos.DataAgro.Entities.Entities;
using System.Data.Entity.ModelConfiguration;

namespace Molinos.DataAgro.Entities
{
    public class TipoTelefonoMap : EntityTypeConfiguration<TipoTelefono>
    {
        public TipoTelefonoMap()
        {
            // Primary Key
            this.HasKey(t => t.TipoTelefonoId);

            // Properties
            this.Property(t => t.Descripcion)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("TipoTelefono");
            this.Property(t => t.TipoTelefonoId).HasColumnName("TipoTelefonoId");
            this.Property(t => t.Descripcion).HasColumnName("Descripcion");
        }
    }
}


