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

            return contexto.Set<Contrato>().Where(x => DbFunctions.TruncateTime(x.Fecha) >= fechaHoy && DbFunctions.TruncateTime(x.Fecha) <= fechaManana && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5)).DefaultIfEmpty()
                       .OrderBy(x => SqlFunctions.DatePart("month", x.Fecha)).Select(x => new ExcelPosicionMaterialDto
                       {
                           Mes = SqlFunctions.DateName("month", x.FechaHasta),
                           Contrato = x.ContratoId.ToString() ?? "",
                           RazonSocial = x.Proveedor.RazonSocial ?? "",
                           Cuit = x.Proveedor.CUIT ?? "",
                           Material = x.MaterialId == 1 ? "Maiz" : x.MaterialId == 2 ? "Trigo" : x.MaterialId == 3 ? "Soja" : "",
                           TipoNegocio = x.TipoNegocio.Descripcion ?? "",
                           Comercial = x.Comercial.Nombres + " " + x.Comercial.Apellido ?? "",
                           Cantidad = (x != null) ? x.Cantidad : 0,
                           CantidadCamiones = x.CantidadCamiones ?? 0,
                           Campana = x.Campana.Descripcion ?? "",
                           FechaDesde = SqlFunctions.DateName("day", x.FechaDesde) != null ? SqlFunctions.DateName("day", x.FechaDesde) + "/" + SqlFunctions.DatePart("month", x.FechaDesde) + "/" + SqlFunctions.DateName("year", x.FechaDesde) : "",
                           FechaHasta = SqlFunctions.DateName("day", x.FechaHasta) != null ? SqlFunctions.DateName("day", x.FechaHasta) + "/" + SqlFunctions.DatePart("month", x.FechaHasta) + "/" + SqlFunctions.DateName("year", x.FechaHasta) : "",
                           Precio = (x != null) ? x.Precio : 0,
                           Moneda = x.Moneda.Descripcion ?? "",
                           Fecha = SqlFunctions.DateName("day", x.Fecha) != null ? SqlFunctions.DateName("day", x.Fecha) + "/" + SqlFunctions.DatePart("month", x.Fecha) + "/" + SqlFunctions.DateName("year", x.Fecha) : "",
                           Provincia = x.Provincia.Nombre ?? "",
                           Localidad = x.Localidad.Nombre ?? "",
                           Boleto = x.Boleto.Descripcion ?? "",
                           Bolsa = x.BolsaId != null ? x.Bolsa.Descripcion : "",
                           Destino = x.Destino.Descripcion ?? "",
                           CondicionFijacion = x.CondicionFijacion.Descripcion ?? "",
                           DesdeFijacion = x.DesdeFijacion.HasValue ? SqlFunctions.DateName("day", x.DesdeFijacion) + "/" + SqlFunctions.DatePart("month", x.DesdeFijacion) + "/" + SqlFunctions.DateName("year", x.DesdeFijacion) : "",
                           HastaFijacion = x.HastaFijacion.HasValue ? SqlFunctions.DateName("day", x.HastaFijacion) + "/" + SqlFunctions.DatePart("month", x.HastaFijacion) + "/" + SqlFunctions.DateName("year", x.HastaFijacion) : "",
                           Base = x.Base == true ? "X" : "",
                           ImporteSustentable = x.ImporteSustentable.HasValue && x.MonedaSustentableId != null ? x.ImporteSustentable.ToString() + " " + x.MonedaSustentable.Descripcion : "",
                           FechaDolarizado = x.FechaDolarizado != null ? SqlFunctions.DateName("day", x.FechaDolarizado) + "/" + SqlFunctions.DatePart("month", x.FechaDolarizado) + "/" + SqlFunctions.DateName("year", x.FechaDolarizado) : "",
                           DiasPesificado = x.DiasPesificado != null ? x.DiasPesificado.ToString() : "",
                           NoInformaSio = x.NoInformaSio == true ? "X" : "",
                           Ampliaciones = x.Ampliaciones.ToString(),
                           Consignatario = x.Consignatario == true ? "X" : "",
                           PlanCanje = x.PlanCanje == true ? "X" : "",
                           Pago = x.PagoDirectoVendedor == true ? "Pago Dir. Vend." : x.CD == true ? "CD" : x.Warrant == true ? "Warrant" : "",
                           CalidadEspecial = x.TrigoEspecial == true ? "X" : "",
                           EstablecimientoPropio = x.EstablecimientoPropio == true ? "Propio" : x.EstablecimientoPropio == false ? "Arrendado" : "",
                           Observacion = x.Observacion ?? ""
                       }).ToList();
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
