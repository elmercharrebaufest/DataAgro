using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class ResearchDto
    {
        public int? Id { get; set; }
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
        public bool Attachments { get; set; }
        public List<ResearchAdjuntoDto> Adjuntos { get; set; }
        public bool? Sincronizado { get; set; }
        public int? LocalidadId { get; set; }
        public string Campaña { get; set; }
        public string MaterialAntecesor { get; set; }
        public string Material { get; set; }
        public int? PartidoId { get; set; }
        public int? ProvinciaId { get; set; }
        public int? ComercialId { get; set; }
        public string Comercial { get; set; }
        public string TipoMuestraUno { get; set; }
        public string TipoMuestraDos { get; set; }
        public string TipoMuestraTres { get; set; }
        public string Estadio { get; set; }
        public string Condicion { get; set; }
        public string HumedadSuelo { get; set; }
        public bool Eliminado { get; set; }
        public DateTime? Fecha { get; set; }
        public string TipoCarga { get; set; }
        public double? RendimientoCalculado { get; set; }
    }

    public class ResearchAdjuntoDto
    {
        public int ResearchAdjuntoId { get; set; }
        public int ResearchId { get; set; }
        public string Path { get; set; }
        public string Nombre { get; set; }
        public string Extension { get { return System.IO.Path.GetExtension(Nombre); } }
    }
}
