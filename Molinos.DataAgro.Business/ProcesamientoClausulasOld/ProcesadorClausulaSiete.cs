using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using System;
using System.Globalization;
using Molinos.DataAgro.Interfaces;
using System.Linq;
using Molinos.DataAgro.Entities.Common.Enums;
using Humanizer;

namespace Molinos.DataAgro.Business.Procesamiento
{
    public class ProcesadorClausulaSiete : ProcesadorClausula<ClausulaSiete>
    {
        public ProcesadorClausulaSiete(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaSiete clausula)
        {
            //CONTRATO CON BASIS EN APERTURA DE PRECIO

            var res = new ResultadoClausula();
			/*
            if (clausula.Basico.ImporteBasis.HasValue && clausula.Basico.ImporteBasis.Value != 0)
            {
                var datosBoleto = EstadoBoleto.EstadoBoleto(clausula.Basico.ContratoSAP, "");
                var condiciones = datosBoleto.CondicionFijacion.FirstOrDefault();
                decimal? importeBasis = (clausula.Basico.ImporteBasis != null && clausula.Basico.ImporteBasis < 0) ? clausula.Basico.ImporteBasis * -1 : clausula.Basico.ImporteBasis;

                res.Texto += $"El vendedor fijará la Mercadería por Mercado {clausula.Basico.TipoPosicionCBOT} posición {CorregirFormatoMes(condiciones.FechaDesde)} desde el {CorregirFormatoFecha(condiciones.FechaDesde)} hasta el {CorregirFormatoFecha(condiciones.FechaHasta)} " +
                    $"menos  {importeBasis?.ToString("N", new CultureInfo("es-AR"))} {clausula.Basico.MonedaBasis}. " +
                    $"Si superado el vencimiento sin que el vendedor haya fijado, el comprador quedará automáticamente facultado para hacerlo.";
            }
			*/
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

        private string CorregirFormatoFecha(string cadena)
        {
            var date = DateTime.Parse(cadena);
            return date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        }
        private string CorregirFormatoMes(string cadena)
        {
            var date = DateTime.Parse(cadena);
            return date.ToString("MM.yyyy", CultureInfo.InvariantCulture);
        }
    }
}
