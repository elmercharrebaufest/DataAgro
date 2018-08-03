using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;

namespace WebDataAgro.Models
{
    public class DatosIniAbmCondicionModel : Resultado
    {
        public Condicion Condicion { get; set; }
    }


    public class ResultIniCondicionModel : Resultado
    {
        public List<CondicionIni> Datos { get; set; }

        public ResultIniCondicionModel()
        {
            this.Datos = new List<CondicionIni>();
        }
    }


    public class AbmCondicionParam
    {
        public int CondicionId { get; set; }
    }


    public class AbmCondicionResult : Resultado
    {
        public Condicion Condicion { get; set; }

        public AbmCondicionResult()
        {
            this.Condicion = new Condicion();
        }
    }


}

