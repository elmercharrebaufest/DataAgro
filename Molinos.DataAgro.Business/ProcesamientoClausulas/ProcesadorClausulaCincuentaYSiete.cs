using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Business.ProcesamientoClausulas
{
    public class ProcesadorClausulaCincuentaYSiete : ProcesadorClausula<ClausulaCincuentaYSiete>
    {
        public ProcesadorClausulaCincuentaYSiete(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto) : base(repositorio, log, estadoBoleto)
        {
        }

        public override ResultadoClausula DevolverClausulas(ClausulaCincuentaYSiete clausula)
        {
            var res = new ResultadoClausula();
            if (clausula.Basico.CorredorId > 0 || clausula.Basico.PagoDirectoVendedor == true)
            {
                string razonSocialProveedor = clausula.Basico.Corredor;
                string razonSocialCorredor = clausula.Basico.Proveedor;
                res.Texto += String.Format("El pago de las Parciales y Finales resultantes del presente contrato serán realizados directamente a {0} ; Los señores {1} no se encuentran autorizados a recibir pagos ni firmar recibos de mercadería.", razonSocialProveedor, razonSocialCorredor);
            }
            return res;
        }
    }
}
