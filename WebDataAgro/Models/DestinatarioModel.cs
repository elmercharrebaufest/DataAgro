using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Mastersoft.Framework.Standard;

using Molinos.DataAgro.Entities;

namespace WebDataAgro.Models
{
    public class DatosIniAbmDestinatarioModel
    {
        public List<MSErrorMessage> Errores { get; set; }
        public Destinatario Destinatario { get; set; }

        public DatosIniAbmDestinatarioModel()
        {
            this.Errores = new List<MSErrorMessage>();
        }
    }


    public class ResultIniDestinatarioModel
    {
        public List<MSErrorMessage> Errores { get; set; }
        public List<DestinatarioIni> Datos { get; set; }

        public ResultIniDestinatarioModel()
        {
            this.Errores = new List<MSErrorMessage>();
            this.Datos = new List<DestinatarioIni>();
        }
    }


    public class AbmDestinatarioParam : IEntityValid
    {
        public int DestinatarioId { get; set; }

        public bool Validate(List<ErrorMessage> oErrorMessages)
        {
            return oErrorMessages.Count == 0;
        }
    }


    public class AbmDestinatarioResult
    {
        public List<MSErrorMessage> Errores { get; set; }
        public Destinatario Destinatario { get; set; }

        public AbmDestinatarioResult()
        {
            this.Errores = new List<MSErrorMessage>();
            this.Destinatario = new Destinatario();
        }
    }


}

