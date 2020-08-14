using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public class ListaCompraDetalleDto
    {
        public List<CampanaMaterialDetallePorMeseExcelDto> ComprasConPrecio { get; set; }
        public List<CampanaMaterialDetallePorMeseExcelDto> RecibidoSinPrecio { get; set; }
        public List<CampanaMaterialDetallePorMeseExcelDto> ARecibirAFijar { get; set; }
        public List<CampanaMaterialDetallePorMeseExcelDto> FasonFas { get; set; }
    }
}
