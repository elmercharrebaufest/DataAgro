using Molinos.DataAgro.Entities.Dto.ControlDeBoletos;
using Molinos.DataAgro.Entities.Dto.ControlDeBoletosValidacionIA;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class TraerValidacionBoletosPendientes : IConsultaEscalar<List<ValidacionDeBoletosIAConsultaDto>>
    {
        private readonly ValidacionBoletoFiltroBusquedaDto filtros;

        public TraerValidacionBoletosPendientes(ValidacionBoletoFiltroBusquedaDto filtros)
        {
            this.filtros = filtros;
        }
        private static List<ValidacionDeBoletosIAConsultaDto> Query(DbContext contexto, ValidacionBoletoFiltroBusquedaDto filtros)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

            try
            {
                var parametros = new SqlParameter[]
                {
                    new SqlParameter("@NegocioSAP",
                        !string.IsNullOrWhiteSpace(filtros.NegocioSAP) ? (object)filtros.NegocioSAP : DBNull.Value),
                    new SqlParameter("@MaterialId",
                        filtros.MaterialId.HasValue ? (object)filtros.MaterialId.Value : DBNull.Value),
                    new SqlParameter("@EstadoValidacionId",
                        filtros.EstadoValidacionId.HasValue ? (object)filtros.EstadoValidacionId.Value : DBNull.Value),
                    new SqlParameter("@FechaValidacionDesde",
                        filtros.FechaValidacionDesde.HasValue ? (object)filtros.FechaValidacionDesde.Value : DBNull.Value),
                    new SqlParameter("@FechaValidacionHasta",
                        filtros.FechaValidacionHasta.HasValue ? (object)filtros.FechaValidacionHasta.Value : DBNull.Value),
                    new SqlParameter("@ProveedorId",
                        filtros.ProveedorId.HasValue ? (object)filtros.ProveedorId.Value : DBNull.Value),
                    new SqlParameter("@BolsaId",
                        filtros.BolsaId.HasValue ? (object)filtros.BolsaId.Value : DBNull.Value)
                };

                return contexto.Database
                    .SqlQuery<ValidacionDeBoletosIAConsultaDto>(
                        "EXEC dbo.DataAgro_BusquedaValidacionBoletosPendientes @NegocioSAP, @MaterialId, @EstadoValidacionId, @FechaValidacionDesde, @FechaValidacionHasta, @ProveedorId, @BolsaId",
                        parametros)
                    .ToList();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error en TraerControlBoletosPendientes: " + ex.Message);
                throw;
            }
        }

        public virtual List<ValidacionDeBoletosIAConsultaDto> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, filtros);
            }
        }
    }
}
