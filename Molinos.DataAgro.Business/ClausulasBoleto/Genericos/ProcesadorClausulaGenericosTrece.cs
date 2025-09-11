using Autofac.Extras.NLog;
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
    public class ProcesadorClausulaGenericosTrece : ProcesadorClausulaGenericos<ClausulaGenericosTrece>
    {
        public ProcesadorClausulaGenericosTrece(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
            : base(repositorio, log, estadoBoleto) { }

        public override ResultadoClausula DevolverClausulas(ClausulaGenericosTrece clausula)
        {
            var res = new ResultadoClausula();
            if (clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR && clausula.Basico.Canje == true)
            {
                res.Texto += $"A los efectos fiscales, únicamente, las partes fijan el valor del presente boleto en la suma de " +
                    $"{clausula.Basico.MonedaCanjeDescripcion} {clausula.Basico.Monto?.ToString("N", new CultureInfo("es-AR"))} ({MetodosUtiles.DevolverNumeroEnLetras(clausula.Basico.Monto.Value)}) " +
                    $"que el impuesto de sellos correspondiente será abonado por el comprador y el vendedor en partes iguales.";
            }
            return res;
        }
    }
}
