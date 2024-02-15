using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using System;
using Humanizer;
using System.Globalization;
using System.Linq;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Resources;

namespace Molinos.DataAgro.Business.Procesamiento
{
    public class ProcesadorClausulaUno : ProcesadorClausula<ClausulaUno>
    {
        public ProcesadorClausulaUno(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaUno clausula)
        {
            var res = new ResultadoClausula();
            res.Texto = $"Los señores { clausula.Basico.RazonSocialProveedor} (en adelante el vendedor) domiciliados en { clausula.Basico.ProveedorDireccion } de la { clausula.Basico.ProveedorLocalidad }, " +
                $"{ clausula.Basico.ProveedorProvincia }, CP { clausula.Basico.ProveedorCP }, { (String.IsNullOrEmpty(clausula.Basico.ContratoCorredor) ? "" : "por intermedio de" + clausula.Basico.RazonSocialCorredor)}" +
                $"entregan a Molinos Agro S.A. domiciliado en AVENIDA PRESIDENTE MANUEL QUINTANA 192, PISO 1° de la Ciudad de Buenos Aires (en adelante el comprador), la cantidad de { clausula.Basico.Cantidad} kg. " +
                $"(kilogramos {((int)Math.Abs(clausula.Basico.Cantidad)).ToWords(CultureInfo.GetCultureInfo("es-AR")).ToUpper() }) de {clausula.Basico.Material } {(clausula.Basico.CantidadCamiones != 0 ? "o el resultante de " + clausula.Basico.CantidadCamiones + "camiones" : "") }" +
                $"y demás condiciones ";
            if (clausula.Basico.StandardDeCalidadId == 7)
            {
                res.Texto += "CALIDAD GRADO 2 ";
            }
            else if (clausula.Basico.TrigoEspecial == true)
            {
                res.Texto += "CALIDAD ESPECIAL ";
            }
            if (clausula.Basico.Calidades != null && clausula.Basico.StandardDeCalidadId != 7)
            {
                foreach (var cal in clausula.Basico.Calidades)
                {
                    res.Texto += cal.CalidadEspecialDesc.ToUpper() + " " + cal.Valor.ToString("N2", CultureInfo.CreateSpecificCulture("es-AR")) + "";
                    if (cal.PorcentajeDesde != null && cal.PorcentajeHasta != null)
                    {
                        res.Texto += "Porc. Desde " + cal.PorcentajeDesde + "% Hasta " + cal.PorcentajeHasta + "% ";
                    }
                }
            }
            res.Texto += $"de la cosecha  { clausula.Basico.Campania } ";
            if (clausula.Basico.TipoNegocioId == 2)
            {
                res.Texto += $"a  { clausula.Basico.PrecioNeto } { clausula.Basico.Moneda } ({DivisaEnLetras(clausula.Basico.Moneda)} {DevolverNumeroEnLetras(clausula.Basico.PrecioNeto.Value)}) más IVA la tonelada, ";

            }
            if (clausula.Basico.TipoNegocioId == 1 && clausula.Basico.Canje != true)
            {
                res.Texto += "con precio a fijar ";
            }
            else if (clausula.Basico.TipoNegocioId == 1 && clausula.Basico.Canje == true)
            {
                res.Texto += $"para cancelar { clausula.Basico.Insumo } - { clausula.Basico.Monto } { clausula.Basico.MonedaCanjeId } " +
                    $"(en adelante, el {"Insumo"}) así como también para cancelar los gastos asociados a los que el Vendedor hubiere incurrido para llevar a cabo la presente operación. " +
                    $"(en adelante, los {"Gastos Asociados"}). Las Partes acuerdan que el Insumo será a retirar en puerto por el Vendedor. ";

            }
            res.Texto += $"puesta sobre camión en: Planta { clausula.Basico.DestinoDescripcion } " +
                   $"Localidad { clausula.Basico.DestinoLocalidad }, de Provincia de { clausula.Basico.DestinoProvincia }. A todos los efectos impositivos los vendedores declaran que " +
                   $"la mercadería { (clausula.Basico.ClasificacionContrato == "PRODUCTOR" ? "SI" : "NO") } es de su propia producción. ";
            if (clausula.Basico.Consignatario == true)
            {
                res.Texto += "El vendedor actúa en carácter de consignatario. ";
            }
            res.Texto += $"Procedencia de la mercadería: { clausula.Basico.Localidad } (localidad) Pcia. de { clausula.Basico.Provincia } ";
            if (clausula.Basico.EstablecimientoPropio == true)
            {
                res.Texto += "Campo es Propio ";
            }
            if (clausula.Basico.EstablecimientoPropio == false)
            {
                res.Texto += "campo arrendado ";
            }
            if (clausula.Basico.PreciosPactados != null && clausula.Basico.PreciosPactados.Count > 0)
            {
                res.Texto += "El PRECIO del contrato se modificará de acuerdo a las siguientes fechas de entrega y recibos, desde el ";
                foreach (var precio in clausula.Basico.PreciosPactados)
                {
                    res.Texto += $"{precio.FechaDesde } al { precio.FechaHasta }" +
                        $"{ precio.MonedaPactadoDesc } { precio.Precio.ToString("N2", CultureInfo.CreateSpecificCulture("es-AR")) } ({ DevolverNumeroEnLetras(precio.Precio) })";
                }
            }
          
            return res;
        }

        public string DevolverNumeroEnLetras(decimal numero)
        {
            var letras = ((int)Math.Abs(numero)).ToWords(CultureInfo.GetCultureInfo("es-AR")).ToUpper();            
            var fraccion = numero - Math.Floor(numero);
            if (fraccion > 0)
            {               
                letras += $" con {((int)(fraccion * 100)).ToWords(CultureInfo.GetCultureInfo("es-AR")).ToUpper() }";
            }
            
            return letras;
        }

        private string DivisaEnLetras(string divisa)
        {
            return divisa == "USD"? Text.Divisa_USD: Text.Divisa_ARP;
        }
    }
}
