
using Molinos.DataAgro.Entities.Entities;
using System.Data.Entity.ModelConfiguration;

namespace Molinos.DataAgro.Entities
{
    public class LocalidadMap : EntityTypeConfiguration<Localidad>
    {
        public LocalidadMap()
        {
            // Primary Key
            this.HasKey(t => t.LocalidadId);

            // Properties
            this.Property(t => t.CodLocalidad)
                .HasMaxLength(10);

            this.Property(t => t.Nombre)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("Localidad");
            this.Property(t => t.LocalidadId).HasColumnName("LocalidadId");
            this.Property(t => t.CodLocalidad).HasColumnName("CodLocalidad");
            this.Property(t => t.Nombre).HasColumnName("Nombre");
            this.Property(t => t.ProvinciaId).HasColumnName("ProvinciaId");
        }
    }
}




