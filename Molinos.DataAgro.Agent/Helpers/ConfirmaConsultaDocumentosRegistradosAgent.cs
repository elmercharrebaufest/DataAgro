using Molinos.DataAgro.Agent.ConfirmaQAConsultaDocumentoRegistradoPdf;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;

namespace Molinos.DataAgro.Agent.Helpers
{
    public class ConfirmaConsultaDocumentosRegistradosAgent : IConfirmaConsultaDocumentosRegistradosAgent
    {
        // ── Dependencias ─────────────────────────────────────────────────────────
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;

        // ── Configuración ────────────────────────────────────────────────────────
        private readonly string userConfirma = ConfigurationManager.AppSettings["ConfirmaUser"];
        private readonly string passConfirma = ConfigurationManager.AppSettings["ConfirmaPass"];
        private readonly string ambientePruebas = ConfigurationManager.AppSettings["AmbientePruebas"];

        // ── Constructor ──────────────────────────────────────────────────────────
        public ConfirmaConsultaDocumentosRegistradosAgent(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        // ════════════════════════════════════════════════════════════════════════
        // PUNTO DE ENTRADA
        // ════════════════════════════════════════════════════════════════════════

        public ConfirmaDocumentoRegistradoDto ConfirmaConsultaDocumentosRegistrados(string bolsa, string documento, string cuit)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaConfirma"] == "1")
                return DevolverResultadoPrueba();
            try
            {
                HabilitarSSLSiAmbientePruebas();

                var request = CrearRequest(bolsa, documento, cuit);
                var logId = GuardarLogXml(request.ToXml());

                var response = LlamarServicio(request);
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

        private ConfirmaDocumentoRegistradoDto DevolverResultadoPrueba()
        {
            return new ConfirmaDocumentoRegistradoDto
            {
                Errores = null,
                EstadoConsulta = "OK",
                NombreArchivo = "DocumentoPrueba.pdf",
                PdfBinario = new byte[] { 0x25, 0x50, 0x44, 0x46 } // Representación binaria de un PDF de prueba
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

        private ConsultaDocumentoRegistradoPdf CrearRequest(string bolsa, string documento, string cuit)
        {
            return new ConsultaDocumentoRegistradoPdf
            {
                CodigoBolsa = bolsa,
                CuitEmpresa = cuit,
                IdDocumento = documento
            };
        }

        private int GuardarLogXml(string xml)
        {
            logger.Debug(xml);

            var log = new Log
            {
                Fecha = DateTime.Now,
                Xml = xml
            };

            var entidad = repositorio.Agregar(log);
            repositorio.GuardarCambios();

            return entidad.Id;
        }

        private ConsultaDocumentoRegistradoPdfResult LlamarServicio(ConsultaDocumentoRegistradoPdf request)
        {
            var client = new ConsultaDocumentoRegistradoPdfServiceClient("Default4");
            client.ClientCredentials.UserName.UserName = userConfirma;
            client.ClientCredentials.UserName.Password = passConfirma;
            return client.ConsultaDocumentoRegistradoPdf(request);
        }

        private ConfirmaDocumentoRegistradoDto MapearRespuesta(ConsultaDocumentoRegistradoPdfResult result)
        {
            return new ConfirmaDocumentoRegistradoDto
            {
                NombreArchivo = result.NombreArchivo,
                PdfBinario = result.PdfBinario,
                EstadoConsulta = result.EstadoConsulta.ToString(),
                Errores = result.Errores != null ? string.Join(", ", result.Errores) : null
            };
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
