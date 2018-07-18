using System;
using System.Collections.Generic;

using Mastersoft.Framework.Standard;

namespace Molinos.DataAgro.Entities
{
    public class DatosIniAbmCentro
    {
        public List<CentroCombo> Centro { get; set; }
    }

    public class DataAbmCentro
    {
        public List<MSErrorMessage> Errores { get; set; }
        public Centro Centro { get; set; }

        public DataAbmCentro()
        {
            Centro = new Centro();
            Errores = new List<MSErrorMessage>();
        }
    }

    public partial class Centro : IEntityKeyValid
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
            if (String.IsNullOrWhiteSpace(this.Descripcion)) {
                 oErrorMessages.Add(new ErrorMessage("El campo 'Descripcion' no debe estar vacio", "Descripcion"));
            }

            if (String.IsNullOrWhiteSpace(this.CodigoSap)) {
                 oErrorMessages.Add(new ErrorMessage("El campo 'CodigoSap' no debe estar vacio", "CodigoSap"));
            }
            
            return oErrorMessages.Count == 0;
        }
    }
    
    public class ResultIniCentro
    {
        public List<CentroIni> Centro { get; set; }
    }
    
    public class CentroIni
    {
        public int Id { get; set; }                  
        public string Descripcion { get; set; }                  
        public string CodigoSap { get; set; }               
    }
}


