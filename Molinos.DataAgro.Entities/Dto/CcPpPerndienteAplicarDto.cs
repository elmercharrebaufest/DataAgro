using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class CcPpPerndienteAplicarDto
    {
        public string Proveedor { get; set; }
        public string Corredor { get; set; }
        public string AgenteCompra { get; set; }
        public string CartasPorte { get; set; }
        public string Material { get; set; }
        public string Centro { get; set; }
        public decimal Cantidad { get; set; }
        public string FechaNeto { get; set; }
        public string FechaIngreso { get; set; }
        public DateTime FechaNetoFecha { get; set; }
        public DateTime FechaIngresoFecha { get; set; }
    }
}


