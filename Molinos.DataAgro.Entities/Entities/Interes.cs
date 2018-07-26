using Mastersoft.Framework.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public class Interes : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int InteresId { get; set; }

        public string Descripcion { get; set; }


        public Interes()
        {
            this.InteresId = 0;
            this.Descripcion = "";

        }
    }
}
