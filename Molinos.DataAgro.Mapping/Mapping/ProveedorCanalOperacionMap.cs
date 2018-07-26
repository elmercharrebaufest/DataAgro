
using Molinos.DataAgro.Entities.Entities;
using System.Data.Entity.ModelConfiguration;

namespace Molinos.DataAgro.Entities
{
    public class ProveedorCanalOperacionMap : EntityTypeConfiguration<ProveedorCanalOperacion>
    {
        public ProveedorCanalOperacionMap()
        {
            // Primary Key
            this.HasKey(t => t.ContactoCanalOperacionId);

            // Properties
            this.Property(t => t.NroItem)
                .IsFixedLength()
                .HasMaxLength(10);

            // Table & Column Mappings
            this.ToTable("ProveedorCanalOperacion");
            this.Property(t => t.ContactoCanalOperacionId).HasColumnName("ContactoCanalOperacionId");
            this.Property(t => t.ProveedorId).HasColumnName("ProveedorId");
            this.Property(t => t.NroItem).HasColumnName("NroItem");
            this.Property(t => t.CanalOperacionId).HasColumnName("CanalOperacionId");
        }
    }
}


