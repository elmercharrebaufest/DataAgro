using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class ResultIniCondicion
    {
        public List<CondicionIni> Condicion { get; set; }
    }


    public class CondicionIni
    {
        public int CondicionId { get; set; }                  
        public string Descripcion { get; set; }
        public bool? Inhabilitado { get; set; }
    }

}


