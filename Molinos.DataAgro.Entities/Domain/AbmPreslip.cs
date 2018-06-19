
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

using Mastersoft.Framework.Standard;

namespace Molinos.DataAgro.Entities
{
    public class DatosIniAbmPreslip
    {
        public List<TipoDeNegocio> TipoDeNegocio { get; set; }
        public List<MaterialCombo> Material { get; set; }
        public List<Campaña> Campaña { get; set; }
    }


    public partial class Preslip : IEntityKeyValid
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



  
    public class ResultIniPreslip
    {
        public List<PreslipIni> Preslip { get; set; }
    }


    public class PreslipIni
    {
        public int PreslipId { get; set; }                  
        public string TdnDescripcion { get; set; }                  
        public string MatDescripcion { get; set; }                  
        public string CamDescripcion { get; set; }                  
        public Nullable<decimal> Cantidad { get; set; }                  
        public Nullable<decimal> Precio { get; set; }                  
        public Nullable<System.DateTime> FechaDesde { get; set; }                  
        public Nullable<System.DateTime> FechaHasta { get; set; }                  
        public Nullable<System.DateTime> FechaDeEntrega { get; set; }                  
        public Nullable<int> ProveedorId { get; set; }                  
        public Nullable<int> EstadoPreslipId { get; set; }                  
    }

}


