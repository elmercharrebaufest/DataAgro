using System;

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
        public double Cantidad { get; set; }
        public decimal Precio { get; set; }
        public string Material { get; set; }
        public int MaterialId { get; set; }
        public string Destino { get; set; }
        public int DestinoId { get; set; }
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public DateTime Fecha { get; set; }
        public string FechaModificacionDesde { get; set; }
        public string FechaModificacion { get; set; }
        public string Estado { get; set; }
        public int EstadoId { get; set; }
        public string Moneda { get; set; }
        public string MonedaId { get; set; }
        public int? ProvinciaId { get; set; }
        public int? LocalidadId { get; set; }
        //public bool? ChequeElectronico { get; set; }
        public bool Sustentable { get; set; }
        public decimal? ImporteSustentable { get; set; }
        public string MonedaSustentableId { get; set; }
        public int? SustentableTipoDBId { get; set; }
        public bool EPA { get; set; }
        public bool EUDR { get; set; }
        public bool? ConDescarga { get; set; }
        public bool? MercsDeposito { get; set; }
        public bool? Consignatario { get; set; }
        public bool? PlanCanje { get; set; }
    }
}
