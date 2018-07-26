using Mastersoft.Framework.Interfaces;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class InformeComercial : Entity
    {

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int InformeComercialId { get; set; }
        public int ProveedorId { get; set; }
        public Nullable<DateTime> FechaAlta { get; set; }
        public Nullable<int> ComercialId { get; set; }
        public Nullable<bool> EmplRelDep { get; set; }
        public string EmplRelDepCant { get; set; }
        public Nullable<int> Rodados { get; set; }
        public string RodadosOtros { get; set; }
        public Nullable<int> Chacra { get; set; }
        public string ChacraOtros { get; set; }
        public string AntigActividad { get; set; }
        public string ActuacionProd { get; set; }
        public string ClienteAnt { get; set; }
        public string Comentarios { get; set; }
        public string RespuestaSap { get; set; }
        public string DomicilioReal { get; set; }
        public Nullable<int> CampañaId { get; set; }
        public Nullable<int> EstadoId { get; set; }

    }
}
