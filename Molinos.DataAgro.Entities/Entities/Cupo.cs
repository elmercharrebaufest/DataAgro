using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class Cupo : ICloneable
    {
        [Key]
        public int Id { get; set; }
        public int ProveedorId { get; set; }
        public int CentroId { get; set; }
        public int MaterialId { get; set; }
        public DateTime FechaIngreso { get; set; }
        public string CupoSap { get; set; }
        public int ZonaCupoId { get; set; }
        public int? ComercialId { get; set; }
        public bool? FleteProcedencia { get; set; }
        public string Calidad { get; set; }
        public string Observaciones { get; set; }
        public bool? Fason { get; set; }
        public string Destinatario { get; set; }
        public DateTime FechaGeneracion { get; set; }
        public int EstadoCupoId { get; set; }
        public int? CupoStop { get; set; }
        public string CreacionStop { get; set; }
        public string ErrorStop { get; set; }

        public string UsuarioCreador { get; set; }
        public string MotivoRechazo { get; set; }


        public string EstadoPlanta { get; set; }
        public string CartaPorte { get; set; }
        public string CTG { get; set; }
        public DateTime? CTGFechaDesde { get; set; }
        public DateTime? CTGFechaHasta { get; set; }
        public string CuitOrigen { get; set; }
        public string RemitenteComercial { get; set; }
        public string CorredorComprador { get; set; }
        public string CorredorVendedor { get; set; }
        public string MercadoATermino { get; set; }
        public string Cosecha { get; set; }
        public string Peso { get; set; }
        public string Km { get; set; }
        public string IntermediarioFlete { get; set; }
        public string Transportista { get; set; }
        public string Chofer { get; set; }
        public string CuitOrigenAfip { get; set; }
        public string CodLocalidadOrigen { get; set; }
        public string NroEstablecimientoOrigen { get; set; }
        public int? TipoNegocioId { get; set; }
        public bool? ConDescarga { get; set; }
        public int? ConfiguracionEspacioDinamicoId { get; set; }
        [ForeignKey("ConfiguracionEspacioDinamicoId")]
        public virtual ConfiguracionEspacioDinamico ConfiguracionEspacioDinamico { get; set; }
        [ForeignKey("ProveedorId")]
        public virtual Proveedor Proveedor { get; set; }
        [ForeignKey("CentroId")]
        public virtual Centro Centro { get; set; }
        [ForeignKey("MaterialId")]
        public virtual Material Material { get; set; }
        [ForeignKey("ComercialId")]
        public virtual Comercial Comercial { get; set; }
        [ForeignKey("ZonaCupoId")]
        public virtual ZonaCupo ZonaCupo { get; set; }
        [ForeignKey("EstadoCupoId")]
        public virtual EstadoCupo EstadoCupo { get; set; }

        //[ForeignKey("FasonId")]
        //public virtual Fason FasonEntidad { get; set; }
        //[ForeignKey("ContratoId")]
        //public virtual Contrato Contrato { get; set; }
        //[ForeignKey("AgenteCompraId")]
        //public virtual AgenteCompra AgenteCompra { get; set; }
        //[ForeignKey("FijacionDePrecioContratoId")]
        //public virtual FijacionDePrecioContrato FijacionDePrecioContrato { get; set; }

        [ForeignKey("NegocioId")]
        public virtual Negocio Negocio { get; set; }

        [ForeignKey("TipoNegocioId")]
        public virtual TipoNegocio TipoNegocio { get; set; }
        public int? NegocioId { get; set; }
        public bool? Cumplimiento { get; set; }

        [ForeignKey("AdministracionCupoId")]
        public virtual AdministracionCupo AdministracionCupo { get; set; }
        public int? AdministracionCupoId { get; set; }
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}