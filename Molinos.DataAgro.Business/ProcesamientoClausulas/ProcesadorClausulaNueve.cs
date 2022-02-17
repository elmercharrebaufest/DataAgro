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
    public class ProcesadorClausulaNueve : ProcesadorClausula<ClausulaNueve>
    {
        public ProcesadorClausulaNueve(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaNueve clausula)
        {
            var res = new ResultadoClausula();

            if (!string.IsNullOrEmpty(clausula.Basico.NivelTarifa))
            {
                res.Texto += $"Molinos Agro S. A. se hará cargo del traslado de la mercadería abonando una tarifa de $ { clausula.Basico.TarifaFlete }" +
                    $"({DevolverNumeroEnLetras(clausula.Basico.TarifaFlete ?? 0) }) por tonelada, la cual se al momento de contratación del " +
                    $"transporte será ajustada sobre la liquidación final o por medio de factura/nota de crédito. ";
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
