
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

using Mastersoft.Framework.Standard;

namespace Molinos.DataAgro.Entities
{


    public partial class Condicion : IEntityKeyValid
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
            if (String.IsNullOrWhiteSpace(this.Descripcion)) {
                 oErrorMessages.Add(new ErrorMessage("El campo 'Descripcion' no debe estar vacio", "Descripcion"));
            }

            return oErrorMessages.Count == 0;
        }
    }



  
    public class ResultIniCondicion
    {
        public List<CondicionIni> Condicion { get; set; }
    }


    public class CondicionIni
    {
        public int CondicionId { get; set; }                  
        public string Descripcion { get; set; }
        public bool? Inhabilitado { get; set; }
    }

}


