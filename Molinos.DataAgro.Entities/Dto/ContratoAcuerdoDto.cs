using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class ContratoAcuerdoDto
    {
        public int Id { get; set; }
        public string Comercial { get; set; }
        public int ComercialId { get; set; }
        public string Proveedor { get; set; }
        public int ProveedorId { get; set; }
        public string Corredor { get; set; }
        public int CorredorId { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public string Material { get; set; }
        public int MaterialId { get; set; }
        public string Destino { get; set; }
        public int DestinoId { get; set; }
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public DateTime Fecha { get; set; }
        public string FechaModificacion { get; set; }
        public string Estado { get; set; }
        public int EstadoId { get; set; }
        public string Moneda { get; set; }
        public string MonedaId { get; set; }
    }
}
