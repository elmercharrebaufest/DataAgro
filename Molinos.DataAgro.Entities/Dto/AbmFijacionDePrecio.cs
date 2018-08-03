
using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
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

    public class DatosIniAbmFijacionDePrecio
    {
        public List<MaterialCombo> Material { get; set; }
    }

    public class ResultIniFijacionDePrecioContrato
    {
        public List<FijacionDePrecioContratoIni> FijacionDePrecioContrato { get; set; }
    }


    public class FijacionDePrecioContratoIni
    {
        public int FijacionDePrecioContratoId { get; set; }
        public int? ContratoId { get; set; }
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


