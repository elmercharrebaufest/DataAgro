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
    public class ProcesadorClausulaGenericosCuarenta : ProcesadorClausulaGenericos<ClausulaGenericosCuarenta>
    {
        public ProcesadorClausulaGenericosCuarenta(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
            : base(repositorio, log, estadoBoleto) { }

        public override ResultadoClausula DevolverClausulas(ClausulaGenericosCuarenta clausula)
        {
            var res = new ResultadoClausula();
            // Se retira clausula por solicitud del Negocio
            //if (clausula.Basico.CorredorId > 0 && clausula.Basico.CD == true)
            //{
            //    res.Texto += "En caso de incumplimiento, podrá ejecutarse la obligación mediante la entrega de la mercadería objeto del CD o, en su defecto, el importe necesario para poder adquirir la misma cantidad de mercadería objeto del boleto a la fecha de vencimiento del plazo de entrega.";
            //}
            return res;
        }
    }

}
