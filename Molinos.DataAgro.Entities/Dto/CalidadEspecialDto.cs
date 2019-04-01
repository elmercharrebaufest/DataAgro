using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class CalidadEspecialDto
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public string CodigoSap { get; set; }
        public int MaterialId { get; set; }
    }

}
   


