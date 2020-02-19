using Molinos.DataAgro.Entities.Dto;
using System.Collections.Generic;

namespace WebDataAgro.Models
{

    public class ReportesModel : Resultado
    {
        public string DownloadKey { get; set; }

        public ReportesModel()
        {
            this.DownloadKey = "";
        }
    }


    public class ReportesModificacionModel : Resultado
    {
        public ParamInformeComercial parametros { get; set; }
        public List<MaterialesModificacionInforme> materiales { get; set; }

        public ReportesModificacionModel()
        {
            this.parametros = new ParamInformeComercial();
            this.materiales = new List<MaterialesModificacionInforme>();
        }
    }

}

