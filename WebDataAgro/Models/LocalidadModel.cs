using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Mastersoft.Framework.Standard;

using Molinos.DataAgro.Entities;

namespace WebDataAgro.Models
{
    public class DatosIniAbmLocalidadModel
    {
        public List<MSErrorMessage> Errores { get; set; }
        public DatosIniAbmLocalidad Datos { get; set; }
        public Localidad Localidad { get; set; }

        public DatosIniAbmLocalidadModel()
        {
            this.Errores = new List<MSErrorMessage>();
            this.Datos = new DatosIniAbmLocalidad();
        }
    }


    public class ResultIniLocalidadModel
    {
        public List<MSErrorMessage> Errores { get; set; }
        public List<LocalidadIni> Datos { get; set; }

        public ResultIniLocalidadModel()
        {
            this.Errores = new List<MSErrorMessage>();
            this.Datos = new List<LocalidadIni>();
        }
    }


    public class AbmLocalidadParam : IEntityValid
    {
        public int LocalidadId { get; set; }

        public bool Validate(List<ErrorMessage> oErrorMessages)
        {
            return oErrorMessages.Count == 0;
        }
    }


    public class AbmLocalidadResult
    {
        public List<MSErrorMessage> Errores { get; set; }
        public Localidad Localidad { get; set; }

        public AbmLocalidadResult()
        {
            this.Errores = new List<MSErrorMessage>();
            this.Localidad = new Localidad();
        }
    }


}

