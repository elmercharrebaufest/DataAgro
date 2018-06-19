
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

using Mastersoft.Framework.Standard;

namespace Molinos.DataAgro.Entities
{


    public partial class Material : IEntityKeyValid
    {
        //--------------------------------------------------------------------------------
        //   Implementacion de IEntityValid
        //--------------------------------------------------------------------------------
  
        public bool ValidateKey(List<ErrorMessage> oErrorMessages)
        {
            return oErrorMessages.Count == 0;
        }
  

        public bool Validate(List<ErrorMessage> oErrorMessages)
        {
            if (String.IsNullOrWhiteSpace(this.Codigo)) {
                 oErrorMessages.Add(new ErrorMessage("El campo 'Código' no debe estar vacio", "Codigo"));
            }

            if (String.IsNullOrWhiteSpace(this.Descripcion)) {
                 oErrorMessages.Add(new ErrorMessage("El campo 'Descripción' no debe estar vacio", "Descripcion"));
            }

            return oErrorMessages.Count == 0;
        }
    }

    
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


