using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Shangpin.Entity.Wfs;
using Shangpin.Ocs.Service.Outlet;
using Shangpin.Ocs.Service.Common;
using Shangpin.Ocs.Service;

namespace Shangpin.Ocs.Web.Areas.Outlet.Controllers
{
    [OCSAuthorization]
    public class MobileADController : BaseController
    {
        //
        // GET: /Outlet/MobileAD/

        public ActionResult Index(string channelNo, string KeyWord = "", string SortKey = "", string StatusKey = "", string startTime = "", string endTime = "", int pageIndex = 1)
        {
            int pageSize = Convert.ToInt32(AppSettingManager.AppSettings["ComonListPageNum"]);
            SWfsMobileAdService service = new SWfsMobileAdService();
            int count = 0;
            IList<SWfsMobileAd> list = service.GetMobileAdList(KeyWord, channelNo, SortKey, startTime, endTime, StatusKey, pageIndex, pageSize, out count);
            ViewBag.List = list;
            ViewBag.Count = count;
            ViewBag.CurrPageIndex = pageIndex;
            ViewBag.pageSize = pageSize;
            ViewBag.KeyWord = KeyWord;
            ViewBag.Sort = SortKey;
            ViewBag.DateBegin = startTime;
            ViewBag.DateEnd = endTime;
            ViewBag.Status = StatusKey;
            ViewBag.ChannelNo = channelNo;
            return View();
        }
        public ActionResult Create(string channelNO)
        {
            SWfsChannelService channelService = new SWfsChannelService();
            List<SWfsChannel> channellist = channelService.GetChannelAllList();
            ViewBag.ChannelList = channellist;
            ViewBag.ChannelNo = channelNO;
            return View();
        }
        [HttpPost]
        public ActionResult CreateMobileAD()
        {
            CommonService commonService = new CommonService();
            Dictionary<string, string> rsPic = new Dictionary<string, string>();
            SWfsMobileAd mobileAd = new SWfsMobileAd();
            SWfsMobileAdService service = new SWfsMobileAdService();
            string name = Request.Params["Name"];
            short status = Convert.ToInt16(Request.Params["Status"]);
            DateTime dateBegin = Convert.ToDateTime(Request.Params["DateBegin"]);
            DateTime dateEnd = Convert.ToDateTime(Request.Params["DateEnd"]);
            int sort = Convert.ToInt32(Request.Params["Sort"]);
            int showType = Convert.ToInt32(Request.Params["ShowType"]);
            string showUrl = Request.Params["ShowUrl"];
            string toChannel = Request.Params["channelList"];
            string channelNo = Request.Params["channelNo"];
            string createUserId = PresentationHelper.GetPassport().UserName;
            DateTime dateCreate = DateTime.Now;
            mobileAd.Name = name;
            mobileAd.Status = status;
            mobileAd.DateBegin = dateBegin;
            mobileAd.DateEnd = dateEnd;
            mobileAd.Sort = sort;
            mobileAd.ShowType = showType;
            if (showType == 3)
            {
                mobileAd.ShowUrl = showUrl;
            }
            else if (showType == 2)
            {
                mobileAd.ShowUrl = toChannel;
            }
            mobileAd.ChannelNo = channelNo;
            mobileAd.CreateUserId = createUserId;
            mobileAd.CreateDate = dateCreate;
            mobileAd.UpdateDate = dateCreate;
            mobileAd.UpdateUserId = createUserId;
            IList<SWfsMobileAd> list = service.GetMobileAdList(channelNo);
            list = list.Where(l => l.DateBegin == dateBegin && l.DateEnd == dateEnd & l.Sort == sort).ToList();
            if (status == 1)
            {
                list = list.Where(m => m.Status == 1).ToList();
            }
            if (list.Count > 0)
            {
                return Json(new { reslut = "0", message = "此位置已经存在已开启的广告！" });
            }
            else
            {
                mobileAd.DateBegin = dateBegin;
                mobileAd.DateEnd = dateEnd;
                mobileAd.Sort = sort;
            }
            if (null != Request.Files["AdPic"] && Request.Files["AdPic"].ContentLength > 0)
            {
                string picSize = AppSettingManager.AppSettings["MobileAdPic"].ToString();
                string picType = AppSettingManager.AppSettings["MobileAdPicType"].ToString();
                rsPic = commonService.PostImg(Request.Files["AdPic"], picSize, picType);
                if (rsPic.Keys.Contains("error"))
                {
                    return Json(new { result = "0", message = rsPic["error"] });
                }
                if (rsPic.Keys.Contains("success"))
                {
                    mobileAd.AdPic = rsPic["success"];
                }
            }
            try
            {
                service.InsertMobileAd(mobileAd);
            }
            catch (Exception ex)
            {
                return Json(new { result = "0", message = ex.Message });
            }
            return Json(new { result = "1", message = "添加成功！" });
        }


        [HttpGet]
        public ActionResult Edit(string ID)
        {
            SWfsMobileAdService mobileAd = new SWfsMobileAdService();
            SWfsMobileAd model = mobileAd.GetMobileAdInfo(Convert.ToInt32(ID));
            SWfsChannelService channelService = new SWfsChannelService();
            List<SWfsChannel> channellist = channelService.GetChannelAllList();
            ViewBag.ChannelList = channellist;
            ViewBag.ShowType = model.ShowType;
            return View(model);
        }

        [HttpPost]
        public ActionResult EditMobileAD()
        {
            int id = Convert.ToInt32(Request.Form["ID"]);
            CommonService commonService = new CommonService();
            Dictionary<string, string> rsPic = new Dictionary<string, string>();
            SWfsMobileAd mobileAd = new SWfsMobileAd();
            SWfsMobileAdService service = new SWfsMobileAdService();
            mobileAd = service.GetMobileAdInfo(id);
            string name = Request.Params["Name"];
            short status = Convert.ToInt16(Request.Params["Status"]);
            DateTime dateBegin = Convert.ToDateTime(Request.Params["DateBegin"]);
            DateTime dateEnd = Convert.ToDateTime(Request.Params["DateEnd"]);
            int sort = Convert.ToInt32(Request.Params["Sort"]);
            int showType = Convert.ToInt32(Request.Params["ShowType"]);
            string showUrl = Request.Params["ShowUrl"];
            string toChannel = Request.Params["channelList"];
            string channelNo = Request.Params["channelNo"];
            string updateUserId = PresentationHelper.GetPassport().UserName;
            DateTime dateUpdate = DateTime.Now;
            mobileAd.Name = name;
            mobileAd.ShowType = showType;
            if (showType == 3)
            {
                mobileAd.ShowUrl = showUrl;
            }
            else if (showType == 2)
            {
                mobileAd.ShowUrl = toChannel;
            }
            mobileAd.ChannelNo = channelNo;
            mobileAd.UpdateDate = dateUpdate;
            mobileAd.UpdateUserId = updateUserId;
            if (mobileAd.Sort != sort || mobileAd.DateBegin != dateBegin || mobileAd.DateEnd != dateEnd || status != mobileAd.Status)
            {
                IList<SWfsMobileAd> list = service.GetMobileAdList(channelNo);
                if (mobileAd.Sort != sort || mobileAd.DateBegin != dateBegin || mobileAd.DateEnd != dateEnd)
                {
                    list = list.Where(l => l.DateBegin == dateBegin && l.DateEnd == dateEnd & l.Sort == sort).ToList();
                    if (list.Count > 0)
                    {
                        return Json(new { reslut = "0", message = "此位置已经存在已开启的广告！" });
                    }
                    else
                    {
                        mobileAd.DateBegin = dateBegin;
                        mobileAd.DateEnd = dateEnd;
                        mobileAd.Sort = sort;

                    }
                }
                if (status != mobileAd.Status)
                {
                    if (status == 1)
                    {
                        list = list.Where(s => s.Status == 1).ToList();
                        if (list.Count > 0)
                        {
                            return Json(new { reslut = "0", message = "此位置已经存在已开启的广告！" });
                        }
                        else
                        {
                            mobileAd.Status = status;
                        }
                    }
                    else
                    {
                        mobileAd.Status = status;
                    }
                }
            }
            if (null != Request.Files["AdPic"] && Request.Files["AdPic"].ContentLength > 0)
            {
                string picSize = AppSettingManager.AppSettings["MobileAdPic"].ToString();
                string picType = AppSettingManager.AppSettings["MobileAdPicType"].ToString();
                rsPic = commonService.PostImg(Request.Files["AdPic"], picSize, picType);
                if (rsPic.Keys.Contains("error"))
                {
                    return Json(new { reslut = "0", message = rsPic["error"] });
                }
                if (rsPic.Keys.Contains("success"))
                {
                    mobileAd.AdPic = rsPic["success"];
                }
            }
            mobileAd.ID = id;
            if (service.Update(mobileAd))
            {
                return Json(new { reslut = "1", message = "修改成功！" });
            }
            else
            {
                return Json(new { reslut = "0", message = "修改失败！" });
            }
        }

        [HttpPost]
        public ActionResult DeleteMobileAd(string IdStrs)
        {
            string[] ids = IdStrs.TrimEnd(',').Split(',');
            SWfsMobileAdService service = new SWfsMobileAdService();
            foreach (string id in ids)
            {
                service.Delete(Convert.ToInt32(id));
            }
            return Json(new { result = "1", message = "删除成功！" });
        }

        public ActionResult AjaxDelete(string id)
        {
            SWfsMobileAdService service = new SWfsMobileAdService();
            try
            {
                int i = service.Delete(Convert.ToInt32(id));
                if (i > 0)
                {
                    return Json(new { result = "1", message = "删除成功！" });
                }
                else
                {
                    return Json(new { result = "0", message = "删除失败！" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { result = "0", message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult MobileADStatusModify(string id, string status, string channel)
        {
            SWfsMobileAdService service = new SWfsMobileAdService();
            string userid = PresentationHelper.GetPassport().UserName;
            DateTime date = DateTime.Now;
            string s = status == "0" ? "1" : "0";
            if (s == "1")
            {
                IList<SWfsMobileAd> list = service.GetListBySort(s, channel);
                if (list.Count > 0)
                {
                    return Json(new { reslut = "0", message = "此位置已经存在已开启的广告！" });
                }
            }
            try
            {
                bool flag = service.MobileAdModify(id, s, userid, date);
                if (flag)
                {
                    return Json(new { result = "1", message = "状态修改成功！" });
                }
                else
                {
                    return Json(new { result = "0", message = "状态修改失败！" });
                }
            }
            catch (Exception e)
            {
                return Json(new { result = "0", message = "状态修改失败！" });
            }
        }

        public ActionResult AjaxMobileAdSort(string id, string sort, string channelNo)
        {
            SWfsMobileAdService service = new SWfsMobileAdService();
            IList<SWfsMobileAd> list = service.GetListBySort(sort, channelNo);
            if (list.Count > 0)
            {
                return Json(new { result = "0", message = "此位置已经存在已开启广告！" });
            }
            else
            {
                bool flag = service.UpdateSort(id, Convert.ToInt32(sort));
                if (flag)
                {
                    return Json(new { result = "1", message = "保存成功！" });
                }
                else
                {
                    return Json(new { result = "0", message = "保存失败！" });
                }
            }
        }
    }
}