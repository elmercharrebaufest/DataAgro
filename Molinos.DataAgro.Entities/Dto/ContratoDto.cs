using System;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class ContratoDto
    {
        public int ContratoId { get; set; } // ContratoId (Primary key)
        public int MaterialId { get; set; } // MaterialId
        public int TipoNegocioId { get; set; } // TipoNegocioId
        public double Cantidad { get; set; } // Cantidad
        public decimal Precio { get; set; } // Precio
        public DateTime FechaEntrega { get; set; } // FechaEntrega
        public int CampanaId { get; set; } // CampañaId
        public DateTime FechaDesde { get; set; } // FechaDesde
        public DateTime FechaHasta { get; set; } // FechaHasta
        public int ProveedorId { get; set; } // ProveedorId
        public string MonedaId { get; set; } // MonedaId (length: 5) 
        public DateTime Fecha { get; set; } // Fecha
        public int GrupoCompra { get; set; } // GrupoCompra
        public int? ComercialId { get; set; } // ComercialId
        public int? ProvinciaId { get; set; } // ProvinciaId
        public int? LocalidadId { get; set; } // LocalidadId 
        public bool? Base { get; set; } // Base
        public decimal? ImporteSustentable { get; set; } // Importe_Sustentable
        public string MonedaSustentableId { get; set; } // MonedaId_Sustentable 
        public DateTime? FechaDolarizado { get; set; } // Fecha_Dolarizado
        public int? DiasPesificado { get; set; } // Dias_Pesificado
        public bool? NoInformaSio { get; set; } // NoInformaSIO
        public bool? TrigoEspecial { get; set; } // TrigoEspecial
        public int EstadoId { get; set; } // Estado (length: 50)
        public string UsuarioId { get; set; } // UsuarioId (length: 100)
        public int? ContratoSAP { get; set; }
        public double? Ampliaciones { get; set; } // Cantidad
        public string Observacion { get; set; }
        public int ClasificacionId { get; set; }//ClasificacionId
        public int? DestinoId { get; set; }
        public int? CantidadCamiones { get; set; }
        public bool? Consignatario { get; set; }
        public bool? PlanCanje { get; set; }
        public int? CondicionFijacionId { get; set; }
        public bool? CD { get; set; }
        public bool? Warrant { get; set; }
        public bool? PagoDirectoVendedor { get; set; }
        public int? StandardDeCalidadId { get; set; }
        public int? CalidadEspecialId { get; set; }
        public decimal? ValorCalidadEspecial { get; set; }
        public bool? EstablecimientoPropio { get; set; }
        public int? BoletoId { get; set; }
        public int? BolsaId { get; set; }
        public DateTime? DesdeFijacion { get; set; }
        public DateTime? HastaFijacion { get; set; }
    }
}



