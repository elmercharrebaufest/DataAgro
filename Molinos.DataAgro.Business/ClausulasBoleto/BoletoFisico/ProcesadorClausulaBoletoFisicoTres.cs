using NLog;
using Molinos.DataAgro.Entities.ClausulasBoleto.BoletoFisico;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;

namespace Molinos.DataAgro.Business.ClausulasBoleto.BoletoFisico
{
    public class ProcesadorClausulaBoletoFisicoTres : ProcesadorClausulaBoletoFisico<ClausulaBoletoFisicoTres>
    {
        public ProcesadorClausulaBoletoFisicoTres(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
            : base(repositorio, log, estadoBoleto) { }

        public override ResultadoClausula DevolverClausulas(ClausulaBoletoFisicoTres clausula)
        {
            var res = new ResultadoClausula();
            // SI EL BOLETO ES DE CONTRATO CON PRECIO
            if (clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO)
            {
                if (clausula.Basico.PorcentajeDePago != null)
                {
                    string porcentajePago = clausula.Basico.PorcentajeDePago.ToString().Replace(",", ".");
                    res.Texto += $"El pago se hará {porcentajePago}% ({MetodosUtiles.DevolverNumeroEnLetras(clausula.Basico.PorcentajeDePago.Value)} por ciento), ";
                }

                if (clausula.Basico.Dias_Pesificado > 0)
                    res.Texto += $"los {clausula.Basico.Dias_Pesificado.Value} días";

                if (clausula.Basico.CD == true)
                    res.Texto += "anticipado";
                else if (clausula.Basico.Warrant == true)
                    res.Texto += "anticipado contra entrega de WARRANT";
                else if (clausula.Basico.Dias_Pesificado == null)
                    res.Texto += "las 72 hs";

                if (clausula.Basico.CD != true && clausula.Basico.Warrant != true)
                    res.Texto += ", con mercadería descargada en planta";

                if (clausula.Basico.PorcentajeDePago != null)
                {
                    decimal porcentaje = 100 - clausula.Basico.PorcentajeDePago.Value;
                    string porcentajePago = porcentaje.ToString().Replace(",", ".");
                    res.Texto += $", liquidándose el {porcentajePago}% ({MetodosUtiles.DevolverNumeroEnLetras(100 - clausula.Basico.PorcentajeDePago.Value)} por ciento) " +
                    $"restante a los 30 (treinta) días del cumplimiento del contrato.";
                }

                if (clausula.Basico.ProveedorId > 0 && clausula.Basico.CorredorId > 0)
                {
                    if (clausula.Basico.PagoDirectoVendedor == true)
                        res.Texto += " El pago se hará en su totalidad al vendedor. ";
                    else
                        res.Texto += " El pago del valor correspondiente a la mercadería se realizará al corredor. ";
                }
            }
            // SI EL BOLETO ES DE CONTRATO SIN PRECIO
            else if (clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR)
            {
                if (clausula.Basico.PorcentajeDePago != null)
                {
                    string porcentajePago = clausula.Basico.PorcentajeDePago.ToString().Replace(",", ".");
                    res.Texto += $"El pago se hará {porcentajePago}% ({MetodosUtiles.DevolverNumeroEnLetras(clausula.Basico.PorcentajeDePago.Value)} por ciento)";
                }
                var esFijacionDeContratoCanje = clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.FIJACION && clausula.Basico.Canje == true;
                var esAFijarSinCanje = clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR && clausula.Basico.Canje != true;
                var esConvenio = clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR && clausula.Basico.Madre == true;
                //SI EL NEGOCIO ES FIJACIÓN DISPONIBLE DE UN CONTRATO DE CANJE O ES UN NEGOCIO A FIJAR O NEGOCIO CONVENIO
                if (esFijacionDeContratoCanje || esAFijarSinCanje || esConvenio)
                {
                    res.Texto += $", 4 DÍAS HÁBILES DE FECHA DE FIJACIÓN, con mercadería descargada en fábrica, liquidándose el 2.5% (dos y medio por ciento) restando a los 30 (treinta) días del cumplimiento del contrato. El pago del valor correspondiente a la mercadería se realizará al corredor.";
                }
            }
            return res;
        }
    }
}
