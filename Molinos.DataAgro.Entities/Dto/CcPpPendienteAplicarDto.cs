using System;

namespace Molinos.DataAgro.Entities.Dto
{
    public class CcPpPendienteAplicarDto
    {
        public string Proveedor { get; set; }
        public string Corredor { get; set; }
        public string AgenteCompra { get; set; }
        public string CartasPorte { get; set; }
        public string Material { get; set; }
        public string Centro { get; set; }
        public decimal Cantidad { get; set; }
        public string FechaNetoString { get; set; }
        public string FechaIngresoString { get; set; }
        public DateTime? FechaNetoDate { get; set; } //fecha de la carta de porte
        public DateTime? FechaIngresoDate { get; set; } //fecha de ingreso del camión. Normalmente es igual a la fecha neto, pero puede no serlo
        public string Almacen { get; set; }
        public bool Canje { get; set; }
        public bool CD { get; set; }
        public bool Warrant { get; set; }
        public bool Sustentable { get; set; }
        public bool EPA { get; set; }
        public string Region { get; set; }
        public string Contrato { get; set; }
        public decimal KgContrato { get; set; }
        public decimal CantidadDisponible { get; set; }
        public decimal CantidadTotal { get; set; }
    }
}