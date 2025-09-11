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
    public class ProcesadorClausulaGenericosVeinteYTres : ProcesadorClausulaGenericos<ClausulaGenericosVeinteYTres>
    {
        public ProcesadorClausulaGenericosVeinteYTres(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
            : base(repositorio, log, estadoBoleto) { }

        public override ResultadoClausula DevolverClausulas(ClausulaGenericosVeinteYTres clausula)
        {
            var res = new ResultadoClausula();
            if (clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR && clausula.Basico.Canje == true)
            {
                res.Texto += $"El Vendedor declara y garantiza al Comprador que (i) a la fecha de la firma del presente Boleto, y (ii) al momento de la entrega de " +
                    $"la Mercadería, se encuentra debidamente inscripto en el Registro de Operadores de Granos. El presente Boleto estará sujeto a la emisión de la " +
                    $"Constancia de Registración de Operación de Compraventa de Granos según Res. 2596 por parte de AFIP. Es obligación del Vendedor informar al " +
                    $"Comprador, inmediatamente, cualquier modificación que sufriera su registro, incluso su caducidad y/o suspensión, en el Registro de Operadores " +
                    $"de Granos.";
            }
            return res;
        }
    }
}
