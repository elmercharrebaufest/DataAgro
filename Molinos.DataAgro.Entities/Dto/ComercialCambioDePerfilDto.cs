using Molinos.DataAgro.Entities.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class ComercialCambioDePerfilDto
    {
        public int ComercialId { get; set; }
        public string Apellido { get; set; }
        public string Nombres { get; set; }
        public int? EmpleadorACargoId { get; set; }
        public int? GrupoDeComprasId { get; set; }
        public string NombreCompleto { get { return Apellido.ToUpper() + " " + Nombres.ToUpper(); } }
        public int? ComercialSuplenteId { get; set; }
        public string EmpleadorACargo { get; set; }
        public string GrupoDeCompras { get; set; }
    }
}