using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Entities.Common.Enums;

namespace Molinos.DataAgro.Business.Procesamiento
{
    public class ProcesadorClausulaTreintaYSiete : ProcesadorClausula<ClausulaTreintaYSiete>
    {
        public ProcesadorClausulaTreintaYSiete(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaTreintaYSiete clausula)
        {
            //SI EL CONTRATO CORRESPONDE A CANJE y es a fijar

            var res = new ResultadoClausula();
            if (clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR && clausula.Basico.Canje == true)
            {
                res.Texto += $"Queda establecido que toda tasa contribución, impuesto provincial y/o municipal que grave la presente operación será a cargo del " +
                    $"Vendedor. Sin perjuicio de ello, se deja expresa constancia que la presente operación de compraventa de mercaderías con pago en especie se " +
                    $"encuentra encuadrada en los términos del Art. 5 de la Ley de IVA Inc. A.";
            }

            return res;
        }
    }
}
