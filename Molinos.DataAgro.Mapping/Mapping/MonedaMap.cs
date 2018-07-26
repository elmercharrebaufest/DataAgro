
using Molinos.DataAgro.Entities.Entities;
using System.Data.Entity.ModelConfiguration;

namespace Molinos.DataAgro.Entities
{
    public class MonedaMap : EntityTypeConfiguration<Moneda>
    {
        public MonedaMap()
        {
            // Primary Key
            this.HasKey(t => t.MonedaId);

            // Properties
           

            this.Property(t => t.Descripcion)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Monedas");
            this.Property(t => t.MonedaId).HasColumnName("MonedaId");
            this.Property(t => t.Descripcion).HasColumnName("Descripcion");
         

           
        }
    }
}




