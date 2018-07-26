using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class ParamAbmTipoNegocio
    {
        public string TipoNegocioId { get; set; }
        public string Descripcion { get; set; }
    }
    
    public class ResultIniTipoNegocio
    {
        public List<TipoNegocioIni> TipoNegocio { get; set; }
    }
    
    public class TipoNegocioIni
    {
        public int TipoNegocioId { get; set; }                  
                
        public string Descripcion { get; set; }        
           
    }

}


