using System.ComponentModel.DataAnnotations;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class CondicionDePagoVentaDto
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public bool? CondicionFijacion { get; set; }
        public bool? CondicionPesificado { get; set; }
    }
}
   



