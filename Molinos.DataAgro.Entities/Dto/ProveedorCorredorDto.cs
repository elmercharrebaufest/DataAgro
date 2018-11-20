
using System;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class ProveedorCorredorDto
    {
        public int ProveedorId { get; set; }
        public string CUIT { get; set; }
        public string RazonSocial { get; set; }
        public string Localidad { get; set; }
        public string Provincia { get; set; }
        public int? LocalidadId { get; set; }
        public int? ProvinciaId { get; set; }
        public string LocalidadCompraNet { get; set; }
        public string ProvinciaCompraNet { get; set; }
        public int? LocalidadCompraNetId { get; set; }
        public int? ProvinciaCompraNetId { get; set; }
        public string Direccion { get; set; }
        public string CodigoPostal { get; set; }
        public int? ClasificacionCompraNetId { get; set; }
        public string ClasificacionDescripcion { get; set; }
        public int ProveedorCorredorId { get; set; }
    }
}
   


