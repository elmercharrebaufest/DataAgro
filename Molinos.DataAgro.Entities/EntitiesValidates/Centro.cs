using System;
using System.Collections.Generic;

using Mastersoft.Framework.Standard;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class Centro : IEntityKeyValid
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

            if (String.IsNullOrWhiteSpace(this.CodigoSap)) {
                 oErrorMessages.Add(new ErrorMessage("El campo 'CodigoSap' no debe estar vacio", "CodigoSap"));
            }
            
            return oErrorMessages.Count == 0;
        }
    }

}


