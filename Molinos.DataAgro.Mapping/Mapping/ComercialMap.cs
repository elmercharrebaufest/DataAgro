
using Molinos.DataAgro.Entities.Entities;
using System.Data.Entity.ModelConfiguration;

namespace Molinos.DataAgro.Entities
{
    public class ComercialMap : EntityTypeConfiguration<Comercial>
    {
        public ComercialMap()
        {
            // Primary Key
            this.HasKey(t => t.ComercialId);

            // Properties
            this.Property(t => t.Apellido)
                .HasMaxLength(50);

            this.Property(t => t.Nombres)
                .HasMaxLength(50);

            this.Property(t => t.IdActiveDirectory)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("Comercial");
            this.Property(t => t.ComercialId).HasColumnName("ComercialId");
            this.Property(t => t.Apellido).HasColumnName("Apellido");
            this.Property(t => t.Nombres).HasColumnName("Nombres");
            this.Property(t => t.PerfilId).HasColumnName("PerfilId");
            this.Property(t => t.EmpleadorACargo).HasColumnName("EmpleadorACargo");
            this.Property(t => t.IdActiveDirectory).HasColumnName("IdActiveDirectory");
            this.Property(t => t.GrupoDeCompras).HasColumnName("GrupoDeCompras");
            this.Property(t => t.Administrador).HasColumnName("Administrador");
        }
    }
}

