using NLog;
using Molinos.DataAgro.Entities.ClausulasBoleto.Confirma;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;

namespace Molinos.DataAgro.Business.ClausulasBoleto.Confirma
{
    public class ProcesadorClausulaConfirmaSiete : ProcesadorClausulaConfirma<ClausulaConfirmaSiete>
    {
        public ProcesadorClausulaConfirmaSiete(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
            : base(repositorio, log, estadoBoleto) { }

        public override ResultadoClausula DevolverClausulas(ClausulaConfirmaSiete clausula)
        {
            var res = new ResultadoClausula();
            if (
                (clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR &&
                 //clausula.Basico.Moneda == "USD" &&
                 clausula.Basico.CorredorId > 0 &&
                    (clausula.Basico.ClasificacionDescripcion?.ToString().ToUpper() == "ACOPIADOR" ||
                     clausula.Basico.ClasificacionDescripcion?.ToString().ToUpper() == "PRODUCTOR" ||
                     clausula.Basico.ClasificacionDescripcion?.ToString().ToUpper() == "OTROS")
                    ) ||
                (clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR &&
                 //clausula.Basico.Moneda == "USD" &&
                    (clausula.Basico.ClasificacionDescripcion?.ToString().ToUpper() == "ACOPIADOR" ||
                    clausula.Basico.ClasificacionDescripcion?.ToString().ToUpper() == "OTROS")
                )
               )
            {
                res.Texto += "Toda vez que el Acopiador/Corredor no proceda a liquidar la mercadería dentro de las 72 horas desde que la misma fuera entregada y aplicada, " + 
                             "las Partes acuerdan que quedará a opción del Comprador determinar el día que se tomará válido para establecer el tipo de cambio a utilizar en los términos dispuestos en el presente boleto.";
            }
            return res;
        }
    }
}
