using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;

namespace WebDataAgro.Models
{
    public class DatosIniAbmProvinciaModel : Resultado
    {
        public Provincia Provincia { get; set; }
    }


    public class ResultIniProvinciaModel : Resultado
    {
        public List<ProvinciaIni> Datos { get; set; }

        public ResultIniProvinciaModel()
        {
            this.Datos = new List<ProvinciaIni>();
        }
    }


    public class AbmProvinciaParam
    {
        public int ProvinciaId { get; set; }
    }


    public class AbmProvinciaResult : Resultado
    {
        public Provincia Provincia { get; set; }

        public AbmProvinciaResult()
        {
            this.Provincia = new Provincia();
        }
    }

    public class AbmProvinciaCrearResult : Resultado
    {
        public ProvinciaDto Provincia { get; set; }

        public AbmProvinciaCrearResult()
        {
            this.Provincia = new ProvinciaDto();
        }
    }
}

