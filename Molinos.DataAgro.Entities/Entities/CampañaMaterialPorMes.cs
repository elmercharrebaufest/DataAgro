
using Mastersoft.Framework.Interfaces;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class CampañaMaterialPorMes : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CampañaMaterialPorMesId { get; set; }
        public Nullable<int> NroItem { get; set; }
        public Nullable<int> Mes { get; set; }
        public Nullable<int> Año { get; set; }
        public Nullable<double> Toneladas { get; set; }
        public Nullable<int> CampañaMaterialId { get; set; }
        public Nullable<int> ComercialId { get; set; }

        public CampañaMaterialPorMes()
        {
            
        }
    }


}
   


