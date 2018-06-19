using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mastersoft.Framework.Standard;
using Molinos.DataAgro.Entities;

namespace Molinos.DataAgro.Entities
{
    public class DatosIniContrato {

        public List<MonedaQry> moneda { get; set; }
        public List<TipoNegocioQry> tiponegocio { get; set; }
        public List<MaterialQry> material { get; set; }
        public List<ProvinciaQry> prov { get; set; }
        public List<LocalidadQry> loc { get; set; }
        public List<ComercialQry> comercial { get; set; }
        public List<CampañaQry> campaña { get; set; }
        public List<ProveedorQry> proveedor { get; set; }
        public List<MonedaQry> monedaSustentable { get; set; }
        public List<EstadosContratos> estadoContrato { get; set; }

        public DatosIniContrato()
        {
            moneda = new List<MonedaQry>();
            tiponegocio = new List<TipoNegocioQry>();
            material = new List<MaterialQry>();
            prov = new List<ProvinciaQry>();
            loc = new List<LocalidadQry>();
            comercial = new List<ComercialQry>();
            campaña = new List<CampañaQry>();
            proveedor = new List<ProveedorQry>();
            monedaSustentable = new List<MonedaQry>();
            estadoContrato = new List<EstadosContratos>();
        }
    }

    public class NuevoContrato {
        public int? ProveedorId { get; set; }
        public int? ComercialId { get; set; }
        public int material { get; set; }
        public int cantidadId { get; set; }
        public string tipoId { get; set; }
        public float precioId { get; set; }
        public string precioMonedaId { get; set; }
        public string campañaId { get; set; }
        public DateTime fechaDesdeId { get; set; }
        public DateTime fechaHastaId { get; set; }
        public int provinciaId { get; set; }
        public int LocalidadId { get; set; }
        public int baseId { get; set; }
        public int sustentableId { get; set; }
        public float sustentablePrecioId { get; set; }
        public string sustentableMonedaId { get; set; }
        public int dolarizadoId { get; set; }
        public DateTime dolarizadoFechaId { get; set; }
        public int pesificadoId { get; set; }
        public int pesificadoDiasId { get; set; }
        public int noInformaSioId { get; set; }
        public int trigoEspecialId { get; set; }
    }
}
