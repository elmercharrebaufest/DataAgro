using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
   public class ServicioValorDto
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public string CodigoSAP { get; set; }
        public decimal Importe { get; set; }
        public string MonedaDescripcion { get; set; }
        public string MaterialDescripcion { get; set; }
        public int MaterialId { get; set; }
        public string MonedaId { get; set; }
        public decimal Desde { get; set; }
        public decimal Hasta { get; set; }
        public int CentroId { get; set; }
        public string Centro { get; set; }
        public string DescripcionServicio { get; set; }
        public int ServicioValorId { get; set; }
        public int TipoServicioId { get; set; }
        public bool Modificado { get; set; }
    }
}
