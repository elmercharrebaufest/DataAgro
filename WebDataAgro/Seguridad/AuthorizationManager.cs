using System.ServiceModel;

namespace WebDataAgro.Seguridad
    public class AuthorizationManager : ServiceAuthorizationManager
    {
        public AuthorizationManager()
        {
        }

        protected override bool CheckAccessCore(OperationContext operationContext)
        {
            return true;
        }

    }
}
