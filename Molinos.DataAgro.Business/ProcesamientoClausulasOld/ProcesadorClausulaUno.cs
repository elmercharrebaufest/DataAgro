using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using System;
using Humanizer;
using System.Globalization;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Entities.Resources;
using Molinos.DataAgro.Entities.Common.Enums;

namespace Molinos.DataAgro.Business.Procesamiento
{
    public class ProcesadorClausulaUno : ProcesadorClausula<ClausulaUno>
    {
        public ProcesadorClausulaUno(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto) : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaUno clausula)
        {
            var res = new ResultadoClausula();
            return res;

            //var res = new ResultadoClausula();
            //if (clausula.Basico.BoletoId == (int)EnumBoletoCompraNet.CONFIRMA)
            //{
            //    res.Texto = $"Destino de la mercadería: {clausula.Basico.DestinoLocalidad}, {clausula.Basico.DestinoProvincia}. Origen: {clausula.Basico.Localidad}, {clausula.Basico.Provincia}.";
            //}
            //return res;
        }
    }
}
