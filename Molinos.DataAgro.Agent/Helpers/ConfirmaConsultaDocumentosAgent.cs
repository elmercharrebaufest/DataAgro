using NLog;
using Molinos.DataAgro.Agent.ConfirmaQAConsultaDocumentos;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Interfaces.Agent;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;

namespace Molinos.DataAgro.Agent.Helpers
{
    public class ConfirmaConsultaDocumentosAgent : IConfirmaConsultaDocumentosAgent
    {
        // ── Dependencias ─────────────────────────────────────────────────────────
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;

        // ── Configuración ────────────────────────────────────────────────────────
        private readonly string userConfirma = ConfigurationManager.AppSettings["ConfirmaUser"];
        private readonly string passConfirma = ConfigurationManager.AppSettings["ConfirmaPass"];
        private readonly string ambientePruebas = ConfigurationManager.AppSettings["AmbientePruebas"];

        // ── Constructor ──────────────────────────────────────────────────────────
        public ConfirmaConsultaDocumentosAgent(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        // ════════════════════════════════════════════════════════════════════════
        // PUNTO DE ENTRADA
        // ════════════════════════════════════════════════════════════════════════

        public List<ConfirmaConsultaDocumentosDto> ConsultaDocumentos(int bolsaId, string documentoId)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaConfirma"] == "1")
                return DevolverResultadoPrueba();

            try
            {
                HabilitarSSLSiAmbientePruebas();

                var rq = CrearRequest(bolsaId, documentoId);

                int logId = GuardarLogXml(rq.ToXml());

                var response = LlamarServicio(rq);

                var resultado = MapearRespuesta(response);

                ActualizarLogXml(logId, resultado.ToXml());

                return resultado;
            }
            catch (Exception e)
            {
                logger.Error(e, "Error: No se pudo procesar la consulta de documentos en WS Confirma");
                throw;
            }
        }

        // ════════════════════════════════════════════════════════════════════════
        // MODO PRUEBA
        // ════════════════════════════════════════════════════════════════════════

        private List<ConfirmaConsultaDocumentosDto> DevolverResultadoPrueba()
        {
            return new List<ConfirmaConsultaDocumentosDto>
            {
                new ConfirmaConsultaDocumentosDto
                {
                    Acciones = new List<Acciones>
                    {
                        new Acciones
                        {
                            Accion        = "1",
                            Apellido      = "Perez",
                            Cargo         = "Gerente",
                            FechaHora     = "2025-08-01",
                            Nombre        = "Juan",
                            NroDocumento  = "12345678",
                            Resultado     = "1",
                            TipoDocumento = "DNI"
                        }
                    }
                }
            };
        }

        // ════════════════════════════════════════════════════════════════════════
        // MÉTODOS PRIVADOS
        // ════════════════════════════════════════════════════════════════════════

        private void HabilitarSSLSiAmbientePruebas()
        {
            if (ambientePruebas == "1")
                ServicePointManager.ServerCertificateValidationCallback +=
                    (sender, certificate, chain, sslPolicyErrors) => true;
        }

        private consultaDocumentoConsultaDocumentoItem[] CrearRequest(int bolsaId, string documentoId)
        {
            return new[]
            {
                new consultaDocumentoConsultaDocumentoItem
                {
                    consultaIdBolsa     = bolsaId.ToString(),
                    consultaIdDocumento = documentoId
                }
            };
        }

        private int GuardarLogXml(string xml)
        {
            logger.Debug(xml);

            var log = new Log
            {
                Fecha = DateTime.Now,
                Xml   = xml
            };
            var entidad = repositorio.Agregar(log);
            repositorio.GuardarCambios();
            return entidad.Id;
        }

        private consultaDocumentosResponse LlamarServicio(consultaDocumentoConsultaDocumentoItem[] rq)
        {
            var client = new ConsultaDocumentosServiceClient("Default6");
            client.ClientCredentials.UserName.UserName = userConfirma;
            client.ClientCredentials.UserName.Password = passConfirma;
            return client.ConsultaEstadoDocumentos(rq);
        }

        private List<ConfirmaConsultaDocumentosDto> MapearRespuesta(consultaDocumentosResponse response)
        {
            var resultado = new List<ConfirmaConsultaDocumentosDto>();

            foreach (var item in response.consultaDocumentoResponse)
            {
                resultado.Add(new ConfirmaConsultaDocumentosDto
                {
                    IdDocumento            = item.IdDocumento,
                    IdBolsa                = item.IdBolsa,
                    EstadoDocumento        = item.EstadoDocumento,
                    ConsultaEstadoDocumento = (int)item.consultaEstadoDocumento,
                    EnPoderDe = new EmpresaConfirmaDto
                    {
                        CUIT       = Convert.ToInt32(item.EnPoderDe.CUIT.ToString()),
                        RazonSocial = item.EnPoderDe.RazonSocial
                    },
                    Acciones = item.Acciones != null
                        ? item.Acciones.Select(t => new Acciones
                        {
                            Accion        = t.Accion.Value,
                            Apellido      = t.Usuario.Apellido,
                            Cargo         = t.Usuario.Cargo,
                            FechaHora     = t.FechaHora,
                            Nombre        = t.Usuario.Nombre,
                            NroDocumento  = t.Usuario.NroDocumento,
                            Resultado     = t.Resultado.Value,
                            TipoDocumento = t.Usuario.TipoDocumento
                        }).ToList()
                        : new List<Acciones>()
                });
            }

            return resultado;
        }

        private void ActualizarLogXml(int logId, string xml)
        {
            logger.Debug(xml);

            var log = repositorio.Obtener<Log>(logId);
            log.Xml += xml;
            repositorio.GuardarCambios();
        }
    }
}
