using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using System;
using Humanizer;
using System.Globalization;
using System.Linq;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Entities.Common.Enums;

namespace Molinos.DataAgro.Business.Procesamiento
{
    public class ProcesadorClausulaVeintiseis : ProcesadorClausula<ClausulaVeintiseis>
    {
        public ProcesadorClausulaVeintiseis(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaVeintiseis clausula)
        {
            var res = new ResultadoClausula();
            if (clausula.Basico.TipoNegocioId == 1 && clausula.Basico.Canje == true)
            {
                res.Texto += $"Toda vez que la presente es una operación de canje, el pago de la Mercadería será imputado directamente por el Comprador a la " +
                    $"cancelación del Costo del Insumo y los Gastos Asociados. La imputación mencionada será realizada por el Comprador a su sola discreción, y " +
                    $"siempre que la Mercadería hubiere sido entregada en las condiciones aquí pactadas, a satisfacción del Comprador, y en el lugar y plazos " +
                    $"indicados en este Boleto. Si por cualquier motivo (incluyendo variación de alícuotas fiscales) se produjera una desigualdad entre las " +
                    $"contraprestaciones durante la vigencia del presente Boleto, la desigualdad deberá ser compensada por la Parte correspondiente mediante " +
                    $"el incremento de Mercadería o Insumo (dependiente el caso) a ser entregado.";
            }
            return res;
        }

        public string DevolverNumeroEnLetras(decimal numero)
        {
            var letras = ((int)Math.Abs(numero)).ToWords(CultureInfo.GetCultureInfo("es-AR")).ToUpper();             
            var fraccion = numero - Math.Floor(numero); 
            if (fraccion > 0)
            {
                letras += $" con {((int)(fraccion * 100)).ToWords(CultureInfo.GetCultureInfo("es-AR")).ToUpper() }";

            }
            return letras;
        }
    }
}
