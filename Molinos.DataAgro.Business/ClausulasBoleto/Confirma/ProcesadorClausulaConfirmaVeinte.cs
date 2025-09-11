using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.ClausulasBoleto.Confirma;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;

namespace Molinos.DataAgro.Business.ClausulasBoleto.Confirma
{
    public class ProcesadorClausulaConfirmaVeinte : ProcesadorClausulaConfirma<ClausulaConfirmaVeinte>
    {
        public ProcesadorClausulaConfirmaVeinte(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
            : base(repositorio, log, estadoBoleto) { }

        public override ResultadoClausula DevolverClausulas(ClausulaConfirmaVeinte clausula)
        {
            var res = new ResultadoClausula();
            return res;
        }
    }
}
