using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using System;
using Humanizer;
using System.Globalization;
using Molinos.DataAgro.Interfaces;

namespace Molinos.DataAgro.Business.Procesamiento
{
    public class ProcesadorClausulaNueve : ProcesadorClausula<ClausulaNueve>
    {
        public ProcesadorClausulaNueve(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaNueve clausula)
        {
            //SI TIENEN INDICADA TARIFA DE FLETE (PUEDE SER CUALQUIER TIPO: POSIBLES VALORES: REAL, ESTIMADO PROVISORIO, ESTIMADO CONFIRMADO)

            var res = new ResultadoClausula();

            if (clausula.Basico.TarifaFlete.HasValue)
            {
                res.Texto += $"Molinos Agro S. A. se hará cargo del traslado de la mercadería abonando una tarifa de $ {clausula.Basico.TarifaFlete?.ToString("N", new CultureInfo("es-AR"))}" +
                    $"({DevolverNumeroEnLetras(clausula.Basico.TarifaFlete.Value)}) por tonelada, la cual se al momento de contratación del " +
                    $"transporte será ajustada sobre la liquidación final o por medio de factura/nota de crédito.";
            }

            return res;
        }

        private string DevolverNumeroEnLetras(decimal numero)
        {
            numero = Math.Round(numero, 2);
            var letras = ((int)Math.Abs(numero)).ToWords(CultureInfo.GetCultureInfo("es-AR")).ToUpper();
            var fraccion = numero - Math.Floor(numero);
            if (fraccion > 0)
            {
                letras += $" con {((int)(fraccion * 100)).ToWords(CultureInfo.GetCultureInfo("es-AR")).ToUpper()}";
            }
            return letras;
        }
    }
}
