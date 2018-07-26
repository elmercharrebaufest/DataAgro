
using Mastersoft.Framework.Interfaces;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class FACACOP : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string CUIT { get; set; }
        public DateTime Fecha1 { get; set; }
        public DateTime Fecha2 { get; set; }
        public string ObservacionesEspeciales { get; set; }

        public FACACOP()
        {
            
        }
    }


}
   


