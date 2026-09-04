using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto.ControlDeBoletos
{
    public class ControlDeBoletosDatosContratoHijoDto
    {
        public string TipoBoleto { get; set; }
        public string BolsaCompraNet { get; set; }
        public string Material { get; set; }
        public string ContratoSAP { get; set; }
        public string ContratoMadreSAP { get; set; }
        public string Version { get; set; }
        public string FechaGeneracion { get; set; }
        public string Proveedor { get; set; }
        public string Comercial { get; set; }
        public int NegocioId { get; set; }
        public bool ExisteControlDeBoletos { get; set; }
    }
}
