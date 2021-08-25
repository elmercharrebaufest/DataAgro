using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class TraerPosicionMaterialMes : IConsulta<ExcelPosicionMaterialDto>
    {
        private readonly DateTime fechaDesde;
        private readonly DateTime fechaHasta;

        public TraerPosicionMaterialMes(DateTime fechaDesde, DateTime fechaHasta)
        {
            this.fechaDesde = fechaDesde;
            this.fechaHasta = fechaHasta;
        }

        private static List<ExcelPosicionMaterialDto> Query(DbContext contexto, DateTime fechaDesde, DateTime fechaHasta)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
            var fechaHoy = fechaDesde.Date;
            var fechaManana = fechaHasta.Date;

            var contratos = contexto.Set<Negocio>().Where(x =>
            DbFunctions.TruncateTime(x.Fecha) >= fechaHoy
            && DbFunctions.TruncateTime(x.Fecha) <= fechaManana
            && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5)
            && (x.TipoNegocioId == 1 || x.TipoNegocioId == 2 || x.TipoNegocioId == 3 || x.TipoNegocioId == 4 || x.TipoNegocioId == 5)
            && (((x is Contrato) && (x as Contrato).Canje != true) || !(x is Contrato))
            && (((x is FijacionDePrecioContrato) && (x as FijacionDePrecioContrato).Canje != true) || !(x is FijacionDePrecioContrato))
            && !(((x is FijacionDePrecioContrato) && (x as FijacionDePrecioContrato).Canje != true && (x as FijacionDePrecioContrato).Virtual != true && (x as FijacionDePrecioContrato).Contrato.Canje == true) || !(x is FijacionDePrecioContrato))
            ).DefaultIfEmpty()
                       .OrderBy(x => SqlFunctions.DatePart("month", x.Fecha)).Select(x => new ExcelPosicionMaterialDto
                       {
                           Mes = SqlFunctions.DateName("month", x.FechaHasta),
                           Contrato = x.Id.ToString() ?? "",
                           RazonSocial = x is AgenteCompra ? ((x as AgenteCompra).Operador.Descripcion ?? "") : x.Proveedor.RazonSocial ?? "",
                           Cuit = x.Proveedor.CUIT ?? "",
                           Material = x.MaterialId == 1 ? "Maiz" : x.MaterialId == 2 ? "Trigo" : x.MaterialId == 3 ? "Soja" : x.MaterialId == 4 ? "Girasol" : x.MaterialId == 5 ? "Girasol Alto Oleico" : "",
                           TipoNegocio = x.TipoNegocio.Descripcion ?? "",
                           Comercial = x.Comercial.Nombres + " " + x.Comercial.Apellido ?? "",
                           Cantidad = (x != null) ? x.Cantidad : 0,
                           CantidadCamiones = x is Contrato ? (x as Contrato).CantidadCamiones ?? 0 : 0,
                           Campana = x.Campana.Descripcion ?? "",
                           FechaDesde = SqlFunctions.DateName("day", x.FechaDesde) != null ? SqlFunctions.DateName("day", x.FechaDesde) + "/" + SqlFunctions.DatePart("month", x.FechaDesde) + "/" + SqlFunctions.DateName("year", x.FechaDesde) : "",
                           FechaHasta = SqlFunctions.DateName("day", x.FechaHasta) != null ? SqlFunctions.DateName("day", x.FechaHasta) + "/" + SqlFunctions.DatePart("month", x.FechaHasta) + "/" + SqlFunctions.DateName("year", x.FechaHasta) : "",
                           Precio = (x != null) ? x.Precio : 0,
                           Moneda = x.Moneda.Descripcion ?? "",
                           Fecha = SqlFunctions.DateName("day", x.Fecha) != null ? SqlFunctions.DateName("day", x.Fecha) + "/" + SqlFunctions.DatePart("month", x.Fecha) + "/" + SqlFunctions.DateName("year", x.Fecha) : "",
                           Provincia = x is Contrato ? (x as Contrato).Provincia.Nombre ?? "" : "",
                           Localidad = x is Contrato ? (x as Contrato).Localidad.Nombre ?? "" : "",
                           Boleto = x is Contrato ? (x as Contrato).Boleto.Descripcion ?? "" : "",
                           Bolsa = x is Contrato ? (x as Contrato).BolsaId != null ? (x as Contrato).Bolsa.Descripcion : "" : "",
                           Destino = x.Destino.Descripcion ?? "",
                           CondicionFijacion = x.CondicionFijacion.Descripcion ?? "",
                           DesdeFijacion = x.DesdeFijacion.HasValue ? SqlFunctions.DateName("day", x.DesdeFijacion) + "/" + SqlFunctions.DatePart("month", x.DesdeFijacion) + "/" + SqlFunctions.DateName("year", x.DesdeFijacion) : "",
                           HastaFijacion = x.HastaFijacion.HasValue ? SqlFunctions.DateName("day", x.HastaFijacion) + "/" + SqlFunctions.DatePart("month", x.HastaFijacion) + "/" + SqlFunctions.DateName("year", x.HastaFijacion) : "",
                           Base = x is Contrato ? (x as Contrato).Base == true ? "X" : "" : "",
                           ImporteSustentable = x is Contrato ? ((x as Contrato).ImporteSustentable.HasValue && (x as Contrato).MonedaSustentableId != null ? (x as Contrato).ImporteSustentable.ToString() + " " + (x as Contrato).MonedaSustentable.Descripcion : "") : "",
                           FechaDolarizado = x.FechaDolarizado != null ? SqlFunctions.DateName("day", x.FechaDolarizado) + "/" + SqlFunctions.DatePart("month", x.FechaDolarizado) + "/" + SqlFunctions.DateName("year", x.FechaDolarizado) : "",
                           DiasPesificado = x.DiasPesificado != null ? x.DiasPesificado.ToString() : "",
                           NoInformaSio = x is Contrato ? (x as Contrato).NoInformaSio == true ? "X" : "" : "",
                           Ampliaciones = x.Ampliaciones.ToString(),
                           Consignatario = x is Contrato ? (x as Contrato).Consignatario == true ? "X" : "" : "",
                           PlanCanje = x is Contrato ? (x as Contrato).PlanCanje == true ? "X" : "" : "",
                           MercaderiaEnDeposito = x is Contrato ? (x as Contrato).MercsDeposito == true ? "X" : "" : "",
                           Pago = x is Contrato ? ((x as Contrato).PagoDirectoVendedor == true ? "Pago Dir. Vend." : x.CD == true ? "CD" : (x as Contrato).Warrant == true ? "Warrant" : "") : "",
                           CalidadEspecial = x.StandardDeCalidad.Descripcion,
                           EstablecimientoPropio = x is Contrato ? ((x as Contrato).EstablecimientoPropio == true ? "Propio" : (x as Contrato).EstablecimientoPropio == false ? "Arrendado" : "") : "",
                           Observacion = x.Observacion ?? ""
                       }).ToList();

            return contratos;
        }

        public virtual List<ExcelPosicionMaterialDto> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, fechaDesde, fechaHasta);
            }
        }
    }
}
