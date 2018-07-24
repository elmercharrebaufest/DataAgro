
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Molinos.DataAgro.Entities
{
    public class ProveedorMap : EntityTypeConfiguration<Proveedor>
    {
        public ProveedorMap()
        {
            // Primary Key
            this.HasKey(t => t.ProveedorId);

            // Properties
            this.Property(t => t.CUIT)
                .HasMaxLength(20);

            this.Property(t => t.RazonSocial)
                .HasMaxLength(100);

            this.Property(t => t.NombreReferente)
                .HasMaxLength(100);

            this.Property(t => t.Intermediario)
                .HasMaxLength(255);

            this.Property(t => t.Direccion)
                .HasMaxLength(100);

            this.Property(t => t.CodigoPostal)
                .HasMaxLength(10);

            this.Property(t => t.GrupoCompras)
                .HasMaxLength(255);
            
            this.Property(t => t.Email1)
                .HasMaxLength(255);

            this.Property(t => t.Email2)
                .HasMaxLength(255);

            this.Property(t => t.Email3)
                .HasMaxLength(255);

            this.Property(t => t.Email4)
                .HasMaxLength(255);

            this.Property(t => t.Telefono1)
                .HasMaxLength(255);

            this.Property(t => t.Telefono2)
                .HasMaxLength(255);

            this.Property(t => t.Telefono3)
                .HasMaxLength(255);

            this.Property(t => t.Telefono4)
                .HasMaxLength(255);

            this.Property(t => t.RiesgoComercialSap)
               .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Proveedor");
            this.Property(t => t.ProveedorId).HasColumnName("ProveedorId");
            this.Property(t => t.CUIT).HasColumnName("CUIT");
            this.Property(t => t.EstadoId).HasColumnName("EstadoId");
            this.Property(t => t.RazonSocial).HasColumnName("RazonSocial");
            this.Property(t => t.SegmentacionId).HasColumnName("SegmentacionId");
            this.Property(t => t.NombreReferente).HasColumnName("NombreReferente");
            this.Property(t => t.Calificacion).HasColumnName("Calificacion");
            this.Property(t => t.Intermediario).HasColumnName("Intermediario");
            this.Property(t => t.Observaciones).HasColumnName("Observaciones");
            this.Property(t => t.Direccion).HasColumnName("Direccion");
            this.Property(t => t.LocalidadId).HasColumnName("LocalidadId");
            this.Property(t => t.ProvinciaId).HasColumnName("ProvinciaId");
            this.Property(t => t.CodigoPostal).HasColumnName("CodigoPostal");
            this.Property(t => t.AreaInfluenciaId).HasColumnName("AreaInfluenciaId");
            this.Property(t => t.AlmacVolAnualTotal).HasColumnName("AlmacVolAnualTotal");
            this.Property(t => t.AlmacHabilitadoSojaSust).HasColumnName("AlmacHabilitadoSojaSust");
            this.Property(t => t.AlmacTonsMaxSojaSust).HasColumnName("AlmacTonsMaxSojaSust");
            this.Property(t => t.AlmacHectSojaSust).HasColumnName("AlmacHectSojaSust");
            this.Property(t => t.ClienteMOA).HasColumnName("ClienteMOA");
            this.Property(t => t.GrupoCompras).HasColumnName("GrupoCompras");
            this.Property(t => t.Email1).HasColumnName("Email1");
            this.Property(t => t.Email2).HasColumnName("Email2");
            this.Property(t => t.Email3).HasColumnName("Email3");
            this.Property(t => t.Email4).HasColumnName("Email4");
            this.Property(t => t.Telefono1).HasColumnName("Telefono1");
            this.Property(t => t.TipoTelefono1Id).HasColumnName("TipoTelefono1Id");
            this.Property(t => t.Telefono2).HasColumnName("Telefono2");
            this.Property(t => t.TipoTelefono2Id).HasColumnName("TipoTelefono2Id");
            this.Property(t => t.Telefono3).HasColumnName("Telefono3");
            this.Property(t => t.TipoTelefono3Id).HasColumnName("TipoTelefono3Id");
            this.Property(t => t.Telefono4).HasColumnName("Telefono4");
            this.Property(t => t.TipoTelefono4Id).HasColumnName("TipoTelefono4Id");
            this.Property(t => t.FechaUltimoContacto).HasColumnName("FechaUltimoContacto");
            this.Property(t => t.RiesgoComercialSap).HasColumnName("RiesgoComercialSap");
            this.Property(t => t.FechaAlta).HasColumnName("FechaAlta");
            this.Property(t => t.ProvinciaCompraNetId).HasColumnName("ProvinciaCompraNetId");
            this.Property(t => t.LocalidadCompraNetId).HasColumnName("LocalidadCompraNetId");
            this.Property(t => t.ClasificacionCompraNetId).HasColumnName("ClasificacionCompraNetId");
            this.Property(t => t.BoletoCompraNetId).HasColumnName("BoletoCompraNetId");
            this.Property(t => t.BolsaCompraNetId).HasColumnName("BolsaCompraNetId");
        }
    }
}


