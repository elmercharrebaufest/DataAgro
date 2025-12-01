using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class BuscarCentroDto
    {
        public List<CentroIni> Datos { get; set; }

        public BuscarCentroDto()
        {
            this.Datos = new List<CentroIni>();
        }
    }
}