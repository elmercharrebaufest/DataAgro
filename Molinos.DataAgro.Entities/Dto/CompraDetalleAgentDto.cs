using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class CompraDetalleAgentDto
    {
        public string VENDEDOR { get; set; }
        public string MATERIAL { get; set; }
        public string COSECHA { get; set; }
        public string CLASE_DOC { get; set; }
        public string CLASIFICACION { get; set; }
        public string CONTRATO { get; set; }
        public string CORREDOR { get; set; }
        public string FECHA { get; set; }
        public decimal PEND_APLICAR { get; set; }
        public decimal PEND_FIJAR { get; set; }
        public decimal TN_AMPLIADAS { get; set; }
        public decimal TN_ANULADAS { get; set; }
        public decimal TN_APLICADAS { get; set; }
        public decimal TN_CONTRATO { get; set; }
        public decimal TN_FIJADAS { get; set; }
        public string FECHA_DESDE { get; set; }
        public string FECHA_HASTA { get; set; }
        public string CENTRO { get; set; }
    }

}


