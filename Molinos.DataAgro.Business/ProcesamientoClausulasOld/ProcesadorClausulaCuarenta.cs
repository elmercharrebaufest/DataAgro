using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;

namespace Molinos.DataAgro.Business.Procesamiento
{
    public class ProcesadorClausulaCuarenta : ProcesadorClausula<ClausulaCuarenta>
    {
        public ProcesadorClausulaCuarenta(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaCuarenta clausula)
        {
            //EL MATERIAL ES MAIZ, SOJA, GIRASOL O SORGO
            //EL MATERIAL ES TRIGO O CEBADA (misma cláusula)
            var res = new ResultadoClausula();
			
			/*
            if (clausula.Basico.BoletoId == (int)EnumBoletoCompraNet.CONFIRMA)
            {
                res.Texto += $"Por Resolución del SENASA nro. 149/2018, a partir del 05/11/18 queda prohibido el uso de los principios activos Diclorvos (DDVP) y Triclorfon, en todas las etapas de la cadena granaría," +
                             $" incluyendo las instalaciones para su almacenamiento. El remitente de la mercadería declara bajo juramento que la misma NO fue tratada con DDVP ni Triclorfon, razón por la cual, si vía" +
                             " análisis se detectaran resultados positivos al momento de la descarga, la mercadería podrá ser rechazada dándose por incumplido el contrato de compra con las sanciones e indemnizaciones que" +
                             " correspondan. Si el resultado del análisis positivo se conociera a posteriori de su descarga, además de dar por incumplido el contrato, en los términos indicados anteriormente, el remitente será" +
                             " sancionado con una multa de U$S 20 por tonelada. En ambos casos se dará intervención al SENASA.";
            }
			*/
			/*
            else
            {
                res.Texto += $"Por Resolución SENASA nro. 149/2018, vigente a partir del 05/11/18, está prohibido el uso de Diclorvos (DDVP) y Triclorfon, en todas las etapas de la cadena granaría," +
                             " incluyendo las instalaciones para su almacenamiento. El remitente de la mercadería declara que la mercadería no fue tratada con DDVP, razón por la cual, si mediante análisis" +
                             " realizado, por alguna de las Cámaras Arbitrales del país de muestras lacradas extraídas del medio de transporte de la mercadería, se detectaran al momento de la descarga resultados" +
                             " positivos de alguna de las sustancias prohibidas antes mencionadas, la mercadería podrá ser rechazada dándose por incumplido el contrato de compra, con las sanciones e indemnizaciones" +
                             " que correspondan. Si el resultado del análisis positivo se conociera a posteriori de su descarga, en los términos indicados anteriormente, el remitente será sancionado con una multa" + 
                             " de U$S 20 por tonelada. En ambos casos, se dará intervención al SENASA. Esta cláusula entrará en vigencia a partir de las entregas de la mercadería realizadas el 1 de marzo de 2019.";
            }
			*/
            return res;
        }
    }
}
