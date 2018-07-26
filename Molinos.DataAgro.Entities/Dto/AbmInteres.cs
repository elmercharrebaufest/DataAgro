using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class ResultIniInteres
    {
        public List<InteresIni> Condicion { get; set; }
    }


    public class InteresIni
    {
        public int InteresId { get; set; }
        public string Descripcion { get; set; }
    }
}
