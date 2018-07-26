using Mastersoft.Framework.Standard;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;

namespace WebDataAgro.Models
{
    public class DatosIniAbmComercialModel
    {
        public List<MSErrorMessage> Errores { get; set; }
        public DatosIniAbmComercial Datos { get; set; }
        public Comercial Comercial { get; set; }

        public DatosIniAbmComercialModel()
        {
            this.Errores = new List<MSErrorMessage>();
            this.Datos = new DatosIniAbmComercial();
        }
    }


    public class ResultIniComercialModel
    {
        public List<MSErrorMessage> Errores { get; set; }
        public List<ComercialIni> Datos { get; set; }

        public ResultIniComercialModel()
        {
            this.Errores = new List<MSErrorMessage>();
            this.Datos = new List<ComercialIni>();
        }
    }


    public class AbmComercialParam : IEntityValid
    {
        public int ComercialId { get; set; }

        public bool Validate(List<ErrorMessage> oErrorMessages)
        {
            return oErrorMessages.Count == 0;
        }
    }


    public class AbmComercialResult
    {
        public List<MSErrorMessage> Errores { get; set; }
        public Comercial Comercial { get; set; }

        public AbmComercialResult()
        {
            this.Errores = new List<MSErrorMessage>();
            this.Comercial = new Comercial();
        }
    }

    public class ResultIniPostItModel
    {
        public List<MSErrorMessage> Errores { get; set; }
        public string Texto { get; set; }

        public ResultIniPostItModel()
        {
            this.Errores = new List<MSErrorMessage>();
        }

    }
}

