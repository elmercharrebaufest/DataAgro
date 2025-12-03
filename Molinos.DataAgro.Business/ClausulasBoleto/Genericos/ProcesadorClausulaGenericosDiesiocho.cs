using NLog;
using Molinos.DataAgro.Entities.ClausulasBoleto.Genericos;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Business.ClausulasBoleto.Genericos
{
    public class ProcesadorClausulaGenericosDiesiocho : ProcesadorClausulaGenericos<ClausulaGenericosDiesiocho>
    {
        public ProcesadorClausulaGenericosDiesiocho(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
            : base(repositorio, log, estadoBoleto) { }

        public override ResultadoClausula DevolverClausulas(ClausulaGenericosDiesiocho clausula)
        {
            var res = new ResultadoClausula();
            if (clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR && clausula.Basico.Canje == true)
            {
                res.Texto += $"La entrega de la Mercadería no es excusable bajo ningún supuesto, no pudiendo alegarse causales de caso fortuito o fuerza mayor. " +
                    $"La obligación de entrega de la Mercadería por parte del Vendedor se reputará cumplida -únicamente- con (i) la entrega total de la Mercadería, " +
                    $"necesaria para cancelar el Costo del Insumo y los Gastos Asociados, y (ii) la Mercadería entregada deberá reunir y cumplir las condiciones de " +
                    $"calidad solicitadas.";
            }
            return res;
        }
    }
}
