using System.Web;
using System.Web.Mvc;

namespace MyAspNetMvcApp
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {            
            //filters.Add(new HandleErrorAttribute()); //Disable display of error message details
        }
    }
}
