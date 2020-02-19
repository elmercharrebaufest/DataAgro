using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;

namespace WebDataAgro.Models
{
    public class DatosIniAbmRangoModel : Resultado
    {
        public DatosIniAbmRango Datos { get; set; }
        public RangoPrecio RangoPrecio { get; set; }

        public DatosIniAbmRangoModel()
        {
            this.Datos = new DatosIniAbmRango();
        }
    }
    
    public class ResultIniRangoModel : Resultado
    {
        public List<RangoIni> Datos { get; set; }

        public ResultIniRangoModel()
        {
            this.Datos = new List<RangoIni>();
        }
    }

    public class AbmRangoParam
    {
        public int Id { get; set; }
    }

    public class AbmRangoResult : Resultado
    {
        
        public RangoPrecioDto Rango { get; set; }
        public AbmRangoResult()
        {
            this.Rango = new RangoPrecioDto();
        }
    }

    public class ResultIniPostRangoModel : Resultado
    {
        public string Texto { get; set; }
    }
}

