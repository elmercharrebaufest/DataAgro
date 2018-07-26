using Molinos.DataAgro.Entities.Entities;
using System.Data.Entity.ModelConfiguration;

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
