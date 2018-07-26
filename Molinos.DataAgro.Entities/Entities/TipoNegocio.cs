using Mastersoft.Framework.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class TipoNegocio : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TipoNegocioId { get; set; }
        public string Descripcion { get; set; }


        public TipoNegocio()
        {
            
        }
    }


}
   


