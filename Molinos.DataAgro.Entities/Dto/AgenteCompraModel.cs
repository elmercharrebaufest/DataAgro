using Molinos.DataAgro.Entities.Dto;
using System.Collections.Generic;
using System.Linq;

namespace Molinos.DataAgro.Entities.Dto
{
    public class AgenteCompraModel
    {
        public List<AgenteCompraDto> ListaAgenteCompras { get; set; }
        public List<AgenteCompraDto.OperadorCantidad> ListaOperadores { get; set; }        
    }

}