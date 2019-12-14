using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Shangpin.Framework.Common.Log;

namespace Shangpin.Ocs.Web
{
    public abstract class BaseController : Controller
    {
        protected string RenderPartialViewToString()
        {
            return RenderPartialViewToString(null, null);
        }

        //protected override void OnException(System.Web.Mvc.ExceptionContext filterContext)
        //{
        //    LoggerUtil.Exception(this.GetType(), "Application_Error", filterContext.Exception);

        //    base.OnException(filterContext);
        //}
        protected string RenderPartialViewToString(string viewName)
        {
            return RenderPartialViewToString(viewName, null);
        }

        protected string RenderPartialViewToString(object model)
        {
            return RenderPartialViewToString(null, model);
        }

        protected string RenderPartialViewToString(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");

            ViewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }
    }

}
