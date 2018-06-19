
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using Mastersoft.Framework.Interfaces;
using Molinos.DataAgro.Entities.Common.Enums;

namespace Molinos.DataAgro.Entities
{
    public partial class Contrato : Entity
    {

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ContratoId { get; set; } // ContratoId (Primary key)
        public int MaterialId { get; set; } // MaterialId
        public int TipoNegocioId { get; set; } // TipoNegocioId
        public double Cantidad { get; set; } // Cantidad
        public decimal Precio { get; set; } // Precio
        public System.DateTime FechaEntrega { get; set; } // FechaEntrega
        public int CampanaId { get; set; } // CampañaId
        public System.DateTime FechaDesde { get; set; } // FechaDesde
        public System.DateTime FechaHasta { get; set; } // FechaHasta
        public int ProveedorId { get; set; } // ProveedorId
        public string MonedaId { get; set; } // MonedaId (length: 5)
        public System.DateTime Fecha { get; set; } // Fecha
        public int GrupoCompra { get; set; } // GrupoCompra
        public int? ComercialId { get; set; } // ComercialId
        public int? ProvinciaId { get; set; } // ProvinciaId
        public int? LocalidadId { get; set; } // LocalidadId
        public bool? Base { get; set; } // Base
        public decimal? ImporteSustentable { get; set; } // Importe_Sustentable
        public string MonedaIdSustentable { get; set; } // MonedaId_Sustentable
        public System.DateTime? FechaDolarizado { get; set; } // Fecha_Dolarizado
        public int? DiasPesificado { get; set; } // Dias_Pesificado
        public bool? NoInformaSio { get; set; } // NoInformaSIO
        public bool? TrigoEspecial { get; set; } // TrigoEspecial
        public int Estado { get; set; } // Estado (length: 50)
        public string UsuarioId { get; set; } // UsuarioId (length: 100)
        public int? ContratoSAP { get; set; }
        public double? Ampliaciones { get; set; } // Cantidad
        public string Observacion { get; set; }
        public Contrato()
        {
            this.Cantidad = 0;
            this.Precio = 0;
            this.CampanaId = 0;
            this.MonedaId = "";
            this.ComercialId = 0;
            this.ProvinciaId = 0;
            this.LocalidadId = 0;
            this.Base = false;
            this.ImporteSustentable = 0;
            this.MonedaIdSustentable = "";
            this.NoInformaSio = false;
            this.TrigoEspecial = false;
            this.Estado = (int)EnumEstadoContrato.Pendiente;
            this.ContratoSAP = 0;
            this.Ampliaciones = 0;
        }
    }

    public class DatosFiltro
    {
        public int? ProveedorId { get; set; }
        public int ContratoSap { get; set; }
        public int ComercialId { get; set; }
        public int MaterialId { get; set; }
        public decimal Precio { get; set; }
        public int CampaniaId { get; set; }
        public string FechaDesde { get; set; }
        public string FechaHasta { get; set; }
        public int EstadoId { get; set; }
        public string Usuario { get; set; }

        public DatosFiltro()
        {
            ProveedorId = 0;
            ContratoSap = 0;
            ComercialId = 0;
            MaterialId = 0;
            Precio = 0;
            CampaniaId = 0;
            FechaDesde = "";
            FechaHasta = "";
            EstadoId = 0;
            Usuario = "";
        }
    }

    public class DatosIniCompraNet
    {
        public List<ProveedorCombo> Proveedor { get; set; }
        public List<ComercialCombo> Comercial { get; set; }
        public List<MaterialCombo> Material { get; set; }
        //public List<MonedaCombo> Moneda { get; set; }
        public List<Campaña> Campaña { get; set; }
        public List<Provincia> Provincia { get; set; }
        public List<LocalidadCombo> Localidad { get; set; }
        //public List<MonedaCombo> MonedaSustentable { get; set; }
    }
}



