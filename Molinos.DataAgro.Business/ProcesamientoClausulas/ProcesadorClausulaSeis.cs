using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;

namespace Molinos.DataAgro.Business.Procesamiento
{
    public class ProcesadorClausulaSeis : ProcesadorClausula<ClausulaSeis>
    {
        public ProcesadorClausulaSeis(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaSeis clausula)
        {
            //No aplica a carta oferta porque no se sella. Es solo para boleto físico y confirma

            var res = new ResultadoClausula();

            if (clausula.Basico.BoletoId != (int)EnumBoletoCompraNet.CARTA_OFERTA && clausula.Basico.BoletoId != (int)EnumBoletoCompraNet.CONFIRMA)
            {
                res.Texto += $"El importe del sellado del presente Contrato como los honorarios de la Cámara Arbitral de la Bolsa de Cereales de {clausula.Basico.BolsaDescripcion} " +
                     $"será abonado por ";
                if (clausula.Basico.SelCargoVendedor == true) //SI TIENE TILDE EN SELLADO 100% A CARGO DEL VENDEDOR
                {
                    res.Texto += $"el vendedor en forma total.";
                }
                else if (clausula.Basico.SelCargoMOA == true) //SI TIENE TILDE EN SELLADO 100% A CARGO DE MOA
                {
                    res.Texto += $"el comprador.";
                }
                else
                {
                    res.Texto += $"las partes en partes iguales.";
                }
            }

            return res;
        }
    }
}
