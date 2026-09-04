using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class TraerNegociosPendientesControlBoleto : IConsultaEscalar<List<int>>
    {
        private static List<int> Query(DbContext contexto)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

            try
            {
                var fechaLimite = DateTime.Now.AddMonths(-2);

                var negocios = contexto.Set<Negocio>();
                var controles = contexto.Set<ControlDeBoletos>();

                return negocios
                    .Where(n => n.ConfirmadoSAP == true
                        && n.EstadoId == (int)EnumEstadoContrato.Finalizado
                        && (
                            ( n.TipoNegocioId != (int)EnumTipoNegocio.FIJACION &&
                             (n.BoletoId == (int)EnumBoletoCompraNet.NINGUNO || n.BoletoId == (int)EnumBoletoCompraNet.SIN_BOLETO)
                            )
                           )
                        && n.FechaConfirmadoSAP >= fechaLimite
                        && n.ContratoMadre.Length == 0
                        && !controles.Any(cb => cb.NegocioId == n.Id))
                    .Select(n => n.Id)
                    .ToList();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error en TraerNegociosPendientesControlBoleto: " + ex.Message);
                throw;
            }
        }

        public virtual List<int> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto);
            }
        }
    }
}
