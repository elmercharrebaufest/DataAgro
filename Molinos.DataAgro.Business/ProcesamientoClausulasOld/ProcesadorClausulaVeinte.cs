using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
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
            //SI EN EL CONTRATO CONTIENE CORREDOR

            var res = new ResultadoClausula();
			/*
            if (clausula.Basico.CorredorId > 0 && !string.IsNullOrEmpty(clausula.Basico.CUITCorredor) && clausula.Basico.BoletoId != (int)EnumBoletoCompraNet.CONFIRMA)
            {
                res.Texto += $"Los señores {clausula.Basico.RazonSocialCorredor}, CUIT N° {FormatoCuit(clausula.Basico.CUITCorredor)}, actúan en la presente operación en carácter de " +
                    $"corredores quedando facultados por los vendedores para fijar el precio, facturar, recibir el pago, firmar recibos de mercadería, ampliaciones " +
                    $"y/o anulaciones y convenir eventuales prorrogas. El vendedor faculta al corredor a firmar en su nombre y representación toda la documentación " +
                    $"necesaria para la instrumentación o formalización del presente.";
            }
			*/
            return res;
        }

        private string FormatoCuit(string cuit)
        {
            if (cuit.Length == 11)
            {
                cuit = $"{cuit.Substring(0, 2)}-{cuit.Substring(2, 8)}-{cuit.Substring(10, 1)}";
            }

            return cuit;
        }
    }
}
