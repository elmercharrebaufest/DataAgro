
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

using Mastersoft.Framework.Standard;

namespace Molinos.DataAgro.Entities
{
    public class DatosIniAbmLocalidad
    {
        public List<Provincia> Provincia { get; set; }
    }


    public partial class Localidad : IEntityKeyValid
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
            if (String.IsNullOrWhiteSpace(this.CodLocalidad)) {
                 oErrorMessages.Add(new ErrorMessage("El campo 'Código' no debe estar vacio", "CodLocalidad"));
            }

            if (String.IsNullOrWhiteSpace(this.Nombre)) {
                 oErrorMessages.Add(new ErrorMessage("El campo 'Localidad' no debe estar vacio", "Nombre"));
            }

            if (this.ProvinciaId == 0) {
                 oErrorMessages.Add(new ErrorMessage("El campo 'Provincia' no debe estar vacio", "ProvinciaId"));
            }

            return oErrorMessages.Count == 0;
        }
    }


    public class ParamAbmLocalidad
    {
        public string Nombre { get; set; }
        public int? ProvinciaId { get; set; }
    }


    public class ResultIniLocalidad
    {
        public List<LocalidadIni> Localidad { get; set; }
    }


    public class LocalidadIni
    {
        public Nullable<int> LocalidadId { get; set; }
        public string CodLocalidad { get; set; }
        public string Nombre { get; set; }
        public string ProNombre { get; set; }
    }

}


