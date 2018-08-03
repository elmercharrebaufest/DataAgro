using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Validations;
using System;

namespace Molinos.DataAgro.Entities.Dto
{
    public class RiesgoComercial : IEntityKeyValid
    {
        public string CUIT { get; set; }
        public string RiesgoComercialDesc { get; set; }

        public RiesgoComercial()
        {
            CUIT = String.Empty;
            RiesgoComercialDesc = String.Empty;
        }

        public bool ValidateKey(Resultado oErrorMessages)
        {
            //TODO
            throw new NotImplementedException();
        }

        public bool Validate(Resultado oErrorMessages)
        {
            throw new NotImplementedException();
        }
    }
}
