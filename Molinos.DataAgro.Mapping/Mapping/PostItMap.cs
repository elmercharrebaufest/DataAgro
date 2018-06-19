using Molinos.DataAgro.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities
{
    public class PostItMap : EntityTypeConfiguration<PostIt>
    {
        public PostItMap()
        {
            // Primary Key
            this.HasKey(t => t.ComercialId);

            // Table & Column Mappings
            this.ToTable("PostIt");
            this.Property(t => t.ComercialId).HasColumnName("ComercialId");
            this.Property(t => t.Texto).HasColumnName("Texto");
        }
    }
}
