using System.Web.Mvc;

namespace Shangpin.Ocs.Web.Areas.Outlet
{
    public class OutletAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Outlet";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.Routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            context.MapRoute(
                 "Outlet_default",
                 "Outlet/{controller}/{action}/{id}",
                 new {controller="Home",action = "Index",id=UrlParameter.Optional }
             );

        }
    }
}
