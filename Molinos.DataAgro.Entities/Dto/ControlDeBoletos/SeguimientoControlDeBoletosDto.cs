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
        public string FeEnviadoFirma { get; set; }
        public string FeEnvio { get; set; }
        public string FeEnvioAfip { get; set; }
        public string FeEnvioBolsa { get; set; }
        public string FeRecepBoleto { get; set; }
        public string FeRecibFirma { get; set; }
        public string FeVueltaAfip { get; set; }
        public string FeVueltaBolsa { get; set; }
        public string FecAcopio { get; set; }
        public string Fecha { get; set; }
        public string Fijacion { get; set; }
        public string Hora { get; set; }
        public string ObsCtrlBoleto { get; set; }
        public string ObsCtrlBoleto2 { get; set; }
        public string RechazadoAfip { get; set; }
        public string TipoBoleto { get; set; }
        public string Usuario { get; set; }
    }
}
