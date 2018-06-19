using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities
{
    public class ReporteProveedor
    {
        public int ProveedorId { get; set; }
		public string CUIT { get; set; }
        public string RazonSocial { get; set; }
        public int? EstadoId { get; set; }
        public string Comerciales { get; set; }
        public string Estado { get; set; }
        
        public ReporteProveedor()
        {
        }
    }
}

