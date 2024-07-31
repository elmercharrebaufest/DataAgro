using System;

namespace Molinos.DataAgro.Entities.Dto
{
    public class NegocioParaSolicitarCupo
    {
        public int NegocioId { get; set; }
        public string ContratoSAP { get; set; }
        public int ProveedorId {  get; set; }
        public string RazonSocialProveedor {  get; set; }
        public int EstadoNegocioId { get; set; }
        public string EstadoNegocio {  get; set; }
        public int MaterialId { get; set; }
        public double KgPendientes { get; set; }
        public int CuposRestantes { get; set; }
        public DateTime FechaHasta { get; set; }
    }
}
