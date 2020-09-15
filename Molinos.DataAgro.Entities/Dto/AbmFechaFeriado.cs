using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class DataAbmFechaFeriado : Resultado
    {
        public FechaFeriadoDto FechaFeriado { get; set; }

        public DataAbmFechaFeriado()
        {
            FechaFeriado = new FechaFeriadoDto();
        }
    }
    
    public class ResultIniFechaFeriado
    {
        public List<FechaFeriadoDto> FechaFeriado { get; set; }
    }
    
    
}


