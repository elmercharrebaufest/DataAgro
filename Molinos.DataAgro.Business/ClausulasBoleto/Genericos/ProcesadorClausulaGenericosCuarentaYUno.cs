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
    public class ProcesadorClausulaGenericosCuarentaYUno : ProcesadorClausulaGenericos<ClausulaGenericosCuarentaYUno>
    {
        public ProcesadorClausulaGenericosCuarentaYUno(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
            : base(repositorio, log, estadoBoleto) { }

        public override ResultadoClausula DevolverClausulas(ClausulaGenericosCuarentaYUno clausula)
        {
            var res = new ResultadoClausula();
            // Nueva clausula Girasol
            if (clausula.Basico.MaterialId == (int)EnumMateriales.GIRASOL && clausula.Basico.DestinoCodigoSap.Trim().Equals("1029"))
            {
                res.Texto += "La mercadería objeto del presente contrato deberá entregarse con un contenido de humedad máximo del once por ciento (11%). En caso de que la humedad supere dicho porcentaje, el Comprador se reserva el derecho de aplicar las bonificaciones y/o rebajas correspondientes conforme a las tablas vigentes, o rechazar la carga, a su exclusivo criterio.";
            }
            return res;
        }
    }
}
