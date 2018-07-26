using Mastersoft.Framework.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class Material : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MaterialId { get; set; }

        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public int? CampañaId { get; set; }

        public Material()
        {
            this.MaterialId = 0;
            this.Codigo = "";
            this.Descripcion = "";
        }
    }
}
   



