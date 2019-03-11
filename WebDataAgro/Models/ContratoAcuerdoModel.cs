using Molinos.DataAgro.Entities.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
            this.Datos = new DatosIniComboContratoAcuerdo();
        }
    }
    public class ResultIniContratoAcuerdoModel : Resultado
    {
        public List<ContratoAcuerdoIni> Datos { get; set; }

        public ResultIniContratoAcuerdoModel()
        {
            this.Datos = new List<ContratoAcuerdoIni>();
        }
    }


    public class AbmContratoAcuerdoResult : Resultado
    {
        public ContratoAcuerdoDto ContratoAcuerdo { get; set; }

        public AbmContratoAcuerdoResult()
        {
            this.ContratoAcuerdo = new ContratoAcuerdoDto();
        }
    }

}