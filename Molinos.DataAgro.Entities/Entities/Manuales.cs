using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class Manuales
    {
        [Key]
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string Path { get; set; }
        public DateTime FechaUltimaActualizacion { get; set; }
        public int Version { get; set; }
        public int CantidadVisitas { get; set; }
    }
}
