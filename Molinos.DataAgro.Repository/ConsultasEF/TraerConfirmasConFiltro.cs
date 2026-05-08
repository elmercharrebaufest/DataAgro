using Molinos.DataAgro.Entities.Dto;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class TraerConfirmasConFiltro : IConsultaEscalar<List<BasicoConfirma>>
    {
        private readonly ConfirmaFiltroBusquedaDto filtros;
        private readonly List<int> equipo;

        public TraerConfirmasConFiltro(ConfirmaFiltroBusquedaDto filtros, List<int> equipo)
        {
            this.filtros = filtros;
            this.equipo = equipo;
        }

        private static List<BasicoConfirma> Query(DbContext contexto, ConfirmaFiltroBusquedaDto filtros, List<int> equipo)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

            try
            {
                var parametros = new SqlParameter[]
                {
                    new SqlParameter("@NegocioSAP",
                        !string.IsNullOrWhiteSpace(filtros.NegocioSAP) ? (object)filtros.NegocioSAP : DBNull.Value),
                    new SqlParameter("@FechaConfirmacionDesde",
                        filtros.FechaConfirmacionDesde.HasValue ? (object)filtros.FechaConfirmacionDesde.Value : DBNull.Value),
                    new SqlParameter("@FechaConfirmacionHasta",
                        filtros.FechaConfirmacionHasta.HasValue ? (object)filtros.FechaConfirmacionHasta.Value : DBNull.Value),
                    new SqlParameter("@ProveedorId",
                        filtros.ProveedorId.HasValue && filtros.ProveedorId.Value > 0 ? (object)filtros.ProveedorId.Value : DBNull.Value),
                    new SqlParameter("@ComercialId",
                        filtros.ComercialId.HasValue && filtros.ComercialId.Value > 0 ? (object)filtros.ComercialId.Value : DBNull.Value),
                    new SqlParameter("@BolsaCompraNetId",
                        filtros.BolsaCompraNetId.HasValue && filtros.BolsaCompraNetId.Value > 0 ? (object)filtros.BolsaCompraNetId.Value : DBNull.Value),
                    new SqlParameter("@MaterialId",
                        filtros.MaterialId.HasValue && filtros.MaterialId.Value > 0 ? (object)filtros.MaterialId.Value : DBNull.Value),
                    new SqlParameter("@Equipo",
                        equipo != null && equipo.Count > 0 ? (object)string.Join(",", equipo) : DBNull.Value),
                };

                return contexto.Database
                    .SqlQuery<BasicoConfirma>(
                        "EXEC dbo.DataAgro_BusquedaBoletoConfirma @NegocioSAP, @FechaConfirmacionDesde, @FechaConfirmacionHasta, @ProveedorId, @ComercialId, @BolsaCompraNetId, @MaterialId, @Equipo",
                        parametros)
                    .ToList();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error en la consulta: " + ex.Message);
                throw;
            }
        }

        public virtual List<BasicoConfirma> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, filtros, equipo);
            }
        }
    }
}
