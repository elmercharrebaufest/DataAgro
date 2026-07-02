using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto.ControlDeBoletos
{
    public class ControlDeBoletosDatosSeguimientoServiceDto
    {
        public string ContratoSAP { get; set; }
        public string Bolsa { get; set; }
        public string TipoBoletoSAP { get; set; }
        public string BoletoSapCaracter { get; set; }
        public string FechaRecepcionBoleto { get; set; }
        public string FechaEnvioFirma { get; set; }
        public string FechaEnvioBolsa { get; set; }
        public string FechaEnvioAfip { get; set; }
        public string FechaRecepcionFirma { get; set; }
        public string FechaRecepcionBolsa { get; set; }
        public string FechaRecepcionAfip { get; set; }
        public string FechaEnvioSellado { get; set; }
        public string ObsCtrlBoleto { get; set; }
        public string ObsCtrlBoleto2 { get; set; }
    }
}
