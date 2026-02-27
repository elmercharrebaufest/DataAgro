using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto.ControlDeBoletos
{
    public class ControlDeBoletosDatosSeguimientoDto
    {
        public int Id { get; set; }
        public int ControlDeBoletosId { get; set; }
        public int BolsaCompraNetId { get; set; }
        public int BoletoCompraNetId { get; set; }
        public string BolsaSellado { get; set; }
        public DateTime? FechaEnviadoFirma { get; set; }
        public DateTime? FechaEnvio { get; set; }
        public DateTime? FechaEnvioAfip { get; set; }
        public DateTime? FechaEnvioBolsa { get; set; }
        public DateTime? FechaRecepBoleto { get; set; }
        public DateTime? FechaRecibFirma { get; set; }
        public DateTime? FechaVueltaAfip { get; set; }
        public DateTime? FechaVueltaBolsa { get; set; }
        public string ObsCtrlBoleto { get; set; }
        public string ObsCtrlBoleto2 { get; set; }
    }
}
