
using Mastersoft.Framework.Interfaces;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class RG2300 : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string CUIT { get; set; }
        public string RazonSocial { get; set; }
        public string Categoria { get; set; }
        public string Situacion { get; set; }
        public string CBU { get; set; }
        public Nullable<System.DateTime> FechaActCBU { get; set; }
        public Nullable<System.DateTime> FechaPubInclusion { get; set; }
        public Nullable<System.DateTime> FechaPubSuspension { get; set; }
        public Nullable<System.DateTime> FechaLevSuspension { get; set; }
        public Nullable<System.DateTime> FechaNotExclusion { get; set; }
        public Nullable<System.DateTime> FechaActRegistro { get; set; }
        public string Observaciones { get; set; }
        public Nullable<System.DateTime> FechaGeneracion { get; set; }

        public RG2300()
        {
            
        }
    }


}
   


