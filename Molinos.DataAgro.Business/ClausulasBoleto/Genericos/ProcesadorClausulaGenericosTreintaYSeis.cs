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

                //Clausula retirada en el ticket  DAT-1048
                /*
                string porcentajeDePago = clausula.Basico.PorcentajeDePago.Value.ToString().Replace(",", ".");
                res.Texto += $"El pago del {porcentajeDePago}% será efectuado por el Comprador en forma anticipada a la entrega de la mercadería. A dichos fines, el Vendedor se constituye en depositario de {clausula.Basico.Cantidad.ToString("#,##0.##", CultureInfo.GetCultureInfo("es-ES"))} kilos de {clausula.Basico.Material}, obligándose a guardarla en su depósito y/o planta ubicada en {clausula.Basico.Localidad}, {clausula.Basico.Provincia}, en forma gratuita y a disposición del Comprador, reconociendo en cabeza de éste la propiedad de la mercadería, en atención al pago anticipado efectuado. Una vez abonada la mercadería, no podrá ser retirada de su lugar de guarda, hasta la fecha de entrega pactada en el presente contrato y/o hasta que sea indicado por el Comprador. " + 
                              "La pérdida de la mercadería por cualquier causa que fuera y/o la falta de entrega de la misma en los plazos pactados en el contrato, será considerada causal de incumplimiento, por lo que el Comprador estará facultado para reclamar al Comprador el monto cobrado anticipadamente con más los intereses que correspondan, y los daños y perjuicios que se hubieren generado por el incumplimiento mencionado.";
                */

                string cantidadContrato = clausula.Basico.Cantidad.ToString("#,##0.##", CultureInfo.GetCultureInfo("es-ES"));
                string materialContrato = clausula.Basico.Material;
                string provinciaContrato = clausula.Basico.Provincia;
                string localidadContrato = clausula.Basico.Localidad;
                res.Texto += $"El Comprador abonará de manera anticipada al Vendedor, conforme lo previsto en este contrato el precio correspondiente a {cantidadContrato} KG de {materialContrato} (la Mercadería Adquirida). ";
                res.Texto += $"El Vendedor transfiere la Mercadería Adquirida al Comprador y se constituye en depositario de la Mercadería Adquirida, obligándose a guardarla en calidad de depósito en su planta ubicada en {localidadContrato}-{provinciaContrato}, ";
                res.Texto += "(el Depósito). El Vendedor reconoce al Comprador su exclusiva propiedad sobre la Mercadería Adquirida y se compromete a mantenerla en todo momento a su total y permanente disponibilidad en el Depósito sin que le corresponda ";
                res.Texto += "a esta última abonar precio alguno por este concepto, por cuanto el costo total del Depósito se encuentra incluido en el precio de venta de la Mercadería Adquirida ya percibido y acordado en este contrato. ";
                res.Texto += "El Vendedor entregará la totalidad de la Mercadería Adquirida al solo y simple requerimiento del Comprador, sin necesidad de intimación previa y contra la sola presentación del presente contrato, en las instalaciones ";
                res.Texto += "que el Comprador posee en San Lorenzo, Provincia de Santa Fe. El Vendedor asume ante el Comprador con respecto a la Mercadería Adquirida, la responsabilidad que como depositario legalmente le corresponde, incluyendo, ";
                res.Texto += "pero no limitando a la guarda, conservación y cuidado activo de la Mercadería Adquirida. La pérdida, destrucción o deterioro en la calidad de la Mercadería Adquirida, por cualquier causa que fuera y/o la falta de ";
                res.Texto += "entrega en la forma pactada en este contrato será considerada causal de incumplimiento y el Comprador estará facultado a reclamar al Vendedor, la entrega inmediata de la Mercadería Adquirida en las condiciones ";
                res.Texto += "pactadas en el contrato o el pago de su equivalente en dólares estadounidenses conforme al valor de mercado al momento del efectivo pago. El valor del mercado utilizado será el que Molinos Agro determine, y se ";
                res.Texto += "le adicionarán los intereses, multas o daños y perjuicios que pudieran corresponder y que el Comprador podrá reclamar en la vía correspondiente conforme lo previsto en este contrato.";

            }
            return res;
        }
    }
}
