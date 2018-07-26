
using Molinos.DataAgro.Entities.Entities;
using System.Data.Entity.ModelConfiguration;

namespace Molinos.DataAgro.Entities
{
    public class RG2300Map : EntityTypeConfiguration<RG2300>
    {
        public RG2300Map()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.CUIT)
                .IsFixedLength()
                .HasMaxLength(11);

            this.Property(t => t.RazonSocial)
                .HasMaxLength(100);

            this.Property(t => t.Categoria)
                .HasMaxLength(50);

            this.Property(t => t.Situacion)
                .HasMaxLength(50);

            this.Property(t => t.CBU)
                .HasMaxLength(229);

            this.Property(t => t.Observaciones)
                .HasMaxLength(1000);

            // Table & Column Mappings
            this.ToTable("RG2300");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.CUIT).HasColumnName("CUIT");
            this.Property(t => t.RazonSocial).HasColumnName("RazonSocial");
            this.Property(t => t.Categoria).HasColumnName("Categoria");
            this.Property(t => t.Situacion).HasColumnName("Situacion");
            this.Property(t => t.CBU).HasColumnName("CBU");
            this.Property(t => t.FechaActCBU).HasColumnName("FechaActCBU");
            this.Property(t => t.FechaPubInclusion).HasColumnName("FechaPubInclusion");
            this.Property(t => t.FechaPubSuspension).HasColumnName("FechaPubSuspension");
            this.Property(t => t.FechaLevSuspension).HasColumnName("FechaLevSuspension");
            this.Property(t => t.FechaNotExclusion).HasColumnName("FechaNotExclusion");
            this.Property(t => t.FechaActRegistro).HasColumnName("FechaActRegistro");
            this.Property(t => t.Observaciones).HasColumnName("Observaciones");
            this.Property(t => t.FechaGeneracion).HasColumnName("FechaGeneracion");
        }
    }
}


