using System.ComponentModel.DataAnnotations;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class ProvinciaDto
    {
        public int ProvinciaId { get; set; }
        public string Nombre { get; set; }
        public bool HabilitadoVenta { get; set; }
    }
}
   



