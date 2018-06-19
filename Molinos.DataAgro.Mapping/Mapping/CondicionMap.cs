
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Molinos.DataAgro.Entities
{
    public class CondicionMap : EntityTypeConfiguration<Condicion>
    {
        public CondicionMap()
        {
            // Primary Key
            this.HasKey(t => t.CondicionId);

            // Properties
            this.Property(t => t.Descripcion)
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("Condicion");
            this.Property(t => t.CondicionId).HasColumnName("CondicionId");
            this.Property(t => t.Descripcion).HasColumnName("Descripcion");
            this.Property(t => t.Inhabilitado).HasColumnName("Inhabilitado");
        }
    }
}




