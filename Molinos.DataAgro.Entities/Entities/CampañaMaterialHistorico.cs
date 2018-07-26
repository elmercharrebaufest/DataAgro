using Mastersoft.Framework.Interfaces;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
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
