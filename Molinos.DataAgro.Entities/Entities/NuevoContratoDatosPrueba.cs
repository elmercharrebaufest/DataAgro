using Mastersoft.Framework.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Molinos.DataAgro.Entities.Helpers;

namespace Molinos.DataAgro.Entities {
    public partial class NuevoContratoDatosPrueba : IEntityKeyValid {
        public bool Validate(List<ErrorMessage> oErrorMessages) {
            throw new NotImplementedException();
        }

        public bool ValidateKey(List<ErrorMessage> oErrorMessages) {
            throw new NotImplementedException();
        }

    }
    
    public class NuevaFijacion {
        public int? ProveedorId { get; set; }
        public int? ComercialId { get; set; }
        public int material { get; set; } 
        public int cantidadId { get; set; }
        public float precioId { get; set; }
        public string precioMonedaId { get; set; }
    }

   
}
