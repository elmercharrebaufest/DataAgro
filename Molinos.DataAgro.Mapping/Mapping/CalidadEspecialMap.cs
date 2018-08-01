using Molinos.DataAgro.Entities.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Molinos.DataAgro.Entities
{
    public class CalidadEspecialMap : EntityTypeConfiguration<CalidadEspecial>
    {
        public CalidadEspecialMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Descripcion)
                .HasMaxLength(100);

            this.Property(t => t.CodigoSap)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("CalidadEspecial");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Descripcion).HasColumnName("Descripcion");
            this.Property(t => t.CodigoSap).HasColumnName("CodigoSap");
            this.Property(t => t.MaterialId).HasColumnName("MaterialId");
        }
    }
}


