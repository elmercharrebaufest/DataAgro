using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using System;
using Humanizer;
using System.Globalization;
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
            var res = new ResultadoClausula();

            if (clausula.Basico.TipoNegocioId == 2)
            {
               var descuentoGeneralSobrePrecio = clausula.Basico.Descuentos.AsQueryable().Where(x => x.TipoPeriodoDBId == 1 && x.TipoDBId == 1).FirstOrDefault();
                if (descuentoGeneralSobrePrecio.Porcentaje > 0)
                {
                    res.Texto += $"De acuerdo a las siguientes fechas de entrega y recibos, desde el  { clausula.Basico.FechaDesde } al { clausula.Basico.FechaHasta } " +
                        $"descontará sobre el precio el { descuentoGeneralSobrePrecio.Porcentaje }% del precio. ";
                }
                if (descuentoGeneralSobrePrecio.Porcentaje < 0)
                {
                    res.Texto += $"De acuerdo a las siguientes fechas de fijación, desde el { clausula.Basico.DesdeFijacion } al { clausula.Basico.HastaFijacion } " +
                        $"se bonificará sobre el precio el { descuentoGeneralSobrePrecio.Porcentaje }% por tonelada. ";
                }
            }

            return res;
        }

        public string DevolverNumeroEnLetras(decimal numero)
        {
            var letras = ((int)Math.Abs(numero)).ToWords(CultureInfo.GetCultureInfo("es-AR")).ToUpper();             
            var fraccion = numero - Math.Floor(numero); 
            if (fraccion > 0)
            {
                letras += $" con {((int)(fraccion * 100)).ToWords(CultureInfo.GetCultureInfo("es-AR")).ToUpper() }";

            }
            return letras;
        }
    }
}
