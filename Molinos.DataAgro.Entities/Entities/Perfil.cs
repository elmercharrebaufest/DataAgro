
using Mastersoft.Framework.Interfaces;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class Perfil : Entity
    {
        public int PerfilId { get; set; }
        public string Descripcion { get; set; }

        public Perfil()
        {
        }
    }
}
   
