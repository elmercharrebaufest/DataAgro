using System.Collections.Generic;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Validations;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class FijacionDePrecioContrato : IEntityKeyValid
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
            return !oErrorMessages.HayErrores;
        }
    }
}


