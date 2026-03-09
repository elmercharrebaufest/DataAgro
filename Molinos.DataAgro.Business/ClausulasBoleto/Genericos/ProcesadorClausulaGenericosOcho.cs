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
    public class ProcesadorClausulaGenericosOcho : ProcesadorClausulaGenericos<ClausulaGenericosOcho>
    {
        public ProcesadorClausulaGenericosOcho(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
            : base(repositorio, log, estadoBoleto) { }

        public override ResultadoClausula DevolverClausulas(ClausulaGenericosOcho clausula)
        {
            var res = new ResultadoClausula();
            //if (clausula.Basico.EPA || clausula.Basico.Sustentable)
            //{
            //    if (clausula.Basico.SustentableTipoDBId == 1)
            //    {
            //        res.Texto += DevolverClausulaBonificacionSobrePrecio(clausula.Basico);
            //    }
            //    if (clausula.Basico.SustentableTipoDBId == 2)
            //    {
            //        res.Texto += DevolverClausulaBonificacionFueraPrecio(clausula.Basico);
            //    }
            //}
            return res;
        }

        //private string DevolverClausulaBonificacionSobrePrecio(BasicoContrato descuentoGeneralSobrePrecio)
        //{
        //    string clausula = string.Empty;
        //    if (descuentoGeneralSobrePrecio?.Importe_Sustentable > 0)
        //    {
        //        clausula += $"Se bonificará sobre el precio {MetodosUtiles.DivisaSimbolica(descuentoGeneralSobrePrecio.Moneda_Sustentable)} {MetodosUtiles.ValorAbsoluto((decimal)descuentoGeneralSobrePrecio.Importe_Sustentable)} " +
        //            $" ({MetodosUtiles.DevolverNumeroEnLetrasConDivisa((decimal)descuentoGeneralSobrePrecio.Importe_Sustentable, descuentoGeneralSobrePrecio.Moneda)}) por tonelada. ";
        //    }
        //    if (descuentoGeneralSobrePrecio?.Importe_Sustentable < 0)
        //    {
        //        clausula += $"Se descontará sobre el precio {MetodosUtiles.DivisaSimbolica(descuentoGeneralSobrePrecio.Moneda_Sustentable)} {MetodosUtiles.ValorAbsoluto((decimal)descuentoGeneralSobrePrecio.Importe_Sustentable)} " +
        //            $" ({MetodosUtiles.DevolverNumeroEnLetrasConDivisa((decimal)descuentoGeneralSobrePrecio?.Importe_Sustentable, descuentoGeneralSobrePrecio.Moneda)}) por tonelada. ";
        //    }
        //    return clausula;
        //}

        //private string DevolverClausulaBonificacionFueraPrecio(BasicoContrato descuentoGeneralFueraPrecio)
        //{
        //    string clausula = string.Empty;
        //    if (descuentoGeneralFueraPrecio?.Importe_Sustentable > 0)
        //    {
        //        clausula += $"Se bonificará por fuera del precio {MetodosUtiles.DivisaSimbolica(descuentoGeneralFueraPrecio.Moneda_Sustentable)} {MetodosUtiles.ValorAbsoluto((decimal)descuentoGeneralFueraPrecio.Importe_Sustentable)}" +
        //            $" ({MetodosUtiles.DevolverNumeroEnLetrasConDivisa((decimal)descuentoGeneralFueraPrecio?.Importe_Sustentable, descuentoGeneralFueraPrecio.Moneda)}) por tonelada. ";
        //    }
        //    if (descuentoGeneralFueraPrecio?.Importe_Sustentable < 0)
        //    {
        //        clausula += $"Se descontará por fuera del precio {MetodosUtiles.DivisaSimbolica(descuentoGeneralFueraPrecio.Moneda_Sustentable)} {MetodosUtiles.ValorAbsoluto((decimal)descuentoGeneralFueraPrecio.Importe_Sustentable)}" +
        //            $" ({MetodosUtiles.DevolverNumeroEnLetrasConDivisa((decimal)descuentoGeneralFueraPrecio?.Importe_Sustentable, descuentoGeneralFueraPrecio.Moneda)}) por tonelada. ";
        //    }
        //    return clausula;
        //}


    }
}
