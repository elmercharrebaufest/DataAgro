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

    public class DatosIniAbmMaterial
    {
        public List<MaterialCombo> Material { get; set; }
        public List<CampaniaCombo> Campania { get; set; }

        public List<CampaniaTableroCombo> CampaniaTablero { get; set; }
    }

    public class MaterialIni
    {
        public int MaterialId { get; set; }                  
        public string Codigo { get; set; }                  
        public string Descripcion { get; set; }        
        public int CampaniaIdActual { get; set; }      
        public string CampaniaActual { get; set; }
        public int CampaniaTableroId { get; set; }
        public string CampaniaTablero { get; set; }
    }

    public class DataAbmMaterial : Resultado
    {
        public MaterialDto Material { get; set; }

        public DataAbmMaterial()
        {
            Material = new MaterialDto();
        }
    }

}


