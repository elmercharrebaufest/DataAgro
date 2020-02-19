using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Validations;
using System;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class Provincia : IEntityKeyValid
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
            if (String.IsNullOrWhiteSpace(this.Nombre))
            {
                oErrorMessages.Error("Nombre", "El campo 'Nombre' no debe estar vacio");
            }

            return !oErrorMessages.HayErrores;
        }
    }

}


