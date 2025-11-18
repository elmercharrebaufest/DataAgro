using Microsoft.Identity.Client;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebDataAgro.Helpers
{
    public static class MsalAppBuilder
    {
        private static readonly string ClientId = System.Configuration.ConfigurationManager.AppSettings["AzureAd_ClientId"];
        private static readonly string TenantId = System.Configuration.ConfigurationManager.AppSettings["AzureAd_TenantId"];
        private static readonly string ClientSecret = System.Configuration.ConfigurationManager.AppSettings["AzureAd_ClientSecret"];
        private static readonly string RedirectUri = System.Configuration.ConfigurationManager.AppSettings["AzureAd_RedirectUri"];
        private static readonly string PostLogoutUri = System.Configuration.ConfigurationManager.AppSettings["AzureAd_PostLogoutUri"];
        private static readonly string Authority = $"https://login.microsoftonline.com/{TenantId}/v2.0";

        /// <summary>
        /// Shared method to create an IConfidentialClientApplication from configuration and attach the application's token cache implementation
        /// </summary>
        /// <returns></returns>
        public static IConfidentialClientApplication BuildConfidentialClientApplication()
        {
            return BuildConfidentialClientApplication(ClaimsPrincipal.Current);
        }

        /// <summary>
        /// Shared method to create an IConfidentialClientApplication from configuration and attach the application's token cache implementation
        /// </summary>
        /// <param name="currentUser">The current ClaimsPrincipal</param>
        public static IConfidentialClientApplication BuildConfidentialClientApplication(ClaimsPrincipal currentUser)
        {
            IConfidentialClientApplication clientapp = ConfidentialClientApplicationBuilder.Create(ClientId)
                  .WithClientSecret(ClientSecret)
                  .WithRedirectUri(RedirectUri)
                  .WithB2CAuthority(Authority)
                  .Build();

            MSALPerUserMemoryTokenCache userTokenCache = new MSALPerUserMemoryTokenCache(clientapp.UserTokenCache, currentUser ?? ClaimsPrincipal.Current);

            return clientapp;
        }

        /// <summary>
        /// Common method to remove the cached tokens for the currently signed in user
        /// </summary>
        /// <returns></returns>
        public static async Task ClearUserTokenCache()
        {
            IConfidentialClientApplication clientapp = ConfidentialClientApplicationBuilder.Create(ClientId)
                .WithB2CAuthority(Authority)
                .WithClientSecret(ClientSecret)
                .WithRedirectUri(RedirectUri)
                .Build();

            // We only clear the user's tokens.
            MSALPerUserMemoryTokenCache userTokenCache = new MSALPerUserMemoryTokenCache(clientapp.UserTokenCache);
            var userAccounts = await clientapp.GetAccountsAsync();

            foreach (var account in userAccounts)
            {
                //Remove the users from the MSAL's internal cache
                await clientapp.RemoveAsync(account);
            }
            userTokenCache.Clear();

        }
    }
}