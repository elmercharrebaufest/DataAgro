
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using Mastersoft.Framework.Interfaces;

namespace Molinos.DataAgro.Entities
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



