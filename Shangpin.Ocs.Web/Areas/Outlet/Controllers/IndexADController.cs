using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Shangpin.Entity.Wfs;
using Shangpin.Ocs.Service.Common;
using Shangpin.Ocs.Entity.Extenstion.Outlet;
using Shangpin.Ocs.Service;
using Shangpin.Ocs.Service.Outlet;

namespace Shangpin.Ocs.Web.Areas.Outlet.Controllers
{
    public class IndexADController : Controller
    {
        // 
        // GET: /Outlet/IndexAD/

        public ActionResult Index(string name, string position, string begindate,string enddate, string gender, int pageIndex = 1, int siteNo = 2)
        {

            int pageSize = int.Parse(AppSettingManager.AppSettings["ComonListPageNum"].ToString());
            ViewBag.CurPage = pageIndex;
            ViewBag.PageSize = pageSize;
            IList<SWfsPictureManager> list = new SWfsPictureManagerService().GetList(name, position, begindate,enddate, gender, siteNo, (int)ADPosition.PagePosition);
            ViewBag.Count = list.Count();
            list = list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();//默认每页显示20条数据
            ViewBag.List = list;
            ViewBag.Name = name??"";
            ViewBag.BTime = begindate ?? "";
            ViewBag.ETime = enddate ?? "";
            ViewBag.Position = position??"";
            ViewBag.Gender = gender ?? "-1";
            return View();
        }

        /// <summary>
        /// 创建和修改的呈现
        /// </summary>
        /// <returns></returns>
        public ActionResult Manage(string act, string picManId = "")
        {
            SWfsPictureManager model = new SWfsPictureManager();
            ViewBag.Act = act;
            SWfsPictureManagerService service = new SWfsPictureManagerService();
            switch (act)
            {
                case "del"://删除
                    
                    service.Del(picManId);
                    return Redirect("/outlet/IndexAD/index" + CommonService.GetTimeStamp("?"));

                case "create"://新建
                    model.DateBegin = Convert.ToDateTime("1900-1-1");
                    model.DateEnd = Convert.ToDateTime("1900-1-1");
                    model.Gender = -1;
                    return View("/Areas/Outlet/Views/IndexAD/Manage.cshtml", model);

                case "edit"://修改

                    model = service.GetModel(picManId);
                    return View("/Areas/Outlet/Views/IndexAD/Manage.cshtml", model);
            }
            return View();
        }

        /// <summary>
        /// 创建或修改
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Manager()
        {
            CommonService commonService = new CommonService();
            Dictionary<string, string> rsPic = new Dictionary<string, string>();
            
            short gender = short.Parse(Request["gender"].ToString());
            string picManId = Request["PictureManageId"];
            string position = Request["Position"];
            //加上修改的逻辑前的读取
            SWfsPictureManager model = new SWfsPictureManager();
            SWfsPictureManagerService service = new SWfsPictureManagerService();

            if (!string.IsNullOrEmpty(picManId) && !picManId.Equals("0"))
            {
                model = service.GetModel(picManId);
                if (null == model)
                {
                    return Json(new { reslut = "error", msg = "记录不存在" });
                }
            }
            model.BankName = string.Empty;
            model.BrandContent = string.Empty;
            model.DateBegin =Convert.ToDateTime(Request["DateBegin"]??"1900-1-1");
            model.DateEnd = Convert.ToDateTime(Request["DateEnd"] ?? "1900-1-1");
            model.ExpandPicFile = string.Empty;
            model.Gender = gender;
            model.InvitationCode = string.Empty;
            model.Keyword = string.Empty;
            model.LinkAddress = Request["LinkAddress"];
            model.OperatorUserId = PresentationHelper.GetPassport().UserName;
            model.PagePosition = (int)ADPosition.PagePosition;
            model.PictureName = Request["PictureName"];
            
            model.Status = 1;
            model.SubTitle = Request["SubTitle"];
            model.WebSite = 2;

            if (string.IsNullOrEmpty(picManId)||picManId.Equals("0")) //创建
            {
                model.Position = int.Parse(position);
                model.DateCreate = DateTime.Now;
                if (null != Request.Files["PicFile"] && Request.Files["PicFile"].ContentLength > 0)
                {
                    rsPic = commonService.PostImg(Request.Files["PicFile"], IndexAD.GetLimitContion(int.Parse(position)));
                    if (rsPic.Keys.Contains("error"))
                    {
                        return Json(new { reslut = "error", msg = rsPic["error"] });
                    }
                    if (rsPic.Keys.Contains("success"))
                    {
                        model.PictureFileNo = rsPic["success"];
                    }
                }
                try
                {
                    model.OperatorUserId = PresentationHelper.GetPassport().UserName;
                    service.Add(model);
                    return Json(new { reslut = "success", msg = "添加成功" });
                }
                catch (Exception e)
                {
                    return Json(new { reslut = "error", msg = e.Message });
                }
            }
            else //修改
            {
                
                if(!model.Position.Equals(int.Parse(position))) //说明修改了广告位置
                {
                   if (null==Request.Files["PicFile"]||Request.Files["PicFile"].ContentLength<=0)
                   {
                       return Json(new { reslut = "error", msg ="修改广告位置后请重新上传广告图"});
                   }
                }
                model.Position = int.Parse(position);
                if (null != Request.Files["PicFile"] && Request.Files["PicFile"].ContentLength > 0)
                {
                    rsPic = commonService.PostImg(Request.Files["PicFile"], IndexAD.GetLimitContion(int.Parse(position)));
                    if (rsPic.Keys.Contains("error"))
                    {
                        return Json(new { reslut = "error", msg = rsPic["error"] });
                    }
                    if (rsPic.Keys.Contains("success"))
                    {
                        model.PictureFileNo = rsPic["success"];
                    }
                }


                model.PictureManageId = int.Parse(picManId);
                bool rs = service.Update(model);
                return Json(new { reslut = rs ? "success" : "error", msg = rs ? "修改成功" : "修改失败" });
            }
        }
    }
}
