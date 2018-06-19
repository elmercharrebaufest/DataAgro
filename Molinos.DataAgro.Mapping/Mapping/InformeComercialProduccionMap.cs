
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities
{
    public class InformeComercialProduccionMap : EntityTypeConfiguration<InformeComercialProduccion>
    {
        public InformeComercialProduccionMap()
        {
            // Primary Key
            this.HasKey(t => t.InformeComerciaProduccionId);

            // Properties
            // Table & Column Mappings
            this.ToTable("InformeComercialProduccion");
            this.Property(t => t.InformeComerciaProduccionId).HasColumnName("InformeComerciaProduccionId");
            this.Property(t => t.InformeComercialId).HasColumnName("InformeComercialId");
            this.Property(t => t.MaterialId).HasColumnName("MaterialId");
            this.Property(t => t.Hectareas).HasColumnName("Hectareas");
            this.Property(t => t.Toneladas).HasColumnName("Toneladas");
            this.Property(t => t.LocalidadId).HasColumnName("LocalidadId");
            this.Property(t => t.Propio).HasColumnName("Propio");
            this.Property(t => t.Alquilado).HasColumnName("Alquilado");
            this.Property(t => t.RtaOkSap).HasColumnName("RtaOkSap");
            this.Property(t => t.MensajeSap).HasColumnName("MensajeSap");
            

        }
    }
}
