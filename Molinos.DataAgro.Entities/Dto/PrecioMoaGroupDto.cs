using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class PrecioMoaGroupDto
    {
        public int MaterialId { get; set; }
        public List<PrecioMoaCompraNetDto> PrecioMoaCompraNetDtoList { get; set; }
    }
}