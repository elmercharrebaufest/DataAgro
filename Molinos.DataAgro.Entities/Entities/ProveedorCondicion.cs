using Mastersoft.Framework.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class ProveedorCondicion : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ContactoCondicionId { get; set; }
        public int ProveedorId { get; set; }
        public int NroItem { get; set; }
        public int CondicionId { get; set; }

        public ProveedorCondicion()
        {
            
        }
    }


}
   


