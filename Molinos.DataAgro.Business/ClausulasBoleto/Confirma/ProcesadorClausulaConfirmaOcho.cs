using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.ClausulasBoleto.Confirma;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
namespace Molinos.DataAgro.Business.ClausulasBoleto.Confirma
{
    public class ProcesadorClausulaConfirmaOcho : ProcesadorClausulaConfirma<ClausulaConfirmaOcho>
    {
        public ProcesadorClausulaConfirmaOcho(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
            : base(repositorio, log, estadoBoleto) { }

        public override ResultadoClausula DevolverClausulas(ClausulaConfirmaOcho clausula)
        {
            var res = new ResultadoClausula();
            if (clausula.Basico.CorredorId > 0 && clausula.Basico.PagoDirectoVendedor == true)
            {
                string productor = clausula.Basico.Corredor;
                string corredor = clausula.Basico.CUITCorredor;
                res.Texto += String.Format("Los señores {0}, CUIT N° {1}, actúan en la presente operación en carácter de corredores quedando facultados por los vendedores para fijar el precio, facturar, recibir el pago, firmar recibos de mercadería, ampliaciones y/o anulaciones y convenir eventuales prorrogas. El vendedor faculta al corredor a firmar en su nombre y representación toda la documentación necesaria para la instrumentación o formalización del presente.", productor, corredor);

            }
            return res;
        }
    }
}
