using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class DatosIniAbmContratoAcuerdo
    {
        public List<ContratoAcuerdoCombo> ContratoAcuerdo { get; set; }
    }

    public class DatosIniComboContratoAcuerdo
    {
        public List<MaterialQry> material { get; set; }
        public List<ComercialQry> comercial { get; set; }
        public List<CentroQry> destino { get; set; }
        public List<MonedaQry> moneda { get; set; }

        public DatosIniComboContratoAcuerdo()
        {

            material = new List<MaterialQry>();
            destino = new List<CentroQry>();
            comercial = new List<ComercialQry>();
            moneda = new List<MonedaQry>();

        }
    }
    public class AbmContratoAcuerdoParam
    {
        public int Id { get; set; }
    }
    public class DataAbmContratoAcuerdo : Resultado
    {
        public ContratoAcuerdoDto ContratoAcuerdo { get; set; }

        public DataAbmContratoAcuerdo()
        {
            ContratoAcuerdo = new ContratoAcuerdoDto();
        }
    }

    public class ResultIniContratoAcuerdo
    {
        public List<ContratoAcuerdoIni> ContratoAcuerdo { get; set; }
    }

    public class ContratoAcuerdoIni
    {
        public int Id { get; set; }
        public string Comercial { get; set; }
        public decimal Precio { get; set; }
        public string Proveedor { get; set; }
        public int Cantidad { get; set; }
        public string Material { get; set; }
        public string Destino { get; set; }
        public DateTime? Fecha { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public decimal PorcentajeCargado { get; set; }
        public string Estado { get; set; }
        public int EstadoId { get; set; }
        public string Moneda { get; set; }
        public string MonedaId { get; set; }
    }
}
