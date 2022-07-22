using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class NuevoCorredor
    {
        public Basico basicos { get; set; }
        public Contacto contacto { get; set; }
        public Produccion produccion { get; set; }
        public List<ContactosComercial> contactocomercial { get; set; }
        public List<NuevoProveedor> proveedorCorredor { get; set; }
        public int? CorredorId { get; set; }        
    }    
}
