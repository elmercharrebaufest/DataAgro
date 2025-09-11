using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Interfaces;
using System.Globalization;

namespace Molinos.DataAgro.Business.Procesamiento
{
    public class ProcesadorClausulaCincuentaYTres : ProcesadorClausula<ClausulaCincuentaYTres>
    {
        public ProcesadorClausulaCincuentaYTres(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaCincuentaYTres clausula)
        {
            var res = new ResultadoClausula();
            // Se corrige la condicion por pedido del usuario y solo aplicaria para CD/CG esta clausula no para Warrant
            //if (clausula.Basico.PorcentajeDePago.HasValue && (clausula.Basico.CD == true || clausula.Basico.Warrant == true))
            
			/*
			if (clausula.Basico.PorcentajeDePago.HasValue && clausula.Basico.CD == true)
            {
                res.Texto += $"El pago del {clausula.Basico.PorcentajeDePago.Value}% será efectuado por el Comprador en forma anticipada a la entrega de la mercadería. A dichos fines, el Vendedor se constituye en depositario de {clausula.Basico.Cantidad.ToString("#,##0.##", CultureInfo.GetCultureInfo("es-ES"))} kilos de {clausula.Basico.Material}, obligándose a guardarla en su depósito y/o planta ubicada en {clausula.Basico.Localidad}, {clausula.Basico.Provincia}, en forma gratuita y a disposición del Comprador, reconociendo en cabeza de éste la propiedad de la mercadería, en atención al pago anticipado efectuado. Una vez abonada la mercadería, no podrá ser retirada de su lugar de guarda, hasta la fecha de entrega pactada en el presente contrato y/o hasta que sea indicado por el Comprador. La pérdida de la mercadería por cualquier causa que fuera y/o la falta de entrega de la misma en los plazos pactados en el contrato, será considerada causal de incumplimiento, por lo que el Comprador estará facultado para reclamar al Vendedor el monto cobrado anticipadamente con más los intereses que correspondan, y los daños y perjuicios que se hubieren generado por el incumplimiento mencionado.";
            }
			*/
            
			return res;
        }
    }
}
