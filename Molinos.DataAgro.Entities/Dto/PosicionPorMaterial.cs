using Molinos.DataAgro.Entities.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public class PosicionPorMaterial
    {
        public int Id { get; set; }
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public double Cantidad { get; set; }
        public double CantidadPonderada { get; set; }
        public decimal Precio { get; set; }
        public string MonedaId { get; set; }
        public int TipoNegocioId { get; set; }
        public int? CampanaMaterialId { get; set; }
        public int? CampanaId { get; set; }
        public string Posicion { get; set; }
        public EnumClasificacionNegocio ClasificacionNegocio { get; set; }
        public string Campana { get; set; }
        public bool FijacionContratoConDescarga { get; set; }
    }
}
