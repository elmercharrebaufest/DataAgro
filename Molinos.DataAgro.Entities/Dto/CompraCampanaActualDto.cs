using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public class CompraCampanaActualDto
    {
        public DateTime Fecha { get; set; }
        public string Contrato { get; set; }       
        public string Comercial { get; set; }
        public string Cuit { get; set; }
        public string RazonSocial { get; set; }
        public string CorredorCuit { get; set; }
        public string Material { get; set; }
        public string Campana { get; set; }
        public double PendienteAFijar { get; set; }
        public double PendienteAplicar { get; set; }

        public double ToneladaAmpliada { get; set; }
        public double ToneladaAnulada { get; set; }
        public double ToneladaAplicada { get; set; }
        public double ToneladaContrato { get; set; }
        public double ToneladaFijada { get; set; }
        public string ClaseDoc { get; set; }
        public string Clasificacion { get; set; }
    }
}
