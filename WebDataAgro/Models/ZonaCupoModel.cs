using Molinos.DataAgro.Entities.Dto;
using System.Collections.Generic;

namespace WebDataAgro.Models
{
    public class DatosIniAbmZonaCupoModel : Resultado
    {
        public DatosIniAbmZonaCupo Datos { get; set; }

        public DatosIniAbmZonaCupoModel()
        {
            this.Datos = new DatosIniAbmZonaCupo();
        }
    }

    public class ResultIniZonaCupoModel : Resultado
    {
        public List<ZonaCupoIni> Datos { get; set; }

        public ResultIniZonaCupoModel()
        {
            this.Datos = new List<ZonaCupoIni>();
        }
    }

    public class AbmZonaCupoParam
    {
        public int Id { get; set; }
    }

    public class AbmZonaCupoResult : Resultado
    {
        public ZonaCupoDto ZonaCupo { get; set; }

        public AbmZonaCupoResult()
        {
            this.ZonaCupo = new ZonaCupoDto();
        }
    }

    public class ResultIniPostZonaCupoModel : Resultado
    {
        public string Texto { get; set; }
    }
}


