using Mastersoft.Framework.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class AreaInfluencia : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AreaInfluenciaId { get; set; }

        public string Descripcion { get; set; }


        public AreaInfluencia()
        {
            this.AreaInfluenciaId = 0;
            this.Descripcion = "";
  
        }
    }
}
   



