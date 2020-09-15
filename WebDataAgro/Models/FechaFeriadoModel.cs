using Molinos.DataAgro.Entities.Dto;
using System.Collections.Generic;

namespace WebDataAgro.Models
{

    public class ResultIniFechaFeriadoModel : Resultado
    {
        public List<FechaFeriadoDto> Datos { get; set; }

        public ResultIniFechaFeriadoModel()
        {
            this.Datos = new List<FechaFeriadoDto>();
        }
    }

    public class AbmFechaFeriadoParam
    {
        public int Id { get; set; }
    }

    public class AbmFechaFeriadoResult : Resultado
    {
        public FechaFeriadoDto FechaFeriado { get; set; }

        public AbmFechaFeriadoResult()
        {
            this.FechaFeriado = new FechaFeriadoDto();
        }
    }

    public class ResultIniPostFechaFeriadoModel : Resultado
    {
        public string Texto { get; set; }
    }
}



