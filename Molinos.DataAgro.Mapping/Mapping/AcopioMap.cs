
using Molinos.DataAgro.Entities.Entities;
using System.Data.Entity.ModelConfiguration;

namespace Molinos.DataAgro.Entities
{
    public class AcopioMap : EntityTypeConfiguration<Acopio>
    {
        public AcopioMap()
        {
            // Primary Key
            this.HasKey(t => t.AcopioId);

            // Properties
            // Table & Column Mappings
            this.ToTable("Acopio");
            this.Property(t => t.AcopioId).HasColumnName("AcopioId");
            this.Property(t => t.NroItem).HasColumnName("NroItem");
            this.Property(t => t.ProveedorId).HasColumnName("ProveedorId");
            this.Property(t => t.LocalidadId).HasColumnName("LocalidadId");
            this.Property(t => t.Coordenadas).HasColumnName("Coordenadas");
            this.Property(t => t.KMZfile).HasColumnName("KMZfile");
            this.Property(t => t.KMZnombre).HasColumnName("KMZnombre");
        }
    }
}


