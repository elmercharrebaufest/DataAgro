using Mastersoft.Framework.Standard;
using Molinos.DataAgro.Entities.Dto;
using System.Collections.Generic;

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
