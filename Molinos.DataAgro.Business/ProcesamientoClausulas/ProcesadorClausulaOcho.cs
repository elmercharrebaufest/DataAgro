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
    public class ProcesadorClausulaOcho : ProcesadorClausula<ClausulaOcho>
    {
        public ProcesadorClausulaOcho(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaOcho clausula)
        {
            var res = new ResultadoClausula();

            //res.Texto += "Ninguna de las partes podrá ceder o transferir en forma alguna, ni total ni parcialmente, ni el contrato ni los derechos y/u obligaciones " +
            //    "emergentes del mismo sin la expresa conformidad de la otra parte.";
            res.Texto += $"Los derechos y obligaciones emergentes del presente contrato no podrán ser cedidos por ninguna de las " + 
                "partes, sin la previa y expresa conformidad de la otra. Sin embargo, dicha conformidad no será necesaria para el caso que Molinos " + "resolviera cederlos a alguna de sus sociedades controladas, vinculadas y/o relacionadas, existentes o no al momento de firma del" + " presente contrato. En tal caso, el cedente o el cesionario notificarán fehacientemente al vendedor";
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
