using Autofac.Extras.NLog;
using Humanizer;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Resources;
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
    public class ProcesadorClausulaCincuentaYSeis : ProcesadorClausula<ClausulaCincuentaYSeis>
    {
        public ProcesadorClausulaCincuentaYSeis(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto) : base(repositorio, log, estadoBoleto)
        {
        }
        public override ResultadoClausula DevolverClausulas(ClausulaCincuentaYSeis clausula)
        {
            var res = new ResultadoClausula();
            var bonificacionAperturaPrecio = clausula.Basico.AperturaPrecios != null ? clausula.Basico.AperturaPrecios.Find(x => x.ConceptoAperturaPrecioId == 4) : new AperturaPrecioDto();

            if (clausula.Basico.Descuentos.Count == 0 && bonificacionAperturaPrecio.Id > 0)
            {
                res.Texto += DevolverClausulaBonificacionAperturaPrecio(bonificacionAperturaPrecio);
            }

            return res;
        }

        private string DevolverClausulaBonificacionAperturaPrecio(AperturaPrecioDto bonificacionAperturaPrecio)
        {
            string clausula = string.Empty;
            if (bonificacionAperturaPrecio?.Importe > 0)
            {
                clausula += $"Se bonificará sobre el precio {DivisaSimbolica(bonificacionAperturaPrecio.Moneda)} {ValorAbsoluto(bonificacionAperturaPrecio.Importe)} " +
                    $" ({DevolverNumeroEnLetrasConDivisa(bonificacionAperturaPrecio.Importe, bonificacionAperturaPrecio.Moneda)}) por tonelada. ";
            }
            if (bonificacionAperturaPrecio?.Importe < 0)
            {
                clausula += $"Se descontará sobre el precio {DivisaSimbolica(bonificacionAperturaPrecio.Moneda)} {ValorAbsoluto(bonificacionAperturaPrecio.Importe)} " +
                    $" ({DevolverNumeroEnLetrasConDivisa(bonificacionAperturaPrecio.Importe, bonificacionAperturaPrecio.Moneda)}) por tonelada. ";
            }
            if (bonificacionAperturaPrecio?.Porcentaje > 0)
            {
                clausula += $"Se bonificará sobre el precio el {bonificacionAperturaPrecio.Porcentaje}% por tonelada. ";
            }
            if (bonificacionAperturaPrecio?.Porcentaje < 0)
            {
                clausula += $"Se descontará sobre el precio el {bonificacionAperturaPrecio.Porcentaje}% por tonelada. ";
            }
            return clausula;
        }


        private string DevolverNumeroEnLetrasConDivisa(decimal numero, string moneda)
        {
            numero = Math.Round(numero, 2);
            var letras = moneda == "USD" ? "Dólares " : "Pesos ";
            letras += ((int)Math.Abs(numero)).ToWords(CultureInfo.GetCultureInfo("es-AR")).ToUpper();
            var fraccion = numero - Math.Floor(numero);
            letras += $" con {((int)(fraccion * 100)).ToWords(CultureInfo.GetCultureInfo("es-AR")).ToUpper()} Centavos";
            return letras;
        }

        private string DivisaSimbolica(string divisa)
        {
            return divisa == "USD" ? Text.USD : Text.ARP;
        }
        private decimal ValorAbsoluto(decimal numero)
        {
            return Math.Abs(numero);
        }
    }
}
