using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.ClausulasBoleto.CartaOferta;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;

namespace Molinos.DataAgro.Business.ClausulasBoleto.CartaOferta
{
    public class ProcesadorClausulaCartaOfertaCuatro : ProcesadorClausulaCartaOferta<ClausulaCartaOfertaCuatro>
    {
        public ProcesadorClausulaCartaOfertaCuatro(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
            : base(repositorio, log, estadoBoleto) { }

        public override ResultadoClausula DevolverClausulas(ClausulaCartaOfertaCuatro clausula)
        {
            var res = new ResultadoClausula();
            res.Texto += $"Todos los firmantes acuerdan que todas las divergencias, cuestiones o reclamaciones que surjan de o que se relacionen con cualquiera " +
                         $"de las relaciones jurídicas que se derivan de este contrato y entre cualesquiera de ellos, serán resueltas en forma definitiva por " +
                         $"la Cámara Arbitral de la Bolsa de Cereales de {clausula.Basico.BolsaDescripcion}. El tribunal actuará como amigable componedor, con aplicación de " +
                         $"las Reglas y Usos del Comercio de Granos y del Reglamento de Procedimientos aprobado por Decreto 931/98 y/o sus futuras modificaciones, ampliaciones o normas complementarias.";
            return res;
        }
    }
}
