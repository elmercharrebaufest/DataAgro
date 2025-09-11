using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Interfaces;
using System.Globalization;
using Molinos.DataAgro.Entities.Common.Enums;

namespace Molinos.DataAgro.Business.Procesamiento
{
    public class ProcesadorClausulaVeinticinco : ProcesadorClausula<ClausulaVeinticinco>
    {
        public ProcesadorClausulaVeinticinco(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaVeinticinco clausula)
        {
            //SI EL CONTRATO CORRESPONDE A CANJE y es a fijar

            var res = new ResultadoClausula();
            /*
			if (clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR && clausula.Basico.Canje == true)
            {
                res.Texto += $"Detalle de los insumos a canjear {clausula.Basico.Insumo} - {clausula.Basico.MonedaCanjeDescripcion} {clausula.Basico.Monto?.ToString("N", new CultureInfo("es-AR"))}";
            }
			*/
            return res;
        }
    }
}
