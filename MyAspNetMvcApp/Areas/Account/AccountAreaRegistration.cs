using System.Web.Mvc;

namespace MyAspNetMvcApp.Areas.Account
{
    public class AccountAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Account";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Account_default",
                "Account/{action}/{id}",
                new { controller = "Account", action = "Index", id = UrlParameter.Optional }
            );
            context.MapRoute(
                "Roles_default",
                "Roles/{action}/{id}",
                new { controller = "Roles", action = "Index", id = UrlParameter.Optional }
            );
            //context.MapRoute(
            //    "Account_default",
            //    "Account/{controller}/{action}/{id}",
            //    new { action = "Index", id = UrlParameter.Optional }
            //);
        }
    }
}