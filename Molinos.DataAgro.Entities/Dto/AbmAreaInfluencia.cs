
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{  
    public class ResultIniAreaInfluencia
    {
        public List<AreaInfluenciaIni> AreaInfluencia { get; set; }
    }

    public class AreaInfluenciaIni
    {
        public int AreaInfluenciaId { get; set; }                  
        public string Descripcion { get; set; }                  
    }

}


