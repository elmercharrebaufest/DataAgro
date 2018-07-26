using Mastersoft.Framework.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class Objetivo : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ObjetivoId { get; set; }
        public int CampañaId { get; set; }
        public int NroItem { get; set; }
        public int ProveedorId { get; set; }
        public int MaterialId { get; set; }
        public double ToneladasObjetivos { get; set; }
        

        public Objetivo()
        {

        }
    }


}



