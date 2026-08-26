using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    [Table("ControlDeBoletosSeguimiento")]
    public class ControlDeBoletosSeguimiento
    {
        [Key]
        public int Id { get; set; }

        public int ControlDeBoletosId { get; set; }

        public int? BolsaCompraNetId { get; set; }

        public int? BoletoSapId { get; set; }

        [StringLength(20)]
        public string BoletoSapCaracter { get; set; }

        [StringLength(100)]
        public string BolsaSellado { get; set; }

        public DateTime? FechaRecepcionBoleto { get; set; }
        public DateTime? FechaEnvioFirma { get; set; }
        public DateTime? FechaEnvioBolsa { get; set; }
        public DateTime? FechaEnvioAfip { get; set; }
        public DateTime? FechaRecepcionFirma { get; set; }
        public DateTime? FechaRecepcionBolsa { get; set; }
        public DateTime? FechaRecepcionAfip { get; set; }
        public DateTime? FechaEnvioSellado { get; set; }

        [StringLength(10)]
        public string RechazadoAfip { get; set; }

        [StringLength(500)]
        public string ObsCtrlBoleto { get; set; }

        [StringLength(500)]
        public string ObsCtrlBoleto2 { get; set; }

        public DateTime FechaCreacion { get; set; }

        public DateTime? FechaModificacion { get; set; }

        [ForeignKey(nameof(ControlDeBoletosId))]
        public virtual ControlDeBoletos ControlDeBoletos { get; set; }

        [ForeignKey(nameof(BolsaCompraNetId))]
        public virtual BolsaCompraNet BolsaCompraNet { get; set; }

        [ForeignKey(nameof(BoletoSapId))]
        public virtual BoletoSap BoletoSap { get; set; }
    }
}
