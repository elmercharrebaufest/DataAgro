using Molinos.DataAgro.Entities.Dto;
using System.Collections.Generic;
using System.Linq;

namespace WebDataAgro.Models
{
    public class AgenteCompraModel
    {
        public List<AgenteCompraDto> ListaAgenteCompras { get; set; }
        public List<AgenteCompraDto.OperadorCantidad> ListaOperadores { get; set; }        
    }

}