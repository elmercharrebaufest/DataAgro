using NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;


namespace Molinos.DataAgro.Business.ProcesamientoClausulas
{
    public class ProcesadorClausulaCincuentaYCinco : ProcesadorClausula<ClausulaCincuentaYCinco>
    {
        public ProcesadorClausulaCincuentaYCinco(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto) : base(repositorio, log, estadoBoleto)
        {
        }
        public override ResultadoClausula DevolverClausulas(ClausulaCincuentaYCinco clausula)
        {
            var res = new ResultadoClausula();
            if (clausula.Basico.CorredorId > 0 && clausula.Basico.PagoDirectoVendedor==true)
            {
                    string productor = clausula.Basico.Corredor;
                string corredor = clausula.Basico.CUITCorredor;
                res.Texto += String.Format("Los señores {0}, CUIT N° {1}, actúan en la presente operación en carácter de corredores quedando facultados por los vendedores para fijar el precio, facturar, recibir el pago, firmar recibos de mercadería, ampliaciones y/o anulaciones y convenir eventuales prorrogas. El vendedor faculta al corredor a firmar en su nombre y representación toda la documentación necesaria para la instrumentación o formalización del presente.", productor, corredor);

            }
            return res;
        }
    }
}
