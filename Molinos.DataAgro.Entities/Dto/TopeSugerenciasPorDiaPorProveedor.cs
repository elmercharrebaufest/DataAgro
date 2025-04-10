
using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{     
    public class TopeSugerenciasPorDiaPorProveedor
    {
        public int ProveedorId { get; set; }                  
        public DateTime Fecha { get; set; }
        public int Disponible { get; set; }
        public int Ingremental { get; set; }
    }

}


