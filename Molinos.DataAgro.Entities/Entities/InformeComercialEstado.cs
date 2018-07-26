using Mastersoft.Framework.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class InformeComercialEstado : Entity
    {

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int EstadoInformeId { get; set; }
        public string Descripcion { get; set; }

    }
}
