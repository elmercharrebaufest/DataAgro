using System;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class DetalleContratoDto
    {
        public string Contrato { get; set; } // ContratoId (Primary key)
        public string RazonSocial { get; set; } // ProveedorId
        public string Cuit { get; set; }
        public string Material { get; set; } // MaterialId
        public string TipoNegocio { get; set; } // TipoNegocioId
        public string Comercial { get; set; } // ComercialId
        public string Cantidad { get; set; } // Cantidad
        public string CantidadCamiones { get; set; }
        public string Campana { get; set; } // CampañaId
        public string FechaDesde { get; set; } // FechaDesde
        public string FechaHasta { get; set; } // FechaHasta
        public string Precio { get; set; } // Precio
        public string PrecioNeto { get; set; } // PrecioBase
        public string Moneda { get; set; } // MonedaId (length: 5) 
        public string Fecha { get; set; } // Fecha
        public string Provincia { get; set; } // ProvinciaId
        public string Localidad { get; set; } // LocalidadId 
        public string Boleto { get; set; }
        public string Bolsa { get; set; }
        public string Destino { get; set; }
        public string CondicionFijacion { get; set; }
        public string DesdeFijacion { get; set; }
        public string HastaFijacion { get; set; }
        public string Base { get; set; } // Base
        public string ImporteSustentable { get; set; } // Importe_Sustentable
        public string FechaDolarizado { get; set; } // Fecha_Dolarizado
        public string DiasPesificado { get; set; } // Dias_Pesificado
        public string NoInformaSio { get; set; } // NoInformaSIO                
        public string Ampliaciones { get; set; } // Cantidad
        public string Consignatario { get; set; }
        public string PlanCanje { get; set; }
        public string Pago { get; set; }
        public string CalidadEspecial { get; set; }
        public string EstablecimientoPropio { get; set; }
        public string Observacion { get; set; }

    }
}



