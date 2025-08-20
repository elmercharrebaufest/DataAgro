using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;

namespace Molinos.DataAgro.Business.Procesamiento
{
    public class ProcesadorClausulaOcho : ProcesadorClausula<ClausulaOcho>
    {
        public ProcesadorClausulaOcho(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaOcho clausula)
        {
            //SI EL CAMPO CESION_PAGOS ESTÁ COMPLETO

            var res = new ResultadoClausula();
            //if (clausula.Basico.Cesion == true)
            //{
            if (clausula.Basico.BoletoId == (int)EnumBoletoCompraNet.FISICO)
            {
                res.Texto = "El presente contrato sólo es transferible con el expreso consentimiento de Molinos Agro SA.";
            }
            //}
            //texto anterior de la cláusula:
            //res.Texto += $"Los derechos y obligaciones emergentes del presente contrato no podrán ser cedidos por ninguna de las partes, sin la previa y expresa conformidad de la otra. " +
            //    $"Sin embargo, dicha conformidad no será necesaria para el caso que Molinos resolviera cederlos a alguna de sus sociedades controladas, vinculadas y/o relacionadas, existentes o no al momento de firma del presente contrato. " +
            //    $"En tal caso, el cedente o el cesionario notificarán fehacientemente al vendedor.";

            return res;
        }
    }
}
