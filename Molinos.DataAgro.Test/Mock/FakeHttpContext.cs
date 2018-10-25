using System;
using System.IO;
using System.Reflection;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.SessionState;

namespace Molinos.DataAgro.Test.Mock
{
    public class FakeContext
    {
        public static HttpContext FakeHttpContext()
        {
            var httpRequest = new HttpRequest("", "http://localhost/", "");
            var stringWriter = new StringWriter();
            var httpResponse = new HttpResponse(stringWriter);
            var httpContext = new HttpContext(httpRequest, httpResponse);

            var sessionContainer = new HttpSessionStateContainer("id", new SessionStateItemCollection(),
                                                    new HttpStaticObjectsCollection(), 10, true,
                                                    HttpCookieMode.AutoDetect,
                                                    SessionStateMode.InProc, false);

            httpContext.Items["AspSession"] = typeof(HttpSessionState).GetConstructor(
                                        BindingFlags.NonPublic | BindingFlags.Instance,
                                        null, CallingConventions.Standard,
                                        new[] { typeof(HttpSessionStateContainer) },
                                        null)
                                .Invoke(new object[] { sessionContainer });


            GenericIdentity MyIdentity = new GenericIdentity("dominio\\nombre", "pass");
            ClaimsIdentity objClaim = new ClaimsIdentity("pass", System.IdentityModel.Claims.ClaimTypes.Name, "Recipient");
            objClaim.AddClaim(new Claim(System.IdentityModel.Claims.ClaimTypes.Name, "dominio\\nombre"));
            objClaim.AddClaim(new Claim(ClaimTypes.AuthenticationMethod, "Level3"));
            objClaim.AddClaim(new Claim(ClaimTypes.Name, "dominio\\nombre"));
            string[] Roles = { "Recipient" };
            GenericPrincipal MyPrincipal = new GenericPrincipal(objClaim, Roles);
            IPrincipal Identity = (IPrincipal)MyPrincipal;
            httpContext.User = Identity;


            return httpContext;
        }
    }
}
