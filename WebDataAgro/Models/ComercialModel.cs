using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;

namespace WebDataAgro.Models
{
    public class DatosIniAbmComercialModel : Resultado
    {
        public DatosIniAbmComercial Datos { get; set; }
        public Comercial Comercial { get; set; }

        public DatosIniAbmComercialModel()
        {
            this.Datos = new DatosIniAbmComercial();
        }
    }


    public class ResultIniComercialModel : Resultado
    {
        public List<ComercialIni> Datos { get; set; }

        public ResultIniComercialModel()
        {
            this.Datos = new List<ComercialIni>();
        }
    }


    public class AbmComercialParam 
    {
        public int ComercialId { get; set; }
    }


    public class AbmComercialResult : Resultado
    {
        public Comercial Comercial { get; set; }

        public AbmComercialResult()
        {
            this.Comercial = new Comercial();
        }
    }

    public class AbmComercialCrearResult : Resultado
    {
        public ComercialDto Comercial { get; set; }

        public AbmComercialCrearResult()
        {
            this.Comercial = new ComercialDto();
        }
    }

    public class ResultIniPostItModel : Resultado
    {
        public string Texto { get; set; }
    }
}

