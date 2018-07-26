using Mastersoft.Framework.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class Campaña : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CampañaId { get; set; }
        public string Descripcion { get; set; }

        public Campaña()
        {
            
        }
    }


}
   


