using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Entities
{
    public class ControlDeBoletosPreCertificacion
    {
        [Key]
        public int Id { get; set; }

        public int ControlDeBoletosId { get; set; }

        public string Fijacion { get; set; }
        public string Oblea { get; set; }

        public int TipoObleaId { get; set; }
        public int BolsaCompraNetId { get; set; }

        public DateTime FechaCertificacion { get; set; }
        public DateTime FechaVencimiento { get; set; }

        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }

        // 🔹 Navigation
        public virtual ControlDeBoletos ControlDeBoletos { get; set; }
        public virtual TipoOblea TipoOblea { get; set; }
        public virtual BolsaCompraNet BolsaCompraNet { get; set; }
    }
}
