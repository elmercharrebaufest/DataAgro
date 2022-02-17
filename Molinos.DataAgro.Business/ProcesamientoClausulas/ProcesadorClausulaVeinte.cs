using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using System;
using Humanizer;
using System.Globalization;
using System.Linq;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Entities.Common.Enums;

namespace Molinos.DataAgro.Business.Procesamiento
{
    public class ProcesadorClausulaVeinte : ProcesadorClausula<ClausulaVeinte>
    {
        public ProcesadorClausulaVeinte(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaVeinte clausula)
        {
            var res = new ResultadoClausula();

            if (clausula.Basico.ContratoId > 0)
            {
                res.Texto += $"Los señores {clausula.Basico.RazonSocialCorredor }, CUIT N° {clausula.Basico.CUITCorredor }, actúan en la presente operación en carácter de " +
                    $"corredores quedando facultados por los vendedores para fijar el precio, facturar, recibir el pago, firmar recibos de mercadería, ampliaciones " +
                    $"y/o anulaciones y convenir eventuales prorrogas. El vendedor faculta al corredor a firmar en su nombre y representación toda la documentación " +
                    $"necesaria para la instrumentación o formalización del presente.";
            }

            return res;
        }

        public string DevolverNumeroEnLetras(decimal numero)
        {
            var letras = ((int)Math.Abs(numero)).ToWords(CultureInfo.GetCultureInfo("es-AR")).ToUpper();             
            var fraccion = numero - Math.Floor(numero); 
            if (fraccion > 0)
            {
                letras += $" con {((int)(fraccion * 100)).ToWords(CultureInfo.GetCultureInfo("es-AR")).ToUpper() }";

            }
            return letras;
        }
    }
}
