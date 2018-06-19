
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

using Mastersoft.Framework.Standard;

namespace Molinos.DataAgro.Entities
{


    public partial class Provincia : IEntityKeyValid
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
            if (String.IsNullOrWhiteSpace(this.Nombre)) {
                 oErrorMessages.Add(new ErrorMessage("El campo 'Nombre' no debe estar vacio", "Nombre"));
            }

            return oErrorMessages.Count == 0;
        }
    }



  
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


