using NLog;
using Molinos.DataAgro.Entities.ClausulasBoleto.CartaOferta;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Entities.Common.Enums;

namespace Molinos.DataAgro.Business.ClausulasBoleto.CartaOferta
{
    public class ProcesadorClausulaCartaOfertaDiesiocho : ProcesadorClausulaCartaOferta<ClausulaCartaOfertaDiesiocho>
    {
        public ProcesadorClausulaCartaOfertaDiesiocho(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
            : base(repositorio, log, estadoBoleto) { }

        public override ResultadoClausula DevolverClausulas(ClausulaCartaOfertaDiesiocho clausula)
        {
            var res = new ResultadoClausula();
            if (clausula.Basico.MaterialId == (int)EnumMateriales.SOJA)
            {
                res.Texto += "El VENDEDOR declara y garantiza que la soja objeto del presente contrato no contiene el evento biotecnológico HB4 y expresamente acepta que tal circunstancia podría generar graves perjuicios al COMPRADOR.";
                res.Texto += "La detección del evento HB4 facultará al COMPRADOR a rechazar la mercadería sin que ello genere derecho a reclamo alguno por parte del VENDEDOR. ";
                res.Texto += "Asimismo, la entrega de mercadería conteniendo el evento biotecnológico HB4 configurará un supuesto de incumplimiento por parte del VENDEDOR. ";
                res.Texto += "Dicho incumplimiento facultará al COMPRADOR a reclamar íntegramente los daños y perjuicios ocasionados, así como también las demás consecuencias previstas en el contrato para casos de incumplimiento.";
            }
            return res;
        }
    }

}
