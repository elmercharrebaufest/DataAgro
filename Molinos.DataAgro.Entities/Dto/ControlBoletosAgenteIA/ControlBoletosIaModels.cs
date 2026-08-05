using Newtonsoft.Json;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.ControlBoletosAgenteIA
{
    public class ValidateTicketRequest
    {
        public string ContractId { get; set; }
        public string FilePath { get; set; }
        public byte[] FileContent { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public string UserId { get; set; }
        public string Source { get; set; }
        public string RequestId { get; set; }
        public string XRequestId { get; set; }
        public string XUserId { get; set; }
        public string XSourceSystem { get; set; }
    }

    public class ErrorResponse
    {
        [JsonProperty("detail")]
        public string Detail { get; set; }

        [JsonProperty("request_id")]
        public string RequestId { get; set; }
    }

    public class ContractLookupResponse
    {
        [JsonProperty("contract_id")]
        public string ContractId { get; set; }

        [JsonProperty("fields")]
        public Dictionary<string, object> Fields { get; set; }
    }

    public class ClauseCompareRequest
    {
        [JsonProperty("contract_id")]
        public string ContractId { get; set; }

        [JsonProperty("document_text")]
        public string DocumentText { get; set; }
    }

    public class ClauseComparisonResponse
    {
        [JsonProperty("contract_id")]
        public string ContractId { get; set; }

        [JsonProperty("matcher")]
        public string Matcher { get; set; }

        [JsonProperty("summary")]
        public ClauseSummaryOut Summary { get; set; }

        [JsonProperty("clauses")]
        public List<ClauseComparisonItem> Clauses { get; set; }
    }

    public class ClauseSummaryOut
    {
        [JsonProperty("total")]
        public int Total { get; set; }

        [JsonProperty("match")]
        public int Match { get; set; }

        [JsonProperty("different")]
        public int Different { get; set; }

        [JsonProperty("missing")]
        public int Missing { get; set; }

        [JsonProperty("needs_review")]
        public int NeedsReview { get; set; }

        [JsonProperty("human_review_required")]
        public bool HumanReviewRequired { get; set; }
    }

    public class ClauseComparisonItem
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("expected_text")]
        public string ExpectedText { get; set; }

        [JsonProperty("document_evidence")]
        public string DocumentEvidence { get; set; }

        [JsonProperty("result")]
        public string Result { get; set; }

        [JsonProperty("confidence")]
        public decimal Confidence { get; set; }

        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("similarity")]
        public decimal? Similarity { get; set; }

        [JsonProperty("entities_match")]
        public bool? EntitiesMatch { get; set; }
    }

    public class ValidationResponse
    {
        [JsonProperty("request_id")]
        public string RequestId { get; set; }

        [JsonProperty("contract_id")]
        public string ContractId { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("summary")]
        public SummaryOut Summary { get; set; }

        [JsonProperty("document")]
        public DocumentOut Document { get; set; }

        [JsonProperty("extracted_fields")]
        public Dictionary<string, ExtractedFieldOut> ExtractedFields { get; set; }

        [JsonProperty("system_fields")]
        public Dictionary<string, object> SystemFields { get; set; }

        [JsonProperty("comparison")]
        public List<ComparisonItem> Comparison { get; set; }

        [JsonProperty("suggested_actions")]
        public List<string> SuggestedActions { get; set; }

        [JsonProperty("audit")]
        public AuditOut Audit { get; set; }

        [JsonProperty("clause_summary")]
        public ClauseSummaryOut ClauseSummary { get; set; }

        [JsonProperty("clause_comparison")]
        public List<ClauseComparisonItem> ClauseComparison { get; set; }
    }

    public class SummaryOut
    {
        [JsonProperty("total_fields")]
        public int TotalFields { get; set; }

        [JsonProperty("ok")]
        public int Ok { get; set; }

        [JsonProperty("differences")]
        public int Differences { get; set; }

        [JsonProperty("missing_in_document")]
        public int MissingInDocument { get; set; }

        [JsonProperty("missing_in_system")]
        public int MissingInSystem { get; set; }

        [JsonProperty("review")]
        public int Review { get; set; }

        [JsonProperty("not_applicable")]
        public int NotApplicable { get; set; }

        [JsonProperty("needs_human_review")]
        public bool NeedsHumanReview { get; set; }
    }

    public class DocumentOut
    {
        [JsonProperty("type_detected")]
        public string TypeDetected { get; set; }

        [JsonProperty("confidence")]
        public decimal Confidence { get; set; }

        [JsonProperty("technical_type")]
        public string TechnicalType { get; set; }

        [JsonProperty("business_type")]
        public string BusinessType { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }
    }

    public class ExtractedFieldOut
    {
        [JsonProperty("value")]
        public object Value { get; set; }

        [JsonProperty("confidence")]
        public decimal Confidence { get; set; }
    }

    public class ComparisonItem
    {
        [JsonProperty("field")]
        public string Field { get; set; }

        [JsonProperty("document_value")]
        public object DocumentValue { get; set; }

        [JsonProperty("system_value")]
        public object SystemValue { get; set; }

        [JsonProperty("result")]
        public string Result { get; set; }

        [JsonProperty("severity")]
        public string Severity { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("match_type")]
        public string MatchType { get; set; }
    }

    public class AuditOut
    {
        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }

        [JsonProperty("model_version")]
        public string ModelVersion { get; set; }

        [JsonProperty("rules_version")]
        public string RulesVersion { get; set; }

        [JsonProperty("duration_ms")]
        public int DurationMs { get; set; }

        [JsonProperty("llm_calls")]
        public int LlmCalls { get; set; }

        [JsonProperty("llm_prompt_tokens")]
        public int LlmPromptTokens { get; set; }

        [JsonProperty("llm_completion_tokens")]
        public int LlmCompletionTokens { get; set; }

        [JsonProperty("llm_total_tokens")]
        public int LlmTotalTokens { get; set; }

        [JsonProperty("ocr_tokens")]
        public int OcrTokens { get; set; }

        [JsonProperty("ocr_calls")]
        public int OcrCalls { get; set; }

        [JsonProperty("analysis_tokens")]
        public int AnalysisTokens { get; set; }

        [JsonProperty("analysis_calls")]
        public int AnalysisCalls { get; set; }
    }
}
