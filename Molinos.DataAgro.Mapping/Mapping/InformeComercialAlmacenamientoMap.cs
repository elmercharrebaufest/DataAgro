using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
