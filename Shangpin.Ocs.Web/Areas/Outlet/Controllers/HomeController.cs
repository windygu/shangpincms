using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shangpin.Ocs.Web.Areas.Outlet.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Outlet/Home/

        public ActionResult Index() 
        {
            return Content("奥莱测试内容");  
        }

    }
}
