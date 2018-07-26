
using Molinos.DataAgro.Entities.Entities;
using System.Data.Entity.ModelConfiguration;

namespace Molinos.DataAgro.Entities
{
    public class ObjetivoMap : EntityTypeConfiguration<Objetivo>
    {
        public ObjetivoMap()
        {
            // Primary Key
            this.HasKey(t => t.ObjetivoId);

            // Properties
            // Table & Column Mappings
            this.ToTable("Objetivo");
            this.Property(t => t.ObjetivoId).HasColumnName("ObjetivoId");
            this.Property(t => t.CampañaId).HasColumnName("CampañaId");
            this.Property(t => t.NroItem).HasColumnName("NroItem");
            this.Property(t => t.ProveedorId).HasColumnName("ProveedorId");
            this.Property(t => t.MaterialId).HasColumnName("MaterialId");
            this.Property(t => t.ToneladasObjetivos).HasColumnName("ToneladasObjetivos");
        }
    }
}


