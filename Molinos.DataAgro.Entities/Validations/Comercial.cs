using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Validations;
using System;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class Comercial : IEntityKeyValid
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
            if (String.IsNullOrWhiteSpace(this.Apellido))
            {
                oErrorMessages.Error("Apellido", "El campo 'Apellido' no debe estar vacío.");
            }

            if (String.IsNullOrWhiteSpace(this.Nombres))
            {
                oErrorMessages.Error("Nombres", "El campo 'Nombres' no debe estar vacío.");
            }

            return !oErrorMessages.HayErrores;
        }
    }
}