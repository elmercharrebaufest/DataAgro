using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.ClausulasBoleto.Confirma;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;

namespace Molinos.DataAgro.Business.ClausulasBoleto.Confirma
{
    public class ProcesadorClausulaConfirmaSiete : ProcesadorClausulaConfirma<ClausulaConfirmaSiete>
    {
        public ProcesadorClausulaConfirmaSiete(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
            : base(repositorio, log, estadoBoleto) { }

        public override ResultadoClausula DevolverClausulas(ClausulaConfirmaSiete clausula)
        {
            var res = new ResultadoClausula();
            res.Texto += "El pago de la liquidación final se hará a los 30 días de la entrega de la mercadería, en caso de corresponder.";
            return res;
        }
    }
}
