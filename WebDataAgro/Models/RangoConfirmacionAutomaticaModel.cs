using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;

namespace WebDataAgro.Models
{
    public class DatosIniAbmRangoConfirmacionAutomaticaModel : Resultado
    {
        public DatosIniAbmRangoConfirmacionAutomatica Datos { get; set; }
        public RangoConfirmacionAutomatica RangoConfirmacion { get; set; }

        public DatosIniAbmRangoConfirmacionAutomaticaModel()
        {
            this.Datos = new DatosIniAbmRangoConfirmacionAutomatica();
        }
    }
    
    public class ResultIniRangoConfirmacionAutomaticaModel : Resultado
    {
        public List<RangoConfirmacionAutomaticaIni> Datos { get; set; }

        public ResultIniRangoConfirmacionAutomaticaModel()
        {
            this.Datos = new List<RangoConfirmacionAutomaticaIni>();
        }
    }

    public class AbmRangoConfirmacionAutomaticaParam
    {
        public int Id { get; set; }
    }

    public class AbmRangoConfirmacionAutomaticaResult : Resultado
    {
        
        public RangoConfirmacionAutomaticaDto Rango { get; set; }
        public AbmRangoConfirmacionAutomaticaResult()
        {
            this.Rango = new RangoConfirmacionAutomaticaDto();
        }
    }

    public class ResultIniPostRangoConfirmacionAutomaticaModel : Resultado
    {
        public string Texto { get; set; }
    }
}

