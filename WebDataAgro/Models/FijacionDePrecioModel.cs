using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Mastersoft.Framework.Standard;

using Molinos.DataAgro.Entities;

namespace WebDataAgro.Models
{
    public class DatosIniAbmFijacionDePrecioModel
    {
        public List<MSErrorMessage> Errores { get; set; }
        public DatosIniAbmFijacionDePrecio Datos { get; set; }
        public FijacionDePrecio FijacionDePrecio { get; set; }

        public DatosIniAbmFijacionDePrecioModel()
        {
            this.Errores = new List<MSErrorMessage>();
            this.Datos = new DatosIniAbmFijacionDePrecio();
        }
    }


    public class ResultIniFijacionDePrecioModel
    {
        public List<MSErrorMessage> Errores { get; set; }
        public List<FijacionDePrecioIni> Datos { get; set; }

        public ResultIniFijacionDePrecioModel()
        {
            this.Errores = new List<MSErrorMessage>();
            this.Datos = new List<FijacionDePrecioIni>();
        }
    }


    public class AbmFijacionDePrecioParam : IEntityValid
    {
        public int FijacionId { get; set; }

        public bool Validate(List<ErrorMessage> oErrorMessages)
        {
            return oErrorMessages.Count == 0;
        }
    }


    public class AbmFijacionDePrecioResult
    {
        public List<MSErrorMessage> Errores { get; set; }
        public FijacionDePrecio FijacionDePrecio { get; set; }

        public AbmFijacionDePrecioResult()
        {
            this.Errores = new List<MSErrorMessage>();
            this.FijacionDePrecio = new FijacionDePrecio();
        }
    }


}

