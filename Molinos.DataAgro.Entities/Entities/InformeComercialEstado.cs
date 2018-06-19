using Mastersoft.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities
{
    public partial class InformeComercialEstado : Entity
    {

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int EstadoInformeId { get; set; }
        public string Descripcion { get; set; }

    }
}
