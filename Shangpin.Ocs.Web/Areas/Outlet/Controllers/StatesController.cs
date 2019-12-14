using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Shangpin.Ocs.Service.Outlet;

namespace Shangpin.Ocs.Web.Areas.Outlet.Controllers
{
    public class StatesController : Controller
    {
        private readonly LivingService _livingService;

        public StatesController()
        {
            _livingService = new LivingService();
        }

        public ActionResult Index()
        {
            return Content(_livingService.GetSate(Request.ServerVariables["Remote_Addr"].ToString()));
        }
    }
}
