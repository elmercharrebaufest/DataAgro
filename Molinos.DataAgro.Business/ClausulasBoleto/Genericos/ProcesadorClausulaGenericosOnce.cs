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
    public class ProcesadorClausulaGenericosOnce : ProcesadorClausulaGenericos<ClausulaGenericosOnce>
    {
        public ProcesadorClausulaGenericosOnce(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
            : base(repositorio, log, estadoBoleto) { }

        public override ResultadoClausula DevolverClausulas(ClausulaGenericosOnce clausula)
        {
            var res = new ResultadoClausula();
            /*
            if (clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO && (clausula.Basico.Dolarizado == true || clausula.Basico.DolarizadoCorredor == true))
            {
                res.Texto += $"Las Partes acuerdan la posibilidad de prorrogar el plazo de pago indicado en las cláusulas previas, a opción del Vendedor hasta el " +
                    $"{clausula.Basico.Fecha_Dolarizado.GetValueOrDefault():dd/MM/yyyy} siempre que hayan transcurrido 30 días desde la entrega de la mercadería. En caso de ejercer la opción, " +
                    $"el Vendedor deberá enviar a Molinos Agro una notificación en la cual manifestará a Molinos Agro que deberá proceder al pago de la Mercadería " +
                    $"dentro de las 72 hs hábiles siguientes al envío de la Notificación, en la medida que sea enviado hasta las 13 horas. Si la Notificación es " +
                    $"enviada con posterioridad a las 13 horas, se considerará enviada el día hábil siguiente. Las Partes acuerdan que el pago se realizará a las " +
                    $"72 hs hábiles siguientes de la Notificación, al tipo de cambio comprador publicado por el Banco de la Nación Argentina del día de la " +
                    $"Notificación o del día hábil siguiente de haber sido enviada después de las 13:00 horas. Si el Vendedor no enviara la Notificación hasta el " +
                    $"{clausula.Basico.Fecha_Dolarizado.GetValueOrDefault():dd/MM/yyyy} el Comprador fijará el tipo de cambio en conformidad al párrafo que antecede, y procederá dentro de las " +
                    $"48 horas hábiles siguientes a abonarle al Vendedor el precio de la mercadería que corresponda. El Precio a pagar será neto de los impuestos y " +
                    $"retenciones impositivas que correspondieran y se hubieran practicado según la condición del Vendedor y las particularidades del negocio. " +
                    $"Toda vez que las Partes han acordado la opción de prorrogar la fecha de pago de la Mercadería, queda expresamente establecido el Vendedor no " +
                    $"podrá invocar mora ni reclamar intereses y/o multas y/o cualquier tipo de penalidad por el tiempo transcurrido entre el plazo de pago originario " +
                    $"y el del ejercicio de la opción de prórroga acordada en la presente Cláusula.";
            }
            */
            return res;
        }
    }
}
