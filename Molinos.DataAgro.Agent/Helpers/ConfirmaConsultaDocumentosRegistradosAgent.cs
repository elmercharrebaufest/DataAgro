using NLog;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System.Configuration;

namespace Molinos.DataAgro.Agent.Helpers
{
    public class ConfirmaConsultaDocumentosRegistradosAgent : IConfirmaConsultaDocumentosRegistradosAgent
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly string UserConfirma = ConfigurationManager.AppSettings["ConfirmaUser"];
        private readonly string PassConfirma = ConfigurationManager.AppSettings["ConfirmaPass"];
        private readonly string PINConfirma = ConfigurationManager.AppSettings["ConfirmaPIN"];

        public ConfirmaConsultaDocumentosRegistradosAgent(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        public string ConfirmaConsultaDocumentosRegistrados()
        {

            return "";
        }
    }
}
