using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;
using Molinos.DataAgro.Entities.ControlBoletosAgenteIA;
namespace Molinos.DataAgro.Agent.Helpers.ControlBoletosAgenteIA
{
    [Headers("Accept: application/json")]
    public interface IControlBoletosIaRefitApi
    {
        [Multipart]
        [Post("/v1/tickets/validate")]
        Task<ValidationResponse> ValidateTicketAsync(
            [AliasAs("file")] StreamPart file,
            [AliasAs("contract_id")] string contractId,
            [AliasAs("user_id")] string userId = null,
            [AliasAs("source")] string source = null,
            [AliasAs("request_id")] string requestId = null,
            [Header("X-API-Token")] string apiToken = null,
            [Header("X-Request-Id")] string xRequestId = null,
            [Header("X-User-Id")] string xUserId = null,
            [Header("X-Source-System")] string xSourceSystem = null);

        [Get("/v1/contracts/{contractId}")]
        Task<ContractLookupResponse> GetContractAsync(
            string contractId,
            [Header("X-API-Token")] string apiToken = null);

        [Post("/v1/clauses/compare")]
        Task<ClauseComparisonResponse> CompareClausesAsync(
            [Body] ClauseCompareRequest request,
            [Header("X-API-Token")] string apiToken = null);

        [Get("/health")]
        Task<Dictionary<string, object>> HealthAsync();
    }
}
