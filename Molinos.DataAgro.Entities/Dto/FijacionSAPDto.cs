using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
    {
    public class FijacionSAPDto
    {
        public string FijacionSAP { get; set; }
        public string ZLSCH { get; set; }
        public string CUENTA_MRP { get; set; }

        public decimal Cantidad { get; set; } // Cantidad
        public decimal Precio { get; set; } // Precio      
        public string FechaDesde { get; set; } // FechaDesde
        public string FechaHasta { get; set; } // FechaHasta
        public string Fecha { get; set; } // Fecha
        public int? DiasPesificado { get; set; } // Dias_Pesificado
        public string TrigoEspecial { get; set; } // TrigoEspecial
        public string ContratoSAP { get; set; }
        public decimal? Ampliaciones { get; set; } // Cantidad
        public string Observacion { get; set; }
        public string Pizarra { get; set; }
        public decimal? PrecioNeto { get; set; }
        public bool? PagoDiferido { get; set; }
        public string Posicion { get; set; }
        public string MotivoRechazo { get; set; }
        public string Centro { get; set; }
        public string CuitCorredor { get; set; }
        public int DiasDiferimiento { get; set; }
        public string Material { get; set; }
        public string Moneda { get; set; }
        public List<AperturaPrecioSap> Apertura { get; set; }
        public string Cosecha { get; set; }
        public string Proveedor { get; set; }
        public string Especial { get; set; }
        public string Comercial { get; set; }
        public string FechaOperacion { get; set; }
        public string Canje { get; set; }
        public string HORAACT { get; set; }
        public string FechaCreacion { get; set; }
        public List<FijacionVirtualSAPDto> FijacionVirtuales { get; set; } = new List<FijacionVirtualSAPDto>();
        public string Virtual { get; set; }
        public string ComercialCreador { get; set; }
    }
    
    public class FijacionVirtualSAPDto
    {        
        public string NumeroFijacionVirtual { get; set; } 
        public int Cantidad { get; set; }

    }

 
}

