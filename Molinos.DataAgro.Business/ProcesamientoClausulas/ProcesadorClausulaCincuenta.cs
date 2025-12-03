using NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Entities.Common.Enums;

namespace Molinos.DataAgro.Business.Procesamiento
{
    public class ProcesadorClausulaCincuenta : ProcesadorClausula<ClausulaCincuenta>
    {
        public ProcesadorClausulaCincuenta(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaCincuenta clausula)
        {
            //SI EL NEGOCIO ES FIJACIÓN DISPONIBLE DE UN CONTRATO DE CANJE O ES UN NEGOCIO A FIJAR O NEGOCIO CONVENIO (separado de cláusula 3 por DAT100-2865)
            var res = new ResultadoClausula();

            if (clausula.Basico.BoletoId != (int)EnumBoletoCompraNet.CARTA_OFERTA)
            {
                var esFijacionDeContratoCanje = clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.FIJACION && clausula.Basico.Canje == true;
                var esAFijarSinCanje = clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR && clausula.Basico.Canje != true;
                var esConvenio = clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR && clausula.Basico.Madre == true;
                if (esAFijarSinCanje || esConvenio || esFijacionDeContratoCanje)
                {

                    res.Texto += $"Las Partes acuerdan la posibilidad de prorrogar el plazo de pago indicado en las cláusulas previas. ";
                    res.Texto += $"El Precio a pagar será neto de los impuestos y retenciones impositivas que correspondieran y se hubieran practicado según la condición del Vendedor y las particularidades del negocio. ";
                    res.Texto += $"Toda vez que las Partes han acordado la posibilidad de prorrogar la fecha de pago de la Mercadería, queda expresamente establecido que el Vendedor no podrá invocar mora ni reclamar intereses y/o multas y/o cualquier tipo de penalidad por el tiempo transcurrido entre el plazo de pago originario y el del ejercicio de la opción de prórroga acordada en la presente Cláusula.";

                    //res.Texto += $"Las Partes acuerdan la posibilidad de prorrogar el plazo de pago indicado en las cláusulas previas, a opción del Vendedor. " +
                    //$"En caso de ejercer la opción al momento de fijar el precio, el Vendedor deberá notificar a Molinos Agro que deberá diferir el pago de la Mercadería por el plazo determinado, y el mismo no podrá extenderse de los 120 días. " +
                    //$"El Precio a pagar será neto de los impuestos y retenciones impositivas que correspondieran y se hubieran practicado según la condición del Vendedor y las particularidades del negocio. " +
                    //$"Toda vez que las Partes han acordado la opción de prorrogar la fecha de pago de la Mercadería, queda expresamente establecido el Vendedor no podrá invocar mora ni reclamar intereses y/o multas y/o cualquier tipo de penalidad por el tiempo transcurrido entre el plazo de pago originario y el del ejercicio de la opción de prórroga acordada en la presente Cláusula.";

                }
            }

            return res;
        }
    }
}
