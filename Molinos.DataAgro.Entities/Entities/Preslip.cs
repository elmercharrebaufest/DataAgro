
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using Mastersoft.Framework.Interfaces;

namespace Molinos.DataAgro.Entities
{
    public partial class Preslip : Entity
    {
        public int PreslipId { get; set; }
        public Nullable<int> TipoDeNegocioId { get; set; }
        public Nullable<int> MaterialId { get; set; }
        public Nullable<int> CampañaId { get; set; }
        public Nullable<decimal> Cantidad { get; set; }
        public Nullable<decimal> Precio { get; set; }
        public Nullable<System.DateTime> FechaDesde { get; set; }
        public Nullable<System.DateTime> FechaHasta { get; set; }
        public Nullable<System.DateTime> FechaDeEntrega { get; set; }
        public Nullable<int> ProveedorId { get; set; }
        public Nullable<int> EstadoPreslipId { get; set; }

        public Preslip()
        {
            
        }
    }


}
   


