using Autofac.Extras.NLog;
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

            // Se modifica por peticion de Santiago para que cuando sea posicion CBOT no se muestre estas clausulas
            bool esPosicionCBOT = clausula.Basico.TipoPosicionCBOTId != null ? (clausula.Basico.TipoPosicionCBOTId == 1 ? true : false) : false;

            if (clausula.Basico.TipoPosicionCBOTId != null)
            {
                if (!esPosicionCBOT)
                {
                    if (clausula.Basico.EPA || clausula.Basico.Sustentable)
                    {
                        if (clausula.Basico.SustentableTipoDBId == 1)
                        {
                            res.Texto += DevolverClausulaBonificacionSobrePrecioAdicionales(clausula.Basico);
                        }
                        if (clausula.Basico.SustentableTipoDBId == 2)
                        {
                            res.Texto += DevolverClausulaBonificacionFueraPrecioAdicionales(clausula.Basico);
                        }
                    }
                }
                else
                {
                    var descuentoGeneralSobrePrecio = clausula.Basico.Descuentos.Find(x => x.TipoPeriodoDBId == 1 && x.TipoDBId == 1);
                    var descuentoGeneralFueraPrecio = clausula.Basico.Descuentos.Find(x => x.TipoPeriodoDBId == 1 && x.TipoDBId == 2);
                    var descuentoPorAperturaDePrecio = (clausula.Basico.AperturaPrecios != null && clausula.Basico.AperturaPrecios.Count > 0) ? clausula.Basico.AperturaPrecios.Where(x => x.ConceptoAperturaPrecioId == 4 && (x.Importe != 0 || x.Porcentaje != 0)).FirstOrDefault() : null;

                    res.Texto += DevolverClausulaBonificacionSobrePrecio(descuentoGeneralSobrePrecio, descuentoPorAperturaDePrecio);
                    res.Texto += DevolverClausulaBonificacionFueraPrecio(descuentoGeneralFueraPrecio);
                }
            }
            else
            {
                var descuentoGeneralSobrePrecio = clausula.Basico.Descuentos.Find(x => x.TipoPeriodoDBId == 1 && x.TipoDBId == 1);
                var descuentoGeneralFueraPrecio = clausula.Basico.Descuentos.Find(x => x.TipoPeriodoDBId == 1 && x.TipoDBId == 2);
                var descuentoPorAperturaDePrecio = (clausula.Basico.AperturaPrecios != null && clausula.Basico.AperturaPrecios.Count > 0) ? clausula.Basico.AperturaPrecios.Where(x => x.ConceptoAperturaPrecioId == 4 && (x.Importe != 0 || x.Porcentaje != 0)).FirstOrDefault() : null;

                res.Texto += DevolverClausulaBonificacionSobrePrecio(descuentoGeneralSobrePrecio, descuentoPorAperturaDePrecio);
                res.Texto += DevolverClausulaBonificacionFueraPrecio(descuentoGeneralFueraPrecio);
            }
            return res;
        }

        private string DevolverClausulaBonificacionSobrePrecio(DescuentoBonificacionDto descuentoGeneralSobrePrecio, AperturaPrecioDto descuentoPorAperturaDePrecio)
        {
            string clausula = string.Empty;

            // En la bonificacion sobre el precio siempre debe mostrarse lo que se tiene como apertura de precio
            decimal? importeSobrePrecio = descuentoPorAperturaDePrecio != null ? descuentoPorAperturaDePrecio.Importe : descuentoGeneralSobrePrecio?.Importe;
            decimal? porcentajeSobrePrecio = descuentoPorAperturaDePrecio != null ? descuentoPorAperturaDePrecio.Porcentaje : descuentoGeneralSobrePrecio?.Porcentaje;
            string monedaSobrePrecio = descuentoPorAperturaDePrecio != null ? descuentoPorAperturaDePrecio.Moneda : descuentoGeneralSobrePrecio?.Moneda;

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

        private string DevolverClausulaBonificacionSobrePrecioAdicionales(BasicoContrato descuentoGeneralSobrePrecio)
        {
            string clausula = string.Empty;
            if (descuentoGeneralSobrePrecio?.Importe_Sustentable > 0)
            {
                clausula += $"Se bonificará sobre el precio {MetodosUtiles.DivisaSimbolica(descuentoGeneralSobrePrecio.Moneda_Sustentable)} {MetodosUtiles.ValorAbsoluto((decimal)descuentoGeneralSobrePrecio.Importe_Sustentable)} " +
                    $" ({MetodosUtiles.DevolverNumeroEnLetrasConDivisa((decimal)descuentoGeneralSobrePrecio.Importe_Sustentable, descuentoGeneralSobrePrecio.Moneda)}) por tonelada. ";
            }
            if (descuentoGeneralSobrePrecio?.Importe_Sustentable < 0)
            {
                clausula += $"Se descontará sobre el precio {MetodosUtiles.DivisaSimbolica(descuentoGeneralSobrePrecio.Moneda_Sustentable)} {MetodosUtiles.ValorAbsoluto((decimal)descuentoGeneralSobrePrecio.Importe_Sustentable)} " +
                    $" ({MetodosUtiles.DevolverNumeroEnLetrasConDivisa((decimal)descuentoGeneralSobrePrecio?.Importe_Sustentable, descuentoGeneralSobrePrecio.Moneda)}) por tonelada. ";
            }
            return clausula;
        }

        private string DevolverClausulaBonificacionFueraPrecioAdicionales(BasicoContrato descuentoGeneralFueraPrecio)
        {
            string clausula = string.Empty;
            if (descuentoGeneralFueraPrecio?.Importe_Sustentable > 0)
            {
                clausula += $"Se bonificará por fuera del precio {MetodosUtiles.DivisaSimbolica(descuentoGeneralFueraPrecio.Moneda_Sustentable)} {MetodosUtiles.ValorAbsoluto((decimal)descuentoGeneralFueraPrecio.Importe_Sustentable)}" +
                    $" ({MetodosUtiles.DevolverNumeroEnLetrasConDivisa((decimal)descuentoGeneralFueraPrecio?.Importe_Sustentable, descuentoGeneralFueraPrecio.Moneda)}) por tonelada. ";
            }
            if (descuentoGeneralFueraPrecio?.Importe_Sustentable < 0)
            {
                clausula += $"Se descontará por fuera del precio {MetodosUtiles.DivisaSimbolica(descuentoGeneralFueraPrecio.Moneda_Sustentable)} {MetodosUtiles.ValorAbsoluto((decimal)descuentoGeneralFueraPrecio.Importe_Sustentable)}" +
                    $" ({MetodosUtiles.DevolverNumeroEnLetrasConDivisa((decimal)descuentoGeneralFueraPrecio?.Importe_Sustentable, descuentoGeneralFueraPrecio.Moneda)}) por tonelada. ";
            }
            return clausula;
        }


    }
}
