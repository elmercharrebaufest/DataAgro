using Mastersoft.Framework.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class ProveedorComercial : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ProveedorComercialId { get; set; }
        public int ProveedorId { get; set; }
        public int NroItem { get; set; }
        public int ComercialId { get; set; }

        public ProveedorComercial()
        {
            
        }
    }


}
   


