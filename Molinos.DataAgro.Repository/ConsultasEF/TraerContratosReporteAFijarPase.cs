using Kendo.DynamicLinq;
using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Globalization;
using System.Linq;
using System.Transactions;





namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class TraerContratosReporteAFijarPase : IConsultaEscalar<DataSourceResult>
    {
        private readonly DataSourceRequest request;
        private readonly List<int> equipo;

        public TraerContratosReporteAFijarPase(DataSourceRequest request, List<int> equipo)
        {
            this.request = request;
            this.equipo = equipo;
        }

        private static DataSourceResult Query(DbContext contexto, DataSourceRequest request, List<int> equipo)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

            var queryContratos =
                    from contrato in contexto.Set<Contrato>()
                    where contrato.TipoNegocioId == 1 && contrato.TipoPosicionCBOTId == 3 && contrato.EstadoId == 5
                    select new ReporteAfijarPaseDto()
                    {
                        Id = contrato.Id,
                        Fecha = contrato.Fecha,
                        FechaOperacion = DbFunctions.TruncateTime(contrato.FechaOperacion),
                        FechaHasta = DbFunctions.TruncateTime(contrato.FechaHasta),
                        FechaEntrega = contrato.FechaEntrega,
                        HastaFijacion = contrato.HastaFijacion,
                        Negocio = contrato.ContratoSAP,
                        ProveedorId = contrato.ProveedorId ?? 0,
                        CorredorId = contrato.CorredorId != null ? contrato.CorredorId.Value : 0,
                        Cuit = contrato.Proveedor == null ? "" : contrato.Proveedor.CUIT,
                        Proveedor = contrato.Proveedor == null ? "" : !string.IsNullOrEmpty(contrato.Proveedor.Alias) ? contrato.Proveedor.Alias + " - " + contrato.Proveedor.RazonSocial : contrato.Proveedor.RazonSocial,
                        Corredor = contrato.Corredor == null ? "" : !string.IsNullOrEmpty(contrato.Corredor.Alias) ? contrato.Corredor.Alias + " - " + contrato.Corredor.RazonSocial : contrato.Corredor.RazonSocial,
                        CUITCorredor = contrato.Corredor == null ? "" : contrato.Corredor.CUIT,
                        Cantidad = contrato.Cantidad,
                        Material = contrato.Material == null ? "" : contrato.Material.Descripcion,
                        MaterialId = contrato.MaterialId,

                        Posicion = SqlFunctions.DatePart("month", contrato.FechaHasta) + "." + SqlFunctions.DateName("year", contrato.FechaHasta),
                        Moneda = contrato.Moneda.Descripcion,
                        PrecioPonderado = contrato.PrecioPonderado,
                        PrecioNetoPonderado = contrato.PrecioNetoPonderado,

                        PorcentajeComision = contrato.AperturaPrecio.FirstOrDefault(t => t.ConceptoAperturaPrecioId == 3).Porcentaje,
                        ImporteComision = contrato.AperturaPrecio.FirstOrDefault(t => t.ConceptoAperturaPrecioId == 3).Importe,
                        MonedaComision = contrato.AperturaPrecio.FirstOrDefault(t => t.ConceptoAperturaPrecioId == 3).Moneda.Descripcion,
                        ImporteBonificacion = contrato.AperturaPrecio.FirstOrDefault(t => t.ConceptoAperturaPrecioId == 4).Importe,
                        MonedaBonificacion = contrato.AperturaPrecio.FirstOrDefault(t => t.ConceptoAperturaPrecioId == 4).Moneda.Descripcion,
                        ImporteRedespacho = contrato.AperturaPrecio.FirstOrDefault(t => t.ConceptoAperturaPrecioId == 2).Importe,
                        MonedaRedespacho = contrato.AperturaPrecio.FirstOrDefault(t => t.ConceptoAperturaPrecioId == 2).Moneda.Descripcion,
                        Plus = contrato.Descuentos.FirstOrDefault(t => t.TipoPeriodoDBId == 1 && t.TipoDBId == 1).Importe,


                        Estado = contrato.EstadoId,
                        Estado_Contrato = contrato.Estado.Descripcion,

                        Destino = contrato.Destino.Descripcion

                    };

            List<string> contratosAFijarPaseSAPList = queryContratos.Select(a => a.Negocio).ToList();
            var fijacionesPase = from a in contexto.Set<FijacionDePrecioContrato>() where a.EstadoId == 5 && contratosAFijarPaseSAPList.Contains(a.ContratoSAP) select new { ContratoSAP = a.ContratoSAP, Cantidad = a.Cantidad };
            var fijacionesKilos = fijacionesPase.GroupBy(a => a.ContratoSAP).Select(x => new BasicoContrato { ContratoSAP = x.Key, Cantidad = x.Sum(y => y.Cantidad) }).ToList();

            List<string> contratoSAPAFijarPasePendientes = new List<string>();
            foreach (var item in queryContratos)
            {
                var cont = fijacionesKilos.Where(x => x.ContratoSAP == item.Negocio).SingleOrDefault();
                if (cont == null || item.Cantidad > cont.Cantidad)
                {
                    contratoSAPAFijarPasePendientes.Add(item.Negocio);
                }
            }

            queryContratos.Where(x => contratoSAPAFijarPasePendientes.Contains(x.Negocio));




            GridHelper.TruncateTime(request.Filter, ref queryContratos);

            var result = queryContratos.ToDataSourceResult(request);
            var cargaDesde = DateTime.Now.Date;
            var cargaHasta = DateTime.Now.Date;

            if (request.Filter != null && request.Filter.Filters != null)
            {
                foreach (var item in request.Filter.Filters)
                {
                    if (item.Field == "Fecha")
                    {
                        if (item.Operator == "gte")
                        {
                            cargaDesde = (DateTime)item.Value;
                        }
                        else
                        {
                            cargaHasta = (DateTime)item.Value;
                        }
                    }
                }
            }
            return result;
        }


        public virtual DataSourceResult Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, request, equipo);
            }
        }
    }
}
