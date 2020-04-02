using System;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class DetalleAgenteDto
    {
        public string Agente { get; set; } // AgenteId (Primary key)
        public string Operador { get; set; } //Operador
        public string Material { get; set; } // MaterialId
        public string Posicion { get; set; } //Posicion(MM.AAAA)
        public double Cantidad { get; set; } // Cantidad
        public string Precio { get; set; } // Precio
        public string Moneda { get; set; } // MonedaId (length: 5) 
        public string Fecha { get; set; } //Fecha
        public string Comercial { get; set; } // ComercialId
    }
}



