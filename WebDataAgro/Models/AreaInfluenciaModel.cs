using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;

namespace WebDataAgro.Models
{
    public class DatosIniAbmAreaInfluenciaModel : Resultado
    {
        public AreaInfluencia AreaInfluencia { get; set; }

    }


    public class ResultIniAreaInfluenciaModel : Resultado
    {
        public List<AreaInfluenciaIni> Datos { get; set; }

        public ResultIniAreaInfluenciaModel()
        {
            this.Datos = new List<AreaInfluenciaIni>();
        }
    }


    public class AbmAreaInfluenciaParam 
    {
        public int AreaInfluenciaId { get; set; }
    }


    public class AbmAreaInfluenciaResult : Resultado
    {
        public AreaInfluenciaDto AreaInfluencia { get; set; }

        public AbmAreaInfluenciaResult()
        {
            this.AreaInfluencia = new AreaInfluenciaDto();
        }
    }


}

