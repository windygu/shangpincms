using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Shangpin.Ocs.Service;
using Shangpin.Ocs.Service.Login;
using Shangpin.Ocs.Entity.Extenstion.Login;
using Shangpin.Ocs.Service.Common;

namespace Shangpin.Ocs.Web.Controllers
{
    public class LoginController : Controller
    {
       
        /// <summary>
        /// 测试！
        /// </summary>
        /// <returns></returns>
        public String Index()
        {
            DemoService service = new DemoService();

            var product = service.GetOneProduct();          

            if (product == null)
                return "Null";

            return product.ProductName;

        }

        public ActionResult SignIn(string flag="",string msg="")
        {
            Passport model = PresentationHelper.GetPassport();
            if (model.IsAuthenticate())
            {
                //return Redirect("/Login/Phoneverification");
                return Redirect("/Shangpin/Brand/AIIBrandsSelect");
            }
            ViewBag.Flag = flag;
            ViewBag.Msg = msg;
            ViewBag.Checked = "0";
            ViewBag.UserName = string.Empty;
            string userName=PresentationHelper.GetCookie("RemberOCSUser");
            if (!string.IsNullOrEmpty(userName))
            {
                ViewBag.Checked = "1";
                ViewBag.UserName = userName;
            }
            return View();
        }

        public ActionResult Loginout()
        {
            ///清空所有逻辑后，执行
            LoginService ls = new LoginService();
            ls.LoginOut();
            return RedirectToAction("SignIn");

        }

        [HttpPost]
        public ActionResult SignInPost()
        {
            string userName = Request.Form["UserName"].ToString();
            string password = Request.Form["Password"].ToString();
            string remberOCSUser = Request.Form["RememberMe"].ToString();
            LoginService ls = new LoginService();
            OcsServiceResult rs = ls.Authenticate(userName, password, remberOCSUser);
            if (rs.IsSuccess)
            {
                //return Redirect("/Login/Phoneverification");
                return Redirect("/Shangpin/Brand/AIIBrandsSelect");
            }

            return RedirectToAction("SignIn", new { flag = rs.ContentDic["Flag"].ToString(), msg = rs.ContentDic["Msg"].ToString() });
        }

        //手机验证
        public ActionResult Phoneverification()
        {
            string sui = HttpContext.Session["suiji"].ToString();
            if (sui == Request.Form["yanzheng"])
            {
                return Redirect("/Shangpin/Brand/AIIBrandsSelect");
            }
            else
            {
                ViewData["tip"] = new HtmlString("<script>alert('验证码有误,请重新输入！！')</script>");
                //return View("<script>alert('验证码有误,请重新输入！！')</script>");
            }
            return View();
        }
    }
}
