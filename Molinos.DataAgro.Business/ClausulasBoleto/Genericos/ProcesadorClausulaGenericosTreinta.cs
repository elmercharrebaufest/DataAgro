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
    public class ProcesadorClausulaGenericosTreinta : ProcesadorClausulaGenericos<ClausulaGenericosTreinta>
    {
        public ProcesadorClausulaGenericosTreinta(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
            : base(repositorio, log, estadoBoleto) { }

        public override ResultadoClausula DevolverClausulas(ClausulaGenericosTreinta clausula)
        {
            var res = new ResultadoClausula();
            if (clausula.Basico.BoletoId != (int)EnumBoletoCompraNet.FISICO)
            {


                if (
                    (clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO &&
                     clausula.Basico.Moneda == "USD" &&
                     clausula.Basico.CorredorId > 0 &&
                        (clausula.Basico.ClasificacionDescripcion?.ToString().ToUpper() == "ACOPIADOR" ||
                         clausula.Basico.ClasificacionDescripcion?.ToString().ToUpper() == "PRODUCTOR" ||
                         clausula.Basico.ClasificacionDescripcion?.ToString().ToUpper() == "OTROS")
                        ) ||
                    (clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO &&
                     clausula.Basico.Moneda == "USD" &&
                        (clausula.Basico.ClasificacionDescripcion?.ToString().ToUpper() == "ACOPIADOR" ||
                        clausula.Basico.ClasificacionDescripcion?.ToString().ToUpper() == "OTROS")
                    )
                   )
                {
                    res.Texto += "Toda vez que el Acopiador/Corredor no proceda a liquidar la mercadería dentro de las 72 horas desde que la misma fuera entregada y aplicada, " +
                                 "las Partes acuerdan que quedará a opción del Comprador determinar el día que se tomará válido para establecer el tipo de cambio a utilizar en los términos dispuestos en el presente boleto.";
                }
            }
            return res;
        }
    }
}
