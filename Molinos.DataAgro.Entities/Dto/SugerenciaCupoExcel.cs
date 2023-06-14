using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class SugerenciaCupoExcel
    {
        public decimal Puntaje { get; set; }
        public string TipoNegocio { get; set; }
        public string ContratoSAP { get; set; }
        public DateTime FechaSugerida { get; set; }
        public string RazonSocial { get; set; }
        public string Comercial { get; set; }
        public string Priorizado { get; set; }
        public int CantidadSugerida { get; set; }
        public string CUIT { get; set; }
        public decimal? Precio { get; set; }
        public string Moneda { get; set; }
        public double KgNegocio { get; set; }
        public double KgPendienteAplicar { get; set; }
        public string Material { get; set; }

        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public string Zona { get; set; }
        public string Centro { get; set; }
        public string CDWarrant { get; set; }
        public string Fason { get; set; }
        public string Destinatario { get; set; }
        public int CuposPendientes { get; set; }
        public int SolicitudesPendientes { get; set; }
        public string Observaciones { get; set; }
        public string Puntuaciones { get; set; }
    }

    public partial class SugerenciaCupoAgrupadasExcel
    {
        public string Material { get; set; }
        public string RazonSocial { get; set; }
      
        public List<DiaCupo> DiaCupo { get; set; }
    }
}