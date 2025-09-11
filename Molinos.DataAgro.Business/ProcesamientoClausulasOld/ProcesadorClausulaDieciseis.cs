using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using System.Linq;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Entities.Common.Enums;

namespace Molinos.DataAgro.Business.Procesamiento
{
    public class ProcesadorClausulaDieciseis : ProcesadorClausula<ClausulaDieciseis>
    {
        public ProcesadorClausulaDieciseis(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaDieciseis clausula)
        {
            //SI EL NEGOCIO ES DE TIPO A PRECIO Y TIENE INDICADO DESCUENTOS Y/O BONIFICACIONES POR PRECIO PACTADO

            var res = new ResultadoClausula();
			/*
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
			*/
            return res;
        }
    }
}
