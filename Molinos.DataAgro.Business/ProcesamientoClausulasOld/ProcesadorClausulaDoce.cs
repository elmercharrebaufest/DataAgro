using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Entities.Common.Enums;

namespace Molinos.DataAgro.Business.Procesamiento
{
    public class ProcesadorClausulaDoce : ProcesadorClausula<ClausulaDoce>
    {
        public ProcesadorClausulaDoce(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaDoce clausula)
        {
            //CLAUSULA SIEMPRE PRESENTE

            var res = new ResultadoClausula();
            /*
			if (clausula.Basico.BoletoId != (int)EnumBoletoCompraNet.CONFIRMA)
            {
                res.Texto += "El comprador otorgará el cupo con un código alfanumérico que obligatoriamente debe consignarse en el campo observaciones de cada carta de porte. " +
                "En caso de que el vendedor remita camiones sin poseer cupo para la descarga, el comprador podrá, a su exclusiva opción, proceder a la descarga de los mismos, " +
                "debiendo en tal caso el vendedor abonar al comprador U$S 10 (DIEZ dólares) por tonelada en concepto de gastos extras por descargas no otorgadas. ";
            }
			*/
            return res;
        }
    }
}
