using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class BuscarMaterialesDto
    {
        public List<MaterialIni> Datos { get; set; }

        public BuscarMaterialesDto()
        {
            this.Datos = new List<MaterialIni>();
        }
    }
}
