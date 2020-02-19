using System;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class LogProveedorDto
    {
        public int LogProveedorId { get; set; } // LogProveedorId (Primary key)
        public DateTime Fecha { get; set; } // Fecha
        public int ProveedorId { get; set; } // ProveedorId
        public int ComercialId { get; set; } // ComercialId
       
    }
}



