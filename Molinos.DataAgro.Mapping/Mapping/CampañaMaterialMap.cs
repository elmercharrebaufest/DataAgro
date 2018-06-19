
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Molinos.DataAgro.Entities
{
    public class CampañaMaterialMap : EntityTypeConfiguration<CampañaMaterial>
    {
        public CampañaMaterialMap()
        {
            // Primary Key
            this.HasKey(t => t.CampañaMaterialId);

            // Properties
            // Table & Column Mappings
            this.ToTable("CampañaMaterial");
            this.Property(t => t.CampañaMaterialId).HasColumnName("CampañaMaterialId");
            this.Property(t => t.CampañaId).HasColumnName("CampañaId");
            this.Property(t => t.NroItem).HasColumnName("NroItem");
            this.Property(t => t.ProveedorId).HasColumnName("ProveedorId");
            this.Property(t => t.MaterialId).HasColumnName("MaterialId");
            this.Property(t => t.ToneladasCompradas).HasColumnName("ToneladasCompradas");
        }
    }
}


