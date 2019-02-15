using Molinos.DataAgro.Entities.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class AgenteCompraDto
    {
        public int MaterialId { get; set; }
        public int TipoAgenteId { get; set; }
        public string TipoAgenteDesc { get; set; }
        public string MaterialDesc { get; set; }
        public List<OperadorCantidad> Operador {get;set;}
        public string Posicion { get; set; }
        public partial class OperadorCantidad
        {
            public int OperadorId { get; set; }
            public string OperadorDesc { get; set; }
            public double Cantidad { get; set; }
        }
        public int? ComercialCreadorId { get; set; }
    }
}

