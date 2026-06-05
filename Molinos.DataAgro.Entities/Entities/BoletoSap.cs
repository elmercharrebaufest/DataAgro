using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Entities
{
    public class BoletoSap
    {
        public int Id { get; set; }

        public string Descripcion { get; set; }

        public string Caracter { get; set; }

        public int? BoletoCompraNetId { get; set; }
        public virtual BoletoCompraNet BoletoCompraNet { get; set; }
    }
}
