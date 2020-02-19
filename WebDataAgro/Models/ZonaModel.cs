using Molinos.DataAgro.Entities.Dto;
using System.Collections.Generic;

namespace WebDataAgro.Models
{
    public class DatosIniAbmZonaModel : Resultado
    {
        public DatosIniAbmZona Datos { get; set; }

        public DatosIniAbmZonaModel()
        {
            this.Datos = new DatosIniAbmZona();
        }
    }

    public class ResultIniZonaModel : Resultado
    {
        public List<ZonaIni> Datos { get; set; }

        public ResultIniZonaModel()
        {
            this.Datos = new List<ZonaIni>();
        }
    }

    public class AbmZonaParam
    {
        public int Id { get; set; }
    }

    public class AbmZonaResult : Resultado
    {
        public ZonaDto Zona { get; set; }

        public AbmZonaResult()
        {
            this.Zona = new ZonaDto();
        }
    }

    public class ResultIniPostZonaModel : Resultado
    {
        public string Texto { get; set; }
    }
}



