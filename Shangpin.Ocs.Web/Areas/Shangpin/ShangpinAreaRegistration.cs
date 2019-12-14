using System.Web.Mvc;

namespace Shangpin.Ocs.Web.Areas.Shangpin
{
    public class ShangpinAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Shangpin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Shangpin_default",
                "Shangpin/{controller}/{action}.html",
                new { action = "Index" }
            );
            context.MapRoute(
                "Shangpin_Venue",
                "Shangpin/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
