using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.ClausulasBoleto.BoletoFisico;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;

namespace Molinos.DataAgro.Business.ClausulasBoleto.BoletoFisico
{
    public class ProcesadorClausulaBoletoFisicoVeinteYUno : ProcesadorClausulaBoletoFisico<ClausulaBoletoFisicoVeinteYUno>
    {
        public ProcesadorClausulaBoletoFisicoVeinteYUno(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
            : base(repositorio, log, estadoBoleto) { }

        public override ResultadoClausula DevolverClausulas(ClausulaBoletoFisicoVeinteYUno clausula)
        {
            var res = new ResultadoClausula();
            if (clausula.Basico.CorredorId > 0)
            {
                res.Texto += "A los fines de cumplir con normas legales e impositivas, el vendedor/corredor se encuentra obligado a remitir al comprador el original del presente boleto, debidamente suscripto, a efectos de su presentación en la Bolsa de Cereales.";
            }
            return res;
        }
    }
}
