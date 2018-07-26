
using Molinos.DataAgro.Entities.Entities;
using System.Data.Entity.ModelConfiguration;

namespace Molinos.DataAgro.Entities
{
    public class CampoMap : EntityTypeConfiguration<Campo>
    {
        public CampoMap()
        {
            // Primary Key
            this.HasKey(t => t.CampoId);

            // Properties
            // Table & Column Mappings
            this.ToTable("Campo");
            this.Property(t => t.CampoId).HasColumnName("CampoId");
            this.Property(t => t.NroItem).HasColumnName("NroItem");
            this.Property(t => t.ProveedorId).HasColumnName("ProveedorId");
            this.Property(t => t.LocalidadId).HasColumnName("LocalidadId");
            this.Property(t => t.Coordenadas).HasColumnName("Coordenadas");
            this.Property(t => t.KMZfile).HasColumnName("KMZfile");
            this.Property(t => t.KMZnombre).HasColumnName("KMZnombre");
            this.Property(t => t.ArrendaPropia).HasColumnName("ArrendaPropia");
            this.Property(t => t.HabilitadoSojaSustentable).HasColumnName("HabilitadoSojaSustentable");
        }
    }
}


