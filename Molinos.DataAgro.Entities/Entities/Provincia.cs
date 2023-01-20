using System.ComponentModel.DataAnnotations;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class Provincia
    {
        [Key]
        public int ProvinciaId { get; set; }

        public string Nombre { get; set; }

        public int Orden { get; set; }
        public bool HabilitadoVenta { get; set; }

        public Provincia()
        {
            this.Nombre = "";
        }
    }
}
   



