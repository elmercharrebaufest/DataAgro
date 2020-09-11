using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public class CampanaMaterialDetallePorMesDto
    {
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
        public int? ProveedorId { get; set; }
        public int MaterialId { get; set; }
        public int CampanaId { get; set; }
        public string Comercial { get; set; }
        public string Material { get; set; }
        public string Proveedor { get; set; }
        public string Campana { get; set; }
        public string CUIT { get; set; }
        public string RazonSocialCorredor { get; set; }
    }
}
