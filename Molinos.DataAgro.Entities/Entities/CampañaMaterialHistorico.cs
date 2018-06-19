using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Mastersoft.Framework.Interfaces;

namespace Molinos.DataAgro.Entities
{
    public partial class CampañaMaterialHistorico : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CampañaMaterialHistoricoId { get; set; }
        public int CampañaId { get; set; }
        public int MaterialId { get; set; }
        public DateTime Fecha { get; set; }

        public CampañaMaterialHistorico()
        {

        }

    }
}
