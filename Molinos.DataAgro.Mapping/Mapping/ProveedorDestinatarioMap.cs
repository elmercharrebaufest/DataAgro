
using Molinos.DataAgro.Entities.Entities;
using System.Data.Entity.ModelConfiguration;

namespace Molinos.DataAgro.Entities
{
    public class ProveedorDestinatarioMap : EntityTypeConfiguration<ProveedorDestinatario>
    {
        public ProveedorDestinatarioMap()
        {
            // Primary Key
            this.HasKey(t => t.ContactoDestinatarioId);

            // Properties
            // Table & Column Mappings
            this.ToTable("ProveedorDestinatario");
            this.Property(t => t.ContactoDestinatarioId).HasColumnName("ContactoDestinatarioId");
            this.Property(t => t.ProveedorId).HasColumnName("ProveedorId");
            this.Property(t => t.NroItem).HasColumnName("NroItem");
            this.Property(t => t.DestinatarioId).HasColumnName("DestinatarioId");
        }
    }
}


