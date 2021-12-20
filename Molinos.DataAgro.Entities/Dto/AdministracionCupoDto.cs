using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class AdministracionCupoDto
    {
        public int Id { get; set; }
        public int ProveedorId { get; set; }
        public int? ComercialId { get; set; }
        public DateTime Fecha { get; set; }
        public int CantidadDeCupo { get; set; }
        public int CantidadFleteProcedencia { get; set; }
        public int CantidadDeCupoMax { get; set; }
        public int CantidadFleteProcedenciaMax { get; set; }
        public int EstadoId { get; set; }       
        public int CentroId { get; set; }
        public int ZonaId { get; set; }
        public int MaterialId { get; set; }
        public string Comercial { get; set; }
        public string Proveedor { get; set; }
        public string Centro { get; set; }
        public string Zona { get; set; }
        public string Material { get; set; }
        public string StandardDeCalidad { get; set; }
        public int TipoNegocioId { get; set; }
        public string Destinatario { get; set; }
        public int? NegocioId { get; set; }
        public int? ConfiguracionEspacioDinamicoId { get; set; }
        public bool Excedente { get; set; }
        public string Estado { get; set; }
        public string TipoAdministracionCupo { get; set; }
        public int TipoAdministracionCupoId { get; set; }
        public string Observacion { get; set; }
    }
}
