using NLog;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Agent.Helpers
{
    public class ConfirmaConsultaLoteAgent : IConfirmaConsultaLoteAgent
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        String UserConfirma = ConfigurationManager.AppSettings["ConfirmaUser"];
        String PassConfirma = ConfigurationManager.AppSettings["ConfirmaPass"];
        String PINConfirma = ConfigurationManager.AppSettings["ConfirmaPIN"];

        public ConfirmaConsultaLoteAgent(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        public string ConfirmaConsultaLote()
        {

            return "";
        }
    }
}
