using Molinos.DataAgro.Entities.Dto;

namespace Molinos.DataAgro.Interfaces.Managers
{
    public interface ITokenManager
    {
        TokenDto GenerarToken(long cuit, string nombreUsuario);
        TokenDto ValidarToken(string token);
    }
}
