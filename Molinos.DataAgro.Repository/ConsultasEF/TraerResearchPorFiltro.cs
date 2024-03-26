using Kendo.DynamicLinq;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using System.Data.Entity;
using System.Linq;
using System.Transactions;


namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class TraerResearchPorFiltro : IConsultaEscalar<DataSourceResult>
    {
        private readonly DataSourceRequest request;

        public TraerResearchPorFiltro(DataSourceRequest request)
        {
            this.request = request;
        }

        private static DataSourceResult Query(DbContext contexto, DataSourceRequest request)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

            var queryContratos =
                    from x in contexto.Set<Research>()
                    select new ResearchDto()
                    {
                        Id = x.Id,
                        IdPowerApp = x.IdPowerApp,
                        Fecha = DbFunctions.TruncateTime(x.FechaAlta),
                        FechaAlta = x.FechaAlta,
                        FechaModificacion = x.FechaModificacion,
                        MaterialId = x.MaterialId,
                        Material = x.Material.Descripcion,
                        MaterialIdAntecesor = x.MaterialIdAntecesor,
                        MaterialAntecesor = x.MaterialAntecesor.Descripcion,

                        Author = x.Author,
                        Editor = x.Editor,
                        ComercialId = x.ComercialId != null ? x.ComercialId : 44, //falta id
                        Comercial = x.ComercialId != null ? x.Comercial.Nombres + " " + x.Comercial.Apellido : x.Author ?? "Desconocido",

                        EstadoConectividad = x.EstadoConectividad,
                        Sincronizado = x.Sincronizado,
                        TipoCargaId = x.TipoCargaId,
                        TipoCarga = x.ResearchTipoCarga.Descripcion,

                        CampañaId = x.CampañaId,
                        Campaña = x.Campana.Descripcion,

                        Latitud = x.Latitud,
                        Longitud = x.Longitud,
                        LocalidadId = x.LocalidadId,
                        PartidoId = x.PartidoId,
                        ProvinciaId = x.ProvinciaId,
                        Localidad = x.Localidad,
                        Partido = x.Partido,
                        Provincia = x.Provincia,

                        Comentarios = x.Comentarios,

                        EstadioId = x.EstadioId,
                        Estadio = x.ResearchEstadio.Descripcion,
                        CondicionId = x.CondicionId,
                        Condicion = x.ResearchCondicion.Descripcion,
                        Coeficiente = x.Coeficiente,
                        Rendimiento = x.Rendimiento,
                        RendimientoCalculado = x.RendimientoCalculado,
                        HumedadSueloId = x.HumedadSueloId,
                        HumedadSuelo = x.ResearchHumedadSuelo.Descripcion,
                        CapitulosGirasol = x.CapitulosGirasol,
                        DistanciaHileras = x.DistanciaHileras,

                        TipoMuestraIdUno = x.TipoMuestraIdUno,
                        TipoMuestraIdDos = x.TipoMuestraIdDos,
                        TipoMuestraIdTres = x.TipoMuestraIdTres,
                        TipoMuestraUno = x.ResearchTipoMuestraUno.Descripcion,
                        TipoMuestraDos = x.ResearchTipoMuestraDos.Descripcion,
                        TipoMuestraTres = x.ResearchTipoMuestraTres.Descripcion,
                        PromedioMuestraUno = x.PromedioMuestraUno,
                        PromedioMuestraDos = x.PromedioMuestraDos,
                        PromedioMuestraTres = x.PromedioMuestraTres,
                        PromedioGranosVaina = x.PromedioGranosVaina,
                        MedidasUno = x.MedidasUno,
                        MedidasDos = x.MedidasDos,
                        MedidasTres = x.MedidasTres,
                        Eliminado = x.Eliminado,
                        Attachments = x.Adjuntos.Any(),
                        Adjuntos = x.Adjuntos.Select(a => new ResearchAdjuntoDto
                        {
                            Id = a.Id,
                            Nombre = a.Nombre,
                            Path = a.Path,
                            ResearchId = a.ResearchId
                        }).ToList(),
                    };

            GridHelper.TruncateTime(request.Filter, ref queryContratos);

            var result = queryContratos.ToDataSourceResult(request);
            //var cargaDesde = DateTime.Now.Date;
            //var cargaHasta = DateTime.Now.Date;

            //if (request.Filter != null && request.Filter.Filters != null)
            //{
            //    foreach (var item in request.Filter.Filters)
            //    {
            //        if (item.Field == "Fecha")
            //        {
            //            if (item.Operator == "gte")
            //            {
            //                cargaDesde = (DateTime)item.Value;
            //            }
            //            else
            //            {
            //                cargaHasta = (DateTime)item.Value;
            //            }
            //        }
            //    }
            //}
            return result;
        }

        public virtual DataSourceResult Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, request);
            }
        }
    }
}
