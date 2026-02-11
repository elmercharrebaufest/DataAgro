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

        [Required]
        public int ControlDeBoletosId { get; set; }

        [Required]
        public int BolsaCompraNetId { get; set; }

        [Required]
        public int BoletoCompraNetId { get; set; }

        [Required]
        [StringLength(100)]
        public string BolsaSellado { get; set; }

        [Required]
        public DateTime FechaEnviadoFirma { get; set; }

        [Required]
        public DateTime FechaEnvio { get; set; }

        [Required]
        public DateTime FechaEnvioAfip { get; set; }

        [Required]
        public DateTime FechaEnvioBolsa { get; set; }

        [Required]
        public DateTime FechaRecepBoleto { get; set; }

        [Required]
        public DateTime FechaRecibFirma { get; set; }

        [Required]
        public DateTime FechaVueltaAfip { get; set; }

        [Required]
        public DateTime FechaVueltaBolsa { get; set; }

        [Required]
        public DateTime FechaAcopio { get; set; }

        [StringLength(500)]
        public string ObsCtrlBoleto { get; set; }

        [StringLength(500)]
        public string ObsCtrlBoleto2 { get; set; }

        [Required]
        public DateTime FechaCreacion { get; set; }

        public DateTime? FechaModificacion { get; set; }

        // 🔗 Navigation properties (FK)
        [ForeignKey(nameof(ControlDeBoletosId))]
        public ControlDeBoletos ControlDeBoletos { get; set; }

        [ForeignKey(nameof(BolsaCompraNetId))]
        public BolsaCompraNet BolsaCompraNet { get; set; }

        [ForeignKey(nameof(BoletoCompraNetId))]
        public BoletoCompraNet BoletoCompraNet { get; set; }
    }
}
