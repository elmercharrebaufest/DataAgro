
using Molinos.DataAgro.Entities.Entities;
using System.Data.Entity.ModelConfiguration;

namespace Molinos.DataAgro.Entities
{
    public class SegmentacionMap : EntityTypeConfiguration<Segmentacion>
    {
        public SegmentacionMap()
        {
            // Primary Key
            this.HasKey(t => t.SegmentacionId);

            // Properties
            this.Property(t => t.Descripcion)
                .HasMaxLength(50);

            this.Property(t => t.Grupo)
                .HasMaxLength(30);

            // Table & Column Mappings
            this.ToTable("Segmentacion");
            this.Property(t => t.SegmentacionId).HasColumnName("SegmentacionId");
            this.Property(t => t.Descripcion).HasColumnName("Descripcion");
            this.Property(t => t.Grupo).HasColumnName("Grupo");
        }
    }
}


