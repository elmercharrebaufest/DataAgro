
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Molinos.DataAgro.Entities
{
    public class CampoMaterialMap : EntityTypeConfiguration<CampoMaterial>
    {
        public CampoMaterialMap()
        {
            // Primary Key
            this.HasKey(t => t.CampoMaterialid);

            // Properties
            // Table & Column Mappings
            this.ToTable("CampoMaterial");
            this.Property(t => t.CampoMaterialid).HasColumnName("CampoMaterialid");
            this.Property(t => t.CampoId).HasColumnName("CampoId");
            this.Property(t => t.NroItem).HasColumnName("NroItem");
            this.Property(t => t.MaterialId).HasColumnName("MaterialId");
            this.Property(t => t.Hectareas).HasColumnName("Hectareas");
            this.Property(t => t.Toneladas).HasColumnName("Toneladas");
            this.Property(t => t.CampañaId).HasColumnName("CampañaId");
        }
    }
}


