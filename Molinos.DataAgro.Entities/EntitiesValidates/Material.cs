
using System;
using System.Collections.Generic;

using Mastersoft.Framework.Standard;

namespace Molinos.DataAgro.Entities.Entities
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
}


