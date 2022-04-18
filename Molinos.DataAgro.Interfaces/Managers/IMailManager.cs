using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;
using System.Net.Mail;

namespace Molinos.DataAgro.Interfaces
{
    public interface IMailManager
    {
        void EnviarMail(Comercial desde, List<string> enviarA, string asunto, string cuerpo, List<string> copia = null, AlternateView vistaAlternativa = null, byte[] archivo = null, string nombreArchivo = null, List<string> emailRemitente = null);

        void EnviarMail(Comercial desde, List<Comercial> enviarA, string asunto, string cuerpo, List<Comercial> copia = null, AlternateView vistaAlternativa = null, byte[] archivo = null, string nombreArchivo = null, List<string> emailRemitente = null);

        void ReenviarMailCierreDia(string asunto, string asuntoNuevoMail, string cuerpo);
        void EnviarMail(List<string> enviarA, string asunto, string cuerpo, List<string> copia = null, AlternateView vistaAlternativa = null, byte[] archivo = null, string nombreArchivo = null, List<string> emailRemitente = null);
        string GetEmailUserActiveDirectory(string UserName);
    }
}
