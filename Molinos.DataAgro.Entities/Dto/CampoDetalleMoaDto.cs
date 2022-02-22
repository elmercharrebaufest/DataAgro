using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public class CampoDetalleTerceroDto
    {

        public string ProveedorCUIT { get; set; }
        public string Campania { get; set; }

        public int LocalidadId { get; set; }

        public string Latitud { get; set; }

        public string Longitud { get; set; }
        public string KMZnombre { get; set; }

        public string KMZfileBase64 { get; set; }

        public string Nombre { get; set; }
        
        public decimal ToneladasAprobadas { get; set; }

        public decimal HectareasTotales { get; set; }

        public decimal HectareasCultivables { get; set; }

        public int Id{ get; set; }
        public string Estado { get; set; }

    }

}
