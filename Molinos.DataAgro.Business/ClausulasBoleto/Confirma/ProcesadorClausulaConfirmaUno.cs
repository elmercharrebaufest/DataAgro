using NLog;
using Molinos.DataAgro.Entities.ClausulasBoleto.Confirma;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
namespace Molinos.DataAgro.Business.ClausulasBoleto.Confirma
{
    public class ProcesadorClausulaConfirmaUno : ProcesadorClausulaConfirma<ClausulaConfirmaUno>
    {
        public ProcesadorClausulaConfirmaUno(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto) : base(repositorio, log, estadoBoleto)
        {
        }
        public override ResultadoClausula DevolverClausulas(ClausulaConfirmaUno clausula)
        {
            var res = new ResultadoClausula();

            if (clausula.Basico.DestinoCodigoSap.Equals("1068"))
            {
                res.Texto = $"Destino de la mercadería: ROSARIO NORTE/SUR OPCIÓN COMPRADOR, {clausula.Basico.DestinoProvincia}. Origen: {clausula.Basico.Localidad}, {clausula.Basico.Provincia}.";
            }
            else
            {
                res.Texto = $"Destino de la mercadería: {clausula.Basico.DestinoLocalidad}, {clausula.Basico.DestinoProvincia}. Origen: {clausula.Basico.Localidad}, {clausula.Basico.Provincia}.";
            }

            //res.Texto = $"Destino de la mercadería: {clausula.Basico.DestinoDescripcion}. Origen: {clausula.Basico.Localidad}, {clausula.Basico.Provincia}.";
            return res;
        }
    }
}
