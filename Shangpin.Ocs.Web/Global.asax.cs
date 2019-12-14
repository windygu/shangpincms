using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Shangpin.Framework.Common;
using Shangpin.Ocs.Web.Areas.Outlet;
using Shangpin.Framework.Common.Cache;
using Shangpin.Ocs.Service.Common;
using System.Web.Security;


namespace Shangpin.Ocs.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            DapperInitializer.Initialize();


            RedisCacheProvider.SetRedisCluster(AppSettingManager.AppSettings["redisServer"]);

            MemcachedProvider.SetMemcachedCluster(AppSettingManager.AppSettings["memcacheServer"].Split(','));
            EnyimMemcachedClient.SetMemcachedCluster(Shangpin.Ocs.Service.Common.AppSettingManager.AppSettings["memcached"].Split(','));


        }

        #region 解决用flash上传图片时登录COOKIE信息丢失问题
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            var Request = HttpContext.Current.Request;
            var Response = HttpContext.Current.Response;

            try
            {
                string auth_param_name = "AUTHID";
                string auth_cookie_name = AppSettingManager.AppSettings["LoginCookieName"].ToString();

                if (HttpContext.Current.Request.Form[auth_param_name] != null)
                {
                    UpdateCookie(auth_cookie_name, HttpContext.Current.Request.Form[auth_param_name]);
                }
                else if (HttpContext.Current.Request.QueryString[auth_param_name] != null)
                {
                    UpdateCookie(auth_cookie_name, HttpContext.Current.Request.QueryString[auth_param_name]);
                }

            }
            catch (Exception)
            {
                Response.StatusCode = 500;
                Response.Write("Error Initializing Session");
            }  
        }

        void UpdateCookie(string cookie_name, string cookie_value)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies.Get(cookie_name);
            if (cookie == null)
            {
                cookie = new HttpCookie(cookie_name);
                //SWFUpload 的Demo中给的代码有问题，需要加上cookie.Expires 设置才可以
                cookie.Expires = DateTime.Now.AddYears(1);
                HttpContext.Current.Request.Cookies.Add(cookie);
            }
            cookie.Value = cookie_value;
            HttpContext.Current.Request.Cookies.Set(cookie);
        }
        #endregion

    }
}