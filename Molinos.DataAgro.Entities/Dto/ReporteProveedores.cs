using Molinos.DataAgro.Entities.Dto;
using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{

    public class ReporteProveedoresModel
    {
        public List<ReporteProveedores> tablero { get; set; } = new List<ReporteProveedores>();

    }

    public class ReporteProveedores
    {
        public int ComercialId { get; set; }
        public int ProveedorId { get; set; }
        public string Apellido { get; set; }
        public string Nombres { get; set; }
        public string RazonSocial { get; set; }
        public string Cuit { get; set; }
        public string EstadoHome { get; set; }
        public string Estado { get; set; }
        public string ComercialAsignado { get { return Apellido.ToUpper() + " " + Nombres.ToUpper(); } }
    }
   
}