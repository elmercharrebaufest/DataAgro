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
    public class ProcesadorClausulaGenericosTreintaYTres : ProcesadorClausulaGenericos<ClausulaGenericosTreintaYTres>
    {
        public ProcesadorClausulaGenericosTreintaYTres(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
            : base(repositorio, log, estadoBoleto) { }

        public override ResultadoClausula DevolverClausulas(ClausulaGenericosTreintaYTres clausula)
        {
            var res = new ResultadoClausula();
            if ( (clausula.Basico.MaterialId == (int)EnumMateriales.TRIGO && clausula.Basico.StandardDeCalidadId == (int)EnumStandarCalidad.GRADO_2) || 
                 (clausula.Basico.MaterialId == (int)EnumMateriales.MAIZ && clausula.Basico.StandardDeCalidadId == (int)EnumStandarCalidad.ESPECIAL)
               )
            {
                res.Texto += $"Condición de la mercadería Grado 2: No bonifica Grado 1, no bonifica ni rebaja Grado 2, rebaja Grado 3 y demás condiciones cámara.";
            }
            return res;
        }
    }
}
