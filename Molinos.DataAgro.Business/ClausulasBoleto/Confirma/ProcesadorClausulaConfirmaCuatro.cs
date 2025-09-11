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
                res.Texto += $"Por Resolución del SENASA nro. 149/2018, a partir del 05/11/18 queda prohibido el uso de los principios activos Diclorvos (DDVP) y Triclorfon, en todas las etapas de la cadena granaría," +
                             $" incluyendo las instalaciones para su almacenamiento. El remitente de la mercadería declara bajo juramento que la misma NO fue tratada con DDVP ni Triclorfon, razón por la cual, si vía" +
                             " análisis se detectaran resultados positivos al momento de la descarga, la mercadería podrá ser rechazada dándose por incumplido el contrato de compra con las sanciones e indemnizaciones que" +
                             " correspondan. Si el resultado del análisis positivo se conociera a posteriori de su descarga, además de dar por incumplido el contrato, en los términos indicados anteriormente, el remitente será" +
                             " sancionado con una multa de U$S 20 por tonelada. En ambos casos se dará intervención al SENASA.";
            return res;
        }
    }
}
