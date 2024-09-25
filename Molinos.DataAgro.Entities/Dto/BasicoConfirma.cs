using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class BasicoConfirma
    {
        public int? Id { get; set; }
        public string ContratoSAP { get; set; }
        public string FijacionSAP { get; set; }
        public string NegocioSAP { get; set; }
        public int? Version_Proxima { get; set; }
        public string Adenda { get; set; }
        public string Estado_Version { get; set; }
        public string TipoBoleto { get; set; }
        public string TipoNegocio { get; set; }
        public string Bolsa { get; set; }
        public DateTime? FechaOperacion { get; set; }
        public DateTime? FechaConfirmacion { get; set; }
        public DateTime? FechaGeneracion { get; set; }
        public DateTime? FechaAnulacion { get; set; }
        public string Corredor { get; set; }
        public string Vendedor { get; set; }
        public string Canje { get; set; }
        public decimal Precio {  get; set; }
        public string Moneda {  get; set; }
        public int? TipoNegocioId { get; set; }
    }
}