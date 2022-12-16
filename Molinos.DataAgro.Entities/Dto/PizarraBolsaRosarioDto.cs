using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public class PizarraBolsaRosarioDto
    {
        public StatusBCR status { get; set; }
        public MetadataBCR metadata { get; set; }
        public List<DataBCR> data { get; set; }
    }

    public class DataBCR
    {
        public int iD_Pizarra { get; set; }
        public DateTime fecha_Operacion_Pizarra { get; set; }
        public int iD_Cotizacion { get; set; }
        public double precio_Cotizacion { get; set; }
        public bool esEstimado_Cotizacion { get; set; }
        public double variacion_Precio_Cotizacion { get; set; }
        public int movimiento_Cotizacion { get; set; }
        public int iD_Grano { get; set; }
        public int id_MaterialDA { get; set; }
        public string nombre_Grano { get; set; }
        public int orden_Grano { get; set; }
        public double precio_Dolar { get; set; }
        public double cotizacion_Dolar { get; set; }
        public DateTime fecha_Cotizacion_Dolar { get; set; }
    }

    public class MetadataBCR
    {
        public int totalRecords { get; set; }
        public int pageSize { get; set; }
        public int currentPage { get; set; }
        public int totalPages { get; set; }
        public object nextPageLink { get; set; }
        public object prevPageLink { get; set; }
    }

    public class StatusBCR
    {
        public int code { get; set; }
        public string message { get; set; }
        public string details { get; set; }
    }
}
