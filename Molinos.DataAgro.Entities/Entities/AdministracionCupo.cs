using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class AdministracionCupo
    {
        [Key]
        public int Id { get; set; }
        public int? ProveedorId { get; set; }
        public int? ComercialId { get; set; }
        public DateTime Fecha { get; set; }
        public int CantidadCupo { get; set; }
        public int CantidadFleteProcedencia { get; set; }
        public int EstadoId { get; set; }
        public int CentroId { get; set; }
        public int ZonaId { get; set; }
        public int MaterialId { get; set; }
        public bool Excedente { get; set; }
        public bool? Fason { get; set; }
        public string Destinatario { get; set; }
        public int TipoAdministracionCupoId { get; set; }
        public int? ComercialCreadorId { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public string Observacion { get; set; }

        public int? SugerenciaCupoId { get; set; }
        public DateTime? FechaDecision { get; set; }
        public bool? ConDescarga { get; set; }

        [ForeignKey("SugerenciaCupoId")]
        public virtual SugerenciaCupo SugerenciaCupo { get; set; }

        [ForeignKey("ComercialCreadorId")]
        public virtual Comercial ComercialCreador { get; set; }
        [ForeignKey("TipoAdministracionCupoId")]
        public virtual TipoAdministracionCupo TipoAdministracionCupo { get; set; }

        [ForeignKey("ProveedorId")]
        public virtual Proveedor Proveedor { get; set; }
        [ForeignKey("ComercialId")]
        public virtual Comercial Comercial { get; set; }

        [ForeignKey("CentroId")]
        public virtual Centro Centro { get; set; }
        [ForeignKey("ZonaId")]
        public virtual ZonaCupo Zona { get; set; }
        [ForeignKey("MaterialId")]
        public virtual Material Material { get; set; }
        public string Calidad { get; set; }
        public string Motivo { get; set; }
    }
}



