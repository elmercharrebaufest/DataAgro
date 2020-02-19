using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class DatosIniAbmRango
    {
        public List<MaterialCombo> Material { get; set; }
        public List<MonedaQry> Moneda { get; set; }
    }

    public class DataAbmRango : Resultado
    {
        public RangoPrecioDto Rango { get; set; }

        public DataAbmRango()
        {
            Rango = new RangoPrecioDto();
        }
    }
    
    public class ResultIniRango
    {
        public List<RangoIni> Rango { get; set; }
    }
    
    public class RangoIni
    {
        public int Id { get; set; }
        public decimal PrecioMinimo { get; set; }
        public decimal PrecioMaximo { get; set; }
        public string Material { get; set; }
        public string Moneda { get; set; }
    }
}


