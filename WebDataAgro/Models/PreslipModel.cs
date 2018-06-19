using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Mastersoft.Framework.Standard;

using Molinos.DataAgro.Entities;

namespace WebDataAgro.Models
{
    public class DatosIniAbmPreslipModel
    {
        public List<MSErrorMessage> Errores { get; set; }
        public DatosIniAbmPreslip Datos { get; set; }
        public Preslip Preslip { get; set; }

        public DatosIniAbmPreslipModel()
        {
            this.Errores = new List<MSErrorMessage>();
            this.Datos = new DatosIniAbmPreslip();
        }
    }


    public class ResultIniPreslipModel
    {
        public List<MSErrorMessage> Errores { get; set; }
        public List<PreslipIni> Datos { get; set; }

        public ResultIniPreslipModel()
        {
            this.Errores = new List<MSErrorMessage>();
            this.Datos = new List<PreslipIni>();
        }
    }


    public class AbmPreslipParam : IEntityValid
    {
        public int PreslipId { get; set; }

        public bool Validate(List<ErrorMessage> oErrorMessages)
        {
            return oErrorMessages.Count == 0;
        }
    }


    public class AbmPreslipResult
    {
        public List<MSErrorMessage> Errores { get; set; }
        public Preslip Preslip { get; set; }

        public AbmPreslipResult()
        {
            this.Errores = new List<MSErrorMessage>();
            this.Preslip = new Preslip();
        }
    }


}

