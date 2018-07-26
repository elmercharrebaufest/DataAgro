using Mastersoft.Framework.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class CanalOperacion : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CanalOperacionId { get; set; }

        public string Descripcion { get; set; }

        public bool? Inhabilitado { get; set; }


        public CanalOperacion()
        {
            this.CanalOperacionId = 0;
            this.Descripcion = "";
            this.Inhabilitado = false;
        }
    }
}
   



