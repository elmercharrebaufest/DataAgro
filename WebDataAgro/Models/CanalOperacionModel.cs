using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Mastersoft.Framework.Standard;

using Molinos.DataAgro.Entities;

namespace WebDataAgro.Models
{
    public class DatosIniAbmCanalOperacionModel
    {
        public List<MSErrorMessage> Errores { get; set; }
        public CanalOperacion CanalOperacion { get; set; }

        public DatosIniAbmCanalOperacionModel()
        {
            this.Errores = new List<MSErrorMessage>();
        }
    }


    public class ResultIniCanalOperacionModel
    {
        public List<MSErrorMessage> Errores { get; set; }
        public List<CanalOperacionIni> Datos { get; set; }

        public ResultIniCanalOperacionModel()
        {
            this.Errores = new List<MSErrorMessage>();
            this.Datos = new List<CanalOperacionIni>();
        }
    }


    public class AbmCanalOperacionParam : IEntityValid
    {
        public int CanalOperacionId { get; set; }

        public bool Validate(List<ErrorMessage> oErrorMessages)
        {
            return oErrorMessages.Count == 0;
        }
    }


    public class AbmCanalOperacionResult
    {
        public List<MSErrorMessage> Errores { get; set; }
        public CanalOperacion CanalOperacion { get; set; }

        public AbmCanalOperacionResult()
        {
            this.Errores = new List<MSErrorMessage>();
            this.CanalOperacion = new CanalOperacion();
        }
    }


}

