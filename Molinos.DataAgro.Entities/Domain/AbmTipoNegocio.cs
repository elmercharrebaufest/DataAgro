
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

using Mastersoft.Framework.Standard;

namespace Molinos.DataAgro.Entities
{


    public partial class TipoNegocio : IEntityKeyValid
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
                 oErrorMessages.Add(new ErrorMessage("El campo 'Descripción' no debe estar vacio", "Descripcion"));
            }

            return oErrorMessages.Count == 0;
        }
    }

    
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


