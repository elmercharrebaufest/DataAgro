using Mastersoft.Framework.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class Condicion : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CondicionId { get; set; }

        public string Descripcion { get; set; }

        public bool? Inhabilitado { get; set; }

        public Condicion()
        {
            this.CondicionId = 0;
            this.Descripcion = "";
            this.Inhabilitado = false;
        }
    }
}
   



