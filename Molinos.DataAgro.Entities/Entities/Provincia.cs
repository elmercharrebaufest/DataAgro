using Mastersoft.Framework.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class Provincia : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ProvinciaId { get; set; }

        public string Nombre { get; set; }


        public Provincia()
        {
            this.ProvinciaId = 0;
            this.Nombre = "";
  
        }
    }
}
   



