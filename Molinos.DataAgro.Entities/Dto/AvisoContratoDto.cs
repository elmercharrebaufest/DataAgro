using System;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class AvisoContratoDto
    {
        public int ContratoId { get; set; } // ContratoId (Primary key)
        public string RazonSocial { get; set; } // ProveedorId
        public double Cantidad { get; set; } // Cantidad
        public decimal Precio { get; set; } // Precio
        public string Moneda { get; set; } // MonedaId (length: 5) 
        public string Fecha { get; set; } // Fecha
        public string ComercialCreadorAD { get; set; }
        public string NombreApellido { get; set; }
    }
}



