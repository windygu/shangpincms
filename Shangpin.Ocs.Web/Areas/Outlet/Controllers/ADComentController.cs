using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Shangpin.Ocs.Service.Outlet;
using Shangpin.Entity.Wfs;
using Shangpin.Ocs.Service.Common;
using Shangpin.Ocs.Service;

namespace Shangpin.Ocs.Web.Areas.Outlet.Controllers
{
    /// <summary>
    /// 奥莱公共管理 
    /// </summary>
    public class ADComentController : Controller  
    {
       //公告标题 	所属网站 	创建时间↑↓ 	公告状态 	操作
        public ActionResult Index(string title, string dateTime, int pageIndex = 1)
        {
            int pageSize = int.Parse(AppSettingManager.AppSettings["ComonListPageNum"].ToString());
            ViewBag.CurPage = pageIndex;
            ViewBag.PageSize = pageSize;

            IList<WfsCmsContent> list = new WfsCmsContentService().GetList(title, dateTime);
            ViewBag.Count = list.Count();
            list = list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();//默认每页显示20条数据
            ViewBag.List = list;
            ViewBag.Name = title ?? "";
            ViewBag.Time = dateTime ?? "";
            return View();
        }

        /// <summary>
        /// 新建和修改及操作--展示及操作
        /// </summary>
        /// <param name="id"></param>
        /// <param name="action"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ActionResult Manage(string id, string act, int value = 1)
        {
            WfsCmsContentService service = new WfsCmsContentService();
            WfsCmsContent modle = new WfsCmsContent();
            switch (act)
            {
                case "create"://新建
                    modle.CmsContentNo = "0";
                    return View("/Areas/Outlet/Views/ADComent/Manage.cshtml", modle);

                case "edit"://修改
                    modle = service.GetModel(id);
                    return View("/Areas/Outlet/Views/ADComent/Manage.cshtml", modle);

                case "del":
                    service.Del(id);
                    return Redirect("/outlet/adcoment/index" + CommonService.GetTimeStamp("?"));

                case "show"://1 显示 2不显示
                    service.Update(id, value);
                    return Redirect("/outlet/adcoment/index");
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
            string tmpCmsContentNo = Request["CmsContentNo"];
            string title = Request["title"];
            string href = Request["Href"];
            string showStatus = Request["ShowStatus"].Equals("0")?"2":Request["ShowStatus"];

            //add 20130906 Alvin
            DateTime dateEnd = (Request["DateBegin"] == null || Request["DateBegin"].ToString().Trim()=="") ? Convert.ToDateTime("1900-1-1") : Convert.ToDateTime(Request["DateBegin"]);
            int contentType = (Request["ContentType"] == null || Convert.ToInt16(Request["ContentType"]) == 0) ? 2 : Convert.ToInt16(Request["ContentType"]);

            WfsCmsContentService service = new WfsCmsContentService();
            WfsCmsContent model = new WfsCmsContent();

            if (!string.IsNullOrEmpty(tmpCmsContentNo) && !tmpCmsContentNo.Equals("0")) //修改
            {
                model = new WfsCmsContentService().GetModel(tmpCmsContentNo);
            }
            else //新增
            {
                model.CmsContentCategoryId = 4;
                
                //delete 20130906 Alvin
                //model.ContentType = 2;
                //model.DateBegin = Convert.ToDateTime("1900-1-1");//暂时未用

                model.DateEnd = Convert.ToDateTime("1900-1-1");//暂时未用
                model.PublishTime = DateTime.Now;
                model.SiteNo = 2;
                model.PublishTime = DateTime.Now;
                model.CmsContentNo = new CommonService().GetNextCounterId("CmsContentNo").ToString("00000");
                model.ContentText = "";
            }
            model.OperateUserId = PresentationHelper.GetPassport().UserName;
            model.Href = href;
            model.ShowStatus = Convert.ToInt16(showStatus);
            model.Title = title;

            //add 20130906 Alvin DateBegin字段记录活动倒计时时间
            model.DateBegin = dateEnd;
            model.ContentType =(short)contentType;
            model.CountdownTime = Convert.ToDateTime("1900-1-1");

            if (null != Request.Files["PicFile"] && Request.Files["PicFile"].ContentLength > 0)
            {
                CommonService commonService = new CommonService();
                Dictionary<string, string> rsPic = commonService.PostImg(Request.Files["PicFile"], "width:980,Height:0,Length:300");
                if (rsPic.Keys.Contains("error"))
                {
                    return Json(new { reslut = "error", msg = rsPic["error"] });
                }
                if (rsPic.Keys.Contains("success"))
                {
                    model.ContentText = rsPic["success"];
                }
            }
            if (!string.IsNullOrEmpty(tmpCmsContentNo) && !tmpCmsContentNo.Equals("0")) //修改
            {
                try
                {
                    service.Update(model);
                    return Json(new { reslut = "success", msg = "修改成功" });
                }
                catch (Exception e)
                {
                    return Json(new { reslut = "error", msg = e.Message });
                }
            }
            else
            {
                try
                {
                    service.Add(model);
                    return Json(new { reslut = "success", msg = "添加成功" });
                }
                catch (Exception e)
                {
                    return Json(new { reslut = "error", msg = e.Message });
                }
            }
        }
    }
}
