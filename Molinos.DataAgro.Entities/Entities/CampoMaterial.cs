using Mastersoft.Framework.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class CampoMaterial : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CampoMaterialid { get; set; }
        public int CampoId { get; set; }
        public int NroItem { get; set; }
        public int MaterialId { get; set; }
        public int? Hectareas { get; set; }
        public int? Toneladas { get; set; }
        public int CampañaId { get; set; }

        public CampoMaterial()
        {
            
        }
    }


}
   


