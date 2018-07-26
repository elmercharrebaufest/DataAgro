using Molinos.DataAgro.Entities.Entities;
using System.Data.Entity.ModelConfiguration;

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
