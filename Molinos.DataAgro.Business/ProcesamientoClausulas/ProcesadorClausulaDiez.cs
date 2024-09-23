using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Entities.Common.Enums;

namespace Molinos.DataAgro.Business.Procesamiento
{
    public class ProcesadorClausulaDiez : ProcesadorClausula<ClausulaDiez>
    {
        public ProcesadorClausulaDiez(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaDiez clausula)
        {
            //SI EN EL CONTRATO SE ESPECIFICA UN PORCENTAJE DE MULTA DISTINTO DE CERO EN CONTRATOS A FIJAR

            var res = new ResultadoClausula();
            if (clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR && clausula.Basico.PlanCanje != true)
            {
                res.Texto += $"Multa por incumplimiento: 10%";
            }

            return res;
        }
    }
}
