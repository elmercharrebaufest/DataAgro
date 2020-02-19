
using System;
using System.Collections.Generic;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Validations;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class Condicion : IEntityKeyValid
    {
        //--------------------------------------------------------------------------------
        //   Implementacion de IEntityValid
        //--------------------------------------------------------------------------------
  
        public bool ValidateKey(Resultado oErrorMessages)
        {
            return !oErrorMessages.HayErrores;
        }
  

        public bool Validate(Resultado oErrorMessages)
        {
            if (String.IsNullOrWhiteSpace(this.Descripcion))
            {
                oErrorMessages.Error("Descripcion", "El campo 'Descripción' no debe estar vacio");
            }

            return !oErrorMessages.HayErrores;
        }
    }
}


