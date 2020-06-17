using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class LogDataAgroDto
    {
        public int Id { get; set; }
        public string Usuario { get; set; }
        public DateTime Fecha { get; set; }
        public string DatoModificado { get; set; }
        public int? NegocioId { get; set; }
        public int? CupoId { get; set; }
        public int? ProveedorId { get; set; }
        public string AccionRealizada { get; set; }
        public string Clase { get; set; }
        public int ClaseId { get; set; }
        public IList<DatoModificadosLogDataAgroDto> CamposCambiados { get; set; }
        public string Campo { get; set; }
        public string Actual { get; set; }
        public string Anterior { get; set; }

    }
}