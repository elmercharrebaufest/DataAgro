using Molinos.DataAgro.Entities.Dto;
using System.ServiceModel;

namespace WebDataAgro.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IAuthService" in both code and config file together.
    [ServiceContract]
    public interface IAuthService
    {
        [OperationContract]
        TokenDto GenerarUrl(long cuit, string nombreUsuario);
    }
}
