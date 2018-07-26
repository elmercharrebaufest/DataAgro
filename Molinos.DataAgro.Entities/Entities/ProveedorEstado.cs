using Mastersoft.Framework.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class ProveedorEstado : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ProveedorEstadoId { get; set; }
        public int ProveedorId { get; set; }
        public int EstadoId { get; set; }
        public int ComercialId { get; set; }

        public ProveedorEstado()
        {

        }
    }


}



