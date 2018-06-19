using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mastersoft.Framework.Standard;
using Molinos.DataAgro.Entities;

namespace WebDataAgro.Models
{
    public class FijacionDePrecioContratoModel
    {

        public List<MSErrorMessage> Errores { get; set; }
        public DatosIniAbmFijacionDePrecioContrato Datos { get; set; }

        public FijacionDePrecioContratoModel()
        {

            this.Errores = new List<MSErrorMessage>();
            this.Datos = new DatosIniAbmFijacionDePrecioContrato();
        }
    }
}
