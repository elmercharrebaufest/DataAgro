using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Entities
{
    public class ControlDeBoletosSeguimiento
    {
        [Key]
        public int Id { get; set; }
        public int ControlDeBoletosId { get; set; }
        public int BolsaCompraNetId { get; set; }
        public int BoletoSapId { get; set; }
        public string BoletoSapCaracter { get; set; }
        public string BolsaSellado { get; set; }

        public DateTime? FechaRecepcionBoleto { get; set; }
        public DateTime? FechaEnvioFirmas { get; set; }
        public DateTime? FechaEnvioBolsa { get; set; }
        public DateTime? FechaEnvioAfip { get; set; }
        public DateTime? FechaRecepcionFirma { get; set; }
        public DateTime? FechaRecepcionBolsa { get; set; }
        public DateTime? FechaRecepcionAfip { get; set; }
        public DateTime? FechaEnvioSellado { get; set; }


        public string ObsCtrlBoleto { get; set; }
        public string ObsCtrlBoleto2 { get; set; }

        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }

        // 🔹 Navigation
        public virtual ControlDeBoletos ControlDeBoletos { get; set; }
        public virtual BolsaCompraNet BolsaCompraNet { get; set; }
        public virtual BoletoSap BoletoSap { get; set; }
    }

}
