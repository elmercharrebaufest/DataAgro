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
    public class ProcesadorClausulaGenericosTres : ProcesadorClausulaGenericos<ClausulaGenericosTres>
    {
        public ProcesadorClausulaGenericosTres(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
            : base(repositorio, log, estadoBoleto) { }


        public override ResultadoClausula DevolverClausulas(ClausulaGenericosTres clausula)
        {
            //SI ESTÁ SELECCIONADA BONIFICACIONES SOBRE PRECIO Y/O POR FUERA DE PRECIO

            var res = new ResultadoClausula();
            var descuentoGeneralSobrePrecio = clausula.Basico.Descuentos.Find(x => x.TipoPeriodoDBId == 1 && x.TipoDBId == 1);
            var descuentoGeneralFueraPrecio = clausula.Basico.Descuentos.Find(x => x.TipoPeriodoDBId == 1 && x.TipoDBId == 2);
            res.Texto += DevolverClausulaBonificacionSobrePrecio(descuentoGeneralSobrePrecio);
            res.Texto += DevolverClausulaBonificacionFueraPrecio(descuentoGeneralFueraPrecio);

            return res;
        }

        private string DevolverClausulaBonificacionSobrePrecio(DescuentoBonificacionDto descuentoGeneralSobrePrecio)
        {
            string clausula = string.Empty;

            // En la bonificacion sobre el precio siempre debe mostrarse lo que se tiene como apertura de precio
            decimal? importeSobrePrecio = descuentoGeneralSobrePrecio?.Importe;
            decimal? porcentajeSobrePrecio = descuentoGeneralSobrePrecio?.Porcentaje;
            string monedaSobrePrecio = descuentoGeneralSobrePrecio?.Moneda;

            if (porcentajeSobrePrecio > 0)
            {
                clausula += $"Se bonificará sobre el precio el {porcentajeSobrePrecio}% por tonelada. ";
            }
            if (porcentajeSobrePrecio < 0)
            {
                clausula += $"Se descontará sobre el precio el {porcentajeSobrePrecio}% por tonelada. ";
            }
            if (importeSobrePrecio > 0)
            {
                clausula += $"Se bonificará sobre el precio {MetodosUtiles.DivisaSimbolica(monedaSobrePrecio)} {MetodosUtiles.ValorAbsoluto((decimal)importeSobrePrecio)} " +
                    $" ({MetodosUtiles.DevolverNumeroEnLetrasConDivisa((decimal)importeSobrePrecio, monedaSobrePrecio)}) por tonelada. ";
            }
            if (importeSobrePrecio < 0)
            {
                clausula += $"Se descontará sobre el precio {MetodosUtiles.DivisaSimbolica(monedaSobrePrecio)} {MetodosUtiles.ValorAbsoluto((decimal)importeSobrePrecio)} " +
                    $" ({MetodosUtiles.DevolverNumeroEnLetrasConDivisa((decimal)importeSobrePrecio, monedaSobrePrecio)}) por tonelada. ";
            }
            return clausula;
        }

        private string DevolverClausulaBonificacionFueraPrecio(DescuentoBonificacionDto descuentoGeneralFueraPrecio)
        {
            string clausula = string.Empty;
            if (descuentoGeneralFueraPrecio?.Porcentaje > 0)
            {
                clausula += $"Se bonificará por fuera del precio el {descuentoGeneralFueraPrecio.Porcentaje}% por tonelada. ";
            }
            if (descuentoGeneralFueraPrecio?.Porcentaje < 0)
            {
                clausula += $"Se descontará por fuera del precio el {descuentoGeneralFueraPrecio.Porcentaje}% por tonelada. ";
            }
            if (descuentoGeneralFueraPrecio?.Importe > 0)
            {
                clausula += $"Se bonificará por fuera del precio {MetodosUtiles.DivisaSimbolica(descuentoGeneralFueraPrecio.Moneda)} {MetodosUtiles.ValorAbsoluto(descuentoGeneralFueraPrecio.Importe)}" +
                    $" ({MetodosUtiles.DevolverNumeroEnLetrasConDivisa(descuentoGeneralFueraPrecio.Importe, descuentoGeneralFueraPrecio.Moneda)}) por tonelada. ";
            }
            if (descuentoGeneralFueraPrecio?.Importe < 0)
            {
                clausula += $"Se descontará por fuera del precio {MetodosUtiles.DivisaSimbolica(descuentoGeneralFueraPrecio.Moneda)} {MetodosUtiles.ValorAbsoluto(descuentoGeneralFueraPrecio.Importe)}" +
                    $" ({MetodosUtiles.DevolverNumeroEnLetrasConDivisa(descuentoGeneralFueraPrecio.Importe, descuentoGeneralFueraPrecio.Moneda)}) por tonelada. ";
            }
            return clausula;
        }
    }
}
