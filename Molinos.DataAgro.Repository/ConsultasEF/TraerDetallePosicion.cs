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
    public class TraerDetallePosicion : IConsulta<string[]>
    {
        private readonly int materialId;
        private readonly int mes;
        private readonly bool? calidad;

        public TraerDetallePosicion(int materialId, int mes, bool? calidad = null)
        {
            this.materialId = materialId;
            this.mes = mes;
            this.calidad = calidad;
        }
        
        private static List<string[]> Query(DbContext contexto,int materialId, int mes, bool? calidad)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
            var fechaHoy = DateTime.Now.Date;

            return contexto.Set<Contrato>().Where(x => DbFunctions.TruncateTime(x.Fecha) == fechaHoy && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5) && x.MaterialId == materialId && SqlFunctions.DatePart("Month", x.FechaHasta) == mes && (calidad == null || (calidad != null && x.TrigoEspecial == calidad))).DefaultIfEmpty()
                       .Select(x => new List<string>
                       {
                           x.ContratoId.ToString()??"",
                           x.Proveedor.RazonSocial??"",
                           x.Proveedor.CUIT??"",
                           x.Material.Descripcion??"",
                           x.TipoNegocio.Descripcion ?? "",
                           x.Comercial.Nombres + " " + x.Comercial.Apellido ?? "",
                           x.Cantidad.ToString() ?? "",
                           x.CantidadCamiones.ToString() ?? "",
                           x.Campana.Descripcion ?? "",
                           SqlFunctions.DateName("day", x.FechaDesde) != null ? SqlFunctions.DateName("day", x.FechaDesde) + "/" + SqlFunctions.DateName("month", x.FechaDesde) + "/" + SqlFunctions.DateName("year", x.FechaDesde) : "",
                           SqlFunctions.DateName("day", x.FechaHasta) != null ? SqlFunctions.DateName("day", x.FechaHasta) + "/" + SqlFunctions.DateName("month", x.FechaHasta) + "/" + SqlFunctions.DateName("year", x.FechaHasta) : "",
                           x.Precio.ToString() ?? "",
                           x.Moneda.Descripcion ?? "",
                           SqlFunctions.DateName("day", x.Fecha) != null ? SqlFunctions.DateName("day", x.Fecha) + "/" + SqlFunctions.DateName("month", x.Fecha) + "/" + SqlFunctions.DateName("year", x.Fecha) : "",
                           x.Provincia.Nombre ?? "",
                           x.Localidad.Nombre ?? "",
                           x.Boleto.Descripcion ?? "",
                           x.BolsaId != null ? x.Bolsa.Descripcion : "",
                           x.Destino.Descripcion ?? "",
                           x.CondicionFijacion.Descripcion ?? "",
                           x.DesdeFijacion.HasValue ? SqlFunctions.DateName("day", x.DesdeFijacion) + "/" + SqlFunctions.DateName("month", x.DesdeFijacion) + "/" + SqlFunctions.DateName("year", x.DesdeFijacion) : "",
                           x.HastaFijacion.HasValue ? SqlFunctions.DateName("day", x.HastaFijacion) + "/" + SqlFunctions.DateName("month", x.HastaFijacion) + "/" + SqlFunctions.DateName("year", x.HastaFijacion) : "",
                           x.Base == true ? "X" : "",
                           x.ImporteSustentable.HasValue && x.MonedaSustentableId != null ? x.ImporteSustentable.ToString() + " " + x.MonedaSustentable.Descripcion : "",
                           x.FechaDolarizado!=null ? SqlFunctions.DateName("day", x.FechaDolarizado) + "/" + SqlFunctions.DateName("month", x.FechaDolarizado) + "/" + SqlFunctions.DateName("year", x.FechaDolarizado) : "",
                           x.DiasPesificado != null ? x.DiasPesificado.ToString() : "",
                           x.NoInformaSio == true ? "X" : "",
                           x.Ampliaciones.ToString(),
                           x.Consignatario == true ? "X" : "",
                           x.PlanCanje == true ? "X" : "",
                           x.PagoDirectoVendedor == true ? "Pago Dir. Vend." : x.CD == true ? "CD" : x.Warrant == true ? "Warrant" : "",
                           x.TrigoEspecial == true ? "X" : "",
                           x.EstablecimientoPropio == true ? "Propio" : x.EstablecimientoPropio == false ? "Arrendado" : "",
                           x.Observacion??""
                       }
                       ).AsEnumerable()
                       .Select(x => x.ToArray()).ToList();
        }

        public virtual List<string[]> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, materialId, mes, calidad);
            }
        }
    }
}
