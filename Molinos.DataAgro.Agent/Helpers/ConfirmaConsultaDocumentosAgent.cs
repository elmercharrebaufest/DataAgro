using NLog;
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
using System.Linq;

namespace Molinos.DataAgro.Agent.Helpers
{
    public class ConfirmaConsultaDocumentosAgent : IConfirmaConsultaDocumentosAgent
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly string UserConfirma = ConfigurationManager.AppSettings["ConfirmaUser"];
        private readonly string PassConfirma = ConfigurationManager.AppSettings["ConfirmaPass"];
        private readonly string AmbientePruebas = ConfigurationManager.AppSettings["AmbientePruebas"];

        public ConfirmaConsultaDocumentosAgent(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        public List<ConfirmaConsultaDocumentosDto> ConsultaDocumentos(int bolsaId, string documentoId)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaConfirma"] == "1")
            {
                return new List<ConfirmaConsultaDocumentosDto>() { new ConfirmaConsultaDocumentosDto()
                    {
                        Acciones  = new List<Acciones>() { new Acciones()
                        {
                            Accion = "1",
                            Apellido = "Perez",
                            Cargo = "Gerente",
                            FechaHora = "2025-08-01",
                            Nombre = "Juan",
                            NroDocumento = "12345678",
                            Resultado = "1",
                            TipoDocumento = "DNI"
                        } 
                        },
                    }
                };
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
                            consultaIdBolsa = bolsaId.ToString(),
                            consultaIdDocumento = documentoId,
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

                    var response = agent.ConsultaEstadoDocumentos(rq);

                    List<ConfirmaConsultaDocumentosDto> listaConfirmaConsultaDocumentosDto = new List<ConfirmaConsultaDocumentosDto>();
                    foreach (var consultaDocumento in response.consultaDocumentoResponse)
                    {
                        var confirmaConsultaDocumentos = new ConfirmaConsultaDocumentosDto()
                        {
                            IdDocumento = consultaDocumento.IdDocumento,
                            IdBolsa = consultaDocumento.IdBolsa,
                            EstadoDocumento = consultaDocumento.EstadoDocumento,
                            ConsultaEstadoDocumento = (int)consultaDocumento.consultaEstadoDocumento,
                            EnPoderDe = new EmpresaConfirmaDto()
                            {
                                CUIT = Convert.ToInt32(consultaDocumento.EnPoderDe.CUIT.ToString()),
                                RazonSocial = consultaDocumento.EnPoderDe.RazonSocial
                            },
                            Acciones = consultaDocumento.Acciones != null
                            ? consultaDocumento.Acciones.Select(t => new Acciones()
                            {
                                Accion = t.Accion.Value,
                                Apellido = t.Usuario.Apellido,
                                Cargo = t.Usuario.Cargo,
                                FechaHora = t.FechaHora,
                                Nombre = t.Usuario.Nombre,
                                NroDocumento = t.Usuario.NroDocumento,
                                Resultado = t.Resultado.Value,
                                TipoDocumento = t.Usuario.TipoDocumento
                            }).ToList()
                            : new List<Acciones>()

                        };
                        listaConfirmaConsultaDocumentosDto.Add(confirmaConsultaDocumentos);
                    }

                    logger.Debug(listaConfirmaConsultaDocumentosDto.ToXml());
                    log = repositorio.Obtener<Log>(logId.Id);
                    log.Xml += listaConfirmaConsultaDocumentosDto.ToXml();
                    repositorio.GuardarCambios();
                    return listaConfirmaConsultaDocumentosDto;
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
