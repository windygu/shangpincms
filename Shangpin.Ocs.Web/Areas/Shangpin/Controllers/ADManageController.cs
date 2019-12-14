using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Shangpin.Ocs.Service.Common;
using Shangpin.Entity.Wfs;
using Shangpin.Ocs.Service.Outlet;
using Shangpin.Ocs.Service.Shangpin;
using Shangpin.Ocs.Service;

namespace Shangpin.Ocs.Web.Areas.Shangpin.Controllers
{
    public class ADManageController : Controller
    {
        #region AD列表
        /// <summary>
        /// AD列表
        /// </summary>
        /// <param name="title"></param>
        /// <param name="dateTime"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public ActionResult Index(int pageIndex = 1)
        {
            int pageSize = int.Parse(AppSettingManager.AppSettings["ComonListPageNum"].ToString());
            ViewBag.CurPage = pageIndex;
            ViewBag.PageSize = pageSize;
            string title = Request["title"];
            string showStatus = Request["ShowStatus"];
            string dateBegin = Request["DateBegin"];
            string dateEnd = Request["DateEnd"];
            string positionId = Request["PositionId"];
            string positionParentId = Request["PositionParentId"];
            string menPositionIds = "2,20001,20002,20003,20004,20005,20006,20007,20008";
            string womenPositionIds = "1,10001,10002,10003,10004,10005,10006,10007,10008";
            List<string> positionIds = new List<string>() { };
            if (positionId == "1" && positionParentId == "0")
            {
                positionIds = womenPositionIds.Split(',').ToList();                
            }
            else if (positionId == "2" && positionParentId == "0")
            {
                positionIds = menPositionIds.Split(',').ToList();
            }
            else if(!string.IsNullOrEmpty(positionId))
            {
                positionIds.Add(positionId);
            }
            Dictionary<string, object> dicParam = new Dictionary<string, object>();
            if (string.IsNullOrEmpty(title))
            {
                dicParam.Add("Title", "");
            }
            else
            {
                dicParam.Add("Title", title.Trim());

            }
            if (string.IsNullOrEmpty(showStatus) || showStatus == "0")
            {
                dicParam.Add("ShowStatus", "");
            }
            else
            {
                dicParam.Add("ShowStatus", int.Parse(Request["ShowStatus"]));
            }
            if (string.IsNullOrEmpty(dateBegin))
            {
                dicParam.Add("DateBegin", "");
            }
            else
            {
                dicParam.Add("DateBegin", Convert.ToDateTime(Request["DateBegin"]));

            }
            if (string.IsNullOrEmpty(dateEnd))
            {
                dicParam.Add("DateEnd", "");
            }
            else
            {
                dicParam.Add("DateEnd", Convert.ToDateTime(Request["DateEnd"]));

            }

            if (positionIds.Count>0)
            {
                dicParam.Add("PositionId", positionIds);               
            }
            else
            {
                dicParam.Add("PositionId", "");                
            }
            if (string.IsNullOrEmpty(positionParentId) || positionParentId == "0")
            {
                dicParam.Add("PositionParentId", "");
            }
            else
            {
                dicParam.Add("PositionParentId", int.Parse(Request["PositionParentId"]));
            }
            dicParam.Add("SiteNo", "1");
            IList<WfsCmsContent> list = new ADService().GetADList(dicParam,positionIds);
            ViewBag.Count = list.Count();
            list = list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();//默认每页显示20条数据
            ViewBag.List = list;
            ViewBag.Titles = title ?? "";
            ViewBag.DateBegin = dateBegin ?? "";
            ViewBag.DateEnd = dateEnd ?? "";
            ViewBag.ShowStatus = showStatus;
            ViewBag.PositionId = positionId;
            ViewBag.PositionParentId = positionParentId;

            return View();
        }
        #endregion

        #region 添加AD视图及操作
        public ActionResult CreateAd()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateAdManage()
        {

            string title = Request["title"];
            string href = Request["Href"];
            string showStatus = Request["ShowStatus"].Equals("0") ? "2" : Request["ShowStatus"];

            #region 时间判断 可以为空 如果不为空必须大于当前时间
            if (Request["ContentType"] == "9999")
            {
                if (!string.IsNullOrEmpty(Request["CountDownTime"]))
                {
                    DateTime dt = Convert.ToDateTime(Request["CountDownTime"]);
                    if (dt < DateTime.Now)
                    {
                        return Json(new { reslut = "error", msg = "倒计时时间不能小于当前时间" });
                    }
                }
            }
            #endregion

            #region 对象赋值
            DateTime CountDownTime = string.IsNullOrEmpty(Request["CountDownTime"]) ? Convert.ToDateTime("1900-1-1") : Convert.ToDateTime(Request["CountDownTime"]);
            if (Request["CountDownTime"] == "0001-1-1 0:00:00")
            {
                CountDownTime = Convert.ToDateTime("1900-1-1");
            }
            int contentType = (Request["ContentType"] == null || Convert.ToInt16(Request["ContentType"]) == 0) ? 2 : Convert.ToInt16(Request["ContentType"]);

            WfsCmsContentService service = new WfsCmsContentService();
            WfsCmsContent model = new WfsCmsContent();
            model.PositionId = int.Parse(Request["PositionId"]);
            model.PositionParentId = int.Parse(Request["PositionParentId"]);
            model.CmsContentCategoryId = 4;
            model.PublishTime = DateTime.Now;
            model.SiteNo = 1;
            model.CmsContentNo = new CommonService().GetNextCounterId("CmsContentNo").ToString("00000");
            model.OperateUserId = PresentationHelper.GetPassport().UserName;
            model.Href = href;
            model.ShowStatus = Convert.ToInt16(showStatus);
            model.Title = title;
            //add 20130906 Alvin DateBegin字段记录活动倒计时时间
            model.CountdownTime = CountDownTime;
            model.ContentType = (short)contentType;
            model.DateBegin = Convert.ToDateTime(Request["DateBegin"]);
            model.DateEnd = Convert.ToDateTime(Request["DateEnd"]);
            //图片地址 不传为空
            model.ContentText = "";
            #endregion

            #region 验证图片
            if (null != Request.Files["PicFile"] && Request.Files["PicFile"].ContentLength > 0)
            {
                CommonService commonService = new CommonService();
                Dictionary<string, string> rsPic = commonService.PostImg(Request.Files["PicFile"], "width:960,Height:0,Length:300");
                if (rsPic.Keys.Contains("error"))
                {
                    return Json(new { reslut = "error", msg = rsPic["error"] });
                }
                if (rsPic.Keys.Contains("success"))
                {
                    model.ContentText = rsPic["success"];
                }
            }
            #endregion

            #region 添加操作
            try
            {
                service.Add(model);
                return Json(new { reslut = "success", msg = "添加成功" });
            }
            catch (Exception e)
            {
                return Json(new { reslut = "error", msg = e.Message });
            }
            #endregion


        }
        #endregion

        #region 编辑视图及操作
        public ActionResult EditAd()
        {
            string tmpCmsContentNo = Request["id"];
            WfsCmsContentService service = new WfsCmsContentService();
            WfsCmsContent modle = new WfsCmsContent();
            modle = service.GetModel(tmpCmsContentNo);
            return View(modle);
        }
        [HttpPost]
        public ActionResult EditAdManage()
        {
            WfsCmsContent obj = new WfsCmsContent();
            string tmpCmsContentNo = Request["CmsContentNo"];
            string title = Request["title"];
            string href = Request["Href"];
            string showStatus = Request["ShowStatus"].Equals("0") ? "2" : Request["ShowStatus"];

            #region 时间判断 可以为空 如果不为空必须大于当前时间
            if (Request["ContentType"] != "2")
            {
                if (!string.IsNullOrEmpty(Request["CountDownTime"]))
                {
                    DateTime dt = Convert.ToDateTime(Request["CountDownTime"]);
                    if (dt < DateTime.Now)
                    {
                        return Json(new { reslut = "error", msg = "倒计时时间不能小于当前时间" });
                    }
                }
            }

            
            #endregion

            #region 对象赋值
            DateTime countDownTime = (Request["CountDownTime"] == null || Request["CountDownTime"].ToString().Trim() == "") ? Convert.ToDateTime("1900-1-1") : Convert.ToDateTime(Request["CountDownTime"]);
            if (Request["CountDownTime"] == "0001-1-1 0:00:00")
            {
                countDownTime = Convert.ToDateTime("1900-1-1");
            }
            int contentType = (Request["ContentType"] == null || Convert.ToInt16(Request["ContentType"]) == 0) ? 2 : Convert.ToInt16(Request["ContentType"]);
            WfsCmsContentService service = new WfsCmsContentService();
            WfsCmsContent model = new WfsCmsContent();
            model = new WfsCmsContentService().GetModel(tmpCmsContentNo);
            model.PositionId = int.Parse(Request["PositionId"]);
            model.PositionParentId = int.Parse(Request["PositionParentId"]);
            model.OperateUserId = PresentationHelper.GetPassport().UserName;
            model.Href = href;
            model.ShowStatus = Convert.ToInt16(showStatus);
            model.Title = title;
            model.CountdownTime = countDownTime;
            model.ContentType = (short)contentType;
            model.DateBegin = Convert.ToDateTime(Request["DateBegin"]);
            model.DateEnd = Convert.ToDateTime(Request["DateEnd"]);
            #endregion

            #region 图片判断
            if (null != Request.Files["PicFile"] && Request.Files["PicFile"].ContentLength > 0)
            {
                CommonService commonService = new CommonService();
                Dictionary<string, string> rsPic = commonService.PostImg(Request.Files["PicFile"], "width:960,Height:0,Length:300");
                if (rsPic.Keys.Contains("error"))
                {
                    return Json(new { reslut = "error", msg = rsPic["error"] });
                }
                if (rsPic.Keys.Contains("success"))
                {
                    model.ContentText = rsPic["success"];
                }
            }

            #endregion

            #region 修改操作
            try
            {
                service.Update(model);
                return Json(new { reslut = "success", msg = "修改成功" });
            }
            catch (Exception e)
            {
                return Json(new { reslut = "error", msg = e.Message });
            }
            #endregion


        }
        #endregion

        #region 删除操作，编辑状态操作
        public ActionResult Manage(string id, string act, int value = 1)
        {
            WfsCmsContentService service = new WfsCmsContentService();
            WfsCmsContent modle = new WfsCmsContent();
            switch (act)
            {
                case "del":
                    service.Del(id);
                    return Redirect("/shangpin/admanage/index.html");

                case "show"://1 显示 2不显示
                    service.Update(id, value);
                    return Redirect("/shangpin/admanage/index.html");
            }
            return View();
        }
        #endregion

    }
}
