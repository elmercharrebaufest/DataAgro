using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.ClausulasBoleto.BoletoFisico;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;

namespace Molinos.DataAgro.Business.ClausulasBoleto.BoletoFisico
{
    public class ProcesadorClausulaBoletoFisicoDiesiocho : ProcesadorClausulaBoletoFisico<ClausulaBoletoFisicoDiesiocho>
    {
        public ProcesadorClausulaBoletoFisicoDiesiocho(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
            : base(repositorio, log, estadoBoleto) { }

        public override ResultadoClausula DevolverClausulas(ClausulaBoletoFisicoDiesiocho clausula)
        {
            var res = new ResultadoClausula();
            if (clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO && (clausula.Basico.Dolarizado == true || clausula.Basico.DolarizadoExpress == true || clausula.Basico.DolarizadoCorredor == true))
            {

                res.Texto += $"Las Partes acuerdan la posibilidad de prorrogar el plazo de pago indicado en las cláusulas previas, a opción " +
                             $"del Vendedor hasta el {clausula.Basico.Fecha_Dolarizado.GetValueOrDefault():dd/MM/yyyy} . El Vendedor podrá hasta la fecha indicada en el párrafo que antecede, requerir al Comprador " +
                             $"proceder al pago de la Mercadería mediante el envío de la notificación. El Comprador deberá abonar los impuestos y el pago parcial " +
                             $"de la Mercadería en forma conjunta. En este caso el Vendedor deberá enviar a Molinos Agro una notificación en la cual le manifestará " +
                             $"que deberá proceder al pago de la Mercadería dentro de las 72 hs hábiles siguientes al envío de la Notificación, en la medida que " +
                             $"sea enviada hasta las 13 horas. Si la Notificación es enviada con posterioridad a las 13 horas, se considerará enviada el día hábil " +
                             $"siguiente. Dicha notificación deberá ser cursada por el Vendedor exclusivamente desde la plataforma autorizada: www.moaoperaciones.com.ar, " +
                             $"ingresando su usuario previamente habilitado en conformidad con los términos y condiciones del sitio. Las Partes acuerdan que la obligación será " +
                             $"pagadera en pesos argentinos al tipo de cambio comprador publicado por el Banco de la Nación Argentina, salvo en el caso en que dicho tipo de cambio " +
                             $"no sea el que resulte aplicable para la liquidación de divisas proveniente de la exportación de granos, en cuyo caso se tomará este último. En ese sentido, " +
                             $"las Partes establecen de común acuerdo que la única forma de pago válida de las liquidaciones parciales y finales es la que se plasma en el presente instrumento, " +
                             $"y la que rige según usos y costumbres del mercado de granos excluyendo cualquier otra que se pretenda o intente aplicar que atente contra el principio de autonomía " +
                             $"de voluntad de las partes y de dichos usos y costumbres. Si el Vendedor no enviara la Notificación hasta el {clausula.Basico.Fecha_Dolarizado.GetValueOrDefault():dd/MM/yyyy}, el Comprador fijará el tipo de cambio en " +
                             $"conformidad al párrafo que antecede, y procederá dentro de las 48 hs hábiles siguientes a abonarle al Vendedor el precio de la mercadería que corresponda. Toda vez " +
                             $"que las Partes han acordado la opción de prorrogar la fecha de pago de la Mercadería, queda expresamente establecido el Vendedor no podrá invocar mora ni reclamar " +
                             $"intereses y/o multas y/o cualquier tipo de penalidad por el tiempo transcurrido entre el plazo de pago originario y el del ejercicio de la opción de prórroga acordada " +
                             $"en la presente Cláusula.";
            }
            else
            {
                if (clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR || (clausula.Basico.DolarizadoExpress == true || clausula.Basico.Dolarizado == true))
                {
                    res.Texto += "Las Partes acuerdan la posibilidad de prorrogar el plazo de pago indicado en las cláusulas previas. El precio a pagar será neto de los impuestos y retenciones impositivas que correspondieran y se hubieran ";
                    res.Texto += "practicado según la condición del Vendedor y las particularidades del negocio. Toda vez que las Partes han acordado la posibilidad de prorrogar la fecha de pago de la Mercadería, queda expresamente ";
                    res.Texto += "establecido que el Vendedor no podrá invocar mora ni reclamar intereses y/o multas y/o cualquier tipo de penalidad por el tiempo transcurrido entre el plazo de pago originario y el del ejercicio de la opción de ";
                    res.Texto += "prórroga acordada en la presente Cláusula.";
                }
            }
            return res;
        }
    }
}
