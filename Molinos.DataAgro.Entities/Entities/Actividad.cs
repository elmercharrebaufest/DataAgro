
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using Mastersoft.Framework.Interfaces;

namespace Molinos.DataAgro.Entities
{
    public partial class Actividad : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ActividadId { get; set; }
        public int TipoActividadId { get; set; }
        public string Detalle { get; set; }
        public int ProveedorId { get; set; }
        public System.DateTime FechaHoraActividad { get; set; }
        public Nullable<System.DateTime> FechaHoraRecordatorio { get; set; }
        public Nullable<int> ComercialId { get; set; }
        public Nullable<int> ContactoComercialId { get; set; }
        public string asunto { get; set; }
        public Nullable<System.DateTime> FechaHoraRecordatorioFin { get; set; }

        public Actividad()
        {
            
        }
    }


}
   


