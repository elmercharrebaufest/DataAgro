using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class ResultIniProvincia
    {
        public List<ProvinciaIni> Provincia { get; set; }
    }

    public class ProvinciaIni
    {
        public int ProvinciaId { get; set; }                  
        public string Nombre { get; set; }                  
    }

}


