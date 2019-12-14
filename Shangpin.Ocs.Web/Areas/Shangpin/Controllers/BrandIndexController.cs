using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Shangpin.Ocs.Service.Common;
using Shangpin.Entity.Wfs;
using Shangpin.Ocs.Service.Shangpin;
using Shangpin.Ocs.Service;
using System.Text;
using Shangpin.Entity.Common;
using Shangpin.Ocs.Entity.Extenstion.ShangPin;

namespace Shangpin.Ocs.Web.Areas.Shangpin.Controllers
{
    public class BrandIndexController : Controller
    {

        #region 尚品品牌首页模板管理
        /// <summary>
        /// 尚品品牌首页模板管理
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            #region 运营广告位管理
            IList<SWfsBrandAdsInfo> list = SWfsBrandIndexService.GetInstance().GetList("", "0", "", "");
            SWfsBrandAdsInfo leftAd = new SWfsBrandAdsInfo();
            SWfsBrandAdsInfo rightAd = new SWfsBrandAdsInfo();
            if (list != null)
            {
                leftAd = list.Where(ad => ad.StartTime <= DateTime.Now && ad.Position == 1).OrderByDescending(a => a.StartTime).FirstOrDefault();
                rightAd = list.Where(ad => ad.StartTime <= DateTime.Now && ad.Position == 2).OrderByDescending(a => a.StartTime).FirstOrDefault();
            }
            ViewBag.LeftAd = leftAd;
            ViewBag.RightAd = rightAd;
            #endregion

            #region 热门品牌模块管理
            //SWfsBrandIndexService.GetInstance().GetBrandIndexDataList(2, 1, 128);
            RecordPage<BrandIndexM> hotBrandList = SWfsBrandIndexService.GetInstance().GetBrandIndexDataListNew(2, 1, 128);
            ViewBag.HotBrandList = hotBrandList;
            #endregion

            #region 官方旗舰店模块管理
            //SWfsBrandIndexService.GetInstance().GetBrandIndexDataList(1, 1, 12);
            RecordPage<BrandIndexM> flagList = SWfsBrandIndexService.GetInstance().GetBrandIndexDataListNew(1, 1, 12);
            ViewBag.FlagList = flagList;
            #endregion

            return View();
        }
        #endregion

        public ActionResult AdIndex(string adName = "", string sTime = "", string eTime = "", string position = "0", int pageIndex = 1)
        {
            int pageSize = int.Parse(AppSettingManager.AppSettings["ComonListPageNum"].ToString());
            ViewBag.CurrentPage = pageIndex;
            ViewBag.PageSize = pageSize;
            IList<SWfsBrandAdsInfo> list = SWfsBrandIndexService.GetInstance().GetList(adName, position, sTime, eTime);
            ViewBag.TotalCount = list.Count();
            list = list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();//默认每页显示20条数据
            ViewBag.AdList = list;
            ViewBag.AdName = adName ?? "";
            ViewBag.Position = position ?? "";
            ViewBag.StartTime = sTime ?? "";
            ViewBag.EndTime = eTime ?? "";
            ViewBag.PageIndex = pageIndex;
            ViewBag.CurrentCount = list.Count();
            return View();
        }

        public ActionResult Manage(string act, string position, int id = 0)
        {
            SWfsBrandAdsInfo model = new SWfsBrandAdsInfo();
            SWfsBrandIndexService service = SWfsBrandIndexService.GetInstance();
            ViewBag.Act = act;
            switch (act)
            {
                case "add"://新建
                    model.StartTime = Convert.ToDateTime("1900-1-1");
                    model.Position = Convert.ToInt16(position);
                    return View("/Areas/Shangpin/Views/BrandIndex/ManageAd.cshtml", model);
                case "edit"://修改
                    model = service.GetModel(id);
                    return View("/Areas/Shangpin/Views/BrandIndex/ManageAd.cshtml", model);
            }
            return View();
        }

        [HttpPost]
        public JsonResult ManagerAd()
        {
            string imgSize = AppSettingManager.AppSettings["BrandAdPic"].ToString();
            CommonService commonService = new CommonService();
            Dictionary<string, string> rsPic = new Dictionary<string, string>();
            int id = Convert.ToInt32(Request["ID"]);
            string position = Request["Position"];
            string picUrl = Request["PicUrl"];
            SWfsBrandAdsInfo model = new SWfsBrandAdsInfo();
            SWfsBrandIndexService service = SWfsBrandIndexService.GetInstance();
            if (id != 0)
            {
                model = service.GetModel(id);
                if (null == model)
                {
                    return Json(new { reslut = -1, msg = "记录不存在" });
                }
            }
            string startTime = Request["StartTime"];            
            model.AdName = Request["AdName"];
            if (!string.IsNullOrEmpty(picUrl) && !picUrl.StartsWith("http://"))
            {
                return Json(new { reslut = -1, msg = "图片链接地址格式不正确" });
            }
            else
            {
                model.PicUrl = picUrl;
            }
            model.Status = 0;
            model.UpdateDate = DateTime.Now;
            model.UpdateUserId = PresentationHelper.GetPassport().UserName;
            if (position == "2")
            {
                imgSize = AppSettingManager.AppSettings["BrandAdPic2"].ToString();
            }
            if (id == 0) //创建
            {
                SWfsBrandAdsInfo adInfo = service.GetByTime(startTime,position);
                if (adInfo != null)
                {
                    return Json(new { reslut = -1, msg = "此运营当前时间已存在数据！" }, "text/plain", Encoding.UTF8);
                }
                model.StartTime = Convert.ToDateTime(startTime);
                model.Position = Convert.ToInt16(position);
                if (null != Request.Files["PicFile"] && Request.Files["PicFile"].ContentLength > 0)
                {
                    rsPic = commonService.PostImg(Request.Files["PicFile"], imgSize, ".jpg");
                    if (rsPic.Keys.Contains("error"))
                    {
                        return Json(new { reslut = -1, msg = rsPic["error"] }, "text/plain", Encoding.UTF8);
                    }
                    if (rsPic.Keys.Contains("success"))
                    {
                        model.PicNo = rsPic["success"];
                    }
                }
                else
                {
                    return Json(new { reslut = -1, msg = "请选择图片" }, "text/plain", Encoding.UTF8);
                }
                try
                {
                    model.CreateUserId = PresentationHelper.GetPassport().UserName;
                    model.CreateDate = DateTime.Now;
                    service.Add(model);
                    return Json(new { reslut = 1, msg = "添加成功" }, "text/plain", Encoding.UTF8);
                }
                catch (Exception e)
                {
                    return Json(new { reslut = 1, msg = e.Message }, "text/plain", Encoding.UTF8);
                }
            }
            else //修改
            {
                if (model.StartTime != Convert.ToDateTime(startTime))
                {
                    SWfsBrandAdsInfo adInfo = service.GetByTime(startTime,position);
                    if (adInfo != null)
                    {
                        return Json(new { reslut = -1, msg = "此运营位当前时间已存在数据！" }, "text/plain", Encoding.UTF8);
                    }
                }
                if (model.Position.ToString() != position) //说明修改了广告位置
                {
                    if (null == Request.Files["PicFile"] || Request.Files["PicFile"].ContentLength <= 0)
                    {
                        return Json(new { reslut = -1, msg = "修改广告位置后请重新上传广告图" });
                    }
                }
                model.StartTime = Convert.ToDateTime(startTime);
                model.Position = Convert.ToInt16(position);
                if (null != Request.Files["PicFile"] && Request.Files["PicFile"].ContentLength > 0)
                {
                    rsPic = commonService.PostImg(Request.Files["PicFile"], imgSize, ".jpg");
                    if (rsPic.Keys.Contains("error"))
                    {
                        return Json(new { reslut = -1, msg = rsPic["error"] }, "text/plain", Encoding.UTF8);
                    }
                    if (rsPic.Keys.Contains("success"))
                    {
                        model.PicNo = rsPic["success"];
                    }
                }
                model.ID = id;
                bool rs = service.Update(model);
                return Json(new { reslut = rs ? 1 : -1, msg = rs ? "修改成功" : "修改失败" }, "text/plain", Encoding.UTF8);
            }
        }

        public JsonResult AdDelete(string id)
        {
            SWfsBrandIndexService service = SWfsBrandIndexService.GetInstance();
            try
            {
                service.Delete(id);
                return Json(new { result = 1, msg = "删除成功" });
            }
            catch (Exception ex)
            {
                return Json(new { result = ex, msg = "删除失败" });

            }
        }
    }
}
