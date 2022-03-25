using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class TraerAdministracionCupoExcedente : IConsultaEscalar<KendoGrid<AdministracionCupoDto>>
    {
        private readonly KendoGridMvcRequest request;
        private readonly int? comercialId;

        public TraerAdministracionCupoExcedente(KendoGridMvcRequest request, int? comercialId)
        {
            this.request = request;
            this.comercialId = comercialId;
        }

        private static KendoGrid<AdministracionCupoDto> Query(DbContext contexto, KendoGridMvcRequest request, int? comercialId)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

            var query =
                from cupo in contexto.Set<AdministracionCupo>()
                where cupo.Excedente == true && (cupo.ComercialId == comercialId || comercialId == null)
                select new AdministracionCupoDto()
                {
                    Id = cupo.Id,
                    Fecha = cupo.Fecha,
                    Estado = cupo.EstadoId == 1 ? "Aceptado" : cupo.EstadoId == 3 ? "Pendiente" : "Rechazado",
                    EstadoId = cupo.EstadoId,
                    CantidadFleteProcedencia = cupo.CantidadFleteProcedencia,
                    CantidadDeCupo = cupo.CantidadCupo,
                    CantidadFleteProcedenciaMax = cupo.CantidadFleteProcedencia,
                    CantidadDeCupoMax = cupo.CantidadCupo,
                    Comercial = cupo.Comercial.Nombres + " " + cupo.Comercial.Apellido,
                    ComercialId = cupo.ComercialId,
                    Proveedor = cupo.Proveedor.RazonSocial,
                    Material = cupo.Material.Descripcion,
                    Centro = cupo.Centro.Descripcion,
                    Zona = cupo.Zona.Descripcion,
                    Excedente = cupo.Excedente,
                    TipoAdministracionCupo = cupo.TipoAdministracionCupo.Descripcion,
                    TipoAdministracionCupoId = cupo.TipoAdministracionCupoId,
                    Observacion = cupo.Observacion,
                    FechaCreacion = DbFunctions.TruncateTime(cupo.FechaCreacion),
                    FechaDecision = cupo.FechaDecision,
                    Hora = SqlFunctions.DateName("hh", cupo.FechaCreacion) + ":" + DbFunctions.Right("00" + SqlFunctions.DateName("n", cupo.FechaCreacion), 2),
                    ConDescarga = cupo.ConDescarga,
                    FechaCreacionConHora = cupo.FechaCreacion,
                    CantidadFleteProcedenciaOriginal = cupo.CantidadFleteProcedencia,
                    CantidadDeCupoOriginal = cupo.CantidadCupo,
                };
            return new KendoGrid<AdministracionCupoDto>(request, query);
        }

        public virtual KendoGrid<AdministracionCupoDto> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, request, comercialId);
            }
        }
    }
}
