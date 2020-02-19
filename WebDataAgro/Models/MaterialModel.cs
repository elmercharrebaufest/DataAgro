using Molinos.DataAgro.Entities.Dto;
using System.Collections.Generic;

namespace WebDataAgro.Models
{
    public class DatosIniAbmMaterialModel : Resultado
    {
        public DatosIniAbmMaterial Datos { get; set; }

        public DatosIniAbmMaterialModel()
        {
            this.Datos = new DatosIniAbmMaterial();
        }
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
        public int Id { get; set; }
    }

    public class AbmMaterialResult : Resultado
    {
        public MaterialDto Material { get; set; }

        public AbmMaterialResult()
        {
            this.Material = new MaterialDto();
        }
    }

    public class ResultIniPostMaterialModel : Resultado
    {
        public string Texto { get; set; }
    }
}