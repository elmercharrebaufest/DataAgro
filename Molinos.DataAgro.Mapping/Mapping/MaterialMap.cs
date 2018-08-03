
//using Molinos.DataAgro.Entities.Entities;
//using System.Data.Entity.ModelConfiguration;

//namespace Molinos.DataAgro.Entities
//{
//    public class MaterialMap : EntityTypeConfiguration<Material>
//    {
//        public MaterialMap()
//        {
//            // Primary Key
//            this.HasKey(t => t.MaterialId);

//            // Properties
//            this.Property(t => t.Codigo)
//                .HasMaxLength(20);

//            this.Property(t => t.Descripcion)
//                .HasMaxLength(50);

//            // Table & Column Mappings
//            this.ToTable("Material");
//            this.Property(t => t.MaterialId).HasColumnName("MaterialId");
//            this.Property(t => t.Codigo).HasColumnName("Codigo");
//            this.Property(t => t.Descripcion).HasColumnName("Descripcion");
//            this.Property(t => t.CampañaId).HasColumnName("CampañaIdActual");
//        }
//    }
//}




