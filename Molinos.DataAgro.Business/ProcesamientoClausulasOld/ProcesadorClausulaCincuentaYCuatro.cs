using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;


namespace Molinos.DataAgro.Business.ProcesamientoClausulas
{
    public class ProcesadorClausulaCincuentaYCuatro : ProcesadorClausula<ClausulaCincuentaYCuatro>
    {
        public ProcesadorClausulaCincuentaYCuatro(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto) : base(repositorio, log, estadoBoleto)
        {
        }
        public override ResultadoClausula DevolverClausulas(ClausulaCincuentaYCuatro clausula)
        {
            var res = new ResultadoClausula();
            /*
			if (clausula.Basico.MaterialId == (int)EnumMateriales.TRIGO)
            {
                res.Texto += $"Conforme lo establecido en la Resolución N° 41/2020 de la Secretaria de Alimentos, Bioeconomía y Desarrollo, que establece que no puede comercializarse el primer trigo genéticamente modificado en el país hasta tanto Brasil haya otorgado el permiso de importación del mismo – aspecto que no ha sucedido-, el vendedor declara que el trigo a entregar no es genéticamente modificado y que en caso contrario asume los daños y perjuicios que pudiera causar por contaminación o rechazo que sufra el comprador en cualquier instancia. A los fines de determinar el tipo de trigo entregado se extraerán dos muestras lacradas por camión, las que quedarán en depósito de nuestra empresa o puerto para su análisis en caso de ser necesario. La responsabilidad asumida incluye todo perjuicio directo o indirecto que deba soportar el comprador.";
            }
			*/
            return res;
        }
    }
}
