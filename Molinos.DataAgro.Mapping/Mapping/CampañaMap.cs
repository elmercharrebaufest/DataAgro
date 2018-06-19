
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Molinos.DataAgro.Entities
{
    public class CampañaMap : EntityTypeConfiguration<Campaña>
    {
        public CampañaMap()
        {
            // Primary Key
            this.HasKey(t => t.CampañaId);

            // Properties
            this.Property(t => t.Descripcion)
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("Campaña");
            this.Property(t => t.CampañaId).HasColumnName("CampañaId");
            this.Property(t => t.Descripcion).HasColumnName("Descripcion");
        }
    }
}


