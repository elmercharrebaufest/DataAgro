using Molinos.DataAgro.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Mapping
{
    public class InteresMap : EntityTypeConfiguration<Interes>
    {
        public InteresMap()
        {
            // Primary Key
            this.HasKey(t => t.InteresId);

            // Properties
            this.Property(t => t.Descripcion)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Interes");
            this.Property(t => t.InteresId).HasColumnName("InteresId");
            this.Property(t => t.Descripcion).HasColumnName("Descripcion");
        }
    }
}
