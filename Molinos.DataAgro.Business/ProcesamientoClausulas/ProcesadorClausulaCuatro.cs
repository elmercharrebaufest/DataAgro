using NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Entities.Common.Enums;

namespace Molinos.DataAgro.Business.Procesamiento
{
    public class ProcesadorClausulaCuatro : ProcesadorClausula<ClausulaCuatro>
    {
        public ProcesadorClausulaCuatro(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaCuatro clausula)
        {
            //CLAUSULA SIEMPRE PRESENTE

            var res = new ResultadoClausula();
            if (clausula.Basico.BoletoId != (int)EnumBoletoCompraNet.CONFIRMA)
            {
                res.Texto += $"Todos los firmantes acuerdan que todas las divergencias, cuestiones o reclamaciones que surjan de o que se relacionen con cualquiera " +
               $"de las relaciones jurídicas que se derivan de este contrato y entre cualesquiera de ellos, serán resueltas en forma definitiva por " +
               $"la Cámara Arbitral de la Bolsa de Cereales de {clausula.Basico.BolsaDescripcion}. El tribunal actuará como amigable componedor, con aplicación de " +
               $"las Reglas y Usos del Comercio de Granos y del Reglamento de Procedimientos aprobado por Decreto 931/98 y/o sus futuras modificaciones, ampliaciones o normas complementarias. ";
            }
            return res;
        }
    }
}
