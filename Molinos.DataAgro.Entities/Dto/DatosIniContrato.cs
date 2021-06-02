using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class DatosIniContrato
    {
        public List<LocalidadQry> localidad;

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
        public List<ClasificacionCompraNetQry> Clasificacion { get; set; }
        public List<BolsaCompraNetQry> Bolsa { get; set; }
        public List<CentroQry> Destino { get; set; }
        public List<CondicionFijacionQry> Condicion { get; set; }
        public List<StandardDeCalidadQry> Standard { get; set; }
        public List<TipoDBQry> TipoDB { get; set; }
        public List<TipoPeriodoDBQry> TipoPeriodoDB { get; set; }
        public List<MonedaQry> MonedaDescuento { get; set; }
        public List<TipoFasonQry> TipoFason { get; set; }
        public List<TipoAgenteCompraQry> TipoAgenteCompra { get; set; }
        public List<OperadorQry> Operador { get; set; }
        public List<ZonaQry> Zona { get; set; }
        public List<NivelTarifaQry> NivelTarifa { get; set; }
        public List<MotivoAnteriorQry> MotivoAnterior { get; set; }
        public List<TipoPosicionCBOTQry> TipoPosicionCBOT { get; set; }

        public DatosIniContrato()
        {
            moneda = new List<MonedaQry>();
            tiponegocio = new List<TipoNegocioQry>();
            material = new List<MaterialQry>();
            prov = new List<ProvinciaQry>();
            loc = new List<LocalidadQry>();
            comercial = new List<ComercialQry>();
            campaña = new List<CampañaQry>();
            monedaSustentable = new List<MonedaQry>();
            estadoContrato = new List<EstadosContratos>();
            Clasificacion = new List<ClasificacionCompraNetQry>();
            Bolsa = new List<BolsaCompraNetQry>();
            Destino = new List<CentroQry>();
            Condicion = new List<CondicionFijacionQry>();
            Standard = new List<StandardDeCalidadQry>();
            TipoDB = new List<TipoDBQry>();
            TipoPeriodoDB = new List<TipoPeriodoDBQry>();
            MonedaDescuento = new List<MonedaQry>();
            TipoFason = new List<TipoFasonQry>();
            TipoAgenteCompra = new List<TipoAgenteCompraQry>();
            Zona = new List<ZonaQry>();
            NivelTarifa = new List<NivelTarifaQry>();
        }
    }

    public class NuevoContrato
    {
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
        public int ClasificacionId { get; set; }
        public int DestinoId { get; set; }
        public bool PlanCanje { get; set; }
        public bool Cd { get; set; }
        public bool Warrant { get; set; }
        public bool PagoDirecto { get; set; }
    }
}
