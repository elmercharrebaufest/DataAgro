using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class ContratoAcuerdo
    {
        [Key]
        public int Id { get; set; }
        public int? ProveedorId { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public int MaterialId { get; set; }
        public int DestinoId { get; set; }
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public DateTime Fecha { get; set; }
        public int ComercialCreadorId { get; set; }
        public string MonedaId { get; set; }
        public int? CorredorId { get; set; }
        public int EstadoId { get; set; }
        public int? StandardDeCalidadId { get; set; }
        [ForeignKey("ProveedorId")]
        public virtual Proveedor Proveedor { get; set; }
        [ForeignKey("DestinoId")]
        public virtual Centro Destino { get; set; }
        [ForeignKey("ComercialCreadorId")]
        public virtual Comercial Comercial { get; set; }
        [ForeignKey("MaterialId")]
        public virtual Material Material { get; set; }
        [ForeignKey("EstadoId")]
        public virtual EstadoContrato Estado { get; set; }
        [ForeignKey("MonedaId")]
        public virtual Moneda Moneda { get; set; }
        [ForeignKey("CorredorId")]
        public virtual Proveedor Corredor { get; set; }
        [ForeignKey("StandardDeCalidadId")]
        public virtual StandardDeCalidad StandardDeCalidad { get; set; }
        [InverseProperty("Acuerdo")]
        public virtual ICollection<Calidad> Calidad { get; set; }

    }
}
