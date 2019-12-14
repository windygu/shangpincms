using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Shangpin.Ocs.Service.Common;
using Shangpin.Entity.Wfs;
using Shangpin.Ocs.Service.Shangpin;
using Shangpin.Ocs.Service;
using Shangpin.Ocs.Entity.Extenstion.ShangPin;
using Shangpin.Ocs.Entity.Extenstion.Outlet;
using Shangpin.Ocs.Entity.Extenstion.Login;
using Shangpin.Framework.Common.Cache;
namespace Shangpin.Ocs.Web.Areas.Shangpin.Controllers
{
    public class NewShelfController : Controller
    {
        #region 上新活动图管理
        /// <summary>
        /// 上新活动图列表
        /// </summary>
        /// <param name="picName"></param>
        /// <param name="picPosition"></param>
        /// <param name="status"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="channelno"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult NewActivityList(string picName = "", string status = "", string startTime = "", string endTime = "", int pageIndex = 1, int pageSize = 20)
        {
            SWfsChannelService service = new SWfsChannelService();
            int total = 0;
            status = status == "-1" ? "" : status;
            string picPosition = "";
            string  endTimeTemp="";
            if (!string.IsNullOrEmpty(endTime))
            {
                endTimeTemp = DateTime.Parse(endTime).AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss");
            }
            IList<SWfsNewAlterPicInfo> list = service.GetNewAlterPictureList(picName.Trim(), picPosition, status, startTime, endTimeTemp, pageIndex, pageSize, out total);
            if (list != null && list.Count() > 0)
            {
                ViewBag.List = list;
            }
            else
            {
                ViewBag.List = new List<SWfsNewAlterPicInfo>();
            }
            ViewBag.PicName = picName ?? "";
            ViewBag.Status = status ?? "";
            ViewBag.StartTime = startTime ?? "";
            ViewBag.EndTime = endTime ?? "";
            ViewBag.TotalCount = total;
            ViewBag.PageIndex = pageIndex;
            ViewBag.PageSize = pageSize;

            return View();
        }

        #region 创建活动图操作
        /// <summary>
        /// 添加上新活动图
        /// </summary>
        /// <returns></returns>
        public ActionResult NewActivityCreate()
        {
            return View();
        }
        [HttpPost]
        public ActionResult NewActivityCreateManage()
        {
            SWfsChannelService service = new SWfsChannelService();

            #region 获取参数信息
            string brandNo = Request.Form["BrandNo"].Trim();//品牌编号
            string picName = Request.Params["PicName"].Trim();//图片名称
            string pcPicture = "";//主站图片
            string pcUrl = Request.Params["NewPicFileUrlPC"].Trim();//主站图片链接地址
            string mobilePicture = "";//移动端图片
            string mobileUrl = Request.Params["NewPicFileUrlMobile"].Trim();//移动端图片链接地址
            int mobilePicType = Convert.ToInt32(Request.Params["PicType"]);//移动端图片类型
            short status = Convert.ToInt16(Request.Params["Status"]);//活动图状态
            DateTime startTime = Convert.ToDateTime(Request.Params["DateBegin"]);//开始时间
            #endregion
            #region 验证图片
            //网站端图片
            if (null != Request.Files["NewPicFilePC"] && Request.Files["NewPicFilePC"].ContentLength > 0)
            {
                CommonService commonService = new CommonService();
                Dictionary<string, string> rsPic = commonService.PostImg(Request.Files["NewPicFilePC"], "width:770,Height:320,Length:500");
                if (rsPic.Keys.Contains("error"))
                {
                    return Json(new { result = "error", message = rsPic["error"] });
                }
                if (rsPic.Keys.Contains("success"))
                {
                    pcPicture = rsPic["success"];
                }
            }
            //移动端图片
            if (null != Request.Files["NewPicFileMobile"] && Request.Files["NewPicFileMobile"].ContentLength > 0)
            {
                CommonService commonService = new CommonService();
                string imgSize = "width:0,Height:0,Length:0";
                if (mobilePicType == 1)
                {
                    imgSize = "width:393,Height:759,Length:0";//长图
                }
                else if (mobilePicType == 2)
                {
                    imgSize = "width:640,Height:426,Length:0";//宽图
                }
                Dictionary<string, string> rsMobilePic = commonService.PostImg(Request.Files["NewPicFileMobile"], imgSize);
                if (rsMobilePic.Keys.Contains("error"))
                {
                    return Json(new { result = "error", message = rsMobilePic["error"] });
                }
                if (rsMobilePic.Keys.Contains("success"))
                {
                    mobilePicture = rsMobilePic["success"];
                }
            }
            #endregion
            #region 验证品牌
            BrandInfo brandInfo = service.GetBrandInfoById(brandNo);
            if (brandInfo == null)
            {
                return Json(new { result = "error", message = "对不起，该品牌不存在，请重新选择。" });
            }
            #endregion

            /*List<SWfsNewAlterPicInfo> newAlterPicList = service.GetNewAlterPicInfoByNo(brandNo, startTime.ToString("yyyy-MM-dd"));
            if (newAlterPicList != null && newAlterPicList.Count() > 0)
            {
                return Json(new { result = "error", message = "对不起，同一个品牌在同一天不能出现两次。" });
            }*/

            SWfsNewAlterPicture model = new SWfsNewAlterPicture();
            model.PictureName = picName;
            model.Position = 0;
            model.BrandNo = brandNo;
            model.PcPictureNo = pcPicture;
            model.PcPictureLinkUrl = pcUrl;
            model.MobilePictureNo = mobilePicture;
            model.MobilePictureLinkUrl = mobileUrl;
            model.MobilePictureType = mobilePicType;
            model.Status = status;
            model.DataStatus = 0;
            model.BeginDate = startTime;
            model.DateCreate = DateTime.Now;
            model.OperatorUserId = PresentationHelper.GetPassport().UserName;

            try
            {
                service.Insert(model);
                return Json(new { result = "success", message = "添加成功。" });
            }
            catch (Exception ex)
            {
                return Json(new { result = "error", message = ex.Message });
            }
        }
        #endregion

        #region 编辑活动图操作
        /// <summary>
        /// 编辑活动图
        /// </summary>
        /// <returns></returns>
        public ActionResult NewActivityEdit()
        {
            SWfsChannelService service = new SWfsChannelService();
            int picid = Convert.ToInt32(Request.QueryString["id"]);
            SWfsNewAlterPicInfo model = service.GetNewAlterPicInfoById(picid);
            if (null == model)
            {
                model = new SWfsNewAlterPicInfo();
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult NewActivityEditManage()
        {
            SWfsChannelService service = new SWfsChannelService();
            int picid = Convert.ToInt32(Request["PictureId"]);
            //获取活动图信息
            SWfsNewAlterPicInfo model = service.GetNewAlterPicInfoById(picid);

            #region 获取参数信息
            string brandNo = Request.Form["BrandNo"].Trim();//品牌编号
            string picName = Request.Params["PicName"].Trim();//图片名称
            string pcPicture = "";//主站图片
            string pcUrl = Request.Params["NewPicFileUrlPC"].Trim();//主站图片链接地址
            string mobilePicture = "";//移动端图片
            string mobileUrl = Request.Params["NewPicFileUrlMobile"].Trim();//移动端图片链接地址
            int mobilePicType = Convert.ToInt32(Request.Params["PicType"]);//移动端图片类型
            short status = Convert.ToInt16(Request.Params["Status"]);//活动图状态
            DateTime startTime = Convert.ToDateTime(Request.Params["DateBegin"]);//开始时间
            #endregion
            #region 验证图片
            //网站图片
            if (Request.Files["NewPicFilePC"] == null || string.IsNullOrEmpty(Request.Files["NewPicFilePC"].FileName))
            {
                if (!string.IsNullOrEmpty(model.PcPictureNo))
                {
                    pcPicture = Request.Params["hidPcPic"].ToString();//获取隐藏域中的值
                }
                else
                {
                    return Json(new { result = "error", message = "请上传图片！" });
                }
            }
            else
            {
                CommonService commonService = new CommonService();
                Dictionary<string, string> rsPic = commonService.PostImg(Request.Files["NewPicFilePC"], "width:770,Height:320,Length:500");
                if (rsPic.Keys.Contains("error"))
                {
                    return Json(new { result = "error", message = rsPic["error"] });
                }
                if (rsPic.Keys.Contains("success"))
                {
                    pcPicture = rsPic["success"];
                }
            }
            //移动端图片
            if (Request.Files["NewPicFileMobile"] == null || string.IsNullOrEmpty(Request.Files["NewPicFileMobile"].FileName))
            {
                if (!string.IsNullOrEmpty(model.MobilePictureNo))
                {
                    if (Request.Params["hidMobilePicType"] == mobilePicType.ToString())
                    {
                        mobilePicture = Request.Params["hidMobilePic"].ToString();//获取隐藏域中的值
                    }
                    else
                    {
                        if (mobilePicType == 1)
                        {
                            return Json(new { result = "error", message = "请上传393*759图片！" });
                        }
                        else if (mobilePicType == 2)
                        {
                            return Json(new { result = "error", message = "请上传640*426图片！" });
                        }
                    }
                }
                else
                {
                    return Json(new { result = "error", message = "请上传图片！" });
                }
            }
            else
            {
                CommonService commonService = new CommonService();
                string imgSize = "width:0,Height:0,Length:0";
                if (mobilePicType == 1)
                {
                    imgSize = "width:393,Height:759,Length:0";//长图
                }
                else if (mobilePicType == 2)
                {
                    imgSize = "width:640,Height:426,Length:0";//宽图
                }
                Dictionary<string, string> rsMobilePic = commonService.PostImg(Request.Files["NewPicFileMobile"], imgSize);
                if (rsMobilePic.Keys.Contains("error"))
                {
                    return Json(new { result = "error", message = rsMobilePic["error"] });
                }
                if (rsMobilePic.Keys.Contains("success"))
                {
                    mobilePicture = rsMobilePic["success"];
                }
            }
            #endregion
            #region 验证品牌
            BrandInfo brandInfo = service.GetBrandInfoById(brandNo);
            if (brandInfo == null)
            {
                return Json(new { result = "error", message = "对不起，该品牌不存在，请重新选择。" });
            }
            #endregion

            /*List<SWfsNewAlterPicInfo> newAlterPicList = service.GetNewAlterPicInfoByNo(brandNo, startTime.ToString("yyyy-MM-dd"));
            if (newAlterPicList != null && newAlterPicList.Count() > 0)
            {
                if (model.BrandNo == brandNo && model.BeginDate == startTime)
                {
                }
                else
                {
                    return Json(new { result = "error", message = "对不起，同一个品牌在同一天不能出现两次。" });
                }
            }*/

            model.PictureName = picName;
            model.Position = 0;
            model.BrandNo = brandNo;
            model.PcPictureNo = pcPicture;
            model.PcPictureLinkUrl = pcUrl;
            model.MobilePictureNo = mobilePicture;
            model.MobilePictureLinkUrl = mobileUrl;
            model.MobilePictureType = mobilePicType;
            model.Status = status;
            model.BeginDate = startTime;

            try
            {
                service.Update(model);
                return Json(new { result = "success", message = "修改成功。" });
            }
            catch (Exception ex)
            {
                return Json(new { result = "error", message = ex.Message });
            }
        }
        #endregion

        #region 更改活动图状态及删除操作
        /// <summary>
        /// 更新活动状态以及删除操作
        /// </summary>
        /// <param name="id">活动图id</param>
        /// <param name="act"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ActionResult Manage(string id, string act, int value = 1)
        {
            SWfsChannelService service = new SWfsChannelService();
            switch (act.ToLower())
            {
                case "show"://活动状态更新：0关闭;1开启
                    service.Update(id, value);
                    return Redirect("/shangpin/newshelf/NewActivityList");
                case "del":
                    service.Del(id);//删除该活动图
                    return Redirect("/shangpin/newshelf/NewActivityList");
                default:
                    break;
            }
            return View();
        }
        #endregion

        #endregion

        #region 上新品牌管理
        /// <summary>
        /// 上新品牌列表
        /// </summary>
        /// <returns></returns>
        public ActionResult NewBrandList()
        {
            SWfsChannelService service = new SWfsChannelService();
            IList<SWfsNewBrandInfo> list = service.GetAllNewBrandsList();
            if (list != null && list.Count() > 0)
            {
                ViewBag.List = list;
            }
            else
            {
                ViewBag.List = new List<SWfsNewBrandInfo>();
            }
            return View();
        }

        public ActionResult NewBrandManage(string weekOfDays = "", string keyWord = "", string startTime = "", string endTime = "")
        {
            SWfsChannelService service = new SWfsChannelService();
            IList<SWfsNewBrandInfo> list = new List<SWfsNewBrandInfo>();
            string endTimeTemp = "";
            if(!string.IsNullOrEmpty(endTime))
            {
                endTimeTemp = DateTime.Parse(endTime).AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss");
            }
            //根据不同条件获取上新品牌信息
            IList<SWfsNewBrandInfo> brandList = service.GetNewBrandListByWeekDays(weekOfDays, keyWord.Trim(), startTime, endTimeTemp);
            if (brandList == null)
            {
                brandList = new List<SWfsNewBrandInfo>();
            }
            ViewBag.List = brandList;
            ViewBag.Week = weekOfDays;
            ViewBag.KeyWord = keyWord;
            ViewBag.StartTime = startTime;
            ViewBag.EndTime = endTime;
            return View();
        }

        #region 上新品牌添加
        [HttpGet]
        public ActionResult NewBrandAdd(string week)
        {
            ViewBag.WeekDays = week;
            return View();
        }
        [HttpPost]
        public ActionResult AddNewBrandManage()
        {
            SWfsChannelService service = new SWfsChannelService();
            string brandNo = Request.Form["BrandNo"].Trim();//品牌编号
            if (string.IsNullOrEmpty(brandNo))
            {
                return Json("请选择品牌");
            }
            int weekofdays = Convert.ToInt32(Request.Params["week"]);//获取日期
            SWfsNewBrandManage newBrandInfo = service.GetNewBrandInfoByBrandNo(brandNo);
            if (newBrandInfo != null)
            {
                if (newBrandInfo.WeekDays == weekofdays)
                {
                    return Json("该品牌已经存在该池子中了。");
                }
                else
                {
                    return Json("该品牌已经存在第" + newBrandInfo.WeekDays + "个池子中了。");
                }
            }
            int BrandIsShow = service.GetIsBrandNOAccordingConditions(brandNo);
            if (BrandIsShow == 0)
            {
                return Json("该品牌不符合展示条件。");
            }
            IList<SWfsNewBrandInfo> list = service.GetAllNewBrandsList();//获取所有的上新品牌
            if (null == list)
            {
                list = new List<SWfsNewBrandInfo>();
            }
            int sort = 0;
            list = list.Where(a => a.WeekDays == weekofdays).ToList();
            if (list != null && list.Count() > 0)
            {
                //获取该池子中品牌的排序最大值，然后在此基础上加1
                sort = list.Select(a => a.Sort).Max() + 1;
            }
            else
            {
                sort = 1;
            }
            #region 操作实体
            SWfsNewBrandManage nbm = new SWfsNewBrandManage();
            nbm.BrandNo = brandNo;
            nbm.WeekDays = weekofdays;
            nbm.Sort = sort;
            nbm.OperatorUserId = PresentationHelper.GetPassport().UserName;
            nbm.DateCreate = DateTime.Now; 
            #endregion
            try
            {
                service.AddNewBrand(nbm);//添加操作
                return Json(1);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }
        #endregion

        #region 删除上新品牌操作
        public ActionResult DelNewBrand(string act, string id, string week)
        {
            SWfsChannelService service = new SWfsChannelService();
            switch (act.ToLower())
            {
                case "delall"://删除该日期下的所有上新品牌
                    service.DelAllBrandByWeek(id);
                    return Redirect("/shangpin/newshelf/NewBrandList");
                case "delweek"://删除该日期下某一上新品牌
                    service.DelBrandById(id);
                    return Redirect("/shangpin/newshelf/NewBrandManage?weekOfDays=" + week + "");
                default:
                    break;
            }
            return View();
        }
        #endregion

        #region 批量修改上新品牌排序值
        public JsonResult EditeListSortValue()
        {
            try
            {
                SWfsChannelService sercive = new SWfsChannelService();
                if (string.IsNullOrEmpty(Request.Form["idList"]))
                {
                    return Json(new { status = 1, message = "品牌编号不存在" }, JsonRequestBehavior.AllowGet);
                }
                string[] idList = Request.Form["idList"].TrimEnd(',').Split(',');
                string[] sortList = Request.Form["sortList"].TrimEnd(',').Split(',');
                int sort = 1;//标识多次置顶不点击保存按钮重复排序值 
                for (int i = 0; i < idList.Length; i++)
                {
                    int temp = Convert.ToInt32(sortList[i]);
                    
                    if (Convert.ToInt32(sortList[0])<= 0) //若第一个值的排序<=0，则将其赋值为1;
                    {
                        temp = 1;
                    }
                    if (i != 0 && Convert.ToInt32(sortList[0]) <= 0)
                    {
                        if (Convert.ToInt32(sortList[i]) == Convert.ToInt32(sortList[0]))
                        {
                            temp = sort + 1;
                            sort++;
                        }
                        else
                        {
                            temp = Convert.ToInt32(sortList[i]) + sort;//若第一个值的排序<=0，则将其赋值为1;
                        }
                        
                    }
                    sercive.EditeBrandSortValue(Convert.ToInt32(idList[i].ToString()), temp);
                }
                EnyimMemcachedClient.Instance.Remove(Request.Form["memcache_key"].ToString());
                return Json(new { status = 0, message = "修改成功" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(new { status = 1, message = "修改失败" + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 置顶操作
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult SetTopOneSortNewBrand(string Id, string WeekDays)
        {
            try
            {
                SWfsChannelService service = new SWfsChannelService();
                service.SetTopOneSortNewBrand(Id,WeekDays);
                return Json(new { result = "success", message = "操作成功" });
            }
            catch (Exception e)
            {
                return Json(new { result = "error", message = e.ToString() });
                throw;
            }
        }
        #endregion

        #endregion

        #region  上新商品列表
        /// <summary>
        /// 管理品牌对应的上新商品列表
        /// </summary>
        /// <returns></returns>

        public ActionResult NewShelfProductList(string brandNo, string startDate, string endDate, string pageIndex, int pageSize = 20)
        {
            pageIndex = pageIndex == null || pageIndex == "" ? "1" : pageIndex;
            NewShelfBrandProductService service = new NewShelfBrandProductService();
            int count = 0;
            Dictionary<string, List<ProductInfoNew>> dic = service.SelectNewBrandWeekDaysProduct(Convert.ToInt32(pageIndex), pageSize, brandNo, startDate, endDate, out count);
            ViewBag.page = pageIndex;
            ViewBag.pageSize = pageSize;
            ViewBag.totalCount = count;
            return View(dic);
        }
        #endregion
        #region 已经生成的品牌上新商品列表页面方法
        /// <summary>
        /// 已经生成的品牌上新商品列表
        /// </summary>
        /// <param name="brandNo">品牌ID</param>
        /// <returns></returns>
        public ActionResult NewShelfBrandProductList(string brandNo, string DateShelf)
        {
            if (brandNo == null || brandNo == "")
            {
                brandNo = "B0543";
            }
            DateShelf = DateShelf == null || DateShelf == "" ? System.DateTime.Now.ToString("yyyy-MM-dd HH") : DateShelf;
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
            NewShelfBrandProductService service = new NewShelfBrandProductService();
            List<ProductInfoNew> list = service.NewProductList(brandNo, Convert.ToDateTime(DateShelf), ViewBag.keyWord, ViewBag.category, ViewBag.Gender);
            for (int i = 0; i < list.Count(); i++)//循环去取商品价格的信息
            {
                list[i] = service.SelectSpfSkuPrice(list[i], list[i].ProductNo);
            }
            return View(list);
        }

        /// <summary>
        /// ajax调用删除方法
        /// </summary>
        /// <param name="brandNo"></param>
        /// <param name="ProductNoStr"></param>
        /// <returns></returns>
        public ActionResult DeleteNewShelfBrandProductList(string idStr, string brandNo, string DateShelf, int IsCount10)
        {
            idStr = idStr.Trim().TrimEnd(',');
            NewShelfBrandProductService service = new NewShelfBrandProductService();
            service.DeleteNewProductList(idStr,brandNo,Convert.ToDateTime(DateShelf),IsCount10);
            return Json(new { message = "删除成功" });
        }

        /// <summary>
        /// ajax保存排序
        /// </summary>
        /// <param name="brandNo"></param>
        /// <param name="ProductNoStr"></param>
        /// <returns></returns>
        public ActionResult SortShelfProduct(string idStr, string sortStr, string memcache_key)
        {
            try
            {
                idStr = idStr.Trim().TrimEnd(',');
                NewShelfBrandProductService service = new NewShelfBrandProductService();
                sortStr = sortStr.Trim().TrimEnd(',');
                service.UpdateShelfProductSort(idStr, sortStr);
                EnyimMemcachedClient.Instance.Remove(memcache_key);
                return Json(new { message = "保存成功" });
            }
            catch (Exception e)
            {
                return Json(new { message = e.ToString() });
                throw;
            }
        }

        /// <summary>
        /// 置顶操作
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="brandNo"></param>
        /// <param name="dateShelf"></param>
        /// <returns></returns>
        public ActionResult TopOneSortShelfProduct(string Id, string brandNo, string dateShelf)
        {
            try
            {
                NewShelfBrandProductService service = new NewShelfBrandProductService();
                service.TopOneSortShelfProduct(Id, brandNo, Convert.ToDateTime(dateShelf));
                return Json(new { message = "操作成功" });
            }
            catch (Exception e)
            {
                return Json(new { message = e.ToString() });
                throw;
            }
        }

        #endregion

        #region 添加上新商品列表方法
        /// <summary>
        /// 添加上新商品列表
        /// </summary>
        /// <param name="brandNo">品牌ID</param>
        /// <returns></returns>
        public ActionResult AddNewShelfProductList(string brandNo, string pageIndex, string DateShelf, int pageSize = 20)
        {

            pageIndex = pageIndex == null || pageIndex == "" ? "1" : pageIndex;
            DateShelf = DateShelf == null || DateShelf == "" ? System.DateTime.Now.ToString("yyyy-MM-dd HH") : DateShelf;
            if (Request.QueryString["keyWord"] != null)
            {
                ViewBag.keyWord = Request.QueryString["keyWord"];
            }
            if (Request.QueryString["CategoryNo"] != null)
            {
                ViewBag.categoryNo = Request.QueryString["CategoryNo"];
            }
            if (Request.QueryString["Gender"] != null)
            {
                ViewBag.Gender = Request.QueryString["Gender"];
            }
            if(Request.QueryString["startDate"]!=null)
            {
                ViewBag.startDate = Request.QueryString["startDate"];
            }
            if (Request.QueryString["endDate"]!=null)
            {
                ViewBag.endDate = Request.QueryString["endDate"];
            }
            NewShelfBrandProductService service = new NewShelfBrandProductService();
            int total = 0;
            List<ProductInfoNew> list = service.BrandNewShelfProductList(brandNo, Convert.ToInt32(pageIndex), pageSize, Convert.ToDateTime(DateShelf), ViewBag.keyword, ViewBag.categoryNo, ViewBag.Gender, ViewBag.startDate, ViewBag.endDate, out total);
            ViewBag.page = pageIndex;
            ViewBag.pageSize = pageSize;
            ViewBag.totalCount = total;
            //找出当前品牌下已经有的上新商品的总数
            List<ProductInfoNew> newPlist = service.NewProductList(brandNo, Convert.ToDateTime(DateShelf), null, null, null);
            ViewBag.NrePcount = newPlist.Count;
            return View(list);
        }

        /// <summary>
        /// ajax调用的添加上新商品的方法
        /// </summary>
        /// <param name="brandNo"></param>
        /// <param name="ProductNoStr"></param>
        /// <returns></returns>
        public ActionResult AddShelfProduct(string brandNo, string ProductNoStr, string DateShelf)
        {
            DateShelf = DateShelf == null || DateShelf == "" ? System.DateTime.Now.ToString("yyyy-MM-dd HH") : DateShelf;
            ProductNoStr = ProductNoStr.Trim().TrimEnd(',');
            NewShelfBrandProductService service = new NewShelfBrandProductService();
            int WeekDays = (int)Convert.ToDateTime(DateShelf).DayOfWeek;
            WeekDays = WeekDays == 0 ? 7 : WeekDays;
            Passport passport = PresentationHelper.GetPassport();
            service.AddShelfProduct(brandNo.ToUpper(), ProductNoStr, passport.UserName, 1, Convert.ToDateTime(DateShelf), WeekDays);//还为添加用户ID
            return Json(new { message = "添加成功" });
        }
        #endregion
    }
}
