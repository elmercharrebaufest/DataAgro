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
    public class ProcesadorClausulaGenericosTreintaYDos : ProcesadorClausulaGenericos<ClausulaGenericosTreintaYDos>
    {
        public ProcesadorClausulaGenericosTreintaYDos(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
            : base(repositorio, log, estadoBoleto) { }

        public override ResultadoClausula DevolverClausulas(ClausulaGenericosTreintaYDos clausula)
        {
            var res = new ResultadoClausula();
            if (clausula.Basico.FechaCierta.HasValue && clausula.Basico.PorcentajeDePago.HasValue)
            {
                res.Texto += $" El pago del {clausula.Basico.PorcentajeDePago.Value}% se efectuará en la fecha indicada en ‘Información sobre pagos’ con la condición que con 72 hs de anticipación se hayan cumplido los requisitos exigibles para el pago. " +
                    $"En caso contrario, el pago se realizará a las 72 hs de cumplidos los requisitos previamente mencionados. El {100 - clausula.Basico.PorcentajeDePago.Value}% restante se pagará durante los 30 días posteriores.";
            }
            return res;
        }
    }
}
