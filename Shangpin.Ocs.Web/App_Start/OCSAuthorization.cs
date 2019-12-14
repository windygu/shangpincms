using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Shangpin.Ocs.Entity.Extenstion.Login;

namespace Shangpin.Ocs.Service.Common
{
    [AttributeUsage(AttributeTargets.All,AllowMultiple=true)]
    public class OCSAuthorization : FilterAttribute,IAuthorizationFilter//,IResultFilter,IActionFilter,IExceptionFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            Passport model = PresentationHelper.GetPassport();
            if (!model.IsAuthenticate())
            {
                //filterContext.HttpContext.Response.Redirect("/SignIn.html",true);
                filterContext.Result = new RedirectResult("/SignIn.html");
            }
        }
    }
}
