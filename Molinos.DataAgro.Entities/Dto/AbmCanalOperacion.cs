
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class ResultIniCanalOperacion
    {
        public List<CanalOperacionIni> CanalOperacion { get; set; }
    }

    public class CanalOperacionIni
    {
        public int CanalOperacionId { get; set; }                  
        public string Descripcion { get; set; }
        public bool Inhabilitado { get; set; }
    }

}


