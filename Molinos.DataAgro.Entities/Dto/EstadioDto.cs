using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
   public partial class EstadioDto
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public int MaterialId { get; set; }

    }
}
