using System.Web.Mvc;

namespace MyAspNetMvcApp.Areas.OrderFramework
{
    public class OrderFrameworkAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "OrderFramework";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "OrderFramework_default",
                "store/{controller}/{action}/{id}",
                new { controller = "Products", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}