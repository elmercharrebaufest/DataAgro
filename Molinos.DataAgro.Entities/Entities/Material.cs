using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class Material
    {
        [Key]
        public int MaterialId { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public int? CampañaId { get; set; }

        [ForeignKey("CampañaId")]
        public virtual Campaña Campaña { get; set; }

        public Material()
        {
            Codigo = "";
            Descripcion = "";
        }
    }
}
   



