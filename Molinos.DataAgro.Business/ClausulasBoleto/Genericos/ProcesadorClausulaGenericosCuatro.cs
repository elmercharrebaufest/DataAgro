using NLog;
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
    public class ProcesadorClausulaGenericosCuatro : ProcesadorClausulaGenericos<ClausulaGenericosCuatro>
    {
        public ProcesadorClausulaGenericosCuatro(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
            : base(repositorio, log, estadoBoleto) { }

        public override ResultadoClausula DevolverClausulas(ClausulaGenericosCuatro clausula)
        {
            var res = new ResultadoClausula();
            if (clausula.Basico.PlanCanje == true)
            {
                res.Texto += "El vendedor declara que la mercadería proviene de un canje o de un pago en especie por lo que de acuerdo a lo estipulado por el Art. 45º de la RG 4310 el presente contrato no se encuentra sujeto a retención de IVA, siempre y cuando se presente la oblea que acredite esta condición.";
            }
            return res;
        }
    }
}
