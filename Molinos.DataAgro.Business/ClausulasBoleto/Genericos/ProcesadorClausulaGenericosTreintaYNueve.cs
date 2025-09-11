using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.ClausulasBoleto.Genericos;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Business.ClausulasBoleto.Genericos
{
    public class ProcesadorClausulaGenericosTreintaYNueve : ProcesadorClausulaGenericos<ClausulaGenericosTreintaYNueve>
    {
        public ProcesadorClausulaGenericosTreintaYNueve(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
            : base(repositorio, log, estadoBoleto) { }

        public override ResultadoClausula DevolverClausulas(ClausulaGenericosTreintaYNueve clausula)
        {
            var res = new ResultadoClausula();
            string razonSocialProveedor = clausula.Basico.Corredor;
            string razonSocialCorredor = clausula.Basico.Proveedor;
            if (clausula.Basico.CorredorId > 0 && clausula.Basico.PagoDirectoVendedor == true)
            {
                res.Texto += String.Format("El pago de las Parciales y Finales resultantes del presente contrato serán realizados directamente a {0} ; Los señores {1} no se encuentran autorizados a recibir pagos ni firmar recibos de mercadería.", razonSocialCorredor, razonSocialProveedor);
            }
            else if (clausula.Basico.CorredorId == 0 && clausula.Basico.PagoDirectoVendedor == true)
            {
                res.Texto += String.Format("El pago de las Parciales y Finales resultantes del presente contrato serán realizados directamente a {0}; Los señores no se encuentran autorizados a recibir pagos ni firmar recibos de mercadería.", razonSocialProveedor);
            }
            return res;
        }
    }
}
