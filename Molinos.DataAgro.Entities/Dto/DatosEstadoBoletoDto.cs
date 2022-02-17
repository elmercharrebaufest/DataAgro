using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class DatosEstadoBoletoDto
    {
        //public string EX_BOLETO { get; set; }
        public string Contrato { get; set; }
        public string Version { get; set; }
        public string Anulado { get; set; }
        public string FechaRecepcionBoleto { get; set; }
        public List<CondicionFijacionEstadoBoletoDto> CondicionFijacion { get; set; }
        public string FechaConfirmacion { get; set; }
        public string Generado { get; set; }
        
        public DatosEstadoBoletoDto()
        {
            CondicionFijacion = new List<CondicionFijacionEstadoBoletoDto>();
        }
    }

    public partial class CondicionFijacionEstadoBoletoDto
    {
        public string Contrato { get; set; }
        public string FechaDesde { get; set; }
        public string FechaHasta { get; set; }
        public decimal CantidadMaxima { get; set; }
        public decimal CantidadMinima { get; set; }
        public string Meins { get; set; }
    }
}
