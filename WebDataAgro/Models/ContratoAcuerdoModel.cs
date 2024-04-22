using Molinos.DataAgro.Entities.Dto;
using System.Collections.Generic;

namespace WebDataAgro.Models
{
    public class ContratoAcuerdoModel
    {
    }
    public class ContratoAcuerdoModel_prueba : Resultado
    {
        public DatosIniComboContratoAcuerdo Datos { get; set; }

        public ContratoAcuerdoModel_prueba()
        {
            Datos = new DatosIniComboContratoAcuerdo();
        }
    }

    public class ResultIniContratoAcuerdoModel : Resultado
    {
        public List<ContratoAcuerdoIni> Datos { get; set; }

        public ResultIniContratoAcuerdoModel()
        {
            Datos = new List<ContratoAcuerdoIni>();
        }
    }

    public class AbmContratoAcuerdoResult : Resultado
    {
        public ContratoAcuerdoDto ContratoAcuerdo { get; set; }

        public AbmContratoAcuerdoResult()
        {
            ContratoAcuerdo = new ContratoAcuerdoDto();
        }
    }
}