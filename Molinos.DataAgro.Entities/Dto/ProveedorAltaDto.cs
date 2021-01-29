using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Molinos.DataAgro.Entities.Dto
{

    public class ProveedorAltaDto
    {
        public int Id { get; set; }
        public string CUIT { get; set; }
        public string RazonSocial { get; set; }
        public string CodigoProveedor { get; set; }
        public string Mail { get; set; }
        public string EstadoAprobacion { get; set; }
        public string Observaciones { get; set; }
        public int? IdDataAgro { get; set; }
        public int? IdComercialDataAgro { get; set; }
        public string EstadoAprobacionDescripcion { get; set; }

        //public virtual List<ProveedorHistorialAprobacionDto> HistorialAprobaciones { get; set; }
        public string Comercial { get; set; }
        public string SISAEstadoCuit { get; set; }
        public string EstadoSIPER { get; set; }
        public DateTime? UltimaEdicion { get; set; }
        public DateTime? FechaSolicitud { get; set; }
        public string RazonSocialCorredor { get; set; }
        public int IdTipoUsuario { get; set; }
        public bool? IngresoAPlanta { get; set; }
        public bool? AltaInterna { get; set; }
        public string Telefono { get; set; }
        public bool? RealizarAnalisisNOSIS { get; set; }
        public string CondicionDePago { get; set; }
        public string ServicioPrestado { get; set; }
        public string OrganizacionDeCompra { get; set; }
        public string RazonDeEleccion { get; set; }
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public long? FacturacionAnual { get; set; }
        public string SolicitanteInterno { get; set; }
        public string Rubro { get; set; }
        public bool? RequiereVerificacionCompras { get; set; }
        public int? IdSituacionIVA { get; set; }
        public string SituacionIVA { get; set; }
        public int? IdIngresoBruto { get; set; }
        public string IngresoBruto { get; set; }
        public string CBU { get; set; }
        public bool? SiperObligatorio { get; set; }

        public string FacturacionAnualString { get { return FacturacionAnual.HasValue ? string.Format("{0:n0}", FacturacionAnual) : ""; } }
    }
}
