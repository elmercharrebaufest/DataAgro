using System;

namespace Molinos.DataAgro.Entities.Dto
{
    public class ConfirmaArchivoDto
    {
        public string Nombre { get; set; }
        public string Url { get; set; }
        public string Tamano { get; set; }
        public string ContratoSAP { get; set; }
        public string FechaUltimaEscritura { get; set; }
        public int Id { get; set; }
        public int NegocioId { get; set; }
        public int ComercialId { get; set; }
        public string FechaGeneracion { get; set; }
        public bool IsWebService { get; set; }
    }
}
