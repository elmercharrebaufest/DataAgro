using Molinos.DataAgro.Entities.Entities;
using System.Data.Entity.ModelConfiguration;

namespace Molinos.DataAgro.Entities
{
    public class BoletoCompraNetMap : EntityTypeConfiguration<BoletoCompraNet>
    {
        public BoletoCompraNetMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Descripcion)
                .HasMaxLength(40);

            // Table & Column Mappings
            this.ToTable("BoletoCompraNet");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Descripcion).HasColumnName("Descripcion");
        }
    }
}


