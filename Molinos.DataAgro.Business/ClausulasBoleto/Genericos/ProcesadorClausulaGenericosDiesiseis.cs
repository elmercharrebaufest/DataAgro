using Autofac.Extras.NLog;
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
    public class ProcesadorClausulaGenericosDiesiseis : ProcesadorClausulaGenericos<ClausulaGenericosDiesiseis>
    {
        public ProcesadorClausulaGenericosDiesiseis(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
            : base(repositorio, log, estadoBoleto) { }

        public override ResultadoClausula DevolverClausulas(ClausulaGenericosDiesiseis clausula)
        {
            var res = new ResultadoClausula();
            if (clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR && clausula.Basico.Canje == true)
            {
                res.Texto += $"Queda expresamente establecido que la cantidad de Mercadería a ser entregada será determinada: En función del precio de acuerdo a " +
                    $"las fijaciones realizadas por el Vendedor hasta la Fecha Límite de Fijación, y en función de las condiciones en las que la Mercadería fue " +
                    $"entregada.";
            }
            return res;
        }
    }
}
