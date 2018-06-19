
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Molinos.DataAgro.Entities
{
    public class FACACOPMap : EntityTypeConfiguration<FACACOP>
    {
        public FACACOPMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.CUIT)
                .IsFixedLength()
                .HasMaxLength(11);

            this.Property(t => t.ObservacionesEspeciales)
                .IsFixedLength()
                .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("FACACOP");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.CUIT).HasColumnName("CUIT");
            this.Property(t => t.Fecha1).HasColumnName("Fecha1");
            this.Property(t => t.Fecha2).HasColumnName("Fecha2");
            this.Property(t => t.ObservacionesEspeciales).HasColumnName("ObservacionesEspeciales");
        }
    }
}


