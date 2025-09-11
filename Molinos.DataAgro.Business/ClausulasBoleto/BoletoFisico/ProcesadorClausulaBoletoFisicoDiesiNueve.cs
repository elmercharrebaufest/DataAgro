using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.ClausulasBoleto.BoletoFisico;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;

namespace Molinos.DataAgro.Business.ClausulasBoleto.BoletoFisico
{
    public class ProcesadorClausulaBoletoFisicoDiesiNueve : ProcesadorClausulaBoletoFisico<ClausulaBoletoFisicoDiesiNueve>
    {
        public ProcesadorClausulaBoletoFisicoDiesiNueve(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
            : base(repositorio, log, estadoBoleto) { }

        public override ResultadoClausula DevolverClausulas(ClausulaBoletoFisicoDiesiNueve clausula)
        {
            var res = new ResultadoClausula();

            res.Texto += "Los firmantes acuerdan y aceptan que en lo sucesivo, todos los documentos que tengan relación con el presente contrato podrán suscribirse " +
                         "mediante firma digital y/o electrónica, la que tendrá plena validez para las Partes.";
            return res;
        }
    }
}
