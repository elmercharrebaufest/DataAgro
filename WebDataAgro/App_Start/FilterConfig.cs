using System.Web.Mvc;
using WebDataAgro.Filters;

namespace WebDataAgro
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //filters.Add(new HandleErrorAttribute());
            filters.Add(new CustomExceptionHandlerAttribute());
            filters.Add(new MyValidateAntiForgeryTokenAttribute());
            filters.Add(new RequireLoginAttribute());
        }
    }


}
