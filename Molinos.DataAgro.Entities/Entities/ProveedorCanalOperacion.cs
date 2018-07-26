using Mastersoft.Framework.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class ProveedorCanalOperacion : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ContactoCanalOperacionId { get; set; }
        public int ProveedorId { get; set; }
        public string NroItem { get; set; }
        public int CanalOperacionId { get; set; }

        public ProveedorCanalOperacion()
        {
            
        }
    }


}
   


