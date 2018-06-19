using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities
{
    public class InformeComercialEstadoMap : EntityTypeConfiguration<InformeComercialEstado>
    {
        public InformeComercialEstadoMap()
        {
            // Primary Key
            this.HasKey(t => t.EstadoInformeId);

            // Properties
            // Table & Column Mappings
            this.ToTable("InformeComercialEstado");
            this.Property(t => t.EstadoInformeId).HasColumnName("EstadoInformeId");
            this.Property(t => t.Descripcion).HasColumnName("Descripcion");
        }

    }
}
