using System.Collections.Generic;

using Mastersoft.Framework.Standard;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class FijacionDePrecioContrato : IEntityKeyValid
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
            return oErrorMessages.Count == 0;
        }
    }
}


