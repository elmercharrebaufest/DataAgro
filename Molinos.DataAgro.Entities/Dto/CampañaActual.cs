using System;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Validations;

namespace Molinos.DataAgro.Entities.Dto
{
    public class CampañaActual : IEntityKeyValid
    {
        public string Material { get; set; }
        public string Campaña { get; set; }

        public CampañaActual()
        {
            Material = String.Empty;
            Campaña = String.Empty;
        }

        public bool ValidateKey(Resultado oErrorMessages)
        {
            //TODO CHEQUEAR
            return !oErrorMessages.HayErrores;
        }

        public bool Validate(Resultado oErrorMessages)
        {
            return !oErrorMessages.HayErrores;
        }
    }    
}
