
using Mastersoft.Framework.Interfaces;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class Estado : Entity
    {
        public int EstadoId { get; set; }
        public string Descripcion { get; set; }

        public Estado()
        {
            
        }
    }
}
   
