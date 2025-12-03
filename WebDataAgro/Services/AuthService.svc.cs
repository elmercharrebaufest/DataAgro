using NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces.Managers;

namespace WebDataAgro.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "AuthService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select AuthService.svc or AuthService.svc.cs at the Solution Explorer and start debugging.
    public class AuthService : IAuthService
    {
        private readonly ILogger logger;
        private readonly ITokenManager tokenManager;
        public AuthService(
            ILogger logger,
            ITokenManager tokenManager
            )
        {
            this.logger = logger;
            this.tokenManager = tokenManager;
        }

        public TokenDto GenerarUrl(long cuit, string nombreUsuario)
        {
            return tokenManager.GenerarToken(cuit, nombreUsuario);
        }
    }
}
