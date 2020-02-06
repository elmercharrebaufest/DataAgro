using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{


    public class ResultIniFormula
    {
        public FormulaIni Formula { get; set; }
    }


    public class FormulaIni
    {
        public int Id { get; set; }
        public CriterioIni Criterio { get; set; }
        public int Inicio { get; set; }
        public int CantDias { get; set; }

        //agrgadosRecien
        public int CriterioId { get; set; }
        public DateTime Fecha { get; set; }
    }


    public class CriterioIni
    {
        public int Id { get; set; }
        public CriterioIni Padre { get; set; }
        public  int? PadreId { get; set; }
        public  ICollection<CriterioIni> Hijos { get; set; }

        public int Prioridad { get; set; }

        public string Descripcion { get; set; }
        public bool Concreta { get; set; }
        public string DisplayName { get; set; }
    }



    public class ResultIniCriterio
    {
        public ICollection<CriterioIni> Criterios { get; set; }
    }


    public class DatosIniAbmFormula
    {
        public ResultIniFormula ultimaFormulaTraida { get; set; }

        public ResultIniCriterio Criterios { get; set; }
    }


    //public class ResultIniCriterio
    //{
    //    public ICollection<CriterioIni> Criterios { get; set; }
    //}


}
