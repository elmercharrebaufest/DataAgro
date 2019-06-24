using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class TotalPesosDolares
    {
        public string Proveedor { get; set; }
        public string Corredor { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public string TipoNegocio { get; set; }
        public string Material { get; set; }
        public double Cantidad { get; set; }
        public double? Ampliaciones { get; set; }
        public string Campania { get; set; }
        public string Negocio { get; set; }
        public DateTime? Fecha { get; set; }
        public string GrupoCompraDescripcion { get; set; }
        public string Comercial { get; set; }
        public string ComercialCreador { get; set; }
        public string DestinoDescripcion { get; set; }
        public string Estado_Contrato { get; set; }
        public decimal TotalPesos { get; set; }
        public decimal TotalDolares { get; set; }
    }
}
