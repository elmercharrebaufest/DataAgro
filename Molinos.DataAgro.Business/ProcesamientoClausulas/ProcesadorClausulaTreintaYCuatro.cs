using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Entities.Common.Enums;

namespace Molinos.DataAgro.Business.Procesamiento
{
    public class ProcesadorClausulaTreintaYCuatro : ProcesadorClausula<ClausulaTreintaYCuatro>
    {
        public ProcesadorClausulaTreintaYCuatro(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaTreintaYCuatro clausula)
        {
            //SI EL CONTRATO CORRESPONDE A CANJE y es a fijar

            var res = new ResultadoClausula();
            if (clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR && clausula.Basico.Canje == true)
            {
                res.Texto += $"El Vendedor declara y garantiza al Comprador que (i) a la fecha de la firma del presente Boleto, y (ii) al momento de la entrega de " +
                    $"la Mercadería, se encuentra debidamente inscripto en el Registro de Operadores de Granos. El presente Boleto estará sujeto a la emisión de la " +
                    $"Constancia de Registración de Operación de Compraventa de Granos según Res. 2596 por parte de AFIP. Es obligación del Vendedor informar al " +
                    $"Comprador, inmediatamente, cualquier modificación que sufriera su registro, incluso su caducidad y/o suspensión, en el Registro de Operadores " +
                    $"de Granos.";
            }

            return res;
        }
    }
}
