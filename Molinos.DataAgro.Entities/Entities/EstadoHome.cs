using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class EstadoHome
    {
        [Key]
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public string Color { get; set; }
    }
}
