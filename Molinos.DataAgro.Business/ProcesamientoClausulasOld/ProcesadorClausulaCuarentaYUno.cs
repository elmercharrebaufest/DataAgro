using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Entities.Common.Enums;

namespace Molinos.DataAgro.Business.Procesamiento
{
    public class ProcesadorClausulaCuarentaYUno : ProcesadorClausula<ClausulaCuarentaYUno>
    {
        public ProcesadorClausulaCuarentaYUno(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaCuarentaYUno clausula)
        {
            //MIENTRAS ESTÉN VIGENTES LOS DNU COVID

            var res = new ResultadoClausula();
            res.Texto += "";
            /*
			if (clausula.Basico.BoletoId != (int)EnumBoletoCompraNet.CONFIRMA && clausula.Basico.BoletoId != (int)EnumBoletoCompraNet.CARTA_OFERTA)
            {
                res.Texto += "A los fines de cumplir con normas legales e impositivas, el vendedor/corredor se encuentra obligado a remitir al comprador el original del presente boleto, debidamente suscripto, a efectos de su presentación en la Bolsa de Cereales.";
            }
			*/
            return res;
        }
    }
}
