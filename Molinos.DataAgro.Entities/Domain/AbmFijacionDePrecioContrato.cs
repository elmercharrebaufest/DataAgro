
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

using Mastersoft.Framework.Standard;

namespace Molinos.DataAgro.Entities
{
    public class DatosIniAbmFijacionDePrecioContrato
    {
        public List<MaterialQry> material { get; set; }
        public List<ProveedorQry> proveedor { get; set; }
        public List<MonedaQry> moneda { get; set; }
        public List<ComercialQry> comercial { get; set; }

        public DatosIniAbmFijacionDePrecioContrato()
        {
            material = new List<MaterialQry>();
            comercial = new List<ComercialQry>();
            proveedor = new List<ProveedorQry>();
            moneda = new List<MonedaQry>();
        }

    }


    public partial class FijacionDePrecioContrato : IEntityKeyValid
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




    public class ResultIniFijacionDePrecioContrato
    {
        public List<FijacionDePrecioContratoIni> FijacionDePrecioContrato { get; set; }
    }


    public class FijacionDePrecioContratoIni
    {
        public int FijacionDePrecioContratoId { get; set; }
        public string ContratoId { get; set; }
        public int ProveedorId { get; set; }
        public string Proveedor { get; set; }
        public int? MaterialId { get; set; }
        public string Material { get; set; }
        public string MonedaId { get; set; }
        public int ComercialId { get; set; }
        public string Comercial { get; set; }
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
        public string Fecha { get; set; }
        public int? Ampliaciones { get; set; }
        public string Estado { get; set; }
        public string Observacion { get; set; }
    }

}


