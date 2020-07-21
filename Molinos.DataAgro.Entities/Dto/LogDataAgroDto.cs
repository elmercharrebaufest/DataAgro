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
        public string AccionRealizada { get; set; }
        public string Clase { get; set; }
        public int ClaseId { get; set; }
        public IList<DatoModificadosLogDataAgroDto> CamposCambiados { get; set; }
        public string Campo { get; set; }
        public string Actual { get; set; }
        public string Anterior { get; set; }
        public string Tipo { get; set; }
        public string Descripcion { get; set; }
    }
    public class LogDataAgroExcel
    {
        public int Id { get; set; }
        public string Usuario { get; set; }
        public DateTime Fecha { get; set; }
        public string AccionRealizada { get; set; }
        public string Clase { get; set; }
        public string Codigo { get; set; }
        public string Campo { get; set; }
        public string Actual { get; set; }
        public string Anterior { get; set; }
    }
}