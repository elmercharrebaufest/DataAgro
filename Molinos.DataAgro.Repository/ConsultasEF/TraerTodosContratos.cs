using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class TraerTodosContratos : IConsultaEscalar<KendoGrid<BasicoContrato>>
    {
        private readonly KendoGridMvcRequest request;
        private readonly List<int> equipo;

        public TraerTodosContratos(KendoGridMvcRequest request, List<int> equipo)
        {
            this.request = request;
            this.equipo = equipo;
        }

        private static KendoGrid<BasicoContrato> Query(DbContext contexto, KendoGridMvcRequest request, List<int> equipo)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

            var queryContratos =
                from contrato in contexto.Set<Contrato>()
                where equipo.Contains(contrato.ComercialId != null ? contrato.ComercialId.Value : 0)
                select new BasicoContrato()
                {
                    ContratoId = SqlFunctions.StringConvert((double)contrato.ContratoId).Trim(),

                    ProveedorId = contrato.ProveedorId,
                    ComercialId = contrato.ComercialId,
                    MaterialId = contrato.MaterialId,
                    TipoNegocioId = contrato.TipoNegocioId,
                    Cantidad = contrato.Cantidad,
                    Precio = contrato.Precio,
                    FechaEntrega = contrato.FechaEntrega,
                    CampanaId = contrato.CampanaId,
                    FechaDesde = DbFunctions.TruncateTime(contrato.FechaDesde),
                    FechaHasta = DbFunctions.TruncateTime(contrato.FechaHasta),
                    MonedaId = contrato.MonedaId,
                    Moneda = contrato.Moneda == null ? "" : contrato.Moneda.Descripcion,
                    Fecha = DbFunctions.TruncateTime(contrato.Fecha),
                    Fecha_Order = contrato.Fecha,
                    GrupoCompra = contrato.GrupoCompra,
                    ProvinciaId = contrato.ProvinciaId,
                    LocalidadId = contrato.LocalidadId,
                    Base = contrato.Base,
                    Importe_Sustentable = ((decimal)contrato.ImporteSustentable),
                    MonedaId_Sustentable = contrato.MonedaSustentableId,
                    Moneda_Sustentable = contrato.MonedaSustentable == null ? "" : contrato.MonedaSustentable.Descripcion,
                    Fecha_Dolarizado = DbFunctions.TruncateTime(contrato.FechaDolarizado),
                    Dias_Pesificado = contrato.DiasPesificado,
                    NoInformaSIO = contrato.NoInformaSio,
                    TrigoEspecial = contrato.TrigoEspecial,
                    Estado = contrato.EstadoId,
                    Estado_Contrato = contrato.Estado.Descripcion,
                    Estado_Order = contrato.Estado.Orden,
                    UsuarioId = contrato.UsuarioId,
                    ContratoSAP = contrato.ContratoSAP,
                    Ampliaciones = contrato.Ampliaciones,
                    Cuit = contrato.Proveedor == null ? "" : contrato.Proveedor.CUIT,
                    Proveedor = contrato.Proveedor == null ? "" : contrato.Proveedor.RazonSocial,
                    Comercial = contrato.Comercial == null ? "" : contrato.Comercial.Nombres + " " + contrato.Comercial.Apellido,
                    Material = contrato.Material == null ? "" : contrato.Material.Descripcion,
                    Campania = contrato.Campana == null ? "" : contrato.Campana.Descripcion,
                    Provincia = contrato.Provincia == null ? "" : contrato.Provincia.Nombre,
                    TipoNegocio = contrato.TipoNegocio == null ? "" : contrato.TipoNegocio.Descripcion,
                    Localidad = contrato.Localidad == null ? "" : contrato.Localidad.Nombre,
                    Observacion = contrato.Observacion != null ? contrato.Observacion : "",
                    FijacionDePrecioContratoId = null,
                    Sustentable = ((decimal)contrato.ImporteSustentable) != null && ((decimal)contrato.ImporteSustentable) > 0,
                    Dolarizado = contrato.FechaDolarizado != null,
                    Pesificado = contrato.DiasPesificado != null,
                    Negocio = (contrato.ContratoSAP == 0 || contrato.ContratoSAP == null) ? contrato.ContratoId : contrato.ContratoSAP,
                    DestinoId = contrato.DestinoId,
                    DestinoDescripcion = contrato.Destino.Descripcion,
                    CantidadCamiones = contrato.CantidadCamiones,
                    Consignatario = contrato.Consignatario,
                    PlanCanje = contrato.PlanCanje,
                    CD = contrato.CD,
                    Warrant = contrato.Warrant,
                    PagoDirectoVendedor = contrato.PagoDirectoVendedor,
                    StandardDeCalidad = contrato.StandardDeCalidadId,
                    CalidadEspecial = contrato.CalidadEspecialId,
                    ValorCalidadEspecial = contrato.ValorCalidadEspecial,
                    EstablecimientoPropio = contrato.EstablecimientoPropio,
                    BoletoId = contrato.BoletoId,
                    BolsaId = contrato.BolsaId,

                    BoletoDescripcion = contrato.Boleto.Descripcion,
                    BolsaDescripcion = contrato.Bolsa.Descripcion,
                    DesdeFijacion = DbFunctions.TruncateTime(contrato.DesdeFijacion),
                    HastaFijacion = DbFunctions.TruncateTime(contrato.HastaFijacion),
                    CondicionFijacion = contrato.CondicionFijacionId,
                    CondicionFijacionDescripcion = contrato.CondicionFijacion.Descripcion,
                    ClasificacionId = contrato.ClasificacionId,
                    ClasificacionDescripcion = contrato.Clasificacion.Descripcion,
                };

            var queryFijacion =
                from fijac in contexto.Set<FijacionDePrecioContrato>()
                where equipo.Contains(fijac.ComercialId)
                select new BasicoContrato()
                {
                    ContratoId = SqlFunctions.StringConvert((double)fijac.ContratoId).Trim(),
                    ProveedorId = fijac.ProveedorId,
                    ComercialId = fijac.ComercialId,
                    MaterialId = fijac.MaterialId != null ? fijac.MaterialId.Value : 0,
                    TipoNegocioId = 3,
                    Cantidad = fijac.Cantidad,
                    Precio = fijac.Precio,
                    FechaEntrega = null,
                    CampanaId = 0,
                    FechaDesde = null,
                    FechaHasta = null,
                    MonedaId = fijac.MonedaId,
                    Moneda = fijac.Moneda == null ? "" : fijac.Moneda.Descripcion,
                    Fecha = DbFunctions.TruncateTime(fijac.Fecha),
                    Fecha_Order = fijac.Fecha,
                    GrupoCompra = 0,
                    ProvinciaId = null,
                    LocalidadId = null,
                    Base = null,
                    Importe_Sustentable = null,
                    MonedaId_Sustentable = "",
                    Moneda_Sustentable = "",
                    Fecha_Dolarizado = null,
                    Dias_Pesificado = null,
                    NoInformaSIO = null,
                    TrigoEspecial = null,
                    Estado = fijac.EstadoId,
                    Estado_Contrato = fijac.Estado.Descripcion,
                    Estado_Order = fijac.Estado.Orden,
                    UsuarioId = "",
                    ContratoSAP = null,
                    Ampliaciones = fijac.Ampliaciones,
                    Cuit = fijac.Proveedor == null ? "" : fijac.Proveedor.CUIT,
                    Proveedor = fijac.Proveedor == null ? "" : fijac.Proveedor.RazonSocial,
                    Comercial = fijac.Comercial == null ? "" : fijac.Comercial.Nombres + " " + fijac.Comercial.Apellido,
                    Material = fijac.Material == null ? "" : fijac.Material.Descripcion,
                    Campania = "",
                    Provincia = "",
                    TipoNegocio = "FIJACION",
                    Localidad = "",
                    Observacion = fijac.Observacion != null ? fijac.Observacion : "",
                    FijacionDePrecioContratoId = fijac.FijacionDePrecioContratoId,
                    Sustentable = false,
                    Dolarizado = false,
                    Pesificado = false,
                    Negocio = null,
                    DestinoId = null,
                    DestinoDescripcion = "",
                    CantidadCamiones = null,
                    Consignatario = false,
                    PlanCanje = false,
                    CD = null,
                    Warrant = null,
                    PagoDirectoVendedor = null,
                    StandardDeCalidad = null,
                    CalidadEspecial = null,
                    ValorCalidadEspecial = null,
                    EstablecimientoPropio = false,
                    BoletoId = null,
                    BolsaId = null,
                    BoletoDescripcion = "",
                    BolsaDescripcion = "",
                    DesdeFijacion = null,
                    HastaFijacion = null,
                    CondicionFijacion = null,
                    CondicionFijacionDescripcion = "",
                    ClasificacionId = null,
                    ClasificacionDescripcion = ""
                };

            queryContratos = queryContratos.Union(queryFijacion);

            return new KendoGrid<BasicoContrato>(request, queryContratos);
        }

        public virtual KendoGrid<BasicoContrato> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, request, equipo);
            }
        }
    }
}
