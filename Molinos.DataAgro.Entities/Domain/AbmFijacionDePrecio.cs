
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

using Mastersoft.Framework.Standard;

namespace Molinos.DataAgro.Entities
{
    public class DatosIniAbmFijacionDePrecio
    {
        public List<MaterialCombo> Material { get; set; }
    }


    public partial class FijacionDePrecio : IEntityKeyValid
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



  
    public class ResultIniFijacionDePrecio
    {
        public List<FijacionDePrecioIni> FijacionDePrecio { get; set; }
    }


    public class FijacionDePrecioIni
    {
        public int FijacionId { get; set; }                  
        public string MatDescripcion { get; set; }                  
        public Nullable<decimal> Precio { get; set; }                  
        public Nullable<System.DateTime> Fecha { get; set; }                  
        public Nullable<int> ProveedorId { get; set; }                  
    }

}


