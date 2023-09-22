using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Molinos.DataAgro.Entities.Dto;



namespace WebDataAgro.Models
{
    public class AbmFormulaResult : Resultado
    {
        public FormulaDto Formula { get; set; }

        public AbmFormulaResult()
        {
            this.Formula = new FormulaDto();
        }

    
    }


    public class ResultIniCriterioModel : Resultado
    {
        public ICollection<CriterioIni> Datos { get; set; }

        public ResultIniCriterioModel()
        {
            this.Datos = new List<CriterioIni>();
        }
    }

    public class ResultIniTipoNegocioExcluidoModel : Resultado
    {
        public ICollection<FormulaTipoNegocioExcluidoDto> Datos { get; set; }

        public ResultIniTipoNegocioExcluidoModel()
        {
            this.Datos = new List<FormulaTipoNegocioExcluidoDto>();
        }
    }

    public class AbmCriterioResult : Resultado
    {
        public CriterioDto Criterio { get; set; }

        public AbmCriterioResult()
        {
            this.Criterio = new CriterioDto();
        }

    }

    public class ResultIniFormulaModel : Resultado
    {
        public DatosIniAbmFormula Datos { get; set; }

        public ResultIniFormulaModel()
        {
            this.Datos = new DatosIniAbmFormula();
        }
    }

}