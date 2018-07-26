using Mastersoft.Framework.Standard;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;

namespace WebDataAgro.Models
{
    public class DatosIniAbmProvinciaModel
    {
        public List<MSErrorMessage> Errores { get; set; }
        public Provincia Provincia { get; set; }

        public DatosIniAbmProvinciaModel()
        {
            this.Errores = new List<MSErrorMessage>();
        }
    }


    public class ResultIniProvinciaModel
    {
        public List<MSErrorMessage> Errores { get; set; }
        public List<ProvinciaIni> Datos { get; set; }

        public ResultIniProvinciaModel()
        {
            this.Errores = new List<MSErrorMessage>();
            this.Datos = new List<ProvinciaIni>();
        }
    }


    public class AbmProvinciaParam : IEntityValid
    {
        public int ProvinciaId { get; set; }

        public bool Validate(List<ErrorMessage> oErrorMessages)
        {
            return oErrorMessages.Count == 0;
        }
    }


    public class AbmProvinciaResult
    {
        public List<MSErrorMessage> Errores { get; set; }
        public Provincia Provincia { get; set; }

        public AbmProvinciaResult()
        {
            this.Errores = new List<MSErrorMessage>();
            this.Provincia = new Provincia();
        }
    }


}

