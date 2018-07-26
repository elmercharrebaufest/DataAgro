
using System;
using System.Collections.Generic;

using Mastersoft.Framework.Standard;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class Localidad : IEntityKeyValid
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
            if (String.IsNullOrWhiteSpace(this.CodLocalidad)) {
                 oErrorMessages.Add(new ErrorMessage("El campo 'Código' no debe estar vacio", "CodLocalidad"));
            }

            if (String.IsNullOrWhiteSpace(this.Nombre)) {
                 oErrorMessages.Add(new ErrorMessage("El campo 'Localidad' no debe estar vacio", "Nombre"));
            }

            if (this.ProvinciaId == 0) {
                 oErrorMessages.Add(new ErrorMessage("El campo 'Provincia' no debe estar vacio", "ProvinciaId"));
            }

            return oErrorMessages.Count == 0;
        }
    }
}


