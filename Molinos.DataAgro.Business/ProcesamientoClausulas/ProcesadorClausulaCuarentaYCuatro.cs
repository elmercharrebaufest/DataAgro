using NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Entities.Common.Enums;

namespace Molinos.DataAgro.Business.Procesamiento
{
    public class ProcesadorClausulaCuarentaYCuatro : ProcesadorClausula<ClausulaCuarentaYCuatro>
    {
        public ProcesadorClausulaCuarentaYCuatro(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaCuarentaYCuatro clausula)
        {
            //SI EL CONTRATO A PRECIO ES DE ACOPIADOR O CON CORREDOR EN USDM

            var res = new ResultadoClausula();
            if (clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO && 
               (clausula.Basico.ClasificacionId == (int)EnumClasificacionCompraNet.Acopiador || clausula.Basico.ClasificacionId == (int)EnumClasificacionCompraNet.Productor || clausula.Basico.ClasificacionId == (int)EnumClasificacionCompraNet.Otros ||  clausula.Basico.CorredorId > 0) && clausula.Basico.Moneda == "USD")
            {
                res.Texto += "Toda vez que el Acopiador/Corredor no proceda a liquidar la mercadería dentro de las 72 horas desde que la misma fuera entregada y aplicada, " +
                    "las Partes acuerdan que quedará a opción del Comprador determinar el día que se tomará válido para establecer el tipo de cambio a utilizar en los términos dispuestos en el presente boleto.";
            }
            return res;
        }
    }
}