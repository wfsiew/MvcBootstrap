using System.Web.Mvc;

namespace MvcBootstrap.Areas.Ng
{
    public class NgAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Ng";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Ng_index",
                "Ng",
                new { controller = "Ng", action = "Index" }
            );

            context.MapRoute(
                "Ng_about",
                "Ng/About",
                new { controller = "Ng", action = "About" }
            );

            context.MapRoute(
                "Ng_default",
                "Ng/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "MvcBootstrap.Areas.Ng.Controllers" }
            );
        }
    }
}
