using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Entities.Common.Enums;

namespace Molinos.DataAgro.Business.Procesamiento
{
    public class ProcesadorClausulaTreintaYCinco : ProcesadorClausula<ClausulaTreintaYCinco>
    {
        public ProcesadorClausulaTreintaYCinco(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaTreintaYCinco clausula)
        {
            //SI EL CONTRATO CORRESPONDE A CANJE y es a fijar

            var res = new ResultadoClausula();
            /*
			if (clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR && clausula.Basico.Canje == true)
            {
                res.Texto += $"Se deja expresa constancia que entre el Comprador y el personal dependiente y/o contratado y/o subcontratado y/o eventual del " +
                    $"Vendedor (en adelante, el {"Personal del Vendedor"}) no existe relación de dependencia y/o contratación de ninguna naturaleza. " +
                    "Las obligaciones laborales, previsionales, y de la seguridad social, estarán a cargo exclusivamente del Vendedor y serán de su exclusiva " +
                    "responsabilidad. El Comprador tendrá amplias facultades de contralor respecto del cumplimiento de esta obligación por parte del Vendedor. " +
                    "En virtud de ello, el Vendedor asume la absoluta y exclusiva responsabilidad por los accidentes de trabajo y enfermedades profesionales que " +
                    "por el hecho o en ocasión de los servicios que sean prestados, sufra el Personal del Vendedor, incluyendo el accidente in-itinere. Asimismo, " +
                    "el Vendedor acuerda indemnizar y mantener indemne y excluir de toda responsabilidad al Comprador (incluyendo gastos, costas y honorarios " +
                    "razonables de abogados) respecto de cualquier reclamo judicial y/o extrajudicial proveniente de (i) el Personal del Vendedor, cualquiera sea " +
                    "la causa de dicho reclamo; y (ii) terceros, por daños causados a sus bienes y/o personas, por el Personal del Vendedor y/o por el Vendedor, " +
                    "aún involuntariamente.";
            }
			*/

            return res;
        }
    }
}
