using Mastersoft.Framework.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class ProveedorDestinatario : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ContactoDestinatarioId { get; set; }
        public int ProveedorId { get; set; }
        public int NroItem { get; set; }
        public int DestinatarioId { get; set; }

        public ProveedorDestinatario()
        {
            
        }
    }


}
   


