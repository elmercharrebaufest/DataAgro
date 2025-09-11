using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.ClausulasBoleto.Genericos;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Business.ClausulasBoleto.Genericos
{
    public class ProcesadorClausulaGenericosCuarentaYUno : ProcesadorClausulaGenericos<ClausulaGenericosCuarentaYUno>
    {
        public ProcesadorClausulaGenericosCuarentaYUno(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
            : base(repositorio, log, estadoBoleto) { }

        public override ResultadoClausula DevolverClausulas(ClausulaGenericosCuarentaYUno clausula)
        {
            var res = new ResultadoClausula();
            if (clausula.Basico.MercsDeposito == true)
            {
                res.Texto += "El total o parte de la mercadería objeto del presente contrato ya se encuentra descargada.";
            }
            return res;
        }
    }
}
