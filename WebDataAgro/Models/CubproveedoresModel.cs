using Molinos.DataAgro.Entities.Dto;
using System.Collections.Generic;

namespace WebDataAgro.Models
{
    public class DatosIniCubProveedoresModel : Resultado
    {
        public DatosIniCubProveedores Datos { get; set; }
        public ParamCubProveedores Param { get; set; }

        public DatosIniCubProveedoresModel()
        {
            this.Datos = new DatosIniCubProveedores();
            this.Param = new ParamCubProveedores();
        }
    }


    public class CubProveedoresModel : Resultado
    {
        public List<ProveedoresCub> Proveedores { get; set; }

        public CubProveedoresModel()
        {
            this.Proveedores = new List<ProveedoresCub>();
        }
    }


}

