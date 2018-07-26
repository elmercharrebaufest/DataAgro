
using Mastersoft.Framework.Interfaces;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class AcopioCampaña : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AcopioCampañaId { get; set; }
        public int AcopioId { get; set; }
        public int NroItem { get; set; }
        public Nullable<double> Toneladas { get; set; }
        public int CampañaId { get; set; }
        public Nullable<bool> HasArrendadas { get; set; }

        public AcopioCampaña()
        {

        }
    }


}



