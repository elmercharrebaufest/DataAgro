using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities
{


    public class BasicoContrato
    {

        public string ContratoId { get; set; }
        public int MaterialId { get; set; }
        public int TipoNegocioId { get; set; }
        public double Cantidad { get; set; }
        public decimal Precio { get; set; }
        public string FechaEntrega { get; set; }

        public int CampanaId { get; set; }
        public string FechaDesde { get; set; }
        public string FechaHasta { get; set; }
        public int ProveedorId { get; set; }
        public string MonedaId { get; set; }
        public string Moneda { get; set; }
        public string Fecha { get; set; }
        public int GrupoCompra { get; set; }
        public int? ComercialId { get; set; }
        public int? ProvinciaId { get; set; }
        public int? LocalidadId { get; set; }

        public bool? Base { get; set; }
        public decimal? Importe_Sustentable { get; set; }
        public string MonedaId_Sustentable { get; set; }
        public string Moneda_Sustentable { get; set; }
        public string Fecha_Dolarizado { get; set; }
        public int? Dias_Pesificado { get; set; }
        public bool? NoInformaSIO { get; set; }

        public bool? TrigoEspecial { get; set; }
        public int? Estado { get; set; }
        public string UsuarioId { get; set; }

        public int? ContratoSAP { get; set; }

        public double? Ampliaciones { get; set; }
        public string TipoNegocio { get; set; }

        public string Proveedor { get; set; }

        public string Comercial { get; set; }
        public string Material { get; set; }

        public string Campania { get; set; }

        public string Provincia { get; set; }

        public string Localidad { get; set; }
        public string Estado_Contrato { get; set; }

        public int? Cantidad_F { get; set; }
        public decimal? Precio_F { get; set; }

        public string Proveedor_F { get; set; }

        public string Fecha_F { get; set; }

        public string Material_F { get; set; }

        public string MonedaId_F { get; set; }
        public string Moneda_F { get; set; }

        public int? Ampliaciones_F { get; set; }

        public DateTime Fecha_Order { get; set; }

        public int Estado_Order { get; set; }

        public string Observacion { get; set; }

        public string Observacion_F { get; set; }

        public string FijacionDePrecioContratoId { get; set; }
    }


    public class StoredPorContratoResult
    {

        public List<BasicoContrato> BasicoContratoTraerPorFltro { get; set; }

        public List<BasicoContrato> ReporteContratoContratoTraerPorFltro { get; set; }
    }


}
