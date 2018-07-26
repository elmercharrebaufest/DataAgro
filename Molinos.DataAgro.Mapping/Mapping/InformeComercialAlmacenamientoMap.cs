using Molinos.DataAgro.Entities.Entities;
using System.Data.Entity.ModelConfiguration;

namespace Molinos.DataAgro.Entities
{
    public class InformeComercialAlmacenamientoMap : EntityTypeConfiguration<InformeComercialAlmacenamiento>
    {
        public InformeComercialAlmacenamientoMap()
        {
            // Primary Key
            this.HasKey(t => t.InformeComercialAlmacenamientoId);

            // Properties
            // Table & Column Mappings
            this.ToTable("InformeComercialAlmacenamiento");
            this.Property(t => t.InformeComercialAlmacenamientoId).HasColumnName("InformeComercialAlmacenamientoId");
            this.Property(t => t.InformeComercialId).HasColumnName("InformeComercialId");
            this.Property(t => t.Toneladas).HasColumnName("Toneladas");
            this.Property(t => t.LocalidadId).HasColumnName("LocalidadId");
            this.Property(t => t.Propia).HasColumnName("Propia");
            this.Property(t => t.Alquilada).HasColumnName("Alquilada");
        }

    }
}
