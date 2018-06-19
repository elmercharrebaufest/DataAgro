
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

using Mastersoft.Framework.Standard;

namespace Molinos.DataAgro.Entities
{


    public partial class Moneda : IEntityKeyValid
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
            if (String.IsNullOrWhiteSpace(this.MonedaId)) {
                 oErrorMessages.Add(new ErrorMessage("El campo 'Código' no debe estar vacio", "Codigo"));
            }

            if (String.IsNullOrWhiteSpace(this.Descripcion)) {
                 oErrorMessages.Add(new ErrorMessage("El campo 'Descripción' no debe estar vacio", "Descripcion"));
            }

            return oErrorMessages.Count == 0;
        }
    }

    
    public class ParamAbmMoneda
    {
        public string MonedaId { get; set; }
        public string Descripcion { get; set; }
    }


    public class ResultIniMoneda
    {
        public List<MonedaIni> Moneda { get; set; }
    }


    public class MonedaIni
    {
        public string MonedaId { get; set; }                  
                
        public string Descripcion { get; set; }        
           
    }

}


