
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Molinos.DataAgro.Entities
{
    public class ActividadMap : EntityTypeConfiguration<Actividad>
    {
        public ActividadMap()
        {
            // Primary Key
            this.HasKey(t => t.ActividadId);

            // Table & Column Mappings
            this.ToTable("Actividad");
            this.Property(t => t.ActividadId).HasColumnName("ActividadId");
            this.Property(t => t.TipoActividadId).HasColumnName("TipoActividadId");
            this.Property(t => t.Detalle).HasColumnName("Detalle");
            this.Property(t => t.ProveedorId).HasColumnName("ProveedorId");
            this.Property(t => t.FechaHoraActividad).HasColumnName("FechaHoraActividad");
            this.Property(t => t.FechaHoraRecordatorio).HasColumnName("FechaHoraRecordatorio");
            this.Property(t => t.FechaHoraRecordatorio).HasColumnName("FechaHoraRecordatorio");
            this.Property(t => t.ComercialId).HasColumnName("ComercialId");
            this.Property(t => t.ContactoComercialId).HasColumnName("ContactoComercialId");

        }
    }
}


