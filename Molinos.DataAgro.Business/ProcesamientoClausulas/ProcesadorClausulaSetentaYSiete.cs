using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac.Extras.NLog;

namespace Molinos.DataAgro.Business.ProcesamientoClausulas
{
    public class ProcesadorClausulaSetentaYSiete : ProcesadorClausula<ClausulaSetentaYSiete>
    {
        public ProcesadorClausulaSetentaYSiete(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaSetentaYSiete clausula)
        {
            var res = new ResultadoClausula();
            if (clausula.Basico.BoletoId == (int)EnumBoletoCompraNet.CONFIRMA && clausula.Basico.CorredorId > 0)
            {
                res.Texto += $"Los Señores ${clausula.Basico.Corredor} quedan facultados por los vendedores con carácter de mandato irrevocable, para firmar en su nombre los eventuales convenios de prórrogas, coberturas, ampliaciones, anulaciones y/o modificaciones o cualquier otra documentación relacionada con el presente contrato.";
            }
            return res;
        }
    }
}
