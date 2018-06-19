
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

using Mastersoft.Framework.Standard;

namespace Molinos.DataAgro.Entities
{
    public class DatosIniAbmComercial
    {
        public List<ComercialCombo> Comercial { get; set; }
        public List<Perfil> Perfil { get; set; }
        public List<GrupoDeCompras> GrupoDeCompras { get; set; }
    }

    public class DataAbmComercial
    {
        public List<MSErrorMessage> Errores { get; set; }
        public List<ComercialCombo> Comercial { get; set; }

        public DataAbmComercial()
        {
            Comercial = new List<ComercialCombo>();
            Errores = new List<MSErrorMessage>();
        }

    }


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



  
    public class ResultIniComercial
    {
        public List<ComercialIni> Comercial { get; set; }
    }


    public class ComercialIni
    {
        public int ComercialId { get; set; }                  
        public string Apellido { get; set; }                  
        public string Nombres { get; set; }                  
        public string PerDescripcion { get; set; }                  
    }

}


