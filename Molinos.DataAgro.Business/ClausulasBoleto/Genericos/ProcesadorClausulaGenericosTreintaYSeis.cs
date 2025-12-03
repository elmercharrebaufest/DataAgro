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
    public class ProcesadorClausulaGenericosTreintaYSeis : ProcesadorClausulaGenericos<ClausulaGenericosTreintaYSeis>
    {
        public ProcesadorClausulaGenericosTreintaYSeis(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
            : base(repositorio, log, estadoBoleto) { }

        public override ResultadoClausula DevolverClausulas(ClausulaGenericosTreintaYSeis clausula)
        {
            var res = new ResultadoClausula();
            if (clausula.Basico.PorcentajeDePago.HasValue && clausula.Basico.CD == true)
            {
                string porcentajeDePago = clausula.Basico.PorcentajeDePago.Value.ToString().Replace(",", ".");
                res.Texto += $"El pago del {porcentajeDePago}% será efectuado por el Comprador en forma anticipada a la entrega de la mercadería. A dichos fines, el Vendedor se constituye en depositario de {clausula.Basico.Cantidad.ToString("#,##0.##", CultureInfo.GetCultureInfo("es-ES"))} kilos de {clausula.Basico.Material}, obligándose a guardarla en su depósito y/o planta ubicada en {clausula.Basico.Localidad}, {clausula.Basico.Provincia}, en forma gratuita y a disposición del Comprador, reconociendo en cabeza de éste la propiedad de la mercadería, en atención al pago anticipado efectuado. Una vez abonada la mercadería, no podrá ser retirada de su lugar de guarda, hasta la fecha de entrega pactada en el presente contrato y/o hasta que sea indicado por el Comprador. " + 
                              "La pérdida de la mercadería por cualquier causa que fuera y/o la falta de entrega de la misma en los plazos pactados en el contrato, será considerada causal de incumplimiento, por lo que el Comprador estará facultado para reclamar al Comprador el monto cobrado anticipadamente con más los intereses que correspondan, y los daños y perjuicios que se hubieren generado por el incumplimiento mencionado.";
            }
            return res;
        }
    }
}
