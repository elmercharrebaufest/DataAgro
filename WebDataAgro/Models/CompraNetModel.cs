using Mastersoft.Framework.Standard;
using Molinos.DataAgro.Entities.Dto;
using System.Collections.Generic;

namespace WebDataAgro.Models
{

    public class DatosIniCompraNetModel {

        public List<MSErrorMessage> Errores { get; set; }
        public DatosIniCompraNet Datos { get; set; }
        //public ParamCompraNet Param { get; set; }

        public DatosIniCompraNetModel() {

            this.Errores = new List<MSErrorMessage>();
            this.Datos = new DatosIniCompraNet(); 
            //this.Param = new ParamCompraNet();
        }
    }


    public class CompraNetModel {

        public List<MSErrorMessage> Errores { get; set; }
        //public List<CompraNet> Proveedores { get; set; }

        public CompraNetModel() {

            this.Errores = new List<MSErrorMessage>();
            //this.Proveedores = new List<CompraNet>();
        }
    }


}