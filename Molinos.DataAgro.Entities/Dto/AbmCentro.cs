using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class DatosIniAbmCentro
    {
        public List<CentroCombo> Centro { get; set; }
    }

    public class DataAbmCentro : Resultado
    {
        public CentroDto Centro { get; set; }

        public DataAbmCentro()
        {
            Centro = new CentroDto();
        }
    }
    
    public class ResultIniCentro
    {
        public List<CentroIni> Centro { get; set; }
    }
    
    public class CentroIni
    {
        public int Id { get; set; }                  
        public string Descripcion { get; set; }                  
        public string CodigoSap { get; set; }               
    }
}


