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
    public class ProcesadorClausulaOchenta : ProcesadorClausula<ClausulaOchenta>
    {
        public ProcesadorClausulaOchenta(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaOchenta clausula)
        {
            var res = new ResultadoClausula();
            if (clausula.Basico.BoletoId == (int)EnumBoletoCompraNet.CONFIRMA && clausula.Basico.CorredorId > 0)
            {
                res.Texto += $"Todos los firmantes acuerdan que todas las divergencias, cuestiones o reclamaciones que surjan de o que se relacionen con cualquiera de las relaciones jurídicas que se derivan de este contrato y entre cualesquiera de ellos, serán resueltas en forma definitiva por la Cámara Arbitral de Cereales de la Bolsa de Comercio de Rosario. El tribunal actuará como amigable componedor, con aplicación de las Reglas y Usos del Comercio de Granos y del Reglamento de Procedimientos aprobado por Decreto 931/98 y/o sus futuras modificaciones, ampliaciones o normas complementarias.";
            }
            return res;
        }
    }
}
