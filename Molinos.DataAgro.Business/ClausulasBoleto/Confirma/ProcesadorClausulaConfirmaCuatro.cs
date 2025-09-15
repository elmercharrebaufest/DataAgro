using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.ClausulasBoleto.Confirma;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using Molinos.DataAgro.Entities.Entities;
namespace Molinos.DataAgro.Business.ClausulasBoleto.Confirma
{
    public class ProcesadorClausulaConfirmaCuatro : ProcesadorClausulaConfirma<ClausulaConfirmaCuatro>
    {
        public ProcesadorClausulaConfirmaCuatro(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
            : base(repositorio, log, estadoBoleto) { }

        public override ResultadoClausula DevolverClausulas(ClausulaConfirmaCuatro clausula)
        {
            var res = new ResultadoClausula();

            if (clausula.Basico.MaterialId == (int)EnumMateriales.SOJA || clausula.Basico.MaterialId == (int)EnumMateriales.MAIZ || clausula.Basico.MaterialId == (int)EnumMateriales.SORGO)
            {
                res.Texto += $"Por Resolución SENASA nro. 149/2018, vigente a partir del 05/11/18, está prohibido el uso de Diclorvos (DDVP) y Triclorfon, en todas las etapas de la cadena granaría," + 
                              " incluyendo las instalaciones para su almacenamiento. El remitente de la mercadería declara que la mercadería no fue tratada con DDVP, razón por la cual, si mediante análisis realizado," + 
                              " por alguna de las Cámaras Arbitrales del país de muestras lacradas extraídas del medio de transporte de la mercadería, se detectaran al momento de la descarga resultados positivos de alguna" + 
                              " de las sustancias prohibidas antes mencionadas, la mercadería podrá ser rechazada dándose por incumplido el contrato de compra, con las sanciones e indemnizaciones que correspondan." + 
                              " Si el resultado del análisis positivo se conociera a posteriori de su descarga, en los términos indicados anteriormente, el remitente será sancionado con una multa de U$S 20 por tonelada." + 
                              " En ambos casos, se dará intervención al SENASA. Esta cláusula entrará en vigencia a partir de las entregas de la mercadería realizadas el 1 de marzo de 2019.";
            }
            else if (clausula.Basico.MaterialId == (int)EnumMateriales.TRIGO)
            {
                res.Texto += $"Por Resolución SENASA nro. 149/2018, vigente a partir del 05/11/18, está prohibido el uso de Diclorvos (DDVP) y Triclorfon, en todas las etapas de la cadena granaría," + 
                              " incluyendo las instalaciones para su almacenamiento. El remitente de la mercadería declara que la mercadería no fue tratada con DDVP, razón por la cual, si mediante análisis realizado," + 
                              " por alguna de las Cámaras Arbitrales del país de muestras lacradas extraídas del medio de transporte de la mercadería, se detectaran al momento de la descarga resultados positivos de alguna" + 
                              " de las sustancias prohibidas antes mencionadas, la mercadería podrá ser rechazada dándose por incumplido el contrato de compra, con las sanciones e indemnizaciones que correspondan." + 
                              " Si el resultado del análisis positivo se conociera a posteriori de su descarga, en los términos indicados anteriormente, el remitente será sancionado con una multa de U$S 20 por tonelada." + 
                              " En ambos casos, se dará intervención al SENASA. Esta cláusula entrará en vigencia a partir de las entregas de la mercadería realizadas el 1 de octubre de 2019.";
            }
            else
            {
                res.Texto += $"Por Resolución SENASA nro. 149/2018, a partir del 05/11/18 queda prohibido el uso de los principios activos Diclorvos (DDVP) y Triclorfon, en todas las etapas de la cadena granaría," +
                             $" incluyendo las instalaciones para su almacenamiento. El remitente de la mercadería declara bajo juramento que la misma NO fue tratada con DDVP ni Triclorfon, razón por la cual, si vía" +
                             " análisis se detectaran resultados positivos al momento de la descarga, la mercadería podrá ser rechazada dándose por incumplido el contrato de compra con las sanciones e indemnizaciones que" +
                             " correspondan. Si el resultado del análisis positivo se conociera a posteriori de su descarga, además de dar por incumplido el contrato, en los términos indicados anteriormente, el remitente será" +
                             " sancionado con una multa de U$S 20 por tonelada. En ambos casos se dará intervención al SENASA.";
            }
            return res;
        }
    }
}
