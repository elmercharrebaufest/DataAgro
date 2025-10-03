using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.ClausulasBoleto.Confirma;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using Molinos.DataAgro.Entities.Entities;
namespace Molinos.DataAgro.Business.ClausulasBoleto.Confirma
{
    public class ProcesadorClausulaConfirmaDiez : ProcesadorClausulaConfirma<ClausulaConfirmaDiez>
    {
        public ProcesadorClausulaConfirmaDiez(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
            : base(repositorio, log, estadoBoleto) { }

        public override ResultadoClausula DevolverClausulas(ClausulaConfirmaDiez clausula)
        {
            var res = new ResultadoClausula();
            res.Texto += "El impuesto de Sellos que corresponda abonar por el presente contrato será soportado por las partes conforme a derecho.";
            return res;
        }
    }
}
