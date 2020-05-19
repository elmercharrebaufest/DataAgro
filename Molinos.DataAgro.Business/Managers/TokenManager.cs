using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces.Managers;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Repository.ConsultasEF;
using System;
using System.Configuration;
using System.Linq;

namespace Molinos.DataAgro.Business.Managers
{
    public class TokenManager : ITokenManager
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        readonly String UrlBaseDataAgro = ConfigurationManager.AppSettings["UrlBaseDataAgro"];

        public TokenManager(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        private string ObtenerToken()
        {
            var date = DateTime.UtcNow;
            var time = BitConverter.GetBytes(date.ToBinary());
            var key = Guid.NewGuid().ToByteArray();
            return Convert.ToBase64String(time.Concat(key).ToArray()).Replace('+', '-').Replace('/', '_');
        }

        public TokenDto ValidarToken(string token)
        {
            var tokenAuth = repositorio.Obtener<TokenAuth, TokenDto>(x => x.Token == token && x.Vencimiento > DateTime.Now,
                x => new TokenDto
                {
                    Cuit = x.Cuit,
                    NombreUsuario = x.NombreUsuario
                });
            if(tokenAuth != null){
                repositorio.EliminarTokens(tokenAuth.Cuit);
            }
            return tokenAuth;
        }

        public TokenDto GenerarToken(long cuit, string nombreUsuario)
        {
            if(cuit <= 0)
            {
                return new TokenDto
                {
                    Error = "El campo Cuit es obligatorio"
                };
            }
            if (string.IsNullOrEmpty(nombreUsuario))
            {
                return new TokenDto
                {
                    Error = "El campo NombreUsuario es obligatorio"
                };
            }

            var fecha = DateTime.Now;
            repositorio.EliminarTokens(cuit);
                        
            var tokenActivo = repositorio.Agregar(new TokenAuth
            {
                Cuit = cuit,
                NombreUsuario = nombreUsuario,
                Token = ObtenerToken(),
                Vencimiento = fecha.AddSeconds(220)
            });
            try
            {
                repositorio.GuardarCambios();
            }
            catch (Exception e)
            {
                logger.Error("Error al generar el token", e);
                return new TokenDto
                {
                    Error = "Error al generar el token"
                };
            }
            return new TokenDto
            {
                Cuit = cuit,
                NombreUsuario = nombreUsuario,
                Url = ObtenerUrl(tokenActivo.Token),
                Vencimiento = tokenActivo.Vencimiento
            };
        }

        private string ObtenerUrl(string token)
        {
            var url = UrlBaseDataAgro;
            //var url = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
            //return url.Replace("services/AuthService.svc", $"auth?t={token}");
            return url + $"auth?t={token}";
        }
    }
}
