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
    public class ProcesadorClausulaGenericosTreintaYUno : ProcesadorClausulaGenericos<ClausulaGenericosTreintaYUno>
    {
        public ProcesadorClausulaGenericosTreintaYUno(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
            : base(repositorio, log, estadoBoleto) { }

        public override ResultadoClausula DevolverClausulas(ClausulaGenericosTreintaYUno clausula)
        {
            var res = new ResultadoClausula();
            if (clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR && (clausula.Basico.ClasificacionContrato == "ACOPIADOR" || clausula.Basico.CorredorId > 0))
            {
                res.Texto += "En caso que el Acopiador/Corredor no proceda a liquidar la mercadería dentro de las 72 horas corridas desde que la misma fuera " +
                    "entregada, aplicada y fijada, las Partes acuerdan que quedará a opción del Comprador determinar el día que se tomará válido para establecer " +
                    "el tipo de cambio a utilizar en los términos dispuestos en el presente boleto.";
            }
            return res;
        }
    }
}
