using NLog;
using Molinos.DataAgro.Entities.ClausulasBoleto.BoletoFisico;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;

namespace Molinos.DataAgro.Business.ClausulasBoleto.BoletoFisico
{
    public class ProcesadorClausulaBoletoFisicoVeinteYDos : ProcesadorClausulaBoletoFisico<ClausulaBoletoFisicoVeinteYDos>
    {
        public ProcesadorClausulaBoletoFisicoVeinteYDos(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
            : base(repositorio, log, estadoBoleto) { }

        public override ResultadoClausula DevolverClausulas(ClausulaBoletoFisicoVeinteYDos clausula)
        {
            var res = new ResultadoClausula();
            res.Texto += "El impuesto de Sellos que corresponda abonar por el presente contrato será soportado por las partes conforme a derecho.";
            return res;
        }
    }
}
