using Autofac.Extras.NLog;
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

namespace Molinos.DataAgro.Business.ProcesamientoClausulas
{
    public class ProcesadorClausulaOchentaYSeis : ProcesadorClausula<ClausulaOchentaYSeis>
    {
        public ProcesadorClausulaOchentaYSeis(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaOchentaYSeis clausula)
        {
            var res = new ResultadoClausula();
            if (clausula.Basico.BoletoId == (int)EnumBoletoCompraNet.CONFIRMA && clausula.Basico.CorredorId > 0)
            {
                res.Texto += $"A la fecha de realización de cada una de las liquidaciones correspondientes al presente contrato, MOA procederá a verificar si el vendedor se encuentra incluido en la base APOC o en cualquier otra base de datos creada por AFIP o por cualquier otro organismo del Estado (nacional/provincial/municipal) de la cual pudiera surgir que el proveedor reviste la calidad de apócrifo. Las partes acuerdan que en caso de verificarse la calificación del vendedor como apócrifo en alguna de dichas bases, el Contrato quedara rescindido de pleno derecho, limitándose su objeto a las prestaciones cumplidas con anterioridad a la verificación llevada a cabo por MOA.";
            }
            return res;
        }
    }
}
