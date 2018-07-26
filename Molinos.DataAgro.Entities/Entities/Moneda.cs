using Mastersoft.Framework.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class Moneda : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string MonedaId { get; set; }
        public string Descripcion { get; set; }
       

        public Moneda()
        {
            
        }
    }    
}
   


