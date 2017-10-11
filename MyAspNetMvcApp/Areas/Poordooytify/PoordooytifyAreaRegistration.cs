using System.Web.Mvc;

namespace MyAspNetMvcApp.Areas.Poordooytify
{
    public class PoordooytifyAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Poordooytify";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Poordooytify_default",
                "Poordooytify/{action}/{id}",
                new { controller = "Poordooytify", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}