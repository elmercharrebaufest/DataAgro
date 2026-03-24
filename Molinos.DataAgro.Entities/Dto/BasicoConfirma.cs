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
        public int? Version { get; set; }
        public string Adenda { get; set; }
        public string Estado_Version { get; set; }
        public string TipoBoleto { get; set; }
        public string TipoNegocio { get; set; }
        public string Bolsa { get; set; }
        public int? BolsaId { get; set; }
        public DateTime? FechaOperacion { get; set; }
        public DateTime? FechaConfirmacion { get; set; }
        public DateTime? FechaGeneracion { get; set; }
        public DateTime? FechaAnulacion { get; set; }
        public string Corredor { get; set; }
        public string Vendedor { get; set; }
        public string Canje { get; set; }
        public decimal Precio { get; set; }
        public string Moneda { get; set; }
        public int? TipoNegocioId { get; set; }
        public string Material { get; set; }
        public string ContratoVendedor { get; set; }
        public string ContratoCorredor { get; set; }
        public string Comercial { get; set; }
        public string UsuarioAnulacion { get; set; }
        public DateTime FechaCarga { get; set; }
        public string ClaseNegocioId { get; set; }
        public int MaterialId { get; set; }
        public int? BoletoId { get; set; }
        public DateTime? FechaConfirmadoSAP { get; set; }
        public int? ComercialId { get; set; }
        public int? ProveedorId { get; set; }
    }
}