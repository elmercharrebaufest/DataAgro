using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class ParamAbmMaterial
    {
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
    }

    public class ResultIniMaterial
    {
        public List<MaterialIni> Material { get; set; }
    }
    
    public class MaterialIni
    {
        public int MaterialId { get; set; }                  
        public string Codigo { get; set; }                  
        public string Descripcion { get; set; }        
        public int CampañaIdActual { get; set; }          
    }

}


