using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public class PartidoDto
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public int ProvinciaId { get; set; }
        public virtual string Provincia { get; set; }
    }
}
