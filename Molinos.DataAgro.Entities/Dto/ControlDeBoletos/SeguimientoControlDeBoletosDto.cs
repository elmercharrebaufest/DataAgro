using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto.ControlDeBoletos
{
    public class SeguimientoControlDeBoletosDto
    {
        public string Bolsa { get; set; }
        public string BolsaSellado { get; set; }
        public string Contrato { get; set; }
        public string FechaEnvioFirma { get; set; }
        public string FechaEnvioSellado { get; set; }
        public string FechaEnvioAfip { get; set; }
        public string FechaEnvioBolsa { get; set; }
        public string FechaRecepBoleto { get; set; }
        public string FechaRecepcionFirma { get; set; }
        public string FechaRecepcionAfip { get; set; }
        public string FechaRecepcionBolsa { get; set; }
        public string FecAcopio { get; set; }
        public string Fecha { get; set; }
        public string Hora { get; set; }
        public string ObsCtrlBoleto { get; set; }
        public string ObsCtrlBoleto2 { get; set; }
        public string RechazadoAfip { get; set; }
        public string TipoBoleto { get; set; }
        public string Usuario { get; set; }
    }
}
