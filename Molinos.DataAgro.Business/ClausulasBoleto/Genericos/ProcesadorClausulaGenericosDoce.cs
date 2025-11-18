using NLog;
using Molinos.DataAgro.Entities.ClausulasBoleto.Genericos;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Business.ClausulasBoleto.Genericos
{
    public class ProcesadorClausulaGenericosDoce : ProcesadorClausulaGenericos<ClausulaGenericosDoce>
    {
        public ProcesadorClausulaGenericosDoce(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
            : base(repositorio, log, estadoBoleto) { }

        public override ResultadoClausula DevolverClausulas(ClausulaGenericosDoce clausula)
        {
            var res = new ResultadoClausula();
            if (clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR && clausula.Basico.Canje == true)
            {
                res.Texto += $"A los efectos del presente contrato LAS PARTES acuerdan que el valor total de EL INSUMO es de {clausula.Basico.MonedaCanjeDescripcion} " +
                    $"(ARP ES PESOS ARGENTINOS Y USD ES DÓLAR AMERICANO) {clausula.Basico.Monto?.ToString("N", new CultureInfo("es-AR"))} ({MetodosUtiles.DevolverNumeroEnLetras(clausula.Basico.Monto.Value)}) " +
                    $"(en adelante el {"COSTO DEL INSUMO"}). EL COSTO DEL INSUMO comprende el costo total de EL INSUMO en dólares estadounidenses con más todos los impuestos nacionales, " +
                    "provinciales y/o municipales que resulten aplicables a los mismos.";
            }
            return res;
        }
    }
}
