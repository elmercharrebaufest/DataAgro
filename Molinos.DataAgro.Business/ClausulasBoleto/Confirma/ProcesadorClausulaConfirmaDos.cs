using NLog;
using Molinos.DataAgro.Entities.ClausulasBoleto.Confirma;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Globalization;
using Molinos.DataAgro.Entities.Entities;
namespace Molinos.DataAgro.Business.ClausulasBoleto.Confirma
{
    public class ProcesadorClausulaConfirmaDos : ProcesadorClausulaConfirma<ClausulaConfirmaDos>
    {
        public ProcesadorClausulaConfirmaDos(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
            : base(repositorio, log, estadoBoleto) { }

        public override ResultadoClausula DevolverClausulas(ClausulaConfirmaDos clausula)
        {
            var res = new ResultadoClausula();
            if (clausula.Basico.PreciosPactados != null && clausula.Basico.PreciosPactados.Count > 0)
            {
                res.Texto += "El PRECIO del contrato se modificará de acuerdo a las siguientes fechas de entrega y recibos, desde el ";
                foreach (var precio in clausula.Basico.PreciosPactados)
                {
                    res.Texto += $"{precio.FechaDesde} al {precio.FechaHasta}" +
                        $" {precio.MonedaPactadoDesc} {precio.Precio.ToString("N2", CultureInfo.CreateSpecificCulture("es-AR"))} ({MetodosUtiles.DevolverNumeroEnLetrasConDivisa(precio.Precio, precio.MonedaPactadoDesc)}),";
                }
                res.Texto = res.Texto.EndsWith(",") ? string.Format("{0}.", res.Texto.Remove(res.Texto.Length - 1)) : res.Texto;
            }

            return res;
        }
    }
}
