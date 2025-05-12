using System;

namespace Molinos.DataAgro.Entities.Dto
{
    public class CupoSapTerceroDto
    {
        public int IdDataAgro { get; set; }
        public string CUIT { get; set; }
        public string RazonSocial { get; set; }
        public string Centro { get; set; }
        public string Material { get; set; }
        public string FechaIngreso { get; set; }
        public string CupoSap { get; set; }
        public string ZonaCupo { get; set; }
        public string ComercialAsignado { get; set; }
        public bool FleteProcedencia { get; set; }
        public string Calidad { get; set; }
        public bool Fason { get; set; }
        public string FechaGeneracion { get; set; }
        public string EstadoCupo { get; set; }
        public int CupoStop { get; set; }
        //public string CreacionStop { get; set; }
        public string TipoNegocio { get; set; }
        public string ContratoSap { get; set; }
        public bool ConDescarga { get; set; }
        public string ComercialCreador { get; set; }
        public bool Sustentable { get; set; }
        public bool EPA { get; set; }
        public bool EUDR { get; set; }
        public string TipoDeCupo { get; set; }
        public string OrigenDeCupo { get; set; }
    }
}
