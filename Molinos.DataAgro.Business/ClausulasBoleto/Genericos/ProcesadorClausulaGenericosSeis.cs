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
    public class ProcesadorClausulaGenericosSeis : ProcesadorClausulaGenericos<ClausulaGenericosSeis>
    {
        public ProcesadorClausulaGenericosSeis(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
            : base(repositorio, log, estadoBoleto) { }

        public override ResultadoClausula DevolverClausulas(ClausulaGenericosSeis clausula)
        {
            var res = new ResultadoClausula();
            if (clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO)
            {
                var descuentoGeneralSobrePrecio = clausula.Basico.Descuentos?.Find(x => x.TipoPeriodoDBId == 1 && x.TipoDBId == 1);

                if (descuentoGeneralSobrePrecio?.Porcentaje > 0)
                {
                    res.Texto += $"De acuerdo a las siguientes fechas de entrega y recibos, desde el {clausula.Basico.FechaDesde.GetValueOrDefault():dd'/'MM'/'yyyy} al {clausula.Basico.FechaHasta.GetValueOrDefault():dd'/'MM'/'yyyy} " +
                        $"se descontará sobre el precio el {descuentoGeneralSobrePrecio.Porcentaje}% del precio. ";
                }
                if (descuentoGeneralSobrePrecio?.Porcentaje < 0)
                {
                    res.Texto += $"De acuerdo a las siguientes fechas de entrega y recibos, desde el {clausula.Basico.FechaDesde.GetValueOrDefault():dd'/'MM'/'yyyy} al {clausula.Basico.FechaHasta.GetValueOrDefault():dd'/'MM'/'yyyy} " +
                        $"se bonificará sobre el precio el {descuentoGeneralSobrePrecio.Porcentaje}% por tonelada. ";
                }
            }
            return res;
        }
    }
}
