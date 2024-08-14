using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class Localidad
    {
        [Key]
        public int LocalidadId { get; set; }
        public string CodLocalidad { get; set; }
        public string CodigoPostal { get; set; }
        public string SubCodigoPostal { get; set; }
        public string Nombre { get; set; }
        public int ProvinciaId { get; set; }
        public int? PartidoId { get; set; }
        [ForeignKey("ProvinciaId")]
        public virtual Provincia Provincia { get; set; }
        [ForeignKey("PartidoId")]
        public virtual Partido Partido { get; set; }
        public Localidad()
        {
            CodLocalidad = "";
            Nombre = "";  
        }

        public string CodigoConfirma
        {
            get
            {
                return CodigoPostal+SubCodigoPostal;
            }
        }
    }
}
   



