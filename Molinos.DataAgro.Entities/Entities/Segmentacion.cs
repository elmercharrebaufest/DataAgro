
using Mastersoft.Framework.Interfaces;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class Segmentacion : Entity
    {
        public int SegmentacionId { get; set; }
        public string Descripcion { get; set; }
        public string Grupo { get; set; }

        public Segmentacion()
        {
            
        }
    }


}
   


