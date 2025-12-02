using NLog;
using Molinos.DataAgro.Entities.ClausulasBoleto.Genericos;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Business.ClausulasBoleto.Genericos
{
    public class ProcesadorClausulaGenericosUno : ProcesadorClausulaGenericos<ClausulaGenericosUno>
    {
        public ProcesadorClausulaGenericosUno(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto) : base(repositorio, log, estadoBoleto)
        {
        }
        public override ResultadoClausula DevolverClausulas(ClausulaGenericosUno clausula)
        {
			//CONTRATO CON BASIS EN APERTURA DE PRECIO
            var res = new ResultadoClausula();
            if (clausula.Basico.ImporteBasis.HasValue && clausula.Basico.ImporteBasis.Value != 0)
            {
                var datosBoleto = EstadoBoleto.EstadoBoleto(clausula.Basico.ContratoSAP, "");
                var condiciones = datosBoleto.CondicionFijacion.FirstOrDefault();
                decimal? importeBasis = (clausula.Basico.ImporteBasis != null && clausula.Basico.ImporteBasis < 0) ? clausula.Basico.ImporteBasis * -1 : clausula.Basico.ImporteBasis;

                res.Texto += $"El vendedor fijará la Mercadería por Mercado {clausula.Basico.TipoPosicionCBOT} posición {MetodosUtiles.CorregirFormatoMes(condiciones.FechaDesde)} desde el {MetodosUtiles.CorregirFormatoFecha(condiciones.FechaDesde)} hasta el {MetodosUtiles.CorregirFormatoFecha(condiciones.FechaHasta)} " +
                    $"menos  {importeBasis?.ToString("N", new CultureInfo("es-AR"))} {clausula.Basico.MonedaBasis}. " +
                    $"Si superado el vencimiento sin que el vendedor haya fijado, el comprador quedará automáticamente facultado para hacerlo.";
            }
            return res;
        }
    }
}
