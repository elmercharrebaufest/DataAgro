using Molinos.DataAgro.Entities.ControlBoletosAgenteIA;
using Molinos.DataAgro.Interfaces.Agent;
using Refit;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Agent.Helpers.ControlBoletosAgenteIA
{
    public class ControlBoletosIaAgent: IControlBoletosIaAgent
    {
        private const string UrlBaseKey = "UrlBaseControlBoletosIa";
        private const string ApiTokenKey = "ApiTokenControlBoletosIa";

        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromMinutes(2);

        private readonly string defaultBaseUrl;
        private readonly string apiToken;
        private readonly IControlBoletosIaRefitApi api;

        public ControlBoletosIaAgent()
            : this(
                ConfigurationManager.AppSettings[UrlBaseKey],
                ConfigurationManager.AppSettings[ApiTokenKey])
        {
        }

        public ControlBoletosIaAgent(string baseUrl, string apiToken)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            this.defaultBaseUrl = ConfigurationManager.AppSettings[UrlBaseKey];
            this.apiToken = apiToken;

            var resolvedBaseUrl = string.IsNullOrWhiteSpace(baseUrl)
                ? defaultBaseUrl
                : baseUrl;

            if (string.IsNullOrWhiteSpace(resolvedBaseUrl))
            {
                throw new InvalidOperationException(
                    $"No se encontró la configuración '{UrlBaseKey}'.");
            }

            if (!Uri.TryCreate(resolvedBaseUrl, UriKind.Absolute, out var uri))
            {
                throw new InvalidOperationException(
                    $"La URL '{resolvedBaseUrl}' no es válida.");
            }

            var httpClient = new HttpClient
            {
                BaseAddress = uri,
                Timeout = DefaultTimeout
            };

            api = RestService.For<IControlBoletosIaRefitApi>(httpClient);
        }

        #region Metodos publicos
        public ValidationResponse ValidateTicket(ValidateTicketRequest request)
        {
            return Execute(() => ValidateTicketAsync(request));
        }
        public ContractLookupResponse GetContract(string contractId)
        {
            return Execute(() => GetContractAsync(contractId));
        }
        public ClauseComparisonResponse CompareClauses(ClauseCompareRequest request)
        {
            return Execute(() => CompareClausesAsync(request));
        }
        public Dictionary<string, object> Health()
        {
            return Execute(() => HealthAsync());
        }
        private static T Execute<T>(Func<Task<T>> action)
        {
            try
            {
                return action().GetAwaiter().GetResult();
            }
            catch (ApiException ex)
            {
                throw new InvalidOperationException(
                    $"Error llamando al servicio. Código: {(int)ex.StatusCode}. Respuesta: {ex.Content}",
                    ex);
            }
        }
        #endregion

        #region Metodos privados
        private async Task<ValidationResponse> ValidateTicketAsync(ValidateTicketRequest request)
        {
            EnsureApiToken();
            ValidateRequest(request);

            Stream uploadStream;
            var streamPart = BuildStreamPart(request, out uploadStream);

            using (uploadStream)
            {
                return await api.ValidateTicketAsync(
                    streamPart,
                    request.ContractId,
                    request.UserId,
                    request.Source,
                    request.RequestId,
                    apiToken,
                    request.XRequestId,
                    request.XUserId,
                    request.XSourceSystem);
            }
        }
        private Task<ContractLookupResponse> GetContractAsync(string contractId)
        {
            EnsureApiToken();
            if (string.IsNullOrWhiteSpace(contractId))
            {
                throw new ArgumentException("contractId es obligatorio.", nameof(contractId));
            }

            return api.GetContractAsync(contractId, apiToken);
        }
        private Task<ClauseComparisonResponse> CompareClausesAsync(ClauseCompareRequest request)
        {
            EnsureApiToken();
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (string.IsNullOrWhiteSpace(request.ContractId))
            {
                throw new ArgumentException("request.contract_id es obligatorio.", nameof(request));
            }

            if (string.IsNullOrWhiteSpace(request.DocumentText))
            {
                throw new ArgumentException("request.document_text es obligatorio.", nameof(request));
            }

            return api.CompareClausesAsync(request, apiToken);
        }
        private Task<Dictionary<string, object>> HealthAsync()
        {
            return api.HealthAsync();
        }
        private void EnsureApiToken()
        {
            if (string.IsNullOrWhiteSpace(apiToken))
            {
                throw new InvalidOperationException("ApiTokenControlBoletosIa no configurado en appSettings.");
            }
        }
        private static void ValidateRequest(ValidateTicketRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (string.IsNullOrWhiteSpace(request.ContractId))
            {
                throw new ArgumentException("contract_id es obligatorio.", nameof(request));
            }

            var hasContent = request.FileContent != null && request.FileContent.Length > 0;
            var hasPath = !string.IsNullOrWhiteSpace(request.FilePath);

            if (!hasContent && !hasPath)
            {
                throw new ArgumentException("Debe enviar FileContent o FilePath.", nameof(request));
            }

            if (hasPath && !File.Exists(request.FilePath))
            {
                throw new FileNotFoundException("No se encontró el archivo a enviar.", request.FilePath);
            }
        }
        private static StreamPart BuildStreamPart(ValidateTicketRequest request, out Stream uploadStream)
        {
            if (request.FileContent != null && request.FileContent.Length > 0)
            {
                uploadStream = new MemoryStream(request.FileContent);
            }
            else
            {
                uploadStream = File.OpenRead(request.FilePath);
            }

            var fileName = !string.IsNullOrWhiteSpace(request.FileName)
                ? request.FileName
                : (string.IsNullOrWhiteSpace(request.FilePath) ? "documento" : Path.GetFileName(request.FilePath));

            var contentType = !string.IsNullOrWhiteSpace(request.ContentType)
                ? request.ContentType
                : InferContentType(fileName);

            return new StreamPart(uploadStream, fileName, contentType);
        }
        private static string InferContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName) ?? string.Empty;
            switch (extension.ToLowerInvariant())
            {
                case ".pdf":
                    return "application/pdf";
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".png":
                    return "image/png";
                case ".tif":
                case ".tiff":
                    return "image/tiff";
                case ".webp":
                    return "image/webp";
                default:
                    return "application/octet-stream";
            }
        }
        #endregion

    }
}
