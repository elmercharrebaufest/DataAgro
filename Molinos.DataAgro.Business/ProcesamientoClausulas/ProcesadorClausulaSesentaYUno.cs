using Autofac.Extras.NLog;
using Humanizer;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Business.ProcesamientoClausulas
{
    public class ProcesadorClausulaSesentaYUno : ProcesadorClausula<ClausulaSesentaYUno>
    {
        public ProcesadorClausulaSesentaYUno(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto) : base(repositorio, log, estadoBoleto)
        {
        }
        public override ResultadoClausula DevolverClausulas(ClausulaSesentaYUno clausula)
        {
            var res = new ResultadoClausula();
            {
                if (clausula.Basico.BoletoId == (int)EnumBoletoCompraNet.CONFIRMA)
                {
                    if (clausula.Basico.PreciosPactados != null && clausula.Basico.PreciosPactados.Count > 0)
                    {
                        res.Texto += "El PRECIO del contrato se modificará de acuerdo a las siguientes fechas de entrega y recibos, desde el ";
                        foreach (var precio in clausula.Basico.PreciosPactados)
                        {
                            res.Texto += $"{precio.FechaDesde} al {precio.FechaHasta}" +
                                $" {precio.MonedaPactadoDesc} {precio.Precio.ToString("N2", CultureInfo.CreateSpecificCulture("es-AR"))} ({DevolverNumeroEnLetras(precio.Precio)}),";
                        }
                        res.Texto = res.Texto.EndsWith(",") ? string.Format("{0}.", res.Texto.Remove(res.Texto.Length - 1)) : res.Texto;
                    }
                }
            }
            return res;
        }
        private string DevolverNumeroEnLetras(decimal numero)
        {
            numero = Math.Round(numero, 2);
            var letras = ((int)Math.Abs(numero)).ToWords(CultureInfo.GetCultureInfo("es-AR")).ToUpper();
            var fraccion = numero - Math.Floor(numero);
            if (fraccion > 0)
            {
                letras += $" con {((int)(fraccion * 100)).ToWords(CultureInfo.GetCultureInfo("es-AR")).ToUpper()}";
            }

            return letras;
        }
    }
}
