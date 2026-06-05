using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public class BoletoSapDto
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public string Caracter { get; set; }
        public int? BoletoCompraNetId { get; set; }
    }
}
