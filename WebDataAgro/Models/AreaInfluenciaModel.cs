using Mastersoft.Framework.Standard;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;

namespace WebDataAgro.Models
{
    public class DatosIniAbmAreaInfluenciaModel
    {
        public List<MSErrorMessage> Errores { get; set; }
        public AreaInfluencia AreaInfluencia { get; set; }

        public DatosIniAbmAreaInfluenciaModel()
        {
            this.Errores = new List<MSErrorMessage>();
        }
    }


    public class ResultIniAreaInfluenciaModel
    {
        public List<MSErrorMessage> Errores { get; set; }
        public List<AreaInfluenciaIni> Datos { get; set; }

        public ResultIniAreaInfluenciaModel()
        {
            this.Errores = new List<MSErrorMessage>();
            this.Datos = new List<AreaInfluenciaIni>();
        }
    }


    public class AbmAreaInfluenciaParam : IEntityValid
    {
        public int AreaInfluenciaId { get; set; }

        public bool Validate(List<ErrorMessage> oErrorMessages)
        {
            return oErrorMessages.Count == 0;
        }
    }


    public class AbmAreaInfluenciaResult
    {
        public List<MSErrorMessage> Errores { get; set; }
        public AreaInfluencia AreaInfluencia { get; set; }

        public AbmAreaInfluenciaResult()
        {
            this.Errores = new List<MSErrorMessage>();
            this.AreaInfluencia = new AreaInfluencia();
        }
    }


}

