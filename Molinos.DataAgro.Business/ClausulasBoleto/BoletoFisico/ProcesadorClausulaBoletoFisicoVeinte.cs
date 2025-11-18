using NLog;
using Molinos.DataAgro.Entities.ClausulasBoleto.BoletoFisico;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;

namespace Molinos.DataAgro.Business.ClausulasBoleto.BoletoFisico
{
    public class ProcesadorClausulaBoletoFisicoVeinte : ProcesadorClausulaBoletoFisico<ClausulaBoletoFisicoVeinte>
    {
        public ProcesadorClausulaBoletoFisicoVeinte(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
            : base(repositorio, log, estadoBoleto) { }

        public override ResultadoClausula DevolverClausulas(ClausulaBoletoFisicoVeinte clausula)
        {
            var res = new ResultadoClausula();
            if (clausula.Basico.MaterialId == (int)EnumMateriales.SOJA)
            {
                res.Texto += $"El productor, acopiador, procesador, acondicionador, exportador o remitente Vendedor acepta irrevocablemente que el grano de soja será analizado y en caso de detectarse la presencia de tecnologías patentadas " +
                    $"se proporcionará al propietario de la tecnología a través de BolsaTech o a quien éste designe la información relativa a dichos cargamentos. " +
                    $"Exclusivamente para las operaciones de compraventa de entrega secundaria en las que el Vendedor no posea licencia del dueño de la tecnología, ni sea participante del sistema BolsaTech, " +
                    $"el Vendedor acepta que en caso de detectarse la presencia de tecnologías patentadas se le descontará, de corresponder, el importe de la regalía correspondiente por cuenta y orden del propietario de la tecnología o de quien éste designe. " +
                    $"Toda controversia derivada de la aplicación de esta cláusula y/o pago de regalías será resuelta con el propietario de la tecnología patentada por la Cámara Arbitral de Cereales de la jurisdicción elegida por la parte que se considere con derecho al reclamo, a cuyo efecto las partes se sujetan y dan por aceptadas las condiciones establecidas en la reglamentación general aplicable. El tribunal elegido actuará como amigable componedor, con aplicación de las Reglas y Usos del Comercio de Granos y del Reglamento de Procedimientos aprobado por Decreto 931/98 y/o sus normas complementarias. " +
                    $"La ejecución del laudo arbitral se efectuará ante los Tribunales Ordinarios de la Ciudad Autónoma de Buenos Aires. Si por cualquier motivo la controversia no pudiere ser resuelta por el tribunal arbitral elegido, queda pactado que serán los Tribunales Ordinarios de la Ciudad Autónoma de Buenos Aires los que resuelvan el asunto.";
            }
            return res;
        }
    }
}
