using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Mastersoft.Framework.Standard;

using Molinos.DataAgro.Entities;

namespace WebDataAgro.Models
{
    public class DatosIniAbmMaterialModel
    {
        public List<MSErrorMessage> Errores { get; set; }
        public Material Material { get; set; }

        public DatosIniAbmMaterialModel()
        {
            this.Errores = new List<MSErrorMessage>();
        }
    }


    public class ResultIniMaterialModel
    {
        public List<MSErrorMessage> Errores { get; set; }
        public List<MaterialIni> Datos { get; set; }

        public ResultIniMaterialModel()
        {
            this.Errores = new List<MSErrorMessage>();
            this.Datos = new List<MaterialIni>();
        }
    }


    public class AbmMaterialParam : IEntityValid
    {
        public int MaterialId { get; set; }

        public bool Validate(List<ErrorMessage> oErrorMessages)
        {
            return oErrorMessages.Count == 0;
        }
    }


    public class AbmMaterialResult
    {
        public List<MSErrorMessage> Errores { get; set; }
        public Material Material { get; set; }

        public AbmMaterialResult()
        {
            this.Errores = new List<MSErrorMessage>();
            this.Material = new Material();
        }
    }


}

