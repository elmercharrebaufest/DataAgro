using System;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace WebDataAgro.Filters
{




    // atributo nuevo (ubicar?)

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class MyValidateAntiForgeryTokenAttribute : FilterAttribute, IAuthorizationFilter
    {

        

        private readonly AcceptVerbsAttribute _acceptVerbs;

        public MyValidateAntiForgeryTokenAttribute()
            : this(HttpVerbs.Post  )
        {

        }

       
      
        
         

        public MyValidateAntiForgeryTokenAttribute(HttpVerbs verbs)
        {
            _acceptVerbs = new AcceptVerbsAttribute(verbs);
        }

  
        


        public void OnAuthorization(AuthorizationContext filterContext)
        {
            var request = filterContext.RequestContext.HttpContext.Request;
            var requestType = request.RequestType;
            if (!_acceptVerbs.Verbs.Contains(requestType))
            {
                return;
            }


            //if (  requestType.ToString() != "GET" &&  requestType.ToString() != "POST")
            //{ return;
            //}

            var url = request.Url;
            var cookie = request.Cookies[AntiForgeryConfig.CookieName];
            var cookieToken = cookie != null ? cookie.Value : "";
            var name = "__RequestVerificationToken";
            var formToken = request.Form[name];

            if (string.IsNullOrWhiteSpace(formToken))
            {
                formToken = request.Headers[name];
            }
            if (string.IsNullOrWhiteSpace(formToken))
            {
                formToken = request.QueryString[name];
            }

            if (System.Configuration.ConfigurationManager.AppSettings["DesactivarToken"] == "0")
            {
                try
                {
                    AntiForgery.Validate(cookieToken, formToken);
                }
                catch
                {
                    throw new HttpException("Error");
                }
                

            }
               
           
          
          
            
 



 



        }
    }


    
}