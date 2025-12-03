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
    public class ProcesadorClausulaGenericosVeinteYDos : ProcesadorClausulaGenericos<ClausulaGenericosVeinteYDos>
    {
        public ProcesadorClausulaGenericosVeinteYDos(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
            : base(repositorio, log, estadoBoleto) { }

        public override ResultadoClausula DevolverClausulas(ClausulaGenericosVeinteYDos clausula)
        {
            var res = new ResultadoClausula();
            if (clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR && clausula.Basico.Canje == true)
            {
                res.Texto += $"Los Gastos Asociados deberán ser cancelados por el Vendedor mediante la entrega de la cantidad de Mercadería necesaria para cancelar " +
                    $"el monto que dichos Gastos Asociados representen, teniendo en cuenta que la Mercadería será valuada en los términos establecidos anteriormente.";
            }
            return res;
        }
    }
}
