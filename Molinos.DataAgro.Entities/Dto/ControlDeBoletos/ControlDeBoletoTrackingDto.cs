using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto.ControlDeBoletos
{
    public class ControlDeBoletoTrackingDto
    {
        public int Id { get; set; }
        public int ControlDeBoletosId { get; set; }
        public string Accion { get; set; }
        public string Resultado { get; set; }
        public string ValorAnterior { get; set; }
        public string ValorNuevo { get; set; }
        public string Cargo { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public DateTime FechaHora { get; set; }
        public DateTime FechaModificacion { get; set; }
        public string UsuarioModificacion { get; set; }
    }
}
