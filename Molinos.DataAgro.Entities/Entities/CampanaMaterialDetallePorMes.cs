using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class CampanaMaterialDetallePorMes
    {
        [Key]
        public int Id { get; set; }
        public string Contrato { get; set; }
        public string CorredorCuit { get; set; }
        public DateTime Fecha { get; set; }

        public double PendienteAplicar { get; set; }
        public double PendienteAFijar { get; set; }
        public double ToneladaAmpliada { get; set; }
        public double ToneladaAnulada { get; set; }
        public double ToneladaAplicada { get; set; }
        public double ToneladaContrato { get; set; }
        public double ToneladaFijada { get; set; }
        public string ClaseDoc { get; set; }
        public string Clasificacion { get; set; }
        public int? CampanaMaterialDetalleId { get; set; }
        public int? ComercialId { get; set; }

        [ForeignKey("CampanaMaterialDetalleId")]
        public virtual CampanaMaterialDetalle CampanaMaterialDetalle { get; set; }
        [ForeignKey("ComercialId")]
        public virtual Comercial Comercial { get; set; }

    }    
}
   


