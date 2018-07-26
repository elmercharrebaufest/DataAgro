
using Molinos.DataAgro.Entities.Entities;
using System.Data.Entity.ModelConfiguration;

namespace Molinos.DataAgro.Entities
{
    public class ContratoMap : EntityTypeConfiguration<Contrato>
    {
        public ContratoMap()
        {
            // Primary Key
            this.ToTable("Contrato");
            this.HasKey(x => x.ContratoId);
            
            Property(x => x.ContratoId).HasColumnName(@"ContratoId").HasColumnType("int").IsRequired().HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            Property(x => x.MaterialId).HasColumnName(@"MaterialId").HasColumnType("int").IsRequired();
            Property(x => x.TipoNegocioId).HasColumnName(@"TipoNegocioId").HasColumnType("int").IsRequired();
            Property(x => x.Cantidad).HasColumnName(@"Cantidad").HasColumnType("float").IsRequired();
            Property(x => x.Precio).HasColumnName(@"Precio").HasColumnType("decimal").IsRequired().HasPrecision(11, 2);
            Property(x => x.FechaEntrega).HasColumnName(@"FechaEntrega").HasColumnType("datetime").IsRequired();
            Property(x => x.CampanaId).HasColumnName(@"CampañaId").HasColumnType("int").IsRequired();
            Property(x => x.FechaDesde).HasColumnName(@"FechaDesde").HasColumnType("datetime").IsRequired();
            Property(x => x.FechaHasta).HasColumnName(@"FechaHasta").HasColumnType("datetime").IsRequired();
            Property(x => x.ProveedorId).HasColumnName(@"ProveedorId").HasColumnType("int").IsRequired();
            Property(x => x.MonedaId).HasColumnName(@"MonedaId").HasColumnType("char").IsRequired().IsFixedLength().IsUnicode(false).HasMaxLength(5);
            Property(x => x.Fecha).HasColumnName(@"Fecha").HasColumnType("datetime").IsRequired();
            Property(x => x.GrupoCompra).HasColumnName(@"GrupoCompra").HasColumnType("int").IsRequired();
            Property(x => x.ComercialId).HasColumnName(@"ComercialId").HasColumnType("int").IsOptional();
            Property(x => x.ProvinciaId).HasColumnName(@"ProvinciaId").HasColumnType("int").IsOptional();
            Property(x => x.LocalidadId).HasColumnName(@"LocalidadId").HasColumnType("int").IsOptional();
            Property(x => x.Base).HasColumnName(@"Base").HasColumnType("bit").IsOptional();
            Property(x => x.ImporteSustentable).HasColumnName(@"Importe_Sustentable").HasColumnType("decimal").IsOptional().HasPrecision(11, 2);
            Property(x => x.MonedaIdSustentable).HasColumnName(@"MonedaId_Sustentable").HasColumnType("char").IsOptional().IsFixedLength().IsUnicode(false).HasMaxLength(5);
            Property(x => x.FechaDolarizado).HasColumnName(@"Fecha_Dolarizado").HasColumnType("datetime").IsOptional();
            Property(x => x.DiasPesificado).HasColumnName(@"Dias_Pesificado").HasColumnType("int").IsOptional();
            Property(x => x.NoInformaSio).HasColumnName(@"NoInformaSIO").HasColumnType("bit").IsOptional();
            Property(x => x.TrigoEspecial).HasColumnName(@"TrigoEspecial").HasColumnType("bit").IsOptional();
            Property(x => x.Estado).HasColumnName(@"Estado").HasColumnType("int").IsRequired();
            Property(x => x.UsuarioId).HasColumnName(@"UsuarioId").HasColumnType("varchar").IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.ContratoSAP).HasColumnName(@"ContratoSAP").HasColumnType("int").IsOptional();
            Property(x => x.Ampliaciones).HasColumnName(@"Ampliaciones").HasColumnType("float").IsOptional();
            Property(x => x.Observacion).HasColumnName(@"Observacion").HasColumnType("varchar").IsOptional().IsUnicode(false);
            Property(x => x.DestinoId).HasColumnName(@"DestinoId").HasColumnType("int").IsRequired();
            Property(x => x.CantidadCamiones).HasColumnName(@"CantidadCamiones").HasColumnType("int").IsOptional();
            Property(x => x.Consignatario).HasColumnName(@"Consignatario").HasColumnType("bit").IsOptional();
            Property(x => x.PlanCanje).HasColumnName(@"PlanCanje").HasColumnType("bit").IsOptional();
            Property(x => x.CondicionFijacionId).HasColumnName(@"CondicionFijacionId").HasColumnType("int").IsOptional();
            Property(x => x.CD).HasColumnName(@"CD").HasColumnType("bit").IsOptional();
            Property(x => x.Warrant).HasColumnName(@"Warrant").HasColumnType("bit").IsOptional();
            Property(x => x.PagoDirectoVendedor).HasColumnName(@"PagoDirectoVendedor").HasColumnType("bit").IsOptional();
            Property(x => x.StandardDeCalidadId).HasColumnName(@"StandardDeCalidadId").HasColumnType("int").IsOptional();
            Property(x => x.CalidadEspecial).HasColumnName(@"CalidadEspecial").HasColumnType("int").IsOptional();
            Property(x => x.ValorCalidadEspecial).HasColumnName(@"ValorCalidadEspecial").HasColumnType("int").IsOptional();
            Property(x => x.EstablecimientoPropio).HasColumnName(@"EstablecimientoPropio").HasColumnType("bit").IsOptional();
            Property(x => x.ClasificacionId).HasColumnName(@"ClasificacionId").HasColumnType("int").IsOptional();
            Property(x => x.BoletoId).HasColumnName(@"BoletoId").HasColumnType("int").IsOptional();
            Property(x => x.DesdeFijacion).HasColumnName(@"DesdeFijacion").HasColumnType("datetime").IsOptional();
            Property(x => x.HastaFijacion).HasColumnName(@"HastaFijacion").HasColumnType("datetime").IsOptional();
        }
    }
}




