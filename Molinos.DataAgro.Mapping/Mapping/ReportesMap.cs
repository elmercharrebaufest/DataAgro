
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Molinos.DataAgro.Entities
{
    public class ReportesMap : EntityTypeConfiguration<Reportes>
    {
        public ReportesMap()
        {
            // Primary Key
            this.HasKey(t => t.Identificador);

            // Properties
            this.Property(t => t.Identificador)
                .HasMaxLength(32);

            this.Property(t => t.FileName)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("Reportes");
            this.Property(t => t.Identificador).HasColumnName("Identificador");
            this.Property(t => t.Contenido).HasColumnName("Contenido");
            this.Property(t => t.FileName).HasColumnName("FileName");
        }
    }
}
