
using System;
using System.Collections.Generic;

using Mastersoft.Framework.Standard;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class Comercial : IEntityKeyValid
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
            if (String.IsNullOrWhiteSpace(this.Apellido)) {
                 oErrorMessages.Add(new ErrorMessage("El campo 'Apellido' no debe estar vacio", "Apellido"));
            }

            if (String.IsNullOrWhiteSpace(this.Nombres)) {
                 oErrorMessages.Add(new ErrorMessage("El campo 'Nombres' no debe estar vacio", "Nombres"));
            }

            if (this.PerfilId == 0) {
                 oErrorMessages.Add(new ErrorMessage("El campo 'Perfil' debe estar informado", "PerfilId"));
            }
            
            return oErrorMessages.Count == 0;
        }
    }
}


