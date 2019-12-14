using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Shangpin.Ocs.Service.Shangpin;
using Shangpin.Entity.Wfs;
using Shangpin.Ocs.Entity.Extenstion.ShangPin;
using Shangpin.Ocs.Service;
using Shangpin.Ocs.Service.Common;
using System.IO;
using Shangpin.Entity.ComBeziPic;
//using Shangpin.Ocs.Service.Outlet;
using Shangpin.Framework.Common;
using Shangpin.Ocs.Entity.Extenstion;
//using Shangpin.Ocs.Entity.Extenstion.Outlet;
using Shangpin.Framework.WebMvc;
using Shangpin.Entity.Common;
using System.Text;
using Shangpin.Ocs.Entity.Extenstion.Outlet;
using Shangpin.Entity.User;
using Shangpin.Framework.Common.Cache;
using System.Web.Script.Serialization;
using Shangpin.Ocs.Service.Outlet;
using Shangpin.Framework.Configuration;
using System.Web.UI;
using Shangpin.Ocs.Entity.Extenstion.Login;



namespace Shangpin.Ocs.Web.Areas.Shangpin.Controllers
{
    [OCSAuthorization]
    public class HomePageController : Controller
    {
        private Dictionary<string, string> rsPic = new Dictionary<string, string>();

        /// <summary>
        /// 首页功能页面
        /// </summary>
        public HomePageController()
        {

        }

        #region 无用的东西，我现在还不想把你怎么着。
        /// <summary>
        /// 首页展现页面   该方法已弃用
        /// </summary>
        public ActionResult Index()
        {
            SWfsPictureManagerService picService = new SWfsPictureManagerService();
            //ProductInfo productSingle = new ProductInfo();
            int total = 0;
            int gender = Rq.GetIntQueryString("gender");
            ViewBag.Banner = picService.GetWomenBannerNew(gender);//头图Banner
            ViewBag.BigPagePosition = picService.GetBigPagePositionNew(gender);//大运营位置图片
            ViewBag.SmallPagePosition = picService.GetSmallPagePositionNew(gender);//小运营位置图片
            ViewBag.ButtonPagePosition = picService.GetButtonPagePositionNew(gender);//底部运营位置图片 
            ViewBag.RecommendBrandList = picService.GetHomeRecommendBrand(gender == 0 ? "A01" : "A02", "", Request.QueryString["startTime"], Request.QueryString["endTime"], 1, 100, out total);
            //SWfsHomeSecKill secKillSingle = picService.GetSWfsHomeSecKillSingle(gender);  //秒杀位置图
            //if (secKillSingle != null)
            //{
            //    productSingle = picService.GetSWfsHomeSecKillByProductNoSingle(secKillSingle.ProductNo);
            //}
            //ViewBag.HomeSecKillSingle = secKillSingle;
            //ViewBag.HomeSecKillByProductNo = productSingle;
            ViewBag.Gender = gender;
            return View();
        }
        #endregion

        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public ActionResult HomeManager()
        {
            SWfsOperationPictureService picService = new SWfsOperationPictureService();
            SWfsBrandWallService brandWallService = new SWfsBrandWallService();
            ViewBag.Banner = picService.GetIndexSwitchPictures("index", "SwitchPicture");//头图Banner
            ViewBag.BottomBanner = picService.GetIndexOperationPictureBottom("index", "bottomBanner");//得到全部底
            ViewBag.MiddleBanner = picService.GetIndexOperationPictureBottom("index", "middleBanner");//得到中部三个广告
            ViewBag.Banner70IsClose = "0";
            ViewBag.Banner70 = picService.GetIndexOperationPicture("index", "BannerADHeight70");//横幅运营位
            ViewBag.Banner70IsClose = "1";
            ViewBag.RecommendBrand = brandWallService.GetHomePageRecommendBrand("index", "");

            ViewBag.BannerImgMap = picService.GetIndexSwitchPicturesImgMap();//热点图Banner A.H
            return View();
        }


        #region  横幅广告状态切换
        public JsonResult IndexBannerADStatus(string PageNo, string value)
        {
            try
            {
                if (!string.IsNullOrEmpty(PageNo))
                    new SWfsProductCommentService().UpdateGlobalConfig(PageNo, value);
            }
            catch (Exception)
            {
                return Json(new { status = -1 }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);
        }
        public bool GetChannelStatus(string PageNo)
        {
            try
            {
                SWfsGlobalConfig config = new SWfsGlobalConfig();
                if (!string.IsNullOrEmpty(PageNo))
                {
                    config = new SWfsProductCommentService().GetGlobalConfigByFNo(PageNo) ?? new SWfsGlobalConfig { ConfigtName = "", ConfigValue = "" };
                }
                if (config!=null&&config.ConfigValue == "1") 
                    return true;
                return false;
            }
            catch (Exception)
            {
                return false;
            }

        }
        #endregion


        #region 首页轮播图

        #region 首页轮播图管理列表
        /// <summary>
        /// 首页轮播图展现页面lxy20140313 
        /// </summary>
        [ValidateInput(false)]
        public ActionResult IndexList()
        {
            PaginationInfoModel pageinfo = new PaginationInfoModel();
            //int gender = Rq.GetIntQueryString("gender");
            string keyWord = Rq.GetStringQueryString("keyword");
            string status = Rq.GetStringQueryString("status");
            string startTime = Rq.GetStringQueryString("StartTime");
            string endTime = Rq.GetStringQueryString("EndTime");
            int currentPage = Rq.GetIntQueryString("pageindex");
            int pictureIndex = Rq.GetIntQueryString("PagePosition");
            string startTimeValue = string.Empty, endTimeValue = string.Empty;
            #region 条件筛选
            startTimeValue = string.IsNullOrEmpty(startTime) ? "" : startTime + " 00:00:00";
            endTimeValue = string.IsNullOrEmpty(endTime) ? "" : endTime + " 23:59:59";
            pageinfo.CurrentPage = currentPage;
            pageinfo.Condition += pictureIndex > 0 ? "PictureIndex=" + pictureIndex : "PictureIndex in(1,2,3,4,5,6,7)  ";
            pageinfo.Condition += (string.IsNullOrEmpty(keyWord) || keyWord == "名称") ? "" : " and (PictureName like '%" + keyWord + "%')";          
            pageinfo.Condition += " and WebSiteNo='shangpin.com' and PageNo='index' and PagePositionNo='SwitchPicture'";           
            pageinfo.Condition += (string.IsNullOrEmpty(status)) ? "" : "and Status =" + status + " ";
            pageinfo.Condition += " and DataState=1";
            pageinfo.Condition += (string.IsNullOrEmpty(startTimeValue)) ? "" : "  and DateBegin >='" + startTimeValue + "'";
            pageinfo.Condition += (string.IsNullOrEmpty(endTimeValue)) ? "" : "  and DateBegin <='" + endTimeValue + "'";
            #endregion

            IList<SWfsOperationPicture> picManagerList = new SWfsOperationPictureService().GetSWfsPictureManagerListTopSwitchV(ref pageinfo);
            ViewBag.PictureManagerList = picManagerList;
            ViewBag.PaginationInfoSingel = pageinfo;
            ViewBag.KeyWord = keyWord;
            ViewBag.Status = status;
            ViewBag.StartTime = startTime;
            ViewBag.EndTime = endTime;
            ViewBag.PagePosition = pictureIndex.ToString();
            //ViewBag.GenderValue = gender.ToString();
            //ViewBag.PagePosition = pictureIndex.ToString();
            //ViewBag.IsMap = isMap;           
            return View();
        }
        #endregion

        #region 添加首页轮播图 加载视图
        /// <summary>
        /// 添加首页轮播图
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateTopSwitch()
        {
            return View();
        }
        #endregion

        #region 修改位置图状态 删除，开启，关闭 首页共用此方法（轮播图，大小运营位置图）
        /// <summary>
        /// 修改位置图状态 删除，开启，关闭 首页共用此方法（轮播图，大小运营位置图）
        /// </summary>
        /// <param name="subjectNo"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PictureManagerStatusModify(string SubjectNo, string status, string DelectStatus)
        {
            bool rs = false;
            string messageStr = string.Empty;
            SWfsOperationPictureService service = new SWfsOperationPictureService();
            SWfsOperationPicture model = service.GetModel(SubjectNo);

            #region 查询是否每一帧是否有一条数据
            //short pictureIndex = model.PictureIndex;
            //if (model.PagePositionNo == "SwitchPicture")
            //{
            //    pictureIndex = 0;
            //}
            //var resultList = service.GetSWfsOperationPictureSwitchCondition(model.PageNo, model.PagePositionNo, pictureIndex);
            #endregion

            if (model != null)
            {
                //if (resultList != null && resultList.Count() <= 1 && DelectStatus!="0")
                //{
                //    messageStr = "必须保留一个";
                //    rs = true;
                //}
                //else
                //{
                if (DelectStatus != "0")
                {
                    int delcount = service.DelSwitchPicture(SubjectNo);
                    if (delcount > 0)
                    {
                        messageStr = "删除成功";
                        rs = true;
                    }
                }
                else
                {
                    model.Status = int.Parse(status);
                    rs = service.Update(model);
                    messageStr = "状态修改成功";
                }
                //}
            }
            try
            {
                if (rs)
                {
                    EnyimMemcachedClient.Instance.Remove("ComBeziWfs_SWfsOperationPicture_GetSWfsOperationPicture_GetIndexTopSwitchPic");
                    return Json(new { result = "1", message = messageStr });
                }
                else
                { return Json(new { result = "0", message = "操作失败！" }); }
            }
            catch (Exception ex)
            {
                return Json(new { result = "0", message = "操作失败！" });
            }
        }
        #endregion

        #region 添加轮播图，更新轮播图
        /// <summary>
        /// 添加首页轮播图位置 更新轮播图
        /// </summary>
        /// <returns></returns>      
        [HttpPost]
        public ActionResult CreateTopSwitchNew()
        {
            SWfsOperationPicture newobj = new SWfsOperationPicture();
            string pictureName = Rq.GetStringForm("SubjectName");
            string Status = Rq.GetStringForm("Status");
            string PagePosition = Rq.GetStringForm("PagePosition");
            string linkAddress = Rq.GetStringForm("HotTwoUrl");
            string DateBegin = Rq.GetStringForm("DateBegin");
            string relationGender = Rq.GetStringForm("relationGender");
            string pictureManageEdit = Rq.GetStringForm("PictureManageEdit");
            string pictureManageId = Rq.GetStringForm("PictureManageId");
            string PictureFileNo = string.Empty;
            #region 获取上传图片参数并且判断
            if (Request.Files["FlapHotTwo"] != null && Request.Files["FlapHotTwo"].ContentLength > 0)
            {
                string picSizeStr = "width:{0},Height:{1},Length:500";
                if (PagePosition == "4")//首页背景广告图片
                { picSizeStr = string.Format(picSizeStr, 0, 0); }
                else
                {//三个普通轮播图
                    picSizeStr = string.Format(picSizeStr, 960, 470);
                }
                PictureFileNo = SaveImg(Request.Files["FlapHotTwo"], picSizeStr);
                if (rsPic.Keys.Contains("error"))
                {
                    return Json(new { result = "-1", message = "图片不合法" + rsPic["error"] + "" }, "text/html");
                }
            }
            newobj = new SWfsOperationPictureService().GetModel(pictureManageId);
            if (newobj == null)
            { newobj = new SWfsOperationPicture(); newobj.DateCreate = DateTime.Now; }
            else
            {
                if (string.IsNullOrEmpty(PictureFileNo))
                {
                    PictureFileNo = newobj.PictureFileNo;
                }
            }
            newobj.PictureFileNo = PictureFileNo;
            newobj.PictureName = pictureName;
            if (!string.IsNullOrEmpty(linkAddress)) { if (linkAddress.ToLower() == "http://") { linkAddress = ""; } }
            newobj.LinkAddress = linkAddress;
            newobj.WebSiteNo = "shangpin.com";
            newobj.PageNo = "index";
            newobj.PagePositionNo = "SwitchPicture";
            newobj.PagePositionName = "首页轮播图";
            newobj.PictureIndex = Int16.Parse(PagePosition);
            newobj.Status = int.Parse(Status);
            newobj.SortId = 1;
            newobj.OperatorUserId = PresentationHelper.GetPassport().UserName;
            newobj.DateBegin = string.IsNullOrEmpty(DateBegin) ? DateTime.Now : DateTime.Parse(DateBegin);
            newobj.DateEnd = DateTime.MaxValue;
            newobj.DataState = 1;
            if (newobj.PictureManageId > 0)
            {
                new SWfsOperationPictureService().Update(newobj);
            }
            else
            {
                new SWfsOperationPictureService().Add(newobj);
            }
            EnyimMemcachedClient.Instance.Remove("ComBeziWfs_SWfsOperationPicture_GetSWfsOperationPicture_GetIndexTopSwitchPic");
            return Json(new { result = "1", message = "保存成功" }, "text/html");
            #endregion
        }
        #endregion

        #region 编辑轮播图加载
        public ActionResult UpdateTopSwitch()
        {
            int pid = Rq.GetIntQueryString("pid");
            SWfsOperationPicture picmanagerSingle = new SWfsOperationPictureService().GetModel(pid.ToString());
            if (picmanagerSingle == null)
            {
                CommonHelp.Alert("请重新选择要修改的位置图信息", "/shangpin/homepage/indexlist.html");
            }
            ViewBag.PicManagerSingle = picmanagerSingle;
           
            return View();
        }
        #endregion



        #endregion

        #region  首页热点图片


        #region 首页橱窗图管理列表
        /// <summary>
        /// 首页轮播橱窗图展现页面 A.H
        /// </summary>
        [ValidateInput(false)]
        public ActionResult IndexListIsMap()
        {
            PaginationInfoModel pageinfo = new PaginationInfoModel();
            int gender = Rq.GetIntQueryString("gender");
            string keyWord = Rq.GetStringQueryString("keyword");
            string status = Rq.GetStringQueryString("status");
            string startTime = Rq.GetStringQueryString("StartTime");
            string endTime = Rq.GetStringQueryString("EndTime");
            int currentPage = Rq.GetIntQueryString("pageindex");
            int pictureIndex = Rq.GetIntQueryString("PagePosition");
            string startTimeValue = string.Empty, endTimeValue = string.Empty;
            #region 条件筛选
            startTimeValue = string.IsNullOrEmpty(startTime) ? "" : startTime + " 00:00:00";
            endTimeValue = string.IsNullOrEmpty(endTime) ? "" : endTime + " 23:59:59";
            pageinfo.CurrentPage = currentPage;
            pageinfo.Condition += " DataState=1";
            pageinfo.Condition += (string.IsNullOrEmpty(keyWord) || keyWord == "图片名称") ? "" : " and (PictureName like '%" + keyWord + "%')";                
            pageinfo.Condition += " and WebSiteNo='shangpin.com' and PageNo='index' and PagePositionNo='SwitchPictureImgMap'";           
            pageinfo.Condition += (string.IsNullOrEmpty(status)) ? "" : "and Status =" + status + " ";        
            pageinfo.Condition += (string.IsNullOrEmpty(startTimeValue)) ? "" : "  and DateBegin >='" + startTimeValue + "'";
            pageinfo.Condition += (string.IsNullOrEmpty(endTimeValue)) ? "" : "  and DateBegin <='" + endTimeValue + "'";
            #endregion

            IList<SWfsOperationPicture> picManagerList = new SWfsOperationPictureService().GetSWfsPictureManagerListTopSwitchV(ref pageinfo);
            ViewBag.PictureManagerList = picManagerList;
            ViewBag.PaginationInfoSingel = pageinfo;
            ViewBag.KeyWord = keyWord;
            ViewBag.Status = status;
            ViewBag.StartTime = startTime;
            ViewBag.EndTime = endTime;
            ViewBag.PagePosition = pictureIndex.ToString();
            ViewBag.GenderValue = gender.ToString();
            ViewBag.PagePosition = pictureIndex.ToString();
            SWfsOperationPictureService picService = new SWfsOperationPictureService();
            ViewBag.TotalCount = picService.GetIndexSwitchPicturesImgMapCount(); ;
            return View();
        }
        #endregion


        #region 添加首页热点图片
        /// <summary>
        /// 添加首页轮播热点图 A.H
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateTopSwitchImgMap()
        {
            return View();
        }
        #endregion

        #region 添加首页轮播热点图 更新轮播热点图
        /// <summary>
        /// 添加首页轮播热点图 更新轮播热点图
        /// </summary>
        /// <returns></returns>      
        [HttpPost]
        public ActionResult CreateTopSwitchNewImgMap()
        {
            SWfsOperationPicture newobj = new SWfsOperationPicture();
            string pictureName = Rq.GetStringForm("SubjectName");
            string Status = Rq.GetStringForm("Status");
            string PagePosition = "";//Rq.GetStringForm("PagePosition");
            string linkAddress = Rq.GetStringForm("Address");
            string DateBegin = Rq.GetStringForm("DateBegin");
            string DateEnd = Rq.GetStringForm("DateEnd");
            string relationGender = Rq.GetStringForm("relationGender");
            string pictureManageEdit = Rq.GetStringForm("PictureManageEdit");
            string pictureManageId = Rq.GetStringForm("PictureManageId");
            string PictureFileNo = string.Empty;
            #region 获取上传图片参数并且判断
            if (Request.Files["FlapHotTwo"] != null && Request.Files["FlapHotTwo"].ContentLength > 0)
            {
                string picSizeStr = "width:{0},Height:{1},Length:500";
                if (PagePosition == "4")//首页背景广告图片
                { picSizeStr = string.Format(picSizeStr, 0, 0); }
                else
                {//三个普通轮播图
                    picSizeStr = string.Format(picSizeStr, 958, 499);
                }
                PictureFileNo = SaveImg(Request.Files["FlapHotTwo"], picSizeStr);
                if (rsPic.Keys.Contains("error"))
                {
                    return Json(new { result = "-1", message = "图片不合法" + rsPic["error"] + "" }, "text/html");
                }
            }
            newobj = new SWfsOperationPictureService().GetModel(pictureManageId);
            if (newobj == null)
            { newobj = new SWfsOperationPicture(); newobj.DateCreate = DateTime.Now; }
            else
            {
                if (string.IsNullOrEmpty(PictureFileNo))
                {
                    PictureFileNo = newobj.PictureFileNo;
                }
            }
            newobj.PictureFileNo = PictureFileNo;
            newobj.PictureName = pictureName;
            //if (!string.IsNullOrEmpty(linkAddress)) { if (linkAddress.ToLower() == "http://") { linkAddress = ""; } }
            newobj.LinkAddress = linkAddress;
            newobj.WebSiteNo = "shangpin.com";
            newobj.PageNo = "index";
            newobj.PagePositionNo = "SwitchPictureImgMap";
            newobj.PagePositionName = "首页轮播热点图";
            newobj.PictureIndex = 1; //Int16.Parse(PagePosition);
            newobj.Status = int.Parse(Status);
            newobj.SortId = 1;
            newobj.OperatorUserId = PresentationHelper.GetPassport().UserName;
            newobj.DateBegin = string.IsNullOrEmpty(DateBegin) ? DateTime.Now : DateTime.Parse(DateBegin);
            newobj.DateEnd = string.IsNullOrEmpty(DateEnd) ? DateTime.MaxValue : DateTime.Parse(DateEnd); ;
            newobj.DataState = 1;
            if (newobj.PictureManageId > 0)
            {
                new SWfsOperationPictureService().Update(newobj);
            }
            else
            {
                pictureManageId = new SWfsOperationPictureService().Add(newobj, true).ToString();
            }

            return Json(new { result = "1", message = "保存成功！" }, "text/html");
            #endregion
        }
        #endregion

        #region 为热点图标注热点区域
        /// <summary>
        /// 添加首页轮播热点图 A.H
        /// </summary>
        /// <returns></returns>
        public ActionResult TaggingTopSwitchImgMap()
        {
            int pid = Rq.GetIntQueryString("pid");
            SWfsOperationPicture picmanagerSingle = new SWfsOperationPictureService().GetModel(pid.ToString());
            if (picmanagerSingle == null)
            {
                CommonHelp.Alert("请重新选择要修改的位置图信息", "/shangpin/homepage/indexlist.html?isMap=1");
            }
            ViewBag.PicManagerSingle = picmanagerSingle;
            return View();
        }

         [HttpPost]
        public ActionResult SetTopSwitchImgMap(FormCollection collection)
        {
           
             if(collection.Count<=1)
             {
                 return Json(new { result = "-1", message = "最少得有需要一个热点" }, "text/html");;
             }
              string str = string.Empty;
            foreach (var item in collection)
            {
                string objName = item.ToString();
                if (objName.Contains("link"))
                { 
                str += collection[item.ToString()];
                }
               
                if (objName.Contains("rect"))
                {
                    str += "=" + collection[item.ToString()];
                }
                if (objName.Contains("title"))
                {
                    str += "=" + collection[item.ToString()] + "|";
                }
            }
            string pictureManageId = Rq.GetStringForm("PictureManageId");
            SWfsOperationPicture  swfsPic = new SWfsOperationPictureService().GetModel(pictureManageId);
            if (swfsPic == null)
            {
                return Json(new { result = "-1", message = "图片信息不存在" }, "text/html");
            }
            swfsPic.LinkAddress = str;
            new SWfsOperationPictureService().Update(swfsPic);
            return Json(new { result = "1", message = "保存成功" }, "text/html");
         }
        #endregion

        #region 编辑轮播热点图加载
        public ActionResult UpdateTopSwitchImgMap()
        {
            int pid = Rq.GetIntQueryString("pid");
            SWfsOperationPicture picmanagerSingle = new SWfsOperationPictureService().GetModel(pid.ToString());
            if (picmanagerSingle == null)
            {
                CommonHelp.Alert("请重新选择要修改的位置图信息", "/shangpin/homepage/indexlist.html?isMap=1");
            }
            ViewBag.PicManagerSingle = picmanagerSingle;
            return View();
        }
        #endregion

        #endregion

        #region 首页大运营位置图
        /// <summary>
        /// 首页大运营位置图展现页面lxy20140313 
        /// </summary>
        public ActionResult IndexBigList()
        {
            PaginationInfoModel pageinfo = new PaginationInfoModel();
            int gender = Rq.GetIntQueryString("gender");
            string keyWord = Rq.GetStringQueryString("keyword");
            string status = Rq.GetStringQueryString("status");
            string startTime = Rq.GetStringQueryString("StartTime");
            string endTime = Rq.GetStringQueryString("EndTime");
            int currentPage = Rq.GetIntQueryString("pageindex");
            int pagePosition = Rq.GetIntQueryString("PagePosition");
            string startTimeValue = string.Empty, endTimeValue = string.Empty;
            #region 条件筛选
            startTimeValue = string.IsNullOrEmpty(startTime) ? "" : startTime + " 00:00:00";
            endTimeValue = string.IsNullOrEmpty(endTime) ? "" : endTime + " 23:59:59";
            pageinfo.CurrentPage = currentPage;
            pageinfo.Condition += pagePosition > 0 ? "Gender=" + gender + " and PagePosition=" + pagePosition : " Gender=" + gender + " and PagePosition in(84,85,86,87,88,94)  ";
            pageinfo.Condition += (string.IsNullOrEmpty(keyWord) || keyWord == "名称") ? "" : " and (PictureName like '%" + keyWord + "%')";
            pageinfo.Condition += (string.IsNullOrEmpty(status)) ? "" : "and Status =" + status + "";
            pageinfo.Condition += (string.IsNullOrEmpty(startTimeValue)) ? "" : "  and DateBegin >='" + startTimeValue + "'";
            pageinfo.Condition += (string.IsNullOrEmpty(endTimeValue)) ? "" : "  and DateBegin <='" + endTimeValue + "'";
            #endregion

            IList<SWfsPictureManager> picManagerList = new SWfsPictureManagerService().GetSWfsPictureManagerListTopSwitchV(ref pageinfo);
            ViewBag.PictureManagerList = picManagerList;
            ViewBag.PaginationInfoSingel = pageinfo;
            ViewBag.KeyWord = keyWord;
            ViewBag.Status = status;
            ViewBag.StartTime = startTime;
            ViewBag.EndTime = endTime;
            ViewBag.GenderValue = gender.ToString();
            ViewBag.PagePosition = pagePosition.ToString();
            return View();
        }
        public ActionResult CreateBigPagePosition()
        {
            return View();
        }
        public ActionResult UpdateBigPagePosition()
        {
            int pid = Rq.GetIntQueryString("pid");
            SWfsPictureManager picmanagerSingle = new SWfsPictureManagerService().GetModel(pid.ToString());
            if (picmanagerSingle == null)
            {
                CommonHelp.Alert("请重新选择要修改的位置图信息", "/shangpin/homepage/indexbiglist.html?gender=0");
            }
            ViewBag.PicManagerSingle = picmanagerSingle;
            return View();
        }
        /// <summary>
        /// 添加首页轮播图位置 更新轮播图
        /// </summary>
        /// <returns></returns>      
        [HttpPost]
        public ActionResult CreateBigPagePositionData()
        {
            SWfsPictureManager newobj = new SWfsPictureManager();
            string pictureName = Rq.GetStringForm("SubjectName");
            string Status = Rq.GetStringForm("Status");
            string PagePosition = Rq.GetStringForm("PagePosition"); ;//固定位置参数 
            string linkAddress = Rq.GetStringForm("HotTwoUrl");
            string DateBegin = Rq.GetStringForm("DateBegin");
            string relationGender = Rq.GetStringForm("relationGender");
            string pictureManageId = Rq.GetStringForm("PictureManageId");
            string PictureFileNo = string.Empty;
            if (string.IsNullOrEmpty(PagePosition))
            {
                return Json(new { result = "-1", message = "请图片位置填写信息，请重新选择位置图片" }, "text/html");
            }
            #region 获取上传图片参数并且判断
            if (Request.Files["FlapHotTwo"] != null && Request.Files["FlapHotTwo"].ContentLength > 0)
            {
                string picSizeStr = "width:{0},Height:{1},Length:500";
                picSizeStr = string.Format(picSizeStr, 580, 316);
                PictureFileNo = SaveImg(Request.Files["FlapHotTwo"], picSizeStr);
                if (rsPic.Keys.Contains("error"))
                {
                    return Json(new { result = "-1", message = "图片不合法" + rsPic["error"] + "" }, "text/html");
                }
            }
            newobj = new SWfsPictureManagerService().GetModel(pictureManageId);
            if (newobj == null)
            {
                newobj = new SWfsPictureManager();
                newobj.DateCreate = DateTime.Now;
            }
            else
            {
                relationGender = newobj.Gender.ToString();
                if (string.IsNullOrEmpty(PictureFileNo))
                {
                    PictureFileNo = newobj.PictureFileNo;
                }
            }
            newobj.PictureFileNo = PictureFileNo;
            newobj.Gender = short.Parse(relationGender);
            newobj.PictureName = pictureName;
            if (!string.IsNullOrEmpty(linkAddress)) { if (linkAddress.ToLower() == "http://") { linkAddress = ""; } }
            newobj.LinkAddress = linkAddress;
            newobj.WebSite = 1;
            newobj.Status = int.Parse(Status);
            newobj.Position = 1;
            newobj.OperatorUserId = PresentationHelper.GetPassport().UserName;
            newobj.PagePosition = int.Parse(PagePosition);
            newobj.Keyword = string.Empty;
            newobj.BrandContent = string.Empty;
            newobj.InvitationCode = string.Empty;
            newobj.BankName = string.Empty;
            newobj.ExpandPicFile = string.Empty;
            newobj.SubTitle = string.Empty;
            newobj.DateBegin = string.IsNullOrEmpty(DateBegin) ? DateTime.Now : DateTime.Parse(DateBegin);
            newobj.DateEnd = DateTime.MaxValue;
            if (newobj.PictureManageId > 0)
            {
                new SWfsPictureManagerService().Update(newobj);
            }
            else
            {
                new SWfsPictureManagerService().Add(newobj);
            }

            return Json(new { result = "1", message = "保存成功" }, "text/html");
            #endregion
        }

        #endregion

        #region 首页小运营位置图
        /// <summary>
        /// 首页小运营位置图展现页面lxy20140313 
        /// </summary>
        public ActionResult IndexSmallList()
        {
            PaginationInfoModel pageinfo = new PaginationInfoModel();
            int gender = Rq.GetIntQueryString("gender");
            string keyWord = Rq.GetStringQueryString("keyword");
            string status = Rq.GetStringQueryString("status");
            string startTime = Rq.GetStringQueryString("StartTime");
            string endTime = Rq.GetStringQueryString("EndTime");
            int currentPage = Rq.GetIntQueryString("pageindex");
            string Ptype = Rq.GetStringQueryString("Ptype");
            int pagePosition = Rq.GetIntQueryString("PagePosition");

            string startTimeValue = string.Empty, endTimeValue = string.Empty;

            #region 条件筛选
            startTimeValue = string.IsNullOrEmpty(startTime) ? "" : startTime + " 00:00:00";
            endTimeValue = string.IsNullOrEmpty(endTime) ? "" : endTime + " 23:59:59";
            pageinfo.CurrentPage = currentPage;
            pageinfo.Condition += pagePosition > 0 ? "Gender=" + gender + " and PagePosition=" + pagePosition : " Gender=" + gender + " and PagePosition in(89,90,91,92,93,95)  ";
            pageinfo.Condition += (string.IsNullOrEmpty(keyWord) || keyWord == "名称") ? "" : " and (PictureName like '%" + keyWord + "%')";
            pageinfo.Condition += (string.IsNullOrEmpty(status)) ? "" : "and Status =" + status + "";
            pageinfo.Condition += (string.IsNullOrEmpty(Ptype)) ? "" : "and InvitationCode ='" + Ptype + "'";
            pageinfo.Condition += (string.IsNullOrEmpty(startTimeValue)) ? "" : "  and DateBegin >='" + startTimeValue + "'";
            pageinfo.Condition += (string.IsNullOrEmpty(endTimeValue)) ? "" : "  and DateBegin <='" + endTimeValue + "'";
            #endregion

            IList<SWfsPictureManager> picManagerList = new SWfsPictureManagerService().GetSWfsPictureManagerListTopSwitchV(ref pageinfo);
            ViewBag.PictureManagerList = picManagerList;
            ViewBag.PaginationInfoSingel = pageinfo;
            ViewBag.KeyWord = keyWord;
            ViewBag.Status = status;
            ViewBag.StartTime = startTime;
            ViewBag.EndTime = endTime;
            ViewBag.InvitationCode = Ptype;
            ViewBag.GenderValue = gender.ToString();
            ViewBag.PagePosition = pagePosition.ToString();
            return View();
        }
        public ActionResult CreateSmallPagePosition()
        {
            return View();
        }
        public ActionResult UpdateSmallPagePosition()
        {
            int pid = Rq.GetIntQueryString("pid");
            SWfsPictureManager picmanagerSingle = new SWfsPictureManagerService().GetModel(pid.ToString());
            if (picmanagerSingle == null)
            {
                CommonHelp.Alert("请重新选择要修改的位置图信息", "/shangpin/homepage/indexsmalllist.html?gender=0");
            }
            string genderValue = picmanagerSingle.Gender == 1 ? "A02" : "A01";
            IList<SWfsSpChannelRecommendLink> recommendLinkList = new Service.Shangpin.SWfsChannelService().GetSWfsSpChannelRecommendLinkByHomePageList(pid.ToString(), genderValue);
            ViewBag.PicManagerSingle = picmanagerSingle;
            ViewBag.RecommendLinkList = recommendLinkList;
            return View();
        }
        /// <summary>
        /// 添加首页轮播图位置 更新轮播图
        /// </summary>
        /// <returns></returns>      
        [HttpPost]
        public ActionResult CreateSmallPagePositionData()
        {

            SWfsPictureManager newobj = new SWfsPictureManager();
            string pictureName = Rq.GetStringForm("SubjectName");
            string Status = Rq.GetStringForm("Status");
            string PagePosition = Rq.GetStringForm("PagePosition"); ;//固定位置参数 
            string linkAddress = Rq.GetStringForm("HotTwoUrl");
            string DateBegin = Rq.GetStringForm("DateBegin");
            string DateEnd = Rq.GetStringForm("DateEnd");
            string relationGender = Rq.GetStringForm("relationGender");
            string pictureManageId = Rq.GetStringForm("PictureManageId");
            string picType = Rq.GetStringForm("PicType");
            string subTitle = Rq.GetStringForm("SubTitle");
            string PictureFileNo = string.Empty;
            string relationGenderValue = relationGender == "1" ? "A02" : "A01";
            if (string.IsNullOrEmpty(PagePosition))
            {
                return Json(new { result = "-1", message = "请图片位置填写信息，请重新选择位置图片" }, "text/html");
            }
            #region 获取上传图片参数并且判断
            if (Request.Files["FlapHotTwo"] != null && Request.Files["FlapHotTwo"].ContentLength > 0)
            {
                /*选择纯图片模板限制为：347*210 小于300k的jpg或gif。选择图文混排模板限制为：170*210小于300k的jpg或gif*/
                string picSizeStr = "width:{0},Height:{1},Length:300";
                if (picType == "1")
                { picSizeStr = string.Format(picSizeStr, 347, 210); }
                else { picSizeStr = string.Format(picSizeStr, 170, 210); }

                PictureFileNo = SaveImg(Request.Files["FlapHotTwo"], picSizeStr);
                if (rsPic.Keys.Contains("error"))
                {
                    return Json(new { result = "-1", message = "图片不合法" + rsPic["error"] + "" }, "text/html");
                }
            }
            newobj = new SWfsPictureManagerService().GetModel(pictureManageId);
            if (newobj == null)
            {
                newobj = new SWfsPictureManager();
                newobj.DateCreate = DateTime.Now;
            }
            else
            {
                relationGender = newobj.Gender.ToString();
                if (string.IsNullOrEmpty(PictureFileNo))
                {
                    if (newobj.InvitationCode != picType)
                    { return Json(new { result = "-1", message = "图片不合法,请按提示上传图片" }, "text/html"); }
                    PictureFileNo = newobj.PictureFileNo;
                }
            }
            newobj.PictureFileNo = PictureFileNo;
            newobj.Gender = short.Parse(relationGender);
            newobj.PictureName = pictureName;
            if (!string.IsNullOrEmpty(linkAddress)) { if (linkAddress.ToLower() == "http://") { linkAddress = ""; } }
            newobj.LinkAddress = linkAddress;
            newobj.WebSite = 1;
            newobj.Status = int.Parse(Status);
            newobj.Position = 1;
            newobj.OperatorUserId = PresentationHelper.GetPassport().UserName;
            newobj.PagePosition = int.Parse(PagePosition);
            newobj.Keyword = string.Empty;
            newobj.BrandContent = string.Empty;
            newobj.InvitationCode = picType;//判断是图片，还是混排图片+文字 1，图片，2混排
            newobj.BankName = string.Empty;
            newobj.ExpandPicFile = string.Empty;
            newobj.SubTitle = subTitle;
            newobj.DateBegin = string.IsNullOrEmpty(DateBegin) ? DateTime.Now : DateTime.Parse(DateBegin);
            newobj.DateEnd = string.IsNullOrEmpty(DateEnd) ? DateTime.MaxValue : DateTime.Parse(DateEnd);
            if (newobj.PictureManageId > 0)
            {
                new SWfsPictureManagerService().Update(newobj);
                if (picType == "2")
                {
                    for (int i = 1; i <= 4; i++)
                    {
                        SWfsSpChannelRecommendLink recommendLinkSingle = new SWfsSpChannelRecommendLink();
                        string RecommendLinkID = Rq.GetStringForm("txtRecommendLinkIDUpdate" + i);
                        recommendLinkSingle.RecommendLinkID = 0;
                        if (!string.IsNullOrEmpty(RecommendLinkID) && RecommendLinkID != "0")
                        {
                            recommendLinkSingle.RecommendLinkID = int.Parse(RecommendLinkID);
                        }
                        string RecommendLinkName = Rq.GetStringForm("txtRecommendLinkID" + i);
                        string RecommendLinkUrl = Rq.GetStringForm("txtLinkID" + i);
                        recommendLinkSingle.ChannelNO = relationGenderValue;
                        recommendLinkSingle.LinkName = RecommendLinkName;
                        recommendLinkSingle.LinkUrl = RecommendLinkUrl;
                        recommendLinkSingle.SortValue = i;
                        recommendLinkSingle.CreateDate = DateTime.Now;
                        recommendLinkSingle.ParentID = newobj.PictureManageId;
                        new Service.Shangpin.SWfsChannelService().EditeSWfsSpChannelRecommendLink(recommendLinkSingle);
                    }
                }
            }
            else
            {
                newobj.PictureManageId = new SWfsPictureManagerService().Add(newobj, true);
                if (picType == "2")
                {

                    for (int i = 1; i <= 4; i++)
                    {
                        SWfsSpChannelRecommendLink recommendLinkSingle = new SWfsSpChannelRecommendLink();
                        recommendLinkSingle.RecommendLinkID = 0;
                        string RecommendLinkName = Rq.GetStringForm("txtRecommendLinkID" + i);
                        string RecommendLinkUrl = Rq.GetStringForm("txtLinkID" + i);
                        recommendLinkSingle.ChannelNO = relationGenderValue;
                        recommendLinkSingle.LinkName = RecommendLinkName;
                        recommendLinkSingle.LinkUrl = RecommendLinkUrl;
                        recommendLinkSingle.SortValue = i;
                        recommendLinkSingle.CreateDate = DateTime.Now;
                        recommendLinkSingle.ParentID = newobj.PictureManageId;
                        new Service.Shangpin.SWfsChannelService().EditeSWfsSpChannelRecommendLink(recommendLinkSingle);
                    }
                }
            }

            return Json(new { result = "1", message = "保存成功" }, "text/html");
            #endregion
        }

        //复制小运营位置
        public ActionResult CopySmallPagePosition(int id, int gender)
        {
            new SWfsPictureManagerService().CopySmallPagePosition(id);
            return Redirect("/shangpin/homepage/indexsmalllist.html?gender=" + gender);
        }
        #endregion

        #region 底部运营位置图
        /// <summary>
        /// 运营位置图 展现页面
        /// </summary>
        public ActionResult IndexBottonList()
        {
            PaginationInfoModel pageinfo = new PaginationInfoModel();
            //int gender = Rq.GetIntQueryString("gender");
            string keyWord = Rq.GetStringQueryString("keyword");
            string status = Rq.GetStringQueryString("status");
            string startTime = Rq.GetStringQueryString("StartTime");
            string endTime = Rq.GetStringQueryString("EndTime");
            int currentPage = Rq.GetIntQueryString("pageindex");
            string pictureIndex = Rq.GetStringQueryString("PictureIndex");
            string pagePositionNo = Rq.GetStringQueryString("PagePositionNo");
            string pagePositionName = Rq.GetStringQueryString("PagePositionName");
            ViewBag.PagePositionName = pagePositionName;
            ViewBag.width = Rq.GetStringQueryString("width");
            ViewBag.height = Rq.GetStringQueryString("height");
            ViewBag.PagePositionNo = pagePositionNo;
            string startTimeValue = string.Empty, endTimeValue = string.Empty;
            #region 条件筛选
            startTimeValue = string.IsNullOrEmpty(startTime) ? "" : startTime + " 00:00:00";
            endTimeValue = string.IsNullOrEmpty(endTime) ? "" : endTime + " 23:59:59";
            pageinfo.CurrentPage = currentPage;
            pageinfo.Condition += "PageNo='index' and PagePositionNo='" + pagePositionNo + "'";
            pageinfo.Condition += (string.IsNullOrEmpty(keyWord) || keyWord == "名称") ? "" : " and (PictureName like '%" + keyWord + "%')";
            pageinfo.Condition += (string.IsNullOrEmpty(status)) ? "" : "and Status =" + status + "";
            pageinfo.Condition += (string.IsNullOrEmpty(pictureIndex)) ? "" : "and pictureIndex =" + pictureIndex + "";
            pageinfo.Condition += " and DataState=1";
            pageinfo.Condition += (string.IsNullOrEmpty(startTimeValue)) ? "" : "  and DateCreate >='" + startTimeValue + "'";
            pageinfo.Condition += (string.IsNullOrEmpty(endTimeValue)) ? "" : "  and DateCreate <='" + endTimeValue + "'";
            #endregion

            IList<SWfsOperationPicture> picManagerList = new SWfsOperationPictureService().GetSWfsPictureManagerListTopSwitchV(ref pageinfo);
            ViewBag.PictureManagerList = picManagerList;
            ViewBag.PaginationInfoSingel = pageinfo;
            ViewBag.KeyWord = keyWord;
            ViewBag.Status = status;
            ViewBag.StartTime = startTime;
            ViewBag.EndTime = endTime;
            //ViewBag.GenderValue = gender.ToString();
            ViewBag.PictureIndex = pictureIndex;
            return View();
        }
        public ActionResult CreateBottonPagePosition()
        {
            ViewBag.positionNo = Rq.GetStringQueryString("PagePositionNo");
            ViewBag.positionName = Rq.GetStringQueryString("PagePositionName");
            return View();
        }
        public ActionResult UpdateBottonPagePosition()
        {
            ViewBag.positionNo = Rq.GetStringQueryString("PagePositionNo");
            ViewBag.positionName = Rq.GetStringQueryString("PagePositionName");
            //ViewBag.PictureIndex = PictureIndex;
            int pid = Rq.GetIntQueryString("pid");
            SWfsOperationPicture picmanagerSingle = new SWfsOperationPictureService().GetModel(pid.ToString());
            if (picmanagerSingle == null)
            {
            }
            ViewBag.PicManagerSingle = picmanagerSingle;
            return View();
        }

        /// <summary>
        /// 添加运营位置图  
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateBottonPagePositionData()
        {
            SWfsOperationPicture newobj = new SWfsOperationPicture();
            string pictureName = Rq.GetStringForm("SubjectName");//图片标题
            string pictureIndex = Rq.GetStringForm("PictureIndex");//位置内索引 
            pictureIndex = string.IsNullOrEmpty(pictureIndex) ? "0" : pictureIndex;
            string linkAddress = Rq.GetStringForm("HotTwoUrl");//链接地址
            string positionName = Rq.GetStringForm("PagePositionName");//位置名称
            string pagePositionNo = Rq.GetStringForm("PagePositionNo");//页面内位置
            string DateBegin = Rq.GetStringForm("DateBegin");
            string pictureManageId = Rq.GetStringForm("PictureManageId");
            string PictureFileNo = string.Empty;
            string PWidth = Rq.GetStringForm("pwidth");
            string PHeight = Rq.GetStringForm("pheight");
            string Status = Rq.GetStringForm("Status");
            if (string.IsNullOrEmpty(pictureIndex) && pagePositionNo == "bottomBanner")
            {
                return Json(new { result = "-1", message = "请选择图片位置填写信息，请重新选择位置图片" }, "text/html");
            }
            #region 获取上传图片参数并且判断
            if (Request.Files["FlapHotTwo"] != null && Request.Files["FlapHotTwo"].ContentLength > 0)
            {
                string picSizeStr = "width:{0},Height:{1},Length:200";
                //if (pagePositionNo.IndexOf("bottomBanner") > -1)
                //{
                //    picSizeStr = string.Format(picSizeStr, 315, 150);
                //}
                //else if (pagePositionNo.IndexOf("middleBanner") > -1)
                //{
                //    picSizeStr = string.Format(picSizeStr, 315, 150);
                //}
                //else
                //{
                //    picSizeStr = string.Format(picSizeStr, 960, 70);
                //}
                picSizeStr = string.Format(picSizeStr, string.IsNullOrEmpty(PWidth) ? "0" : PWidth, string.IsNullOrEmpty(PHeight) ? "0" : PHeight);
                PictureFileNo = SaveImg(Request.Files["FlapHotTwo"], picSizeStr);
                if (rsPic.Keys.Contains("error"))
                {
                    return Json(new { result = "-1", message = "图片不合法" + rsPic["error"] + "" }, "text/html");
                }
            }
            newobj = new SWfsOperationPictureService().GetModel(pictureManageId);
            if (newobj == null)
            {
                newobj = new SWfsOperationPicture();
                newobj.DateCreate = DateTime.Now;
            }
            else
            {
                if (string.IsNullOrEmpty(PictureFileNo))
                {
                    PictureFileNo = newobj.PictureFileNo;
                }
            }
            newobj.PictureFileNo = PictureFileNo;
            newobj.PictureName = pictureName;
            if (!string.IsNullOrEmpty(linkAddress)) { if (linkAddress.ToLower() == "http://") { linkAddress = ""; } }
            newobj.LinkAddress = linkAddress;
            newobj.Status = Convert.ToInt32(Status);
            newobj.DataState = 1;
            newobj.SortId = 1;
            newobj.OperatorUserId = PresentationHelper.GetPassport().UserName;
            newobj.WebSiteNo = "shangpin.com";
            newobj.PageNo = "index";
            newobj.PagePositionNo = pagePositionNo;
            newobj.PagePositionName = positionName;
            newobj.PictureIndex = Int16.Parse(pictureIndex);
            newobj.DateBegin = string.IsNullOrEmpty(DateBegin) ? DateTime.Now : Convert.ToDateTime(DateBegin);
            newobj.DateEnd = DateTime.MaxValue;
            if (newobj.PictureManageId > 0)
            {
                new SWfsOperationPictureService().Update(newobj);
            }
            else
            {
                new SWfsOperationPictureService().Add(newobj);
            }
            //清楚缓存
            EnyimMemcachedClient.Instance.Remove("ComBeziWfs_SWfsOperationPicture_GetCurrentSWfsOperationPictureByModuleId_GetCurrentSWfsOperationPictureByModuleId");
            EnyimMemcachedClient.Instance.Remove("ComBeziWfs_SWfsOperationPicture_GetSWfsOperationPictureBottom_GetIndexOperationPictureBottom");
            EnyimMemcachedClient.Instance.Remove("ComBeziWfs_SWfsOperationPicture_GetSWfsOperationPictureByPageNo_GetIndexOperationPicture");

            return Json(new { result = "1", message = "保存成功" }, "text/html");
            #endregion
        }

        #endregion

        /// <summary>
        /// 保存区块内容
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost, ValidateInput(false)]
        public string SaveSWfsPictureManagerContent()
        {
            //return Json(new { status = 1, message = obj });
            JavaScriptSerializer json = new JavaScriptSerializer();
            string width = "0";
            string height = "0";
            string pagePositionID = Rq.GetStringForm("PagePositionID");
            string relationTypeID = Rq.GetStringForm("RelationTypeID");
            string imgNO = Rq.GetStringForm("ImgNO");
            string pictureName = Rq.GetStringForm("PictureName");
            string linkAddress = Rq.GetStringForm("LinkAddress");
            string relationGender = Rq.GetStringForm("RelationGender");
            string picSize = Rq.GetStringForm("savePicSize");
            if (string.IsNullOrEmpty(relationGender))
            { relationGender = "0"; }

            SWfsPictureManager newobj = new SWfsPictureManager();

            #region 数据验证
            if (string.IsNullOrEmpty(pagePositionID) || string.IsNullOrEmpty(relationTypeID))
            {
                string error = string.Empty;
                if (Request.Files["imgfile"] == null || Request.Files["imgfile"].ContentLength <= 0)
                {
                    error = "请选择要上传的图片";
                }
                return "{\"status\" : 0, \"message\" : \"数据不合法" + error + "\", \"relationtype\":3}";
            }
            string PagePositionID = Request.Form["PagePositionID"];
            newobj = new SWfsPictureManagerService().GetSWfsPictureManagerSingle(pagePositionID, int.Parse(relationGender));
            if (newobj == null)
            {
                newobj = new SWfsPictureManager();
                newobj.DateCreate = DateTime.Now;
            }
            newobj.Gender = short.Parse(relationGender);
            newobj.PictureName = pictureName;
            if (!string.IsNullOrEmpty(linkAddress)) { if (linkAddress.ToLower() == "http://") { linkAddress = ""; } }
            newobj.LinkAddress = linkAddress;
            newobj.WebSite = 1;
            newobj.Status = 1;
            newobj.Position = 1;
            newobj.OperatorUserId = PresentationHelper.GetPassport().UserName;
            newobj.PagePosition = int.Parse(pagePositionID);
            newobj.Keyword = string.Empty;
            newobj.BrandContent = string.Empty;
            newobj.InvitationCode = string.Empty;
            newobj.BankName = string.Empty;
            newobj.ExpandPicFile = string.Empty;
            newobj.SubTitle = string.Empty;
            newobj.DateBegin = DateTime.Now;
            newobj.DateEnd = DateTime.MaxValue;

            if (Request.Files["imgfile"] != null && Request.Files["imgfile"].ContentLength > 0)
            {
                if (!string.IsNullOrEmpty(picSize))
                {
                    if (picSize.Split('*').Count() > 1)
                    {
                        width = picSize.Split('*')[0];
                        height = picSize.Split('*')[1];
                    }
                }
                string picSizeStr = "width:{0},Height:{1},Length:500";
                picSizeStr = string.Format(picSizeStr, width, height);
                newobj.PictureFileNo = SaveImg(Request.Files["imgfile"], picSizeStr);
                if (rsPic.Keys.Contains("error"))
                {
                    return "{\"status\" : 0, \"message\" : \"图片不合法" + rsPic["error"] + "\", \"relationtype\":3}";
                }
            }
            else
            {
                if (string.IsNullOrEmpty(newobj.PictureFileNo))
                {
                    return "{\"status\" : 0, \"message\" : \"请选择要上传的图片\", \"relationtype\":3}";
                }
            }
            #endregion

            if (newobj.PictureManageId > 0)//如果是模块三图片位置
            {
                bool updateBool = new SWfsPictureManagerService().Update(newobj);
                newobj.ExpandPicFile = ServicePic.ResolveUGCImage("2", newobj.PictureFileNo, 0, 0);
                return "{\"status\" : 1, \"message\" : " + json.Serialize(newobj) + ", \"relationtype\":3}";
            }
            else
            {
                new SWfsPictureManagerService().Add(newobj);
                newobj.ExpandPicFile = ServicePic.ResolveUGCImage("2", newobj.PictureFileNo, 0, 0);
                return "{\"status\":1, \"message\":" + json.Serialize(newobj) + ", \"relationtype\":3}";
            }
        }


        //加载自定义链接
        public string GetPictureManagerJson()
        {
            JavaScriptSerializer json = new JavaScriptSerializer();
            SWfsPictureManager newobj = new SWfsPictureManager();
            if (!string.IsNullOrEmpty(Request.Form["RelationTypeID"]) && Request.Form["PagePositionID"] + "" != "0")
            {
                string PagePositionID = Request.Form["PagePositionID"];
                string relationGender = Request.Form["RelationGender"] ?? "0";
                newobj = new SWfsPictureManagerService().GetSWfsPictureManagerSingle(PagePositionID, int.Parse(relationGender));
                //  newobj.PictureFileNo = ServicePic.ResolveUGCImage("2", newobj.PictureFileNo, 0, 0);

            }
            return json.Serialize(newobj);
        }

        //通用保存图片
        private string SaveImg(HttpPostedFileBase file, string imgInfo)
        {
            CommonService commonService = new CommonService();
            rsPic.Clear();
            //Dictionary<string, string> rsPic = new Dictionary<string, string>();
            if (file.ContentLength > 0)
            {
                rsPic = commonService.PostImg(file, imgInfo);
                if (rsPic.Keys.Contains("error"))
                {
                    return rsPic["error"];
                }
                if (rsPic.Keys.Contains("success"))
                {
                    return rsPic["success"];
                }
            }
            return "";
        }
        #region 秒杀
        public ActionResult CreateSecKill(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var homeSecKill = DapperUtil.Query<HomeSecKill>("ComBeziWfs_SWfsHomeSecKill_SelectById", new { SecKillId = id }).FirstOrDefault();
                ViewBag.HomeSecKill = homeSecKill;
            }
            return View();
        }

        public string SaveHomeSecKill()
        {
            string title = Request.Form["title"];
            string type = Request.Form["type"];
            string status = Request.Form["status"];
            string productNo = Request.Form["productNo"];
            DateTime showTime = Convert.ToDateTime(Request.Form["dateShow"]);
            DateTime startTime = Convert.ToDateTime(Request.Form["dateBegin"]);
            string linkAddress = Request.Form["linkAddress"];
            string secKillId = Request.Form["secKillId"];
            string gender = Request.Form["gender"];
            if (string.IsNullOrEmpty(gender))
            {
                gender = "0";
            }
            string channelNo = gender == "0" ? "A01" : "A02";
            HomeSecKillService homeSecKillService = new HomeSecKillService();
            if (showTime.Date != startTime.Date)
            {
                return "{ \"result\":\"error\", \"msg\":\"展示时间和开始时间应在同一天内\" }";
            }

            var hasSecKill = homeSecKillService.GetHomeSekKillByTime(showTime, short.Parse(type), channelNo, short.Parse(secKillId));
            if (hasSecKill)
            {
                return "{ \"result\":\"error\", \"msg\":\"爆款和秒杀不能同一天存在，一天中只能一款爆款或多场秒杀\" }";
            }
            if (homeSecKillService.CheckTime(showTime, startTime, channelNo, int.Parse(secKillId)))
            {
                return "{ \"result\":\"error\", \"msg\":\"时间段与其他时间段有交集\" }";
            }

            if (showTime >= startTime)
            {
                return "{ \"result\":\"error\", \"msg\":\"开始时间晚于展示时间\" }";
            }

            var homeSecKill = new SWfsHomeSecKill();
            if (secKillId != "0")
            {
                homeSecKill = DapperUtil.QueryByIdentity<SWfsHomeSecKill>(secKillId);
            }
            else
            {
                homeSecKill.CreateTime = DateTime.Now;
            }
            homeSecKill.ChannelNo = channelNo;
            homeSecKill.LinkAddress = linkAddress;
            homeSecKill.ProductNo = productNo;
            homeSecKill.SecKillTitle = title;
            homeSecKill.SecKillType = short.Parse(type);
            homeSecKill.ShowTime = Convert.ToDateTime(showTime);
            homeSecKill.StartTime = Convert.ToDateTime(startTime);
            homeSecKill.Status = short.Parse(status);
            if (secKillId != "0")
            {
                DapperUtil.Update<SWfsHomeSecKill>(homeSecKill);
            }
            else
            {
                DapperUtil.Insert<SWfsHomeSecKill>(homeSecKill);
            }
            return "{\"result\":\"success\"}";
        }


        public ActionResult HomeSecKillList(int pageIndex = 1, int pageSize = 10)
        {
            HomeSecKillService homeSecKillService = new HomeSecKillService();
            string title = Request.Form["SecKillTitle"];
            string status = Request.Form["status"];
            string productNo = Request.Form["productNo"];
            string startTime = Rq.GetStringForm("dateBegin");
            string endTime = Rq.GetStringForm("dateEnd");
            string type = Request.Form["type"];
            string gender = Request.QueryString["gender"];
            if (string.IsNullOrEmpty(gender))
            {
                gender = "0";
            }
            string channelNo = gender == "0" ? "A01" : "A02";
            Dictionary<string, object> dicParam = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(title))
            {
                dicParam.Add("SecKillTitle", title);
            }
            else
            {
                dicParam.Add("SecKillTitle", "");
            }
            if (!string.IsNullOrEmpty(productNo))
            {
                dicParam.Add("ProductNo", productNo);
            }
            else
            {
                dicParam.Add("ProductNo", "");
            }

            if (!string.IsNullOrEmpty(startTime))
            {
                dicParam.Add("StartTime", DateTime.Parse(startTime));
            }
            else
            {
                dicParam.Add("StartTime", "");
            }
            if (!string.IsNullOrEmpty(endTime))
            {
                dicParam.Add("EndTime", DateTime.Parse(endTime));
            }
            else
            {
                dicParam.Add("EndTime", "");
            }
            if (!string.IsNullOrEmpty(status) && status != "-1")
            {
                dicParam.Add("SecKillStatus", short.Parse(status));
            }
            else
            {
                dicParam.Add("SecKillStatus", "");
            }
            if (!string.IsNullOrEmpty(type) && type != "-1")
            {
                dicParam.Add("SecKillType", short.Parse(type));
            }
            else
            {
                dicParam.Add("SecKillType", "");
            }
            if (!string.IsNullOrEmpty(channelNo))
            {
                dicParam.Add("ChannelNo", channelNo);
            }
            else
            {
                dicParam.Add("ChannelNo", "");
            }

            var result = homeSecKillService.SelectSecKillList(dicParam, pageSize, pageIndex);
            int count = DapperUtil.Query<int>("ComBeziWfs_SWfsHomeSecKill_SelectListCount", dicParam, new { SecKillTitle = dicParam["SecKillTitle"], ProductNo = dicParam["ProductNo"], StartTime = dicParam["StartTime"], EndTime = dicParam["EndTime"], Status = dicParam["SecKillStatus"], SecKillType = dicParam["SecKillType"], ChannelNo = dicParam["ChannelNo"] }).FirstOrDefault();
            ViewBag.SecKillTitle = title;
            ViewBag.status = status;
            ViewBag.productNo = productNo;
            ViewBag.dateBegin = startTime;
            ViewBag.dateEnd = endTime;
            ViewBag.PageIndex = pageIndex;
            ViewBag.totalcount = count;
            ViewBag.HomeSecKill = result;
            ViewBag.gender = gender;
            ViewBag.type = type;
            return View();
        }

        public JsonResult DeleteHomeSecKillById(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                DapperUtil.Execute("ComBeziWfs_SWfsHomeSecKill_DeleteById", new { SecKillId = id });
            }
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateHomeSecKillStatus(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var homeSeckillInfo = DapperUtil.QueryByIdentityWithNoLock<SWfsHomeSecKill>(id);
                if (homeSeckillInfo.Status == 0)
                {
                    HomeSecKillService homeSecKillService = new HomeSecKillService();
                    var hasSecKill = homeSecKillService.GetHomeSekKillByTime(homeSeckillInfo.ShowTime, (short)homeSeckillInfo.SecKillType, homeSeckillInfo.ChannelNo, short.Parse(id));
                    if (hasSecKill)
                    {
                        return Json(new { success = false, ErrorMsg = "爆款和秒杀不能同一天存在，一天中只能一款爆款或多场秒杀" }, JsonRequestBehavior.AllowGet);
                    }
                    if (homeSecKillService.CheckTime(homeSeckillInfo.ShowTime, homeSeckillInfo.StartTime, homeSeckillInfo.ChannelNo, int.Parse(id)))
                    {
                        return Json(new { success = false, ErrorMsg = "时间段与其他时间段有交集" }, JsonRequestBehavior.AllowGet);
                    }
                }
                if (homeSeckillInfo != null)
                {
                    homeSeckillInfo.Status = (short)(homeSeckillInfo.Status == 0 ? 1 : 0);
                    DapperUtil.Update<SWfsHomeSecKill>(homeSeckillInfo);
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CheckProduct(string productNo)
        {
            string erroMsg = "";
            if (!string.IsNullOrEmpty(productNo))
            {
                var productInfo = DapperUtil.Query<ProductInfo>("ComBeziWfs_WfsProduct_SelectByProductNo", new { ProductNo = productNo }).FirstOrDefault();
                if (productInfo != null)
                {

                    //商品所属站点不对
                    if (productInfo.IsLimitedOutlet == 2)
                    {
                        erroMsg = "商品所属范围奥莱";
                    }
                    //商品非上架
                    if (productInfo.IsShelf != 1)
                    {
                        erroMsg = "商品不是上架状态";
                    }
                    SWfsProductService service = new SWfsProductService();
                    ProductInventory inventory = service.GetInventoryByProductNo(productInfo.ProductNo);
                    int Quantity = inventory.SumQuantity;
                    if (Quantity != 0)
                    {
                        return Json(new { Success = true, Result = productInfo.BrandEnName + productInfo.ProductName, ErrorMsg = erroMsg, InventoryQuantity = Quantity }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        erroMsg = "可用库存不足";
                        return Json(new { Success = false, ErrorMsg = erroMsg }, JsonRequestBehavior.AllowGet); ;
                    }
                }
                else
                {
                    erroMsg = "商品信息不存在";
                }
            }
            return Json(new { Success = false, ErrorMsg = erroMsg }, JsonRequestBehavior.AllowGet); ;
        }
        #endregion

        #region 首页管理 -hbq

        SWfsIndexModuleService sWfsIndexModuleService = new SWfsIndexModuleService();
        SWfsIndexModuleLinkService sWfsIndexModuleLinkService = new SWfsIndexModuleLinkService();

        #region 楼层全局设置
        /// <summary>
        /// 楼层全局设置
        /// </summary>
        /// <returns></returns>
        public ActionResult HomeManager_FloorGlobalSet(string moduleId, string moduleName, int Sort)
        {
            SWfsIndexModule module = new SWfsIndexModule();
            int id = 0;
            module.Sort = Sort;
            module.ModuleName = moduleName;
            if (!string.IsNullOrEmpty(moduleId) && int.TryParse(moduleId, out id) && id > 0)
                module = sWfsIndexModuleService.GetSWfsIndexModuleById(id);
            return View(module);
        }
        public ActionResult AlterFloorName(int ModuleId, string ModuleName)
        {
            try
            {
                SWfsIndexModule module = sWfsIndexModuleService.GetSWfsIndexModuleById(ModuleId);
                module.ModuleName = ModuleName;
                sWfsIndexModuleService.InsertOrUpdateSWfsIndexModule(module);
                return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost, ValidateInput(false)]
        public string HomeManager_FloorGlobalSet(FormCollection F)
        {
            try
            {
                SWfsIndexModule module = new SWfsIndexModule();

                Dictionary<string, string> dic = new Dictionary<string, string>();
                SWfsOperationPicture picTopModel = new SWfsOperationPicture();
                SWfsOperationPicture picBottomModel = new SWfsOperationPicture();
                string picSizeStr = F["moduleType"] == "2" ? "width:340,Height:220,Length:200" : "width:340,Height:442,Length:200";
                int floorId = 0;
                DateTime now = DateTime.Now;
                DateTime minTime = Convert.ToDateTime("1990/12/26 1:01:01");
                string floorName = F["floorName"];


                #region 数据验证
                if (!string.IsNullOrEmpty(F["moduleType"]) && !(string.IsNullOrEmpty(floorName) || string.IsNullOrEmpty(F["floorId"])) && int.TryParse(F["floorId"], out floorId))
                {
                    if (floorId == 0 && (sWfsIndexModuleService.GetAllFloors().Where(a => a.ADShowType == 1 || a.ADShowType == 2).Count() >= 8))
                    {
                        return "{\"status\" : 8, \"message\" : \"楼层数量最多为8个,请返回首页管理页面,点击页面顶部'首页布局管理'修改排序重复的楼层\"}";
                    }
                    string error = string.Empty;
                    if ((F["moduleType"] == "2" && (Request.Files["picFileBottom"] == null || Request.Files["picFileBottom"].ContentLength <= 0)) || (Request.Files["picFileTop"] == null || Request.Files["picFileTop"].ContentLength <= 0))
                    {
                        dic["error"] = "请选择要上传的图片";
                        return "{\"status\" : 0, \"message\" : \"数据不合法" + dic["error"] + "\", \"relationtype\":3}";
                    }
                    DateTime time = new DateTime();
                    #region 初始化图片
                    if (Request.Files["picFileTop"] != null && Request.Files["picFileTop"].ContentLength > 0)
                    {
                        dic = sWfsIndexModuleService.SaveImg(Request.Files["picFileTop"], picSizeStr);
                        if (dic.Keys.Contains("error"))
                        {
                            return "{\"status\" : 0, \"message\" : \"图片不合法" + dic["error"] + "\"}";
                        }
                        else if (dic.Keys.Contains("success")) { picTopModel.PictureFileNo = dic["success"]; }
                        picTopModel.PictureIndex = 0;
                        if (F["moduleType"] == "1")
                            picTopModel.PictureIndex = 2;
                        picTopModel.PictureName = F["picTitleTop"];
                        picTopModel.LinkAddress = F["picLinkTop"];
                        picTopModel.DataState = (short)1;
                        picTopModel.Status = 1;
                        // picTopModel.SortId = 0;
                        picTopModel.OperatorUserId = PresentationHelper.GetPassport().UserName;
                        picTopModel.WebSiteNo = "shangpin.com";
                        picTopModel.PageNo = "index";
                        picTopModel.DateCreate = now;
                        picTopModel.LinkAddress = F["picLinkTop"];
                        picTopModel.PagePositionName = module.ModuleName;
                        picTopModel.DateBegin = Convert.ToDateTime(F["startTimeTop"]);
                        picTopModel.DateEnd = minTime;
                    }
                    if (F["moduleType"] == "2" && Request.Files["picFileBottom"] != null && Request.Files["picFileBottom"].ContentLength > 0)
                    {
                        dic = sWfsIndexModuleService.SaveImg(Request.Files["picFileBottom"], picSizeStr);
                        if (dic.Keys.Contains("error"))
                        {
                            return "{\"status\" : 0, \"message\" : \"图片不合法" + dic["error"] + "\"}";
                        }
                        else if (dic.Keys.Contains("success")) { picBottomModel.PictureFileNo = dic["success"]; }
                        picBottomModel.PictureIndex = 1;
                        picBottomModel.PictureName = F["picTitleBottom"];
                        picBottomModel.DateCreate = now;
                        picBottomModel.LinkAddress = F["picLinkBottom"];
                        picBottomModel.DataState = (short)1;
                        picBottomModel.Status = 1;
                        //  picBottomModel.SortId = 1;
                        picBottomModel.OperatorUserId = PresentationHelper.GetPassport().UserName;
                        picBottomModel.WebSiteNo = string.Empty;
                        picBottomModel.PageNo = "index";
                        picBottomModel.WebSiteNo = "shangpin.com";
                        picBottomModel.PagePositionName = module.ModuleName;
                        picBottomModel.DateBegin = Convert.ToDateTime(F["startTimeTop"]);
                        picBottomModel.DateEnd = minTime;
                    }
                    #endregion
                    #region 保存操作
                    #region 初始化字段
                    if (floorId > 0)//类型转换更改广告类型切换时间 
                    {
                        module = sWfsIndexModuleService.GetSWfsIndexModuleById(floorId);
                        if (module.ADShowType.ToString() != F["moduleType"])//类型转变 不查重
                        {
                            module.ADShowTypeChangeDate = picTopModel.DateBegin;
                        }
                        if (sWfsIndexModuleService.CheckModulePictureSameTime(module.ModuleId, picTopModel.PictureIndex, picTopModel.DateBegin) > 0)
                        {
                            return "{\"status\" :0, \"message\" : \"此楼层相同广告位,已存在相同开始时间图片,请调整!\"}";
                        } 
                    }
                    else
                    {
                        module.ADShowTypeChangeDate = Convert.ToDateTime("2099-01-01");
                    }

                    module.ModuleId = floorId;
                    module.ModuleName = floorName;
                    module.Sort = string.IsNullOrEmpty(F["Sort"]) ? 0 : Convert.ToInt16(F["Sort"]);
                    /// module.ADShowType = F["moduleType"] == "2" ? (short)2 : (short)1; //关闭 由时间驱动自动更新
                    module.Stutas = 1;
                    module.DataState = (short)1;
                    if (floorId == 0)
                    {//插入 
                        module.UpdateDate = minTime;
                        module.DateCreate = now;
                        module.OperateUserId = PresentationHelper.GetPassport().UserName;
                        module.UpdateOperateUserId = string.Empty;
                    }
                    if (floorId > 0)
                    {//更新

                        module.UpdateDate = now;
                        module.UpdateOperateUserId = PresentationHelper.GetPassport().UserName;
                    }
                    #endregion
                    module = sWfsIndexModuleService.InsertOrUpdateSWfsIndexModule(module, module.ModuleId <= 0);
                    if (module.ModuleId > 0)
                    {
                        // sWfsIndexModuleService.DeleteSWfsIndexModuleRefPic(module.ModuleId); //删除图片
                        picTopModel.PagePositionNo = module.ModuleId.ToString();
                        picBottomModel.PagePositionNo = module.ModuleId.ToString();
                        sWfsIndexModuleService.InsertSwfsIndexModuleRefPic(picTopModel);//插入上图
                        if (F["moduleType"] == "2")
                            sWfsIndexModuleService.InsertSwfsIndexModuleRefPic(picBottomModel);//插入下图
                    }
                    #endregion
                }
                else
                {
                    return "{\"status\" :0, \"message\" : \"参数不正确!\"}";
                }
                #endregion
                return "{\"status\" : 1, \"message\" : \"保存成功!\"}";
            }
            catch (Exception)
            {
                return "{\"status\" : 0, \"message\" : \"保存失败,请稍后再试!\"}";
            }
        }
        #endregion
        #region 楼层链接管理
        /// <summary>
        /// 楼层链接管理
        /// </summary>
        /// <returns></returns>
        public ActionResult HomeManager_FloorLinkSet(string moduleId, string moduleName)
        {
            int id = 0;
            List<SWfsIndexModuleLink> links = new List<SWfsIndexModuleLink>();
            if (int.TryParse(moduleId, out id) && id > 0)
            {
                links = sWfsIndexModuleLinkService.GetSWfsIndexModuleLinkByModuleId(id).Where(a => a.LinkType == 1).FilterList() ?? new List<SWfsIndexModuleLink>();
            }
            ViewData["links"] = links;
            ViewBag.moduleId = moduleId;
            ViewBag.moduleName = moduleName;
            return View();
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult HomeManager_FloorLinkSet(FormCollection F)
        {
            try
            {
                int moduleId = 0;
                List<SWfsIndexModuleLink> links = new List<SWfsIndexModuleLink>();
                DateTime min = Convert.ToDateTime("1990-12-26 01:01:00");
                if (!string.IsNullOrEmpty(F["moduleId"]) && int.TryParse(F["moduleId"], out moduleId) && moduleId > 0)
                {
                    for (int i = 1; i < 7; i++)
                    {
                        #region 对象赋值
                        SWfsIndexModuleLink link = new SWfsIndexModuleLink();
                        link.LinkId = Convert.ToInt32(F["linkid" + i]);
                        if (link.LinkId > 0)
                        {
                            link = sWfsIndexModuleLinkService.GetSwfsIndexModuleLinkByLinkId(link.LinkId);
                        }
                        link.ModuleId = moduleId;
                        link.LinkName = F["word" + i];
                        link.LinkHref = F["link" + i];
                        link.CategoryNo = string.Empty;
                        link.LinkType = (short)1;
                        link.SortId = i - 1;
                        if (link.LinkId == 0)
                        {
                            link.DateCreate = DateTime.Now;
                            link.UpdateDate = min;
                            link.OperateUserId = PresentationHelper.GetPassport().UserName;
                        }
                        else
                        {
                            link.UpdateDate = DateTime.Now;
                        }
                        link.UpdateDate = Convert.ToDateTime("1991/02/10 01:01:00");
                        link.UpdateOperateUserId = link.LinkId > 0 ? PresentationHelper.GetPassport().UserName : string.Empty;
                        link.Stutas = 1;
                        link.DataState = 1;
                        #endregion
                        links.Add(link);
                    }
                    sWfsIndexModuleLinkService.InsertOrUpdateFloorSWfsIndexModuleLink(links);
                    //清除缓存
                    EnyimMemcachedClient.Instance.Remove("ComBeziWfs_SWfsIndexModuleLink_GetSWfsIndexModuleLinkByModuleId_GetSWfsIndexModuleLinkByModuleId");
                    return Json(new { status = 1, message = "操作成功!" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { status = 1, message = "数据格式不正确!" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(new { status = 0, message = "操作失败!" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #region 楼层广告图管理

        /// <summary>
        /// 楼层广告图管理列表
        /// </summary>
        /// <returns></returns>
        [ValidateInput(false)]
        public ActionResult HomeManager_FloorADPicManager(int moduleId = 0, int pageIndex = 1, int pageSize = 20)
        {
            ViewBag.moduleId = moduleId;
            SWfsIndexModule Module = sWfsIndexModuleService.GetSWfsIndexModuleById(moduleId);
            ViewBag.type = Module.ADShowType == 2 ? "双广告位" : "单广告位";
            ViewBag.ADShowType = Module.ADShowType;
            ViewBag.moduleName = Module.ModuleName;
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("PictureName", string.IsNullOrEmpty(Request.QueryString["PictureName"]) ? "" : Request.QueryString["PictureName"]);
            dic.Add("DateBegin", string.IsNullOrEmpty(Request.QueryString["DateBegin"]) ? "" : Request.QueryString["DateBegin"]);
            dic.Add("DateEnd", string.IsNullOrEmpty(Request.QueryString["DateEnd"]) ? "" : Request.QueryString["DateEnd"]);
            dic["moduleId"] = moduleId.ToString();
            int count = 0;
            ViewBag.list = sWfsIndexModuleService.GetSWfsindexModuleRefPic(dic, pageIndex, pageSize, out count);
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;
            ViewBag.TotalCount = count;

            return View();
        }
        /// <summary>
        /// 编辑楼层广告图
        /// </summary>
        /// <param name="linkId"></param>
        /// <returns></returns>
        public ActionResult HomeManager_EditFloorAD(string picId, int moduleId, int pictureIndex)
        {
            int id = 0;
            SWfsOperationPicture pic = new SWfsOperationPicture();
            SWfsIndexModule Module = sWfsIndexModuleService.GetSWfsIndexModuleById(moduleId);
            pic.PictureIndex = Convert.ToInt16(pictureIndex);
            ViewBag.sizeTip = pictureIndex == 2 ? "尺寸：340*442;格式：jpg gif;大小：150K以下" : "尺寸：340*220;格式：jpg gif;大小：150K以下";
            ViewBag.moduleId = moduleId;
            ViewBag.moduleName = Module.ModuleName;
            ViewBag.ADShowType = Module.ADShowType;
            ViewBag.PicType = pic.PictureIndex == 2 ? "单广告图" : pic.PictureIndex == 0 ? "上广告图" : "下广告图";
            if (int.TryParse(picId, out id) && id > 0)
            {
                pic = sWfsIndexModuleService.GetSWfsOperationPictureByPictureManageId(id);
                pic.PictureIndex = (short)pictureIndex;//重新定位
            }
            return View(pic);
        }
        [HttpPost]
        public string HomeManager_EditFloorAD(FormCollection F)
        {
            try
            {
                SWfsOperationPicture picTopModel = new SWfsOperationPicture();
                Dictionary<string, string> dic = new Dictionary<string, string>();
                string picSizeStr = F["PictureIndex"] == "2" ? "width:340,Height:442,Length:150" : "width:340,Height:220,Length:150";
                DateTime minTime = Convert.ToDateTime("1991/02/10 01:01:00");
                int picId = Convert.ToInt32(F["PictureManageId"]);
                string picOldFileId = "";
                if (picId > 0)
                {
                    picTopModel = sWfsIndexModuleService.GetSWfsOperationPictureByPictureManageId(picId);
                    picOldFileId = picTopModel.PictureFileNo;
                }
                if (Request.Files["picFile"] != null && Request.Files["picFile"].ContentLength > 0)
                {
                    dic = sWfsIndexModuleService.SaveImg(Request.Files["picFile"], picSizeStr);
                    if (dic.Keys.Contains("error"))
                    {
                        return "{\"status\" : 0, \"message\" : \"图片不合法" + dic["error"] + "\"}";
                    }
                    else if (dic.Keys.Contains("success")) { picTopModel.PictureFileNo = dic["success"]; }
                    if (!string.IsNullOrEmpty(picOldFileId))//删除旧图片
                    {
                        sWfsIndexModuleService.DeleteImg(picOldFileId);
                    }
                }

                picTopModel.PictureIndex = Convert.ToInt16(F["pictureIndex"]);
                picTopModel.PictureName = F["picTitleTop"];
                picTopModel.DataState = (short)1;
                picTopModel.Status = 1;
                // picTopModel.SortId = Convert.ToInt32(F["SortId"]);
                picTopModel.OperatorUserId = PresentationHelper.GetPassport().UserName;
                picTopModel.WebSiteNo = "shangpin.com";
                picTopModel.PageNo = "index";
                if (picId == 0)
                {
                    picTopModel.DateCreate = DateTime.Now;
                }
                picTopModel.PagePositionNo = F["moduleId"];
                picTopModel.LinkAddress = F["LinkAddress"];
                picTopModel.PagePositionName = F["moduleName"];
                picTopModel.DateBegin = Convert.ToDateTime(F["startTimeTop"]);
                picTopModel.DateEnd = minTime;
                //时间判定查重
                if (picId == 0 && sWfsIndexModuleService.CheckModulePictureSameTime(Convert.ToInt32(picTopModel.PagePositionNo), picTopModel.PictureIndex, picTopModel.DateBegin) > 0)
                {

                    return "{\"status\" : -1, \"message\" : \"此楼层同广告位已存在相同开始时间图片\"}";

                }
                sWfsIndexModuleService.InsertOrUpdateSWfsOperationPicture(picTopModel);
                return "{\"status\" : 1, \"message\" : \"保存成功\"}";
            }
            catch (Exception)
            {
                return "{\"status\" : -1, \"message\" : \"操作失败请刷新页面重试\"}";
            }
        }
        /// <summary>
        /// 删除楼层广告图 通过广告图ID
        /// </summary>
        /// <param name="picId"></param>
        /// <returns></returns>
        public ActionResult DeleteFloorPic(string picId)
        {
            try
            {
                int id = 0;
                if (int.TryParse(picId, out id) && id > 0)
                {
                    SWfsOperationPicture pic = sWfsIndexModuleService.GetSWfsOperationPictureByPictureManageId(id);
                    SWfsIndexModule module = sWfsIndexModuleService.GetSWfsIndexModuleById(Convert.ToInt32(pic.PagePositionNo));
                    List<SWfsOperationPicture> pics = sWfsIndexModuleService.GetAllSWfsOperationPictureByModuleId(module.ModuleId, module.ADShowType);
                    if (pic.DateBegin > DateTime.Now || pics.Count(a => a.PictureManageId == pic.PictureManageId) == 0) { }
                    else
                    {
                        if (module.ADShowType == 1 && pic.PictureIndex == 2 && pics.Count(a => a.PictureIndex == 2) <= 1)
                        {
                            return Json(new { status = -1, message = "删除无效,当前楼层为单广告位,至少保留一张有效图片" }, JsonRequestBehavior.AllowGet);
                        }
                        else if (module.ADShowType == 2 && (pic.PictureIndex == 1 || pic.PictureIndex == 0) && pics.Count(a => a.PictureIndex == pic.PictureIndex) <= 1)
                        {
                            return Json(new { status = -1, message = "删除无效,当前楼层为双广告位,至少保留两张有效图片" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    sWfsIndexModuleService.DeleteSWfsOperationPictureByPicId(id);

                }
                else
                    return Json(new { status = -1 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = -1 }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 楼层广告图设置
        /// </summary>
        /// <returns></returns>
        public ActionResult HomeManager_FloorADPicSet(string moduleId, string moduleName, int pictureManageId)
        {
            return View();
        }
        #endregion
        #region 今日style管理
        /// <summary>
        /// 设置今日style
        /// </summary>
        /// <returns></returns>
        public ActionResult HomeManager_TodayStyleSet()
        {
            SWfsIndexModule style = new SWfsIndexModule();
            SWfsIndexModuleLink link = new SWfsIndexModuleLink();
            SWfsIndexModule temp = sWfsIndexModuleService.GetSWfsIndexModuleByTodayStyle();
            if (temp != null)
            {
                style = temp;
                link = sWfsIndexModuleLinkService.GetSWfsIndexModuleLinkByModuleId(temp.ModuleId).FirstOrDefault();
                if (link == null)
                    link = new SWfsIndexModuleLink();
            }
            ViewBag.LinkId = link.LinkId;
            ViewBag.LinkName = string.IsNullOrEmpty(link.LinkName) ? "" : link.LinkName;
            ViewBag.LinkHref = link.LinkHref;
            return View(style);
        }
        /// <summary>
        /// 保存或更新今日sylte 及链接
        /// </summary>
        /// <param name="F"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveTodayStyleSet(FormCollection F)
        {
            try
            {
                DateTime now = DateTime.Now;
                DateTime min = Convert.ToDateTime("1990/12/26 01:01:00");
                SWfsIndexModule style = sWfsIndexModuleService.GetSWfsIndexModuleByTodayStyle() ?? new SWfsIndexModule();

                if (style.ModuleId > 0)
                {//更新  
                    style.UpdateDate = now;
                    style.UpdateOperateUserId = PresentationHelper.GetPassport().UserName;
                }
                else
                {
                    style.Stutas = 1;
                    style.UpdateDate = min;
                    style.DateCreate = now;
                    style.OperateUserId = PresentationHelper.GetPassport().UserName;
                }

                style.ModuleName = F["ModuleName"];
                style.DataState = 1;
                style.ADShowType = 3;
                style.UpdateDate = min;
                style.ADShowTypeChangeDate = min;
                style = sWfsIndexModuleService.InsertOrUpdateSWfsIndexModule(style, style.ModuleId <= 0);
                int linkId = (!string.IsNullOrEmpty(F["LinkId"])) ? Convert.ToInt16(F["LinkId"]) : 0;
                SWfsIndexModuleLink link = sWfsIndexModuleLinkService.GetSwfsIndexModuleLinkByLinkId(linkId) ?? new SWfsIndexModuleLink();
                link.LinkName = F["LinkName"];
                link.LinkHref = F["LinkHref"];
                link.ModuleId = style.ModuleId;
                link.DataState = 1;
                link.LinkType = 1;
                link.CategoryNo = string.Empty;
                link.SortId = 0;
                link.Stutas = 1;
                if (link.LinkId == 0)
                {
                    link.OperateUserId = PresentationHelper.GetPassport().UserName;
                    link.DateCreate = now;
                    link.UpdateDate = min;
                }
                else
                {
                    link.UpdateOperateUserId = PresentationHelper.GetPassport().UserName;
                    link.UpdateDate = now;
                }
                sWfsIndexModuleLinkService.InsertOrUpdateFloorSWfsIndexModuleLink(new List<SWfsIndexModuleLink> { link });
            }
            catch (Exception)
            {
                return Json(new { status = "0", message = "保存失败" });
            }
            return Json(new { status = "1", message = "保存成功" });
        }
        public ActionResult HomeManager_EditTodyStylePic(int ManageId, int PictureIndex)
        {
            ViewBag.PictureIndex = PictureIndex;
            ViewBag.ManageId = ManageId;
            SWfsOperationPicture pic = new SWfsOperationPicture();
            pic.PictureManageId = ManageId;
            pic.LinkAddress = string.Empty;
            if (ManageId > 0)
                pic = sWfsIndexModuleService.GetSWfsOperationPictureByPictureManageId(ManageId);
            ViewBag.titleTip = PictureIndex == 0 ? "今日STYLE左侧大图设置" : "今日STYLE专题右专题设置";
            ViewBag.sizeTip = PictureIndex == 0 ? "尺寸：466*530;格式：jpg gif;大小：150K以下" : "尺寸：480*260;格式：jpg gif;大小：150K以下";
            ViewBag.picWidth = PictureIndex == 0 ? 466 : 480;
            ViewBag.picHeight = PictureIndex == 0 ? 530 : 260;
            pic.PictureIndex = (short)PictureIndex;//重新定义位置
            return View(pic);
        }
        /// <summary>
        /// 保存今日style图片
        /// </summary>
        /// <param name="F"></param>
        /// <returns></returns>
        [HttpPost]
        public string SaveTodayStylePic(FormCollection F)
        {
            try
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                DateTime now = DateTime.Now;
                DateTime min = Convert.ToDateTime("1990/12/26 01:01:00");
                SWfsIndexModule todayStyle = sWfsIndexModuleService.GetSWfsIndexModuleByTodayStyle();
                SWfsOperationPicture pic = new SWfsOperationPicture();
                pic.PictureManageId = Convert.ToInt32(F["PictureManageId"]);
                pic.PictureIndex = Convert.ToInt16(F["PictureIndex"]);
                if (pic.PictureManageId > 0)
                {
                    pic = sWfsIndexModuleService.GetSWfsOperationPictureByPictureManageId(pic.PictureManageId);
                }
                else
                {
                    pic.DateCreate = now;
                }
                string picOldFileId = pic.PictureFileNo;//旧图片
                string picSizeStr = pic.PictureIndex == 0 ? "width:466,Height:530,Length:150" : "width:480,Height:260,Length:150";
                if (Request.Files["picFile"] != null && Request.Files["picFile"].ContentLength > 0)
                {
                    dic = sWfsIndexModuleService.SaveImg(Request.Files["picFile"], picSizeStr);
                    if (dic.Keys.Contains("error"))
                    {
                        return "{\"status\" : 0, \"message\" : \"图片不合法" + dic["error"] + "\"}";
                    }
                    else if (dic.Keys.Contains("success")) { pic.PictureFileNo = dic["success"]; }
                    if (!string.IsNullOrEmpty(picOldFileId))//删除旧图片
                    {
                        sWfsIndexModuleService.DeleteImg(picOldFileId);
                    }
                }
                else if (pic.PictureManageId == 0)
                {
                    return "{\"status\" : -1, \"message\" : \"请上传图片\"}";
                }

                pic.PictureIndex = Convert.ToInt16(F["pictureIndex"]);
                pic.PictureName = "";
                pic.LinkAddress = F["LinkAddress"];
                pic.DataState = (short)1;
                pic.Status = 1;
                pic.OperatorUserId = PresentationHelper.GetPassport().UserName;
                pic.WebSiteNo = "shangpin.com";
                pic.PageNo = "index";
                if (pic.PictureManageId == 0)
                {
                    pic.DateCreate = DateTime.Now;
                }
                pic.PagePositionName = todayStyle.ModuleName;
                pic.PagePositionNo = todayStyle.ModuleId.ToString();
                pic.DateEnd = min;
                pic.DateBegin = min;

                sWfsIndexModuleService.InsertOrUpdateSWfsOperationPicture(pic);
            }
            catch (Exception)
            {
                return "{\"status\" : -1, \"message\" : \"\"}";
            }
            return "{\"status\" : 1, \"message\" : \"保存成功\"}";
        }

        #endregion
        #region 楼层品类商品
        public ActionResult FloorCategoryPicSet(string LinkId, string SortId, string ModuleId)
        {
            ViewBag.LinkId = LinkId;
            ViewBag.SortId = SortId;
            ViewBag.ModuleId = ModuleId;
            return View();
        }
        public ActionResult InsertOrUpdateLink(int ModuleId, int LinkId, string CategoryNo, int SortId, string CategoryName)
        {
            DateTime minTime = Convert.ToDateTime("1990/12/26 1:01:01");
            SWfsIndexModuleLink link = new SWfsIndexModuleLink();
            if (LinkId == 0)
            {
                link.OperateUserId = PresentationHelper.GetPassport().UserName;
                link.DateCreate = DateTime.Now;
                link.UpdateDate = minTime;
            }
            else
            {
                link = sWfsIndexModuleLinkService.GetSwfsIndexModuleLinkByLinkId(LinkId);
                link.UpdateOperateUserId = PresentationHelper.GetPassport().UserName;
                link.UpdateDate = DateTime.Now;
            }
            link.CategoryNo = CategoryNo;
            link.SortId = SortId;
            link.DataState = 1;
            link.LinkType = 2;
            link.LinkName = CategoryName;
            link.ModuleId = ModuleId;
            link.Stutas = 1;
            int result = sWfsIndexModuleLinkService.InsertOrUpdateLink(ref link, link.LinkId == 0);
            return Json(new { status = result, LinkId = link.LinkId }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 根据ParentNo获取分类集合 categrys
        /// </summary>
        /// <param name="ParentNo"></param>
        /// <returns></returns>
        public ActionResult GetSWfsCategoryByParentNo(string ParentNo = "")
        {
            List<Category> categorys = OCSCategoryManager.Categorys.Where(a => a.ParentCategoryNo == ParentNo).Select(b => new Category { ParentNo = b.ParentCategoryNo, CategoryName = b.CategoryName, CategoryNo = b.value, Sort = 1 }).ToList();
            if (categorys == null || categorys.Count == 0)
            {
                List<SpfShopCategory>  shopCategorys = sWfsIndexModuleService.GetSWfsCategoryByParentNo(ParentNo).OrderBy(b => b.SortId).ToList();
                return Json(new { data = shopCategorys.Select(a => new { a.CategoryNo, CategoryName = a.CategoryCName }) }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { data = categorys.Select(a => new { a.CategoryNo, CategoryName = a.CategoryName }) }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region 楼层排序
        /// <summary>
        /// 楼层排序
        /// </summary>
        /// <returns></returns>
        public ActionResult HomeManager_FloorSort()
        {
            ViewData["floors"] = sWfsIndexModuleService.GetAllFloors();
            return View();
        }
        [HttpPost]
        public ActionResult SaveFloorSort(FormCollection F)
        {
            List<SWfsIndexModule> modules = new List<SWfsIndexModule>();
            foreach (string key in F.AllKeys)
            {
                if (key.EndsWith("floor"))
                {
                    string moduleid = F[key].Split('_')[0];
                    string sort = F[key].Split('_')[1];
                    string Stutas = F[key].Split('_')[2];
                    modules.Add(new SWfsIndexModule
                    {
                        Sort = Convert.ToInt32(sort),
                        ModuleId = Convert.ToInt32(moduleid),
                        Stutas = Convert.ToInt16(Stutas)
                    });
                }
            }
            if (sWfsIndexModuleService.UpdateSWfsIndexModuleSort(modules) > 0)
            {
                //清除缓存
                EnyimMemcachedClient.Instance.Remove("ComBeziWfs_SWfsIndexModule_GetSWfsIndexModuleIncludeTodayStyle_GetAllFloors");
                return Json(new { status = 1 });
            }
            return Json(new { status = 0 });
        }

        #endregion
        #endregion

        #region 品牌墙管理

        /// <summary>
        /// 已选择品牌列表
        /// </summary>
        /// <param name="brandNoAndBrandName"></param>
        /// <param name="channelNo"></param>
        /// <param name="pagePositionNo"></param>
        /// <returns></returns>
        public ActionResult BrandSelectList(string pageNo = "index", string pagePositionNo = "", int isRecommendBrand = 0)
        {
            string startTime = Rq.GetStringQueryString("starttime");
            string endTime = Rq.GetStringQueryString("endtime");
            string brandNoAndBrandName = Rq.GetStringQueryString("brandNoAndBrandName");
            SWfsBrandWallService brandWallService = new SWfsBrandWallService();

            IEnumerable<SpHomeRecommendBrandExtends> list = brandWallService.GetBrandList(pageNo, pagePositionNo, isRecommendBrand, startTime, endTime, brandNoAndBrandName);
            //已添加品牌总数
            ViewBag.BrandCount = list == null ? 0 : list.Count();
            return View(list);
        }
        /// <summary>
        /// 推荐品牌列表
        /// </summary>
        /// <param name="pageNo"></param>
        /// <param name="pagePositionNo"></param>
        /// <param name="isRecommendBrand"></param>
        /// <returns></returns>
        public ActionResult RecomemendBrandSelectList(string pageNo = "index", string pagePositionNo = "", int isRecommendBrand = 1, int pageIndex = 1, int pageSize = 20)
        {
            int total = 0;
            //PaginationInfoModel pageinfo = new PaginationInfoModel();
            string startTime = Rq.GetStringQueryString("starttime");
            string endTime = Rq.GetStringQueryString("endtime");
            string brandNoAndBrandName = Rq.GetStringQueryString("brandNoAndBrandName");
            //int currentPage = Rq.GetIntQueryString("pageindex");
            int status = Convert.ToInt32(string.IsNullOrEmpty(Rq.GetStringQueryString("status")) ? "-1" : Rq.GetStringQueryString("status"));
            int pictureIndex = Convert.ToInt32(string.IsNullOrEmpty(Rq.GetStringQueryString("PictureIndex")) ? "-1" : Rq.GetStringQueryString("PictureIndex"));
            //string pagePositionNo = Rq.GetStringQueryString("PagePositionNo");
            //string startTimeValue = string.Empty, endTimeValue = string.Empty;
            SWfsBrandWallService brandWallService = new SWfsBrandWallService();


            IEnumerable<SpHomeRecommendBrandExtends> list = brandWallService.GetSWfsSpHomeRecommendBrandList(pageNo, pagePositionNo, startTime, endTime, brandNoAndBrandName, status, pictureIndex,pageIndex, pageSize, out total);
            //ViewBag.PictureManagerList = picManagerList;
            //ViewBag.PaginationInfoSingel = pageinfo;
            //ViewBag.KeyWord = keyWord;
            ViewBag.Status = status;
            ViewBag.StartTime = startTime;
            ViewBag.EndTime = endTime;
            ViewBag.PictureIndex = pictureIndex.ToString();
            ViewBag.PagePositionNo = pagePositionNo;
            ViewBag.PageNo = pageNo;
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;
            
            //IEnumerable<SpHomeRecommendBrandExtends> list = brandWallService.GetBrandList(pageNo, pagePositionNo, isRecommendBrand, startTime, endTime, brandNoAndBrandName);
            //已添加品牌总数
            ViewBag.totalCount = total;
            return View(list);
        }
        /// <summary>
        /// 添加品牌 
        /// </summary>
        /// <returns></returns>
        public ActionResult AddBrands()
        {
            return View();
        }

        /// <summary>
        /// 添加品牌
        /// </summary>
        /// <param name="channelNo"></param>
        /// <param name="brandNo"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddBrands(string pageNo, string pagePositionName, string pagePositionNo)
        {
            SWfsBrandWallService service = new SWfsBrandWallService();

            string brandSelectedValue = Request.Params["brandSelected"] ?? "";
            if (string.IsNullOrEmpty(brandSelectedValue))
            {
                return Json(new { status = 1, message = "请选择品牌" });
            }

            //干掉最后一个“，”
            brandSelectedValue = brandSelectedValue.Remove(brandSelectedValue.Length - 1);

            #region 判断库里的数据和当前页面已选择的品牌总数是否大于等于24个

            int LimiteNum = 0;
            switch (pagePositionNo)
            {
                case "PopularBrand":
                case "InternationalBrand":
                    LimiteNum = 24;
                    break;
                default:
                    LimiteNum = 7;
                    break;
            }
            var brandList = service.GetBrandList(pageNo, pagePositionNo, 0, "", "", "");
            if (brandList != null && brandList.Count() > 0)
            {
                if (brandList.Count() + brandSelectedValue.Split(',').Except(new string[] { "" }).Count() < LimiteNum)
                {
                    return Json(new { status = 1, message = "选择品牌数量必须大于等于" + LimiteNum + "个" });
                }
            }
            else
            {
                if (brandSelectedValue.Split(',').Except(new string[] { "" }).Count() < LimiteNum)
                {
                    return Json(new { status = 1, message = "选择品牌数量必须大于等于" + LimiteNum + "个" });
                }
            }
            #endregion

            var brands = brandSelectedValue.Split(',');

            foreach (var brandNo in brands)
            {
                SWfsSpHomeRecommendBrand recommentBrand = new SWfsSpHomeRecommendBrand()
                {
                    PagePositionNo = pagePositionNo,
                    IsRecommendBrand = 0,
                    CreateDate = DateTime.Now,
                    DataState = 1,
                    UpdateDate = DateTime.Now,
                    BrandNO = brandNo,
                    Status = 1,
                    OperateUserId = PresentationHelper.GetPassport().UserName,
                    PageNo = "index",
                    PictureFileNo = "",
                    UpdateOperateUserId = "",
                    WebSiteNo = "shangpin.com",
                    PagePositionName = pagePositionName,
                    DateBegin = DateTime.Now
                };

                service.InsertSWfsSpChannelRecommendBrand(recommentBrand);
            }
            EnyimMemcachedClient.Instance.Remove("ComBeziWfs_SWfsSpHomeRecommendBrand_GetHomePageRecommendByPageNo_GetHomePageRecommendBrand");
            return Json(new { status = 2, message = "保存成功" });
        }

        /// <summary>
        /// 推荐品牌管理 
        /// </summary>
        /// <returns></returns>
        public ActionResult RecommendBrandManager()
        {
            return View();
        }

        /// <summary>
        /// 获取当前可用楼层
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetUsableFloor()
        {
            SWfsBrandWallService service = new SWfsBrandWallService();
            var floors = service.GetUsableFloor().FilterList();

            if (floors.FilterList() == null || floors.Count == 0)
            {
                return Json(new { status = 1, message = "当前没有可有楼层" });
            }

            return Json(new { status = 2, Data = floors });
        }

        #region 首页推荐品牌
        //推荐品牌列表查询
        public ActionResult IndexBrandList(string channelNo, string brandNoAndBrandName, string startTime, string endTime, int pageIndex = 1, int pageSize = 200)
        {
            SWfsPictureManagerService picService = new SWfsPictureManagerService();
            int total = 0;
            IEnumerable<ChannelRecommendBrandExtends> list = picService.GetHomeRecommendBrand(channelNo, brandNoAndBrandName,
                startTime, endTime, pageIndex, pageSize, out total);
            ViewBag.totalCount = total;
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;
            return View(list);
        }
        //添加首页推荐品牌
        public ActionResult AddHomeBrand()
        {
            SWfsBrandWallService service = new SWfsBrandWallService();
            string pageNo = Rq.GetStringQueryString("PageNo");
            string pagePositionNo = Rq.GetStringQueryString("PagePositionNo");
            string pagePositionName = Rq.GetStringQueryString("PagePositionName");
            string recommendBrandID=Rq.GetStringQueryString("recommendBrandID");
            string isEdit = Rq.GetStringQueryString("edit");
            if (isEdit == "1")
            {
                var list = service.GetBrandByPk(Convert.ToInt32(recommendBrandID));
                if (list == null||list.Count()==0)
                    return View();
                return View(list);
            }
            return View();
        }
        [HttpPost]
        public JsonResult SaveHomeBrand()
        {
            string pageNo = Rq.GetStringForm("pageNo");
            string pagePositionNo = Rq.GetStringForm("pagePositionNo");
            string pagePositionName = Rq.GetStringForm("pagePositionName");
            string brandNo = Rq.GetStringForm("brandNo");
            string pictureTitle = Rq.GetStringForm("pictureTitle");
            string pictureStatus = Rq.GetStringForm("pictureStatus");
            string brandPosition = Rq.GetStringForm("brandPosition");
            string DateBegin = Rq.GetStringForm("DateBegin");
            int PrimaryKey = Convert.ToInt32(Rq.GetStringForm("PrimaryKey"));
            string PictureFileNo = Rq.GetStringForm("PictureFileNo");
            string recommendBrandID = Rq.GetStringForm("RecommendBrandID");
            SWfsBrandWallService service = new SWfsBrandWallService();
            if (string.IsNullOrEmpty(pageNo))
            {
                return Json(new { result = "0", message = "页面编号错误！" });
            }
            if (string.IsNullOrEmpty(brandNo))
            {
                return Json(new { result = "0", message = "品牌编号不能为空" });
            }
            if (service.IsExistHomeBrand(pageNo, pagePositionNo, brandNo, recommendBrandID) > 0)
            {
                return Json(new { result = "0", message = "已存在该品牌" });
            }
            if (service.IsExistHomeBrand(pageNo, pagePositionNo, brandNo) > 200)
            {
                return Json(new { result = "0", message = "不能添加超过200个品牌" });
            }
            //string PictureFileNo = string.Empty;
            if (Request.Files["FlapHotTwo"] != null && Request.Files["FlapHotTwo"].ContentLength > 0)
            {
                string picSizeStr = "width:{0},Height:{1},Length:500";

                picSizeStr = string.Format(picSizeStr, 168, 260);

                PictureFileNo = SaveImg(Request.Files["FlapHotTwo"], picSizeStr);
                if (rsPic.Keys.Contains("error"))
                {
                    return Json(new { result = "-1", message = "图片不合法" + rsPic["error"] + "" }, "text/html");
                }
            }

            SWfsSpHomeRecommendBrand recommentBrand = new SWfsSpHomeRecommendBrand()
            {
                RecommendBrandID = PrimaryKey,
                PagePositionNo = pagePositionNo,
                IsRecommendBrand = 1,
                CreateDate = DateTime.Now,
                DataState = 1,
                UpdateDate = DateTime.Now,
                BrandNO = brandNo,
                Status = Convert.ToInt16(pictureStatus),
                OperateUserId = PresentationHelper.GetPassport().UserName,
                PageNo = "index",
                PictureFileNo = PictureFileNo,
                UpdateOperateUserId = "",
                WebSiteNo = "shangpin.com",
                PagePositionName = pagePositionName,
                DateBegin = Convert.ToDateTime(DateBegin),
                PictureFileTitle = pictureTitle,
                PictureIndex = Convert.ToInt32(brandPosition)
            };

            if (PrimaryKey <= 0)
            {
                if(service.InsertSWfsSpHomeRecommendBrand(recommentBrand)>0)
                {
                    return Json(new { result = 1, message = "添加成功" });
                }
                else
                {
                    return Json(new { result = 0, message = "添加失败" }); 
                }
            }
            else
            {
                if(service.UpdateSWfsSpHomeRecommendBrand(recommentBrand))
                {
                    return Json(new { result = 1, message = "修改成功" });
                }
                else
                {
                    return Json(new { result = 0, message = "修改失败" });
                }
            }
        }
        //按主键删除首页推荐品牌
        public ActionResult DeleteHomeBrandByID(int id, string pageNo, int brandCount, string pagePositionNo)
        {
            SWfsBrandWallService service = new SWfsBrandWallService();
            brandCount = service.GetHomePageRecommendBrand(pageNo, pagePositionNo).Count();
            int LimiteNum = 0;
            switch (pagePositionNo)
            {
                case "PopularBrand":
                case "InternationalBrand":
                    LimiteNum = 24;
                    break;
                default:
                    LimiteNum = 7;
                    break;
            }
            if ((brandCount <= LimiteNum) && pagePositionNo.IndexOf("RecommendBrand")<0)
            {
                return Json(new { status = 0, message = "品牌最少为" + LimiteNum + "个,删除无效" });
            }
            else
            {
                SWfsOperationPictureService picSercive = new SWfsOperationPictureService();
                picSercive.DeleteHomeBrand(id);
            }
            //return Redirect("/Shangpin/HomePage/BrandSelectList?PageNo=" + Request.QueryString["PageNo"] + "&PagePositionNo=" + Request.QueryString["PagePositionNo"] + "&PagePositionName=" + Request.QueryString["PagePositionName"] + "&alertStr=" + deleteStr);
            return Json(new { status = 1, message = "删除成功" });
        }
        //按主键修改排序值
        public JsonResult EditeSortValue(int id, int sortValue)
        {
            SWfsPictureManagerService picSercive = new SWfsPictureManagerService();
            if (picSercive.EditeBrandSortValue(id, sortValue))
            {
                return Json(new { status = 0, message = "修改成功" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = 1, message = "修改失败" }, JsonRequestBehavior.AllowGet);
            }
        }
        //批量修改排序值
        public JsonResult EditeListSortValue()
        {
            try
            {
                SWfsBrandWallService picSercive = new SWfsBrandWallService();
                if (string.IsNullOrEmpty(Request.Form["bList"]))
                {
                    return Json(new { status = 1, message = "品牌编号不存在" }, JsonRequestBehavior.AllowGet);
                }
                string[] bList = Request.Form["bList"].Split(',');
                string[] bIndex = Request.Form["bIndex"].Split(',');
                for (int i = 0; i < bList.Length; i++)
                {
                    picSercive.EditeBrandSortValue(int.Parse(bList[i]), int.Parse(bIndex[i]));
                }
                return Json(new { status = 0, message = "修改成功" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(new { status = 1, message = "修改失败" + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult HomeRecommendBrandModify(string SubjectNo, string status, string DelectStatus)
        {
            bool rs = false;
            string messageStr = string.Empty;
            SWfsBrandWallService service = new SWfsBrandWallService();
            SWfsSpHomeRecommendBrand model = service.GetHomeBrandByPK(Convert.ToInt32(SubjectNo));

            if (model != null)
            {
                    model.Status = Convert.ToInt16(status);
                    rs = service.UpDateHomeBrand(model);
                    messageStr = "状态修改成功";
            }
            try
            {
                if (rs)
                {
                    EnyimMemcachedClient.Instance.Remove("ComBeziWfs_SWfsSpHomeRecommendBrand_GetHomePageRecommendByPageNo_GetHomePageRecommendBrand_" + model.PageNo + "_" + model.PagePositionNo);
                    return Json(new { result = "1", message = messageStr });
                }
                else
                { return Json(new { result = "0", message = "操作失败！" }); }
            }
            catch (Exception ex)
            {
                return Json(new { result = "0", message = "操作失败！" });
            }
        }
        #endregion

        #endregion

        #region 导航管理

        /// <summary>
        /// 导航管理
        /// </summary>
        /// <returns></returns>
        public ActionResult NavManger()
        {
            SWfsPictureManagerService picService = new SWfsPictureManagerService();
            List<SWfsNavigationManage> list = picService.GetNavigaList();
            return View(list);
        }

        /// <summary>
        ///添加导航
        /// </summary>
        /// <param name="name"></param>
        /// <param name="link"></param>
        /// <returns></returns>
        public string AddNavManger(string name, string link)
        {
            string result = "0";
            SWfsPictureManagerService picService = new SWfsPictureManagerService();
            result = picService.AddNaviga(name, link) + "";
            return result;
        }

        /// <summary>
        /// 修改信息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="link"></param>
        /// <param name="nid"></param>
        /// <returns></returns>
        public string EidtNavManger(string name, string link, string nid)
        {
            string result = "";
            SWfsPictureManagerService picService = new SWfsPictureManagerService();
            result = picService.UpdateNavigaInfo(name, link, nid) + "";
            return result;
        }
        /// <summary>
        /// 删除导航信息
        /// </summary>
        /// <param name="nid"></param>
        /// <returns></returns>
        public string delNavManger(string nid)
        {
            string result = "";
            SWfsPictureManagerService picService = new SWfsPictureManagerService();
            result = picService.delNavigaInfoById(nid) + "";
            return result;
        }
        /// <summary>
        /// 移动节点
        /// </summary>
        /// <param name="newid"></param>
        /// <param name="newsortid"></param>
        /// <param name="oldid"></param>
        /// <param name="oldsortid"></param>
        /// <returns></returns>
        public string MoveNavg(string newid, string newsortid, string oldid, string oldsortid)
        {
            string result = "";
            SWfsPictureManagerService picService = new SWfsPictureManagerService();
            result = picService.MoveNaviga(newid, oldsortid) + "";
            result = picService.MoveNaviga(oldid, newsortid) + "";
            return result;
        }

        #endregion

        #region 本周热门商品管理
        public ActionResult IndexHotProductList(string categoryNo="NV001")
        {
            SWfsIndexHotProductListTempService service = new SWfsIndexHotProductListTempService();
            NewsChannelsSservice ncService = new NewsChannelsSservice();
            List<IndexHotProductInfo> list = service.GetIndexHotProudecList(categoryNo);
            foreach (var item in list)
            {
                ProductInventoryShoesNew inventory = ncService.GetInventoryByProductNo(item.ProductNo);
                item.Quantity = inventory.SumQuantity;
                item.LockQuantity = inventory.SumLockQuantity;
            }
            return View(list);
        }

        #region 修改热门商品排序值
        public JsonResult UpdateSortValue()
        {
            try
            {
                SWfsIndexHotProductListTempService sercive = new SWfsIndexHotProductListTempService();
                if (string.IsNullOrEmpty(Request.Form["idList"]))
                {
                    return Json(new { status = 1, message = "商品编号不存在" }, JsonRequestBehavior.AllowGet);
                }
                string[] idList = Request.Form["idList"].TrimEnd(',').Split(',');
                string[] sortList = Request.Form["sortList"].TrimEnd(',').Split(',');
                for (int i = 0; i < idList.Length; i++)
                {
                    sercive.UpdateHotProductSortValue(Convert.ToInt32(idList[i].ToString()), idList.Length - i);
                }
                return Json(new { status = 0, message = "修改成功" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(new { status = 1, message = "修改失败" + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #region 删除热门商品操作
        public ActionResult DelIndexHotProductList(string idStr)
        {
            idStr = idStr.Trim().TrimEnd(',');
            SWfsIndexHotProductListTempService service = new SWfsIndexHotProductListTempService();
            try
            {
                service.DelHotProductProduct(idStr);
                return Json(new { result = "success", message = "删除成功。" }); 
            }
            catch (Exception ex)
            {
                return Json(new { result = "error", message = ex.Message });
            }
        }
        #endregion

        #region 添加热门商品
        public ActionResult AddIndexHotProductList(string HotCategoryNo, int pageIndex = 1, int pageSize = 20)
        {
            SWfsIndexHotProductListTempService service = new SWfsIndexHotProductListTempService();
            int total = 0;
            if (Request.QueryString["keyWord"] != null)
            {
                ViewBag.keyWord = Request.QueryString["keyWord"];
            }
            if (Request.QueryString["CategoryNo"] != null)
            {
                ViewBag.category = Request.QueryString["CategoryNo"];
            }
            if (Request.QueryString["Gender"] != null)
            {
                ViewBag.Gender = Request.QueryString["Gender"];
            }
            if (Request.QueryString["brandNO"] != null)
            {
                ViewBag.BrandNO = Request.QueryString["brandNO"];
            }
            if (Request.QueryString["startDate"] != null && Request.QueryString["startDate"] != "")
            {
                ViewBag.StartDateShelf = Request.QueryString["startDate"];
            }
            if (Request.QueryString["endDate"] != null && Request.QueryString["endDate"] != "")
            {
                string endtime = Request.QueryString["endDate"];
                ViewBag.EndDateShelf = Convert.ToDateTime(endtime).AddDays(1).AddSeconds(-1).ToString();
            }
            List<IndexHotProductInfo> list = null;
            if (string.IsNullOrEmpty(ViewBag.keyWord) && string.IsNullOrEmpty(ViewBag.category) && string.IsNullOrEmpty(ViewBag.Gender) && string.IsNullOrEmpty(ViewBag.BrandNO)
                && string.IsNullOrEmpty(ViewBag.StartDateShelf) && string.IsNullOrEmpty(ViewBag.EndDateShelf))
            {
                list = new List<IndexHotProductInfo>();
            }
            else
            {
                NewsChannelsSservice ncService = new NewsChannelsSservice();
                list = service.GetSWfsProductList(ViewBag.Gender, ViewBag.BrandNO, ViewBag.category, ViewBag.keyWord, ViewBag.StartDateShelf, ViewBag.EndDateShelf, pageIndex, pageSize, out total);
                foreach (var item in list)
                {
                    ProductInventoryShoesNew inventory = ncService.GetInventoryByProductNo(item.ProductNo);
                    item.Quantity = inventory.SumQuantity;
                    item.LockQuantity = inventory.SumLockQuantity;
                }
            }
            //查询当前该类别下已经有的热门商品的总数
            List<IndexHotProductInfo> newProList = service.GetIndexHotProudecList(HotCategoryNo);
            ViewBag.NrePcount = newProList.Count;
            ViewBag.page = pageIndex;
            ViewBag.pageSize = pageSize;
            ViewBag.totalCount = total;
            return View(list);
        }
        /// <summary>
        /// ajax调用的添加上新商品的方法
        /// </summary>
        /// <param name="newArrivalId"></param>
        /// <param name="ProductNoStr"></param>
        /// <returns></returns>
        public ActionResult AddHotProduct(string categoryNo, string ProductNoStr)
        {
            SWfsIndexHotProductListTempService service = new SWfsIndexHotProductListTempService();
            ProductNoStr = ProductNoStr.Trim().TrimEnd(',');
            Passport passport = PresentationHelper.GetPassport();
            try
            {
                string[] pnolist = ProductNoStr.Split(',');
                foreach (string pno in pnolist)
                {
                    List<SWfsIndexHotProductListTemp> hotproductlist = service.GetIndexHotProductByProductNo(categoryNo, pno);
                    if (null != hotproductlist && hotproductlist.Count()>0)   //判断同一类别下的商品是否有重复添加
                    {
                        return Json(new { result = "error", message = "对不起，该商品编号:" + pno + "已添加,请重新选择。" });
                    }
                }
                service.AddHotProduct(categoryNo, ProductNoStr, passport.UserName);
                return Json(new { result = "success", message = "添加成功" }); 
            }
            catch (Exception ex)
            {
                return Json(new { result = "error", message = "添加失败，失败原因：" + ex.Message });
            }
        }
        #endregion 
        #endregion
    }
}
