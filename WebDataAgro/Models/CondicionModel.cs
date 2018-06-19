using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Mastersoft.Framework.Standard;

using Molinos.DataAgro.Entities;

namespace WebDataAgro.Models
{
    public class DatosIniAbmCondicionModel
    {
        public List<MSErrorMessage> Errores { get; set; }
        public Condicion Condicion { get; set; }

        public DatosIniAbmCondicionModel()
        {
            this.Errores = new List<MSErrorMessage>();
        }
    }


    public class ResultIniCondicionModel
    {
        public List<MSErrorMessage> Errores { get; set; }
        public List<CondicionIni> Datos { get; set; }

        public ResultIniCondicionModel()
        {
            this.Errores = new List<MSErrorMessage>();
            this.Datos = new List<CondicionIni>();
        }
    }


    public class AbmCondicionParam : IEntityValid
    {
        public int CondicionId { get; set; }

        public bool Validate(List<ErrorMessage> oErrorMessages)
        {
            return oErrorMessages.Count == 0;
        }
    }


    public class AbmCondicionResult
    {
        public List<MSErrorMessage> Errores { get; set; }
        public Condicion Condicion { get; set; }

        public AbmCondicionResult()
        {
            this.Errores = new List<MSErrorMessage>();
            this.Condicion = new Condicion();
        }
    }


}

