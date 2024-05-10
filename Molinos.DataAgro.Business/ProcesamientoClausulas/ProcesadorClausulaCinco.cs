using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Interfaces;

namespace Molinos.DataAgro.Business.Procesamiento
{
    public class ProcesadorClausulaCinco : ProcesadorClausula<ClausulaCinco>
    {
        public ProcesadorClausulaCinco(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaCinco clausula)
        {
            //CLAUSULA SIEMPRE PRESENTE

            var res = new ResultadoClausula();

            res.Texto += $"A la fecha de realización de cada una de las liquidaciones correspondientes al presente contrato, MOA procederá a verificar si el vendedor " +
                            $"se encuentra incluido en la base APOC o en cualquier otra base de datos creada por la AFIP o por cualquier otro organismo del Estado " +
                            $"(nacional/provincial/municipal) de la cual pudiera surgir que el proveedor reviste la calidad de apócrifo. Las partes acuerdan que en " +
                            $"caso de verificarse la calificación del vendedor como apócrifo en alguna de dichas bases, el Contrato quedará rescindido de pleno derecho, " +
                            $"limitándose su objeto a las prestaciones cumplidas con anterioridad a la verificación llevada a cabo por MOA. ";

            return res;
        }
    }
}
