using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Shangpin.Framework.Common;


namespace Shangpin.Ocs.Web
{
    public class RouteConfig
    {
        //此处用来注册公用路由--例如登录等
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


            routes.MapRoute("Ocsloginout", "Loginout.html",
                 new { controller = "Login", action = "Loginout", id = UrlParameter.Optional, }
                 );
            routes.MapRoute("OcsLogin", "SignIn.html",
                 new { controller = "Login", action = "SignIn", id = UrlParameter.Optional, }
                 );
            routes.MapRoute("Defalut", "{controller}/{action}/{id}", //配置网站默认首页
                 new { controller = "Login", action = "SignIn", id = UrlParameter.Optional }
                 );
          
        }
    }
}