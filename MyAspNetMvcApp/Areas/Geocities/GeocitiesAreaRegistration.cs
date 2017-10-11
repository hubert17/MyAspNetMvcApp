using System.Web.Mvc;

namespace MyAspNetMvcApp.Areas.Geocities
{
    public class GeocitiesAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Geocities";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Geocities_default",
                "Geocities/{action}/{id}",
                new { controller = "Geocities", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}