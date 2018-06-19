using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Mastersoft.Framework.Standard;

using Molinos.DataAgro.Entities;

namespace WebDataAgro.Models
{
    public class DatosIniCubProveedoresModel
    {
        public List<MSErrorMessage> Errores { get; set; }
        public DatosIniCubProveedores Datos { get; set; }
        public ParamCubProveedores Param { get; set; }

        public DatosIniCubProveedoresModel()
        {
            this.Errores = new List<MSErrorMessage>();
            this.Datos = new DatosIniCubProveedores();
            this.Param = new ParamCubProveedores();
        }
    }


    public class CubProveedoresModel
    {
        public List<MSErrorMessage> Errores { get; set; }
        public List<ProveedoresCub> Proveedores { get; set; }

        public CubProveedoresModel()
        {
            this.Errores = new List<MSErrorMessage>();
            this.Proveedores = new List<ProveedoresCub>();
        }
    }


}

