using System.Web.Mvc;

namespace MyAspNetMvcApp.Areas.BernardGabonDotCom
{
    public class BernardGabonDotComAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "BernardGabonDotCom";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {

            context.MapRoute(
                "BernardGabonDotCom_default",
                "BernardGabonDotCom/{controller}/{action}/{id}",
                new { controller = "BernardGabonDotCom", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}