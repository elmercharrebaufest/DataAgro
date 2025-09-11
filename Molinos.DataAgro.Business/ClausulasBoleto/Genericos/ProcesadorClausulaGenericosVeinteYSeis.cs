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
    public class ProcesadorClausulaGenericosVeinteYSeis : ProcesadorClausulaGenericos<ClausulaGenericosVeinteYSeis>
    {
        public ProcesadorClausulaGenericosVeinteYSeis(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
            : base(repositorio, log, estadoBoleto) { }

        public override ResultadoClausula DevolverClausulas(ClausulaGenericosVeinteYSeis clausula)
        {
            var res = new ResultadoClausula();
            if (clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR && clausula.Basico.Canje == true && clausula.Basico.BoletoId != (int)EnumBoletoCompraNet.CONFIRMA)
            {
                res.Texto += $"Queda establecido que toda tasa contribución, impuesto provincial y/o municipal que grave la presente operación será a cargo del " +
                    $"Vendedor. Sin perjuicio de ello, se deja expresa constancia que la presente operación de compraventa de mercaderías con pago en especie se " +
                    $"encuentra encuadrada en los términos del Art. 5 de la Ley de IVA Inc. A.";
            }
            return res;
        }
    }
}
