using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Interfaces;

namespace Molinos.DataAgro.Business.Procesamiento
{
    public class ProcesadorClausulaSeis : ProcesadorClausula<ClausulaSeis>
    {
        public ProcesadorClausulaSeis(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaSeis clausula)
        {
            //CLAUSULA SIEMPRE PRESENTE

            var res = new ResultadoClausula();

            res.Texto += $"El importe del sellado del presente Contrato como los honorarios de la Cámara Arbitral de la Bolsa de Cereales de {clausula.Basico.BolsaDescripcion} " +
                 $"será abonado por: ";
            if (clausula.Basico.SelCargoVendedor == true) //SI TIENE TILDE EN SELLADO 100% A CARGO DEL VENDEDOR
            {
                res.Texto += $"el vendedor."; 
            }
            else if (clausula.Basico.SelCargoMOA == true) //SI TIENE TILDE EN SELLADO 100% A CARGO DE MOA
            {
                res.Texto += $"el comprador.";
            }
            else
            {
                res.Texto += $"las partes en partes iguales.";
            }

            return res;
        }
    }
}
