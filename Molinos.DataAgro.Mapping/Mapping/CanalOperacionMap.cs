
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Molinos.DataAgro.Entities
{
    public class CanalOperacionMap : EntityTypeConfiguration<CanalOperacion>
    {
        public CanalOperacionMap()
        {
            // Primary Key
            this.HasKey(t => t.CanalOperacionId);

            // Properties
            this.Property(t => t.Descripcion)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("CanalOperacion");
            this.Property(t => t.CanalOperacionId).HasColumnName("CanalOperacionId");
            this.Property(t => t.Descripcion).HasColumnName("Descripcion");
            this.Property(t => t.Inhabilitado).HasColumnName("Inhabilitado");
        }
    }
}




