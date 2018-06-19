
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Molinos.DataAgro.Entities
{
    public class PerfilMap : EntityTypeConfiguration<Perfil>
    {
        public PerfilMap()
        {
            // Primary Key
            this.HasKey(t => t.PerfilId);

            // Properties
            this.Property(t => t.Descripcion)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Perfil");
            this.Property(t => t.PerfilId).HasColumnName("PerfilId");
            this.Property(t => t.Descripcion).HasColumnName("Descripcion");
        }
    }
}

