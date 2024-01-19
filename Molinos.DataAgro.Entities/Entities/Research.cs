using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class Research
    {
        [Key]
        public int? ResearchId { get; set; }
        public int MaterialId { get; set; }
        public int? MaterialIdAntecesor { get; set; }
        public int? EstadioId { get; set; }
        public int? CondicionId { get; set; }
        public int? HumedadSueloId { get; set; }
        public string Comentarios { get; set; }
        public string Partido { get; set; }
        public string Localidad { get; set; }
        public string Provincia { get; set; }
        public double? Latitud { get; set; }
        public double? Longitud { get; set; }
        public int? TipoMuestraIdUno { get; set; }
        public string MedidasUno { get; set; }
        public double? PromedioMuestraUno { get; set; }
        public int? TipoMuestraIdDos { get; set; }
        public string MedidasDos { get; set; }
        public double? PromedioMuestraDos { get; set; }
        public int? TipoMuestraIdTres { get; set; }
        public string MedidasTres { get; set; }
        public double? PromedioMuestraTres { get; set; }
        public double? DistanciaHileras { get; set; }
        public double? Coeficiente { get; set; }
        public int? CampañaId { get; set; }
        public double? CapitulosGirasol { get; set; }
        public DateTime? FechaAlta { get; set; }
        public double? Rendimiento { get; set; }
        public int? TipoCargaId { get; set; }
        public string EstadoConectividad { get; set; }
        public int? IdPowerApp { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public string Author { get; set; }
        public string Editor { get; set; }
        public bool? Attachments { get; set; }
        public bool? Sincronizado { get; set; }
        public int? LocalidadId { get; set; }
        public int? ComercialId { get; set; }
        [ForeignKey("MaterialId")]
        public virtual Material Material { get; set; }
        [ForeignKey("MaterialIdAntecesor")]
        public virtual Material MaterialAntecesor { get; set; }
        [ForeignKey("EstadioId")]
        public virtual ResearchEstadio ResearchEstadio { get; set; }
        [ForeignKey("CondicionId")]
        public virtual ResearchCondicion ResearchCondicion { get; set; }
        [ForeignKey("HumedadSueloId")]
        public virtual ResearchHumedadSuelo ResearchHumedadSuelo { get; set; }
        [ForeignKey("TipoMuestraIdUno")]
        public virtual ResearchTipoMuestra ResearchTipoMuestraUno { get; set; }
        [ForeignKey("TipoMuestraIdDos")]
        public virtual ResearchTipoMuestra ResearchTipoMuestraDos { get; set; }
        [ForeignKey("TipoMuestraIdTres")]
        public virtual ResearchTipoMuestra ResearchTipoMuestraTres { get; set; }
        [ForeignKey("CampañaId")]
        public virtual Campaña Campana { get; set; }
        [ForeignKey("TipoCargaId")]
        public virtual ResearchTipoCarga ResearchTipoCarga { get; set; }
        [ForeignKey("LocalidadId")]
        public virtual Localidad LocalidadObj { get; set; }
        [ForeignKey("ComercialId")]
        public virtual Comercial Comercial { get; set; }
        public bool Eliminado { get; set; }

        [InverseProperty("Research")]
        public virtual List<ResearchAdjunto> Adjuntos { get; set; } = new List<ResearchAdjunto>();
        public int? ProvinciaId { get; set; }
        public int? PartidoId { get; set; }
        //[ForeignKey("ProvinciaId")]
        //public virtual Provincia Provincia { get; set; }
        //[ForeignKey("PartidoId")]
        //public virtual Partido Partido { get; set; }
    }

    public class ResearchAdjunto
    {
        [Key]
        public int ResearchAdjuntoId { get; set; }
        public int ResearchId { get; set; }
        public string Path { get; set; }
        public string Nombre { get; set; }
        [ForeignKey("ResearchId")]
        public virtual Research Research { get; set; }
    }
}
