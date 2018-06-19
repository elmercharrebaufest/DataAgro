
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Molinos.DataAgro.Entities
{
    public class CampañaMaterialPorMesMap : EntityTypeConfiguration<CampañaMaterialPorMes>
    {
        public CampañaMaterialPorMesMap()
        {
            // Primary Key
            this.HasKey(t => t.CampañaMaterialPorMesId);

            // Properties
            // Table & Column Mappings
            this.ToTable("CampañaMaterialPorMes");
            this.Property(t => t.CampañaMaterialPorMesId).HasColumnName("CampañaMaterialPorMesId");
            this.Property(t => t.NroItem).HasColumnName("NroItem");
            this.Property(t => t.Año).HasColumnName("Año");
            this.Property(t => t.Toneladas).HasColumnName("Toneladas");
            this.Property(t => t.CampañaMaterialId).HasColumnName("CampañaMaterialId");
            this.Property(t => t.ComercialId).HasColumnName("ComercialId");

        }
    }
}


