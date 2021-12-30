using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class Centro
    {
        [Key]
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public string CodigoSap { get; set; }
        public bool Acopio { get; set; }
        public bool ValidaRedespacho { get; set; }
        public int? LocalidadId { get; set; }
        public string CodigoPostal { get; set; }
        public string Direccion { get; set; }
        public bool Comision { get; set; }
        public bool CargaNegocios { get; set; }
        public bool CargaCupos { get; set; }

        [ForeignKey("LocalidadId")]
        public virtual Localidad Localidad { get; set; }


    }

}
   


