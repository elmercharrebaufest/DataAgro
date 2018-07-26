
using Molinos.DataAgro.Entities.Entities;
using System.Data.Entity.ModelConfiguration;

namespace Molinos.DataAgro.Entities
{
    public class AcopioCampañaMap : EntityTypeConfiguration<AcopioCampaña>
    {
        public AcopioCampañaMap()
        {
            // Primary Key
            this.HasKey(t => t.AcopioCampañaId);

            // Properties
            // Table & Column Mappings
            this.ToTable("AcopioCampaña");
            this.Property(t => t.AcopioCampañaId).HasColumnName("AcopioCampañaId");
            this.Property(t => t.AcopioId).HasColumnName("AcopioId");
            this.Property(t => t.NroItem).HasColumnName("NroItem");
            this.Property(t => t.Toneladas).HasColumnName("Toneladas");
            this.Property(t => t.CampañaId).HasColumnName("CampañaId");
            this.Property(t => t.HasArrendadas).HasColumnName("HasArrendadas");





        }
    }
}


