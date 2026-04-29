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

                return contexto.Set<Negocio>()
                    .Where(n => n.ConfirmadoSAP == true
                        && n.EstadoId == (int)EnumEstadoContrato.Finalizado
                        && (
                            (n.TipoNegocioId != (int)EnumTipoNegocio.FIJACION && n.BoletoId == (int)EnumBoletoCompraNet.NINGUNO)
                            ||
                            (n.TipoNegocioId == (int)EnumTipoNegocio.FIJACION
                                && (n as FijacionDePrecioContrato).Contrato != null
                                && (n as FijacionDePrecioContrato).Contrato.BoletoId == (int)EnumBoletoCompraNet.NINGUNO)
                        )
                        && n.FechaConfirmadoSAP >= fechaLimite)
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
