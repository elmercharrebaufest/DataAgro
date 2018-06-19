
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Molinos.DataAgro.Entities
{
    public class ProveedorCondicionMap : EntityTypeConfiguration<ProveedorCondicion>
    {
        public ProveedorCondicionMap()
        {
            // Primary Key
            this.HasKey(t => t.ContactoCondicionId);

            // Properties
            // Table & Column Mappings
            this.ToTable("ProveedorCondicion");
            this.Property(t => t.ContactoCondicionId).HasColumnName("ContactoCondicionId");
            this.Property(t => t.ProveedorId).HasColumnName("ProveedorId");
            this.Property(t => t.NroItem).HasColumnName("NroItem");
            this.Property(t => t.CondicionId).HasColumnName("CondicionId");
        }
    }
}


