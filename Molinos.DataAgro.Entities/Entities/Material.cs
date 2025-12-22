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
        public int? CampaniaTableroId { get; set; }
        public int? CodigoEspecie { get; set; }

        public decimal? IVA { get; set; }

        public int DestinoId { get; set; }

        [ForeignKey("CampañaId")]
        public virtual Campaña Campaña { get; set; }
        [ForeignKey("CampaniaTableroId")]
        public virtual Campaña CampaniaTablero { get; set; }

        [ForeignKey("DestinoId")]
        public virtual Centro Destino { get; set; }
        public Material()
        {
            Codigo = "";
            Descripcion = "";
        }
    }
}




