using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class CalidadEspecial
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public string CodigoSap { get; set; }
        public int MaterialId { get; set; }
        public bool? PermiteRango { get; set; }

        [ForeignKey("MaterialId")]
        public virtual Material Material { get; set; }
    }

}
   


