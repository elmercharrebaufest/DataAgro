using Mastersoft.Framework.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class EstadoContrato : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int EstadoContratoId { get; set; }
        public string Descripcion { get; set; }
        public int Orden { get; set; }
    }
}
