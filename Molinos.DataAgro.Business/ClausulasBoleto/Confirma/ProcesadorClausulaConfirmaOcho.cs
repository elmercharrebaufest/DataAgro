using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.ClausulasBoleto.Confirma;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
namespace Molinos.DataAgro.Business.ClausulasBoleto.Confirma
{
    public class ProcesadorClausulaConfirmaOcho : ProcesadorClausulaConfirma<ClausulaConfirmaOcho>
    {
        public ProcesadorClausulaConfirmaOcho(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
            : base(repositorio, log, estadoBoleto) { }

        public override ResultadoClausula DevolverClausulas(ClausulaConfirmaOcho clausula)
        {
            var res = new ResultadoClausula();
            if (clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR || clausula.Basico.DolarizadoExpress == true || clausula.Basico.Dolarizado == true)
            {
                res.Texto += "Las Partes acuerdan la posibilidad de prorrogar el plazo de pago indicado en las cláusulas previas. El precio a pagar será neto de los impuestos y retenciones impositivas que correspondieran y se hubieran ";
                res.Texto += "practicado según la condición del Vendedor y las particularidades del negocio. Toda vez que las Partes han acordado la posibilidad de prorrogar la fecha de pago de la Mercadería, queda expresamente ";
                res.Texto += "establecido que el Vendedor no podrá invocar mora ni reclamar intereses y/o multas y/o cualquier tipo de penalidad por el tiempo transcurrido entre el plazo de pago originario y el del ejercicio de la opción de ";
                res.Texto += "prórroga acordada en la presente Cláusula.";
            }
            return res;
        }
    }
}
