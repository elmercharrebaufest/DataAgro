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
    public class ProcesadorClausulaGenericosVeinte : ProcesadorClausulaGenericos<ClausulaGenericosVeinte>
    {
        public ProcesadorClausulaGenericosVeinte(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
            : base(repositorio, log, estadoBoleto) { }

        public override ResultadoClausula DevolverClausulas(ClausulaGenericosVeinte clausula)
        {
            var res = new ResultadoClausula();
            if (clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR && clausula.Basico.Canje == true)
            {
                res.Texto += $"En caso de que el Vendedor entregare la Mercadería en forma total y la Mercadería no cumpliera -total o parcialmente- con las " +
                    $"condiciones Fábrica, y ello implicaré un saldo a favor del Comprador, entonces el Vendedor quedará obligado a proceder en idénticas " +
                    $"condiciones a las mencionadas en la cláusula previa de este Boleto.";
            }
            return res;
        }
    }
}
