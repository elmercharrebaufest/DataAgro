using Molinos.DataAgro.Entities.Dto.ControlDeBoletos;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class TraerControlBoletosPendientes : IConsultaEscalar<List<ControlDeBoletosConsultaDto>>
    {
        private readonly ControlDeBoletoFiltroBusquedaDto filtros;

        public TraerControlBoletosPendientes(ControlDeBoletoFiltroBusquedaDto filtros)
        {
            this.filtros = filtros;
        }

        private static List<ControlDeBoletosConsultaDto> Query(DbContext contexto, ControlDeBoletoFiltroBusquedaDto filtros)
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
                    new SqlParameter("@EstadoControlId",
                        filtros.EstadoControlId.HasValue ? (object)filtros.EstadoControlId.Value : DBNull.Value),
                    new SqlParameter("@EsConfirma",
                        filtros.EsConfirma ? (object)true : DBNull.Value),
                    new SqlParameter("@FechaCargaDesde",
                        filtros.FechaCargaDesde.HasValue ? (object)filtros.FechaCargaDesde.Value : DBNull.Value),
                    new SqlParameter("@FechaCargaHasta",
                        filtros.FechaCargaHasta.HasValue ? (object)filtros.FechaCargaHasta.Value : DBNull.Value),
                    new SqlParameter("@ProveedorId",
                        filtros.Proveedor.HasValue ? (object)filtros.Proveedor.Value : DBNull.Value),
                    new SqlParameter("@BolsaId",
                        filtros.BolsaId.HasValue ? (object)filtros.BolsaId.Value : DBNull.Value),
                    new SqlParameter("@ComercialId",
                        filtros.ComercialId.HasValue ? (object)filtros.ComercialId.Value : DBNull.Value),
                };

                return contexto.Database
                    .SqlQuery<ControlDeBoletosConsultaDto>(
                        "EXEC dbo.DataAgro_BusquedaControlBoletosPendientes @NegocioSAP, @MaterialId, @EstadoControlId, @EsConfirma, @FechaCargaDesde, @FechaCargaHasta, @ProveedorId, @BolsaId, @ComercialId",
                        parametros)
                    .ToList();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error en TraerControlBoletosPendientes: " + ex.Message);
                throw;
            }
        }

        public virtual List<ControlDeBoletosConsultaDto> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, filtros);
            }
        }
    }
}
