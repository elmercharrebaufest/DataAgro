using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;

namespace WebDataAgro.Models
{
    public class DatosIniAbmDestinatarioModel : Resultado
    {
        public Destinatario Destinatario { get; set; }
    }


    public class ResultIniDestinatarioModel : Resultado
    {
        public List<DestinatarioIni> Datos { get; set; }

        public ResultIniDestinatarioModel()
        {
            this.Datos = new List<DestinatarioIni>();
        }
    }


    public class AbmDestinatarioParam
    {
        public int DestinatarioId { get; set; }
    }


    public class AbmDestinatarioResult : Resultado
    {
        public Destinatario Destinatario { get; set; }

        public AbmDestinatarioResult()
        {
            this.Destinatario = new Destinatario();
        }
    }


}

