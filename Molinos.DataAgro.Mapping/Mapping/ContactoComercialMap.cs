
using Molinos.DataAgro.Entities.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Molinos.DataAgro.Entities
{
    public class ContactoComercialMap : EntityTypeConfiguration<ContactoComercial>
    {
        public ContactoComercialMap()
        {
            // Primary Key
            this.HasKey(t => t.ContactoComercialId);

            // Properties
            this.Property(t => t.Apellido)
                .HasMaxLength(50);

            this.Property(t => t.Nombres)
                .HasMaxLength(50);

            this.Property(t => t.Puesto)
                .HasMaxLength(50);

            this.Property(t => t.Telefono1)
                .HasMaxLength(100);

            this.Property(t => t.Telefono2)
                .HasMaxLength(100);

            this.Property(t => t.Telefono3)
                .HasMaxLength(100);

            this.Property(t => t.Email1)
                .HasMaxLength(100);

            this.Property(t => t.Email2)
                .HasMaxLength(100);

            this.Property(t => t.Email3)
                .HasMaxLength(100);

            this.Property(t => t.EsPrincipal);

            this.Property(t => t.Cargo)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("ContactoComercial");
            this.Property(t => t.ContactoComercialId).HasColumnName("ContactoComercialId");
            this.Property(t => t.ProveedorId).HasColumnName("ProveedorId");
            this.Property(t => t.Apellido).HasColumnName("Apellido");
            this.Property(t => t.Nombres).HasColumnName("Nombres");
            this.Property(t => t.Puesto).HasColumnName("Puesto");
            this.Property(t => t.Telefono1).HasColumnName("Telefono1");
            this.Property(t => t.TipoTelefono1Id).HasColumnName("TipoTelefono1Id");
            this.Property(t => t.Telefono2).HasColumnName("Telefono2");
            this.Property(t => t.TipoTelefono2Id).HasColumnName("TipoTelefono2Id");
            this.Property(t => t.Telefono3).HasColumnName("Telefono3");
            this.Property(t => t.TipoTelefono3Id).HasColumnName("TipoTelefono3Id");
            this.Property(t => t.Email1).HasColumnName("Email1");
            this.Property(t => t.Email2).HasColumnName("Email2");
            this.Property(t => t.Email3).HasColumnName("Email3");
            this.Property(t => t.FechaNacimiento).HasColumnName("FechaNacimiento");
            this.Property(t => t.CategoriaId).HasColumnName("CategoriaId");
            this.Property(t => t.OtrosIntereses).HasColumnName("OtrosIntereses");
            this.Property(t => t.EsPrincipal).HasColumnName("EsPrincipal");
            this.Property(t => t.Cargo).HasColumnName("Cargo");
        }
    }
}


