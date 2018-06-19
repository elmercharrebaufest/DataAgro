
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Molinos.DataAgro.Entities
{
    public class AcopioMaterialMap : EntityTypeConfiguration<AcopioMaterial>
    {
        public AcopioMaterialMap()
        {
            // Primary Key
            this.HasKey(t => t.AcopioMaterialId);

            // Properties
            // Table & Column Mappings
            this.ToTable("AcopioMaterial");
            this.Property(t => t.AcopioMaterialId).HasColumnName("AcopioMaterialId");
            this.Property(t => t.AcopioId).HasColumnName("AcopioId");
            this.Property(t => t.NroItem).HasColumnName("NroItem");
            this.Property(t => t.Toneladas).HasColumnName("Toneladas");
            this.Property(t => t.CampañaId).HasColumnName("CampañaId");
            this.Property(t => t.MaterialId).HasColumnName("MaterialId");
            




        }
    }
}


