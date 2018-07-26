using Mastersoft.Framework.Standard;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;

namespace WebDataAgro.Models
{
    public class DatosIniAbmCentroModel
    {
        public List<MSErrorMessage> Errores { get; set; }
        public DatosIniAbmCentro Datos { get; set; }

        public DatosIniAbmCentroModel()
        {
            this.Errores = new List<MSErrorMessage>();
            this.Datos = new DatosIniAbmCentro();
        }
    }
    
    public class ResultIniCentroModel
    {
        public List<MSErrorMessage> Errores { get; set; }
        public List<CentroIni> Datos { get; set; }

        public ResultIniCentroModel()
        {
            this.Errores = new List<MSErrorMessage>();
            this.Datos = new List<CentroIni>();
        }
    }

    public class AbmCentroParam : IEntityValid
    {
        public int Id { get; set; }

        public bool Validate(List<ErrorMessage> oErrorMessages)
        {
            return oErrorMessages.Count == 0;
        }
    }

    public class AbmCentroResult
    {
        public List<MSErrorMessage> Errores { get; set; }
        public Centro Centro { get; set; }

        public AbmCentroResult()
        {
            this.Errores = new List<MSErrorMessage>();
            this.Centro = new Centro();
        }
    }

    public class ResultIniPostCentroModel
    {
        public List<MSErrorMessage> Errores { get; set; }
        public string Texto { get; set; }

        public ResultIniPostCentroModel()
        {
            this.Errores = new List<MSErrorMessage>();
        }
    }
}

