
using Mastersoft.Framework.Interfaces;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class ComercialZona : Entity
    {
        public int ComercialZonaId { get; set; }
        public int ComercialId { get; set; }
        public int NroItem { get; set; }
        public int ZonaId { get; set; }

        public ComercialZona()
        {
            
        }
    }


}
   


