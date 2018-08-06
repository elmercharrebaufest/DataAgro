using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;

namespace WebDataAgro.Models
{
    public class DatosIniAbmCentroModel : Resultado
    {
        public DatosIniAbmCentro Datos { get; set; }

        public DatosIniAbmCentroModel()
        {
            this.Datos = new DatosIniAbmCentro();
        }
    }
    
    public class ResultIniCentroModel : Resultado
    {
        public List<CentroIni> Datos { get; set; }

        public ResultIniCentroModel()
        {
            this.Datos = new List<CentroIni>();
        }
    }

    public class AbmCentroParam
    {
        public int Id { get; set; }
    }

    public class AbmCentroResult : Resultado
    {
        public CentroDto Centro { get; set; }

        public AbmCentroResult()
        {
            this.Centro = new CentroDto();
        }
    }

    public class ResultIniPostCentroModel : Resultado
    {
        public string Texto { get; set; }
    }
}

