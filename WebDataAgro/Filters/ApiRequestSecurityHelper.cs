using System;
using System.Configuration;
using System.Web;

namespace WebDataAgro.Filters
{
    internal static class ApiRequestSecurityHelper
    {
        private const string ApiRoutePrefix = "~/api/ValidacionBoletos";
        private const string ApiAbsolutePathPrefix = "/api/validacionboletos";
        private const string ApiTokenEnvironmentVariable = "ApiTokenDataAgro";

        public static bool IsValidacionBoletosApiRequest(HttpRequestBase request)
        {
            if (request == null)
            {
                return false;
            }

            return IsApiPath(request.Url?.AbsolutePath, string.Concat(request.AppRelativeCurrentExecutionFilePath ?? string.Empty, request.PathInfo ?? string.Empty));
        }

        public static bool IsValidacionBoletosApiRequest(HttpRequest request)
        {
            if (request == null)
            {
                return false;
            }

            return IsApiPath(request.Url?.AbsolutePath, string.Concat(request.AppRelativeCurrentExecutionFilePath ?? string.Empty, request.PathInfo ?? string.Empty));
        }

        public static string GetExpectedToken()
        {
            var environmentToken = Environment.GetEnvironmentVariable(ApiTokenEnvironmentVariable);
            if (IsConfiguredToken(environmentToken))
            {
                return environmentToken;
            }

            var configuredToken = ConfigurationManager.AppSettings["ApiTokenDataAgro"];
            if (IsConfiguredToken(configuredToken))
            {
                return configuredToken;
            }

            return null;
        }

        private static bool IsApiPath(string absolutePath, string appRelativePath)
        {
            if (!string.IsNullOrWhiteSpace(absolutePath)
                && absolutePath.StartsWith(ApiAbsolutePathPrefix, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return !string.IsNullOrWhiteSpace(appRelativePath)
                && appRelativePath.StartsWith(ApiRoutePrefix, StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsConfiguredToken(string token)
        {
            return !string.IsNullOrWhiteSpace(token);
        }
    }
}
