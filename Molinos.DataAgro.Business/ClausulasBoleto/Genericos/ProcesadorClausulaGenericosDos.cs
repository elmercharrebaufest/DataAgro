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
    public class ProcesadorClausulaGenericosDos : ProcesadorClausulaGenericos<ClausulaGenericosDos>
    {
        public ProcesadorClausulaGenericosDos(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
            : base(repositorio, log, estadoBoleto) { }

        public override ResultadoClausula DevolverClausulas(ClausulaGenericosDos clausula)
        {
            var res = new ResultadoClausula();

            if (clausula.Basico.TarifaFlete.HasValue)
            {
                res.Texto += $"Molinos Agro S. A. se hará cargo del traslado de la mercadería abonando una tarifa de $ {clausula.Basico.TarifaFlete?.ToString("N", new CultureInfo("es-AR"))}" +
                    $" ({MetodosUtiles.DevolverNumeroEnLetras(clausula.Basico.TarifaFlete.Value)}) por tonelada, la cual al momento de contratación del " +
                    $"transporte será ajustada sobre la liquidación final o por medio de factura/nota de crédito.";
            }

            return res;
        }
    }
}
