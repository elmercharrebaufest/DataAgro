using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Entities.Common.Enums;

namespace Molinos.DataAgro.Business.Procesamiento
{
    public class ProcesadorClausulaCincuentaYDos : ProcesadorClausula<ClausulaCincuentaYDos>
    {
        public ProcesadorClausulaCincuentaYDos(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaCincuentaYDos clausula)
        {
            //Clausula Solo para confirma

            var res = new ResultadoClausula();

            if (clausula.Basico.BoletoId == (int)EnumBoletoCompraNet.CONFIRMA)
            {
                res.Texto += "El pago de la liquidación final se hará a los 30 días de la entrega de la mercadería, en caso de corresponder.";
            }

            return res;
        }
    }
}
