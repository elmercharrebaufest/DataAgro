
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class ResultIniDestinatario
    {
        public List<DestinatarioIni> Destinatario { get; set; }
    }
    
    public class DestinatarioIni
    {
        public int DestinatarioId { get; set; }                  
        public string Descripcion { get; set; }
        public bool Inhabilitado { get; set; }
    }

}


