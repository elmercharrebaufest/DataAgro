using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public class CapacidadProductivaPendienteDto
    {
        public decimal CAP_PROD { get; set; }
        public decimal CAP_PROD_PORC { get; set; }
        public decimal COMPRAS_ACT { get; set; }
        public decimal COMPRAS_ANT { get; set; }
        public string CUIT { get; set; }
        public string UNIDAD { get; set; }
        public string MATERIAL { get; set; }
    }
}
