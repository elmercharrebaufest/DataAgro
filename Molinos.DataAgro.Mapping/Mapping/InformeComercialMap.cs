using Molinos.DataAgro.Entities.Entities;
using System.Data.Entity.ModelConfiguration;

namespace Molinos.DataAgro.Entities
{
    public class InformeComercialMap : EntityTypeConfiguration<InformeComercial>
    {
        public InformeComercialMap()
        {
            // Primary Key
            this.HasKey(t => t.InformeComercialId);

            // Properties
            // Table & Column Mappings
            this.ToTable("InformeComercial");
            this.Property(t => t.InformeComercialId).HasColumnName("InformeComercialId");
            this.Property(t => t.ProveedorId).HasColumnName("ProveedorId");
            this.Property(t => t.FechaAlta).HasColumnName("FechaAlta");
            this.Property(t => t.ComercialId).HasColumnName("ComercialId");
            this.Property(t => t.EmplRelDep).HasColumnName("EmplRelDep");
            this.Property(t => t.EmplRelDepCant).HasColumnName("EmplRelDepCant");
            this.Property(t => t.Rodados).HasColumnName("Rodados");
            this.Property(t => t.RodadosOtros).HasColumnName("RodadosOtros");
            this.Property(t => t.Chacra).HasColumnName("Chacra");
            this.Property(t => t.ChacraOtros).HasColumnName("ChacraOtros");
            this.Property(t => t.AntigActividad).HasColumnName("AntigActividad");
            this.Property(t => t.ActuacionProd).HasColumnName("ActuacionProd");
            this.Property(t => t.ClienteAnt).HasColumnName("ClienteAnt");
            this.Property(t => t.Comentarios).HasColumnName("Comentarios");
            this.Property(t => t.RespuestaSap).HasColumnName("RespuestaSap");
            this.Property(t => t.CampañaId).HasColumnName("CampañaId");
            this.Property(t => t.EstadoId).HasColumnName("EstadoId");
            this.Property(t => t.DomicilioReal).HasColumnName("DomicilioReal");
            

        }

    }
}
