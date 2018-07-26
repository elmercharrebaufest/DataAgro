using Mastersoft.Framework.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class Acopio : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AcopioId { get; set; }
        public int NroItem { get; set; }
        public int ProveedorId { get; set; }
        public int? LocalidadId { get; set; }
        public string Coordenadas { get; set; }
        public string KMZnombre { get; set; }
        public string KMZfile { get; set; }

        public Acopio()
        {
            
        }
    }


}
   


