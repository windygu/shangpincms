using System.Web.Mvc;

namespace Shangpin.Ocs.Web.Areas.ProductFlat
{
    public class ProductFlatAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "ProductFlat";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "ProductFlat_default",
                "ProductFlat/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
