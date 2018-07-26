using Mastersoft.Framework.Standard;
using Molinos.DataAgro.Entities.Dto;
using System.Collections.Generic;

namespace WebDataAgro.Models
{
    public class ContratoModel_prueba {

        public List<MSErrorMessage> Errores { get; set; }
        public DatosIniContrato Datos { get; set; }

        public ContratoModel_prueba() {

            this.Errores = new List<MSErrorMessage>();
            this.Datos = new DatosIniContrato();
        }
    }
}
