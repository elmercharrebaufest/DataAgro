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
    public class ControlBoletosIaAgent
    {
        private const string DefaultBaseUrl = "http://localhost:8000";

        private readonly string apiToken;
        private readonly IControlBoletosIaRefitApi api;

        public ControlBoletosIaAgent()
            : this(
                  ConfigurationManager.AppSettings["UrlBaseControlBoletosIa"],
                  ConfigurationManager.AppSettings["ApiTokenControlBoletosIa"])
        {
        }

        public ControlBoletosIaAgent(string baseUrl, string apiToken)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            this.apiToken = apiToken;
            var resolvedBaseUrl = string.IsNullOrWhiteSpace(baseUrl) ? DefaultBaseUrl : baseUrl;

            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(resolvedBaseUrl.TrimEnd('/')),
                Timeout = TimeSpan.FromMinutes(2)
            };

            api = RestService.For<IControlBoletosIaRefitApi>(httpClient);
        }

        public ValidationResponse ValidateTicket(ValidateTicketRequest request)
        {
            return ValidateTicketAsync(request).GetAwaiter().GetResult();
        }

        public async Task<ValidationResponse> ValidateTicketAsync(ValidateTicketRequest request)
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

        public ContractLookupResponse GetContract(string contractId)
        {
            return GetContractAsync(contractId).GetAwaiter().GetResult();
        }

        public Task<ContractLookupResponse> GetContractAsync(string contractId)
        {
            EnsureApiToken();
            if (string.IsNullOrWhiteSpace(contractId))
            {
                throw new ArgumentException("contractId es obligatorio.", nameof(contractId));
            }

            return api.GetContractAsync(contractId, apiToken);
        }

        public ClauseComparisonResponse CompareClauses(ClauseCompareRequest request)
        {
            return CompareClausesAsync(request).GetAwaiter().GetResult();
        }

        public Task<ClauseComparisonResponse> CompareClausesAsync(ClauseCompareRequest request)
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

        public Dictionary<string, object> Health()
        {
            return HealthAsync().GetAwaiter().GetResult();
        }

        public Task<Dictionary<string, object>> HealthAsync()
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
    }
}
