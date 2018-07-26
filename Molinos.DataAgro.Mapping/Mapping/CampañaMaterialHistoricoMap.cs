
using Molinos.DataAgro.Entities.Entities;
using System.Data.Entity.ModelConfiguration;

namespace Molinos.DataAgro.Entities
{
    public class CampañaMaterialHistoricoMap : EntityTypeConfiguration<CampañaMaterialHistorico>
    {

        

        public CampañaMaterialHistoricoMap()
        {
            // Primary Key
            this.HasKey(t => t.CampañaMaterialHistoricoId);


            this.ToTable("CampañaMaterialHistorico");
            this.Property(t => t.CampañaMaterialHistoricoId).HasColumnName("CampañaMaterialHistoricoId");
            this.Property(t => t.CampañaId).HasColumnName("CampañaId");
            this.Property(t => t.MaterialId).HasColumnName("MaterialId");
            this.Property(t => t.Fecha).HasColumnName("Fecha");
        }
    }
}
