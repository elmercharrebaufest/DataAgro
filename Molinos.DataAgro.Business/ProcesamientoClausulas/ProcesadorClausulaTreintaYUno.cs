using NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Entities.Common.Enums;

namespace Molinos.DataAgro.Business.Procesamiento
{
    public class ProcesadorClausulaTreintaYUno : ProcesadorClausula<ClausulaTreintaYUno>
    {
        public ProcesadorClausulaTreintaYUno(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaTreintaYUno clausula)
        {
            //SI EL CONTRATO CORRESPONDE A CANJE y es a fijar

            var res = new ResultadoClausula();
            if (clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR && clausula.Basico.Canje == true)
            {
                res.Texto += $"En caso de que el Vendedor entregare la Mercadería en forma total y la Mercadería no cumpliera -total o parcialmente- con las " +
                    $"condiciones Fábrica, y ello implicaré un saldo a favor del Comprador, entonces el Vendedor quedará obligado a proceder en idénticas " +
                    $"condiciones a las mencionadas en la cláusula previa de este Boleto.";
            }

            return res;
        }
    }
}
