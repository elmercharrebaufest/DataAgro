
using Mastersoft.Framework.Interfaces;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class FijacionDePrecioContrato : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int FijacionDePrecioContratoId { get; set; }
        public int ContratoId { get; set; }
        public int ProveedorId { get; set; }
        public int? MaterialId { get; set; }
        public string MonedaId { get; set; }
        public int ComercialId { get; set; }
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
        public DateTime Fecha { get; set; }
        public int? Ampliaciones { get; set; }
        public int Estado { get; set; }
        public string Observacion { get; set; }
        public FijacionDePrecioContrato()
        {
            this.FijacionDePrecioContratoId = 0;
        }
    }
}

