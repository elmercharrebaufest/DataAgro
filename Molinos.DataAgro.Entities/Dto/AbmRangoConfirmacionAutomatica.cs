using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class DatosIniAbmRangoConfirmacionAutomatica
    {
        public List<MaterialCombo> Material { get; set; }
        public List<MonedaQry> Moneda { get; set; }
        public List<ZonaQry> Zona { get; set; }
    }

    public class DataAbmRangoConfirmacionAutomatica : Resultado
    {
        public RangoConfirmacionAutomaticaDto Rango { get; set; }

        public DataAbmRangoConfirmacionAutomatica()
        {
            Rango = new RangoConfirmacionAutomaticaDto();
        }
    }
    
    public class ResultIniRangoConfirmacionAutomatica
    {
        public List<RangoConfirmacionAutomaticaIni> Rango { get; set; }
    }
    
    public class RangoConfirmacionAutomaticaIni
    {
        public int Id { get; set; }
        public decimal PrecioMinimo { get; set; }
        public decimal PrecioMaximo { get; set; }
        public string Material { get; set; }
        public string Moneda { get; set; }
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public int ZonaId { get; set; }
        public string Zona { get; set; }
        public int Cantidad { get; set; }
        public int DesdeMes { get; set; }
        public int DesdeAnio { get; set; }
        public int HastaMes { get; set; }
        public int HastaAnio { get; set; }
    }
}


