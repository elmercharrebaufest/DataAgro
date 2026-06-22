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
    public class TraerBoletosParaModificar : IConsultaEscalar<List<ControlDeBoletosParaModificarDto>>
    {
        private readonly ControlDeBoletosParaModificarFiltroDto filtros;

        public TraerBoletosParaModificar(ControlDeBoletosParaModificarFiltroDto filtros)
        {
            this.filtros = filtros;
        }

        private static List<ControlDeBoletosParaModificarDto> Query(DbContext contexto, ControlDeBoletosParaModificarFiltroDto filtros)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

            try
            {
                var parametros = new SqlParameter[]
                {
                    new SqlParameter("@ContratoSAP",
                        !string.IsNullOrWhiteSpace(filtros?.ContratoSAP) ? (object)filtros.ContratoSAP : DBNull.Value)
                };

                return contexto.Database
                    .SqlQuery<ControlDeBoletosParaModificarDto>(
                        "EXEC dbo.DataAgro_GetBoletosParaModificar @ContratoSAP",
                        parametros)
                    .ToList();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error en TraerBoletosParaModificar: " + ex.Message);
                throw;
            }
        }

        public virtual List<ControlDeBoletosParaModificarDto> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, filtros);
            }
        }
    }
}
