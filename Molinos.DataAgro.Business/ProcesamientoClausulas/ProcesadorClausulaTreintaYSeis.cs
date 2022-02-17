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
    public class ProcesadorClausulaTreintaYSeis : ProcesadorClausula<ClausulaTreintaYSeis>
    {
        public ProcesadorClausulaTreintaYSeis(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaTreintaYSeis clausula)
        {
            var res = new ResultadoClausula();
            if (clausula.Basico.TipoNegocioId == 1 && clausula.Basico.Canje == true)
            {
                res.Texto += $"Serán a cargo del Vendedor la totalidad de los gastos que ocasionare a su persona como al Comprador, el incumplimiento de sus " +
                    $"obligaciones bajo el presente Boleto.";
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
