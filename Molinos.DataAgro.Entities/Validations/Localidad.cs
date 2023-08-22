using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Validations;
using System;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class Localidad : IEntityKeyValid
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
            if (String.IsNullOrWhiteSpace(this.CodLocalidad))
            {
                oErrorMessages.Error("CodLocalidad", "El campo 'CodLocalidad' no debe estar vacio");
            }

            if (String.IsNullOrWhiteSpace(this.Nombre))
            {
                oErrorMessages.Error("Nombre", "El campo 'Nombre' no debe estar vacio");
            }

            //if (this.Provincia == null)
            //{
            //    oErrorMessages.Error("Provincia", "El campo 'Provincia' no debe estar vacio");
            //}

            return !oErrorMessages.HayErrores;
        }
    }
}