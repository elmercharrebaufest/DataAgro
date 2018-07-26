using Mastersoft.Framework.Standard;
using Molinos.DataAgro.Entities.Dto;
using System.Collections.Generic;

namespace WebDataAgro.Models
{

    public class ReportesModel
    {
        public List<MSErrorMessage> Errores { get; set; }
        public string DownloadKey { get; set; }

        public ReportesModel()
        {
            this.Errores = new List<MSErrorMessage>();
            this.DownloadKey = "";
        }
    }


    public class ReportesModificacionModel
    {
        public List<MSErrorMessage> Errores { get; set; }
        public ParamInformeComercial parametros { get; set; }
        public List<MaterialesModificacionInforme> materiales { get; set; }

        public ReportesModificacionModel()
        {
            this.Errores = new List<MSErrorMessage>();
            this.parametros = new ParamInformeComercial();
            this.materiales = new List<MaterialesModificacionInforme>();
        }
    }

}

