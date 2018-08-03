using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;

namespace WebDataAgro.Models
{
    public class DatosIniAbmMaterialModel : Resultado
    {
        public Material Material { get; set; }
        
    }


    public class ResultIniMaterialModel : Resultado
    {
        public List<MaterialIni> Datos { get; set; }

        public ResultIniMaterialModel()
        {
            this.Datos = new List<MaterialIni>();
        }
    }


    public class AbmMaterialParam
    {
        public int MaterialId { get; set; }
        
    }


    public class AbmMaterialResult : Resultado
    {
        public Material Material { get; set; }

        public AbmMaterialResult()
        {
            this.Material = new Material();
        }
    }


}

