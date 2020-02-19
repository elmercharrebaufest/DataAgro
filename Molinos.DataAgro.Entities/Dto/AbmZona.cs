using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class DatosIniAbmZona
    {
        public List<ZonaCombo> Zona { get; set; }
    }

    public class DataAbmZona : Resultado
    {
        public ZonaDto Zona { get; set; }

        public DataAbmZona()
        {
            Zona = new ZonaDto();
        }
    }
    
    public class ResultIniZona
    {
        public List<ZonaIni> Zona { get; set; }
    }
    
    public class ZonaIni
    {
        public int Id { get; set; }                  
        public string Descripcion { get; set; }                  
        public string CodigoSap { get; set; }               
    }
}


