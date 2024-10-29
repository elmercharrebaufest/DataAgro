using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.ConfirmaQAConsultaDocumentos;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;

namespace Molinos.DataAgro.Agent.Helpers
{
    public class ConfirmaConsultaDocumentosAgent : IConfirmaConsultaDocumentosAgent
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        String UserConfirma = ConfigurationManager.AppSettings["ConfirmaUser"];
        String PassConfirma = ConfigurationManager.AppSettings["ConfirmaPass"];
        String AmbientePruebas = ConfigurationManager.AppSettings["AmbientePruebas"];

        public ConfirmaConsultaDocumentosAgent(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        public List<ConfirmaConsultaDocumentosDto> ConsultaDocumentos(int bolsaId, string documentoId)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaConfirma"] == "1")
            {
                return new List<ConfirmaConsultaDocumentosDto>() { new ConfirmaConsultaDocumentosDto() {
                    estadoDocumento = 2,
                    idBolsa = "1",
                    idDocumento = "25007039",
                    consultaEstado = 1,
                    consultaEstadoDocumento = 1,
                }};
            }
            else
            {
                try
                {
                    // Deshabilita temporalmente la validación del certificado SSL. Aplicar sólo para UAT/Staging. No para PRD.
                    if (AmbientePruebas == "1") ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

                    ConsultaDocumentosServiceClient agent = new ConsultaDocumentosServiceClient();
                    agent.ClientCredentials.UserName.UserName = UserConfirma;
                    agent.ClientCredentials.UserName.Password = PassConfirma;

                    var rq = new consultaDocumentoConsultaDocumentoItem[]
                    {
                        new consultaDocumentoConsultaDocumentoItem()
                        {
                            //consultaIdBolsa = bolsaId.ToString(),
                            //consultaIdDocumento = documentoId,
                            consultaIdBolsa = "1",
                            consultaIdDocumento = "112906131",
                        }
                    };

                    var log = new Log
                    {
                        Fecha = DateTime.Now,
                        Xml = rq.ToXml()
                    };
                    var logId = repositorio.Agregar(log);
                    repositorio.GuardarCambios();
                    logger.Debug(rq.ToXml());

                    var valor = agent.ConsultaEstadoDocumentos(rq);

                    List<ConfirmaConsultaDocumentosDto> miLista = new List<ConfirmaConsultaDocumentosDto>();
                    //foreach (var v in valor)
                    //{
                    //    var vv = new ConfirmaConsultaDocumentosDto()
                    //    {
                    //        estadoDocumentoField = v.EstadoDocumento,
                    //        idBolsaField = v.IdBolsa,
                    //        idDocumentoField = v.IdDocumento
                    //    };
                    //    miLista.Add(vv);
                    //}

                    logger.Debug(valor.ToXml());
                    log = repositorio.Obtener<Log>(logId.Id);
                    log.Xml += valor.ToXml();
                    repositorio.GuardarCambios();
                    return miLista;
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    throw;
                }
            }

        }

    }
}
