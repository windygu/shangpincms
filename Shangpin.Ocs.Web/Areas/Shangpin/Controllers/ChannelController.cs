using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Shangpin.Ocs.Service.Common;
using Shangpin.Ocs.Service.Shangpin;
using Shangpin.Entity.Wfs;
using Shangpin.Ocs.Service;
using System.Web.Script.Serialization;
//using Shangpin.Framework.WebMvc;
using Shangpin.Ocs.Entity.Extenstion.ShangPin;
using Shangpin.Framework.Common;
using Shangpin.Ocs.Entity.Extenstion.Outlet;
using Shangpin.Framework.Common.Cache;



namespace Shangpin.Ocs.Web.Areas.Shangpin.Controllers
{
    [OCSAuthorization]
    public class ChannelController : Controller
    {
        //
        // GET: /Shangpin/ChannelVenue/

        private static string loginCookieName = AppSettingManager.AppSettings["LoginCookieName"].ToString();
        private readonly SWfsChannelService channelService;
        private Dictionary<string, string> rsPic = new Dictionary<string, string>();
        double a = 10.000;

        public ChannelController()
        {
            channelService = new SWfsChannelService();
        }

        #region 会场管理
        //会场列表
        public ActionResult ChannelList(int pageIndex = 1, int pageSize = 20)
        {
            string channelno = null;
            if (Request.QueryString["channelgender"] == "0")
            {
                channelno = Request.QueryString["womanchannelno"];
            }
            else if (Request.QueryString["channelgender"] == "1")
            {
                channelno = Request.QueryString["manchannelno"];
            }
            int total = 0;
            IEnumerable<SWfsSpChannel> list = channelService.GetChannelList(pageIndex,
                pageSize, channelno, out total);
            ViewBag.totalCount = total;
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;
            return View(list);
        }
        //编辑会场
        public ActionResult EditeChannel(int id = 0)
        {
            if (id == 0)
            {
                return View(new SWfsSpChannel());
            }
            SWfsSpChannel obj = channelService.GetSwfsChannelObjByID(id);
            if (obj == null)
            {
                return View(new SWfsSpChannel());
            }
            if (obj.ChannelNO != null)
            {
                SWfsSpChannelDetail detailobj = channelService.GetSWfsSpChannelDetailByChannelNO(obj.ChannelNO);
                ViewBag.WebTemplateNO = detailobj == null ? "" : detailobj.WebTemplateNO;
            }
            return View(obj);
        }
        [HttpPost]
        public ActionResult EditeChannel(SWfsSpChannel obj)
        {
            if (obj.ChannelID == 0)
            {
                if (Request.Form["channelGender"] == "0")
                {
                    obj.ChannelNO = Request.Form["womanchannelno"];
                }
                else if (Request.Form["channelGender"] == "1")
                {
                    obj.ChannelNO = Request.Form["manchannelno"];
                }
            }
            int result = channelService.EditeChannel(obj, Request.Form["WebTemplateNO"]);
            if (result > 0)
            {
                ViewData["tip"] = new HtmlString("<script>alert('操作成功');location.href='/shangpin/Channel/ChannelList'</script>");
            }
            else
            {
                if (result == -1)
                {
                    ViewData["tip"] = new HtmlString("<script>alert('频道页已存在')</script>");
                }
                else
                {
                    ViewData["tip"] = new HtmlString("<script>alert('操作失败')</script>");
                }

            }
            return View(obj);
        }
        //选择模板列表
        public ActionResult SelectMeetingTemplate(int pageIndex = 1, int pageSize = 3)
        {
            int total = 0;
            IEnumerable<SWfsSpChannelTemplate> list = channelService.GetChannelTemplateList(pageIndex,
                pageSize, Request.QueryString["templateno"], Request.QueryString["templatename"], out total);
            ViewBag.totalCount = total;
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;
            return View(list);
        }
        //清除频道下的所有缓存
        public JsonResult ClearChannelCache(string channelno, string cno, string sex)
        {
            try
            {
                EnyimMemcachedClient.Instance.Remove("GetSpChannel_" + channelno);
                EnyimMemcachedClient.Instance.Remove("GetSpChannelDetail_" + channelno);
                EnyimMemcachedClient.Instance.Remove("GetSpChannelTopAdver_" + channelno);
                EnyimMemcachedClient.Instance.Remove("GetSpChannelAlterPic_" + channelno);
                EnyimMemcachedClient.Instance.Remove("GetSpChannelRecommBrand_" + channelno);
                //20140220新加功能设置清除
                EnyimMemcachedClient.Instance.Remove("GetSpChannelRecommendCategoryList_" + channelno);
                EnyimMemcachedClient.Instance.Remove("GetSpChannelBrandList_" + channelno);
                EnyimMemcachedClient.Instance.Remove("GetSWfsCategoryListBFD_" + channelno);
                EnyimMemcachedClient.Instance.Remove("GetListAlterGroupList_" + channelno);
                EnyimMemcachedClient.Instance.Remove("GetSpChannelRecommendBrandList_" + channelno);
                //MemcachedProvider.Instance.Delete("VenueAll_SWfsMeetingInfo_" + id);

                //string cacheKey = string.Format("{0}master_page_html_key{1}", AppSettingManager.AppSettings["ShangpinSiteUrl"].Replace("http://", ""), gender);

                if (!string.IsNullOrEmpty(cno))
                    EnyimMemcachedClient.Instance.Remove("GetNaviInfoHead_" + cno);
                //GetNaviInfoHead_
                string cacheKey = string.Format("{0}master_page_html_key{1}", AppSettingManager.AppSettings["ShangpinDomain"].Replace("http://", ""), sex);
                EnyimMemcachedClient.Instance.Remove(cacheKey);
                return Json("清除成功", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("清除异常", JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 模板管理
        //模板列表
        public ActionResult TemplateList(int pageIndex = 1, int pageSize = 20)
        {
            int total = 0;
            IEnumerable<SWfsSpChannelTemplate> list = channelService.GetChannelTemplateList(pageIndex,
                pageSize, Request.QueryString["templateno"], Request.QueryString["templatename"], out total);
            ViewBag.totalCount = total;
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;
            return View(list);
        }
        //编辑模板
        public ActionResult EditeTemplate(int id = 0)
        {
            if (id == 0)
            {
                return View(new SWfsSpChannelTemplate() { TemplateNO = DateTime.Now.ToString("yyyyMMddHHmmss") });
            }
            SWfsSpChannelTemplate obj = channelService.GetSwfsChannelTemplateObjByID(id);
            if (obj == null)
            {
                return View(new SWfsSpChannelTemplate() { TemplateNO = DateTime.Now.ToString("yyyyMMddHHmmss") });
            }
            return View(obj);
        }
        [HttpPost]
        public ActionResult EditeTemplate(SWfsSpChannelTemplate obj)
        {
            int result = channelService.EditeSwfsChannelTemplate(obj);
            if (result > 0)
            {
                ViewData["tip"] = new HtmlString("<script>alert('操作成功');location.href='/Shangpin/Channel/TemplateList';</script>");
                return View(obj);
            }
            else
            {
                if (result == -1)
                {
                    ViewData["tip"] = new HtmlString("<script>alert('已存在该模板');</script>");
                }
                else
                {
                    ViewData["tip"] = new HtmlString("<script>alert('操作失败');</script>");
                }
            }
            return View(obj);
        }
        //删除模板
        public ActionResult DeleteTemplate(int id)
        {
            channelService.DeleteTemplateByID(id);
            return Redirect("/shangpin/channel/TemplateList");
        }
        //模板代码编辑
        [HttpPost]
        public string EditeTemplateContent(int tempID)
        {
            SWfsSpChannelTemplate obj = channelService.GetSwfsChannelTemplateObjByID(tempID);
            if (obj == null)
            {
                return null;
            }
            return channelService.GetTemplateContent(obj.TemplatePath);
        }
        //保存模板代码
        [HttpPost, ValidateInput(false)]
        public int SaveTempContent(int tempID, string tempContent)
        {
            SWfsSpChannelTemplate obj = channelService.GetSwfsChannelTemplateObjByID(tempID);
            if (obj == null)
            {
                return 0;
            }
            return channelService.SaveTemplateContent(obj.TemplatePath, tempContent);
        }
        #endregion

        #region 区块管理
        //按会场ID模板编号加载模板数据
        public ActionResult VenueTemplateEditeByID(string channelNO)
        {
            SWfsSpChannelDetail obj = channelService.GetSWfsSpChannelDetailByChannelNO(channelNO);
            if (obj == null)
            {
                return View(new SWfsSpChannelDetail());
            }
            return View(obj);
        }
        [HttpPost]
        public string SaveRegionRelationContent(SWfsSpChannelTemplateRegion obj)
        {
            JavaScriptSerializer json = new JavaScriptSerializer();
            #region 数据验证
            if (obj.ChannelNO == null || obj.RegionID <= 0 || obj.RelationType <= 0 || string.IsNullOrEmpty(obj.TemplateNO))
            {
                return "{\"status\" : 0, \"message\" : \"数据不合法\"}";
            }
            if (Request.Form["selectedType"] == null)
            {
                return "{\"status\" : 0, \"message\" : \"类别不存在\"}";
            }
            else
            {
                if (Request.Form["selectedType"] == "1")//关联链接
                {
                    obj.RelationContent = Request.Form["linkurl"];
                    obj.RelationType = 5;
                }
                else//关联活动
                {
                    if (Request.Form["dataobj"] == null)
                    {
                        if (Request.Form["RelationContent"] == null)
                        {
                            return "{\"status\" : 0, \"message\" : \"请选择关联活动\"}";
                        }
                    }
                    else
                    {
                        string[] activeInfo = Request.Form["dataobj"].Split('_');
                        if (activeInfo.Length < 3)
                        {
                            return "{\"status\" : 0, \"message\" : \"关联活动数据格式异常\"}";
                        }
                        obj.RelationContent = activeInfo[0];
                        if (activeInfo[1] == "1")//尚品
                        {
                            if (activeInfo[2] == "1")//活动
                            {
                                obj.RelationType = 1;
                            }
                            else//专题
                            {
                                obj.RelationType = 4;
                            }
                        }
                        else//奥莱
                        {
                            if (activeInfo[2] == "1")//活动
                            {
                                obj.RelationType = 2;
                            }
                            else//专题
                            {
                                obj.RelationType = 3;
                            }
                        }

                    }
                }
            }
            if (Request.Files["imgfile"] != null && Request.Files["imgfile"].ContentLength > 0)
            {
                obj.ImgNO = SaveImg(Request.Files["imgfile"], "width:0,Height:0,Length:1000");
                if (rsPic.Keys.Contains("error"))
                {
                    return "{\"status\" : 0, \"message\" : \"图片不合法\"}";
                }
            }
            #endregion
            if (!channelService.SaveRegionRelationInfo(obj))
            {
                return "{\"status\" : 0, \"message\" : \"数据保存异常\"}";
            }
            return "{\"status\" : 1, \"message\" : " + json.Serialize(obj) + "}";
        }
        //加载自定义链接
        public string GetRelationLinkJson()
        {
            JavaScriptSerializer json = new JavaScriptSerializer();
            SWfsSpChannelTemplateRegion obj = new SWfsSpChannelTemplateRegion();
            if (!string.IsNullOrEmpty(Request.Form["relationID"]) && Request.Form["relationID"] + "" != "0")
            {
                obj = channelService.GetRegionRelationInfoByCondition(int.Parse(Request.Form["relationID"]), "", "", "", "").FirstOrDefault();
            }
            else
            {
                //查询会场区块信息的关联内容--根据（会场编号--区域块ID--关联模块RelationType)
                obj = channelService.GetRegionRelationInfoByCondition(0, Request.Form["channelNO"], Request.Form["regionID"], "", Request.Form["TemplateNO"]).FirstOrDefault();
            }
            if (obj == null)
            {
                obj = new SWfsSpChannelTemplateRegion() { RelationType = 5 };
            }
            return json.Serialize(obj);
        }
        //加载活动分页列表
        public string GetActiveListJsonByPage()
        {
            JavaScriptSerializer json = new JavaScriptSerializer();
            int totalCount = 0;
            SWfsSpChannelTemplateRegion obj = new SWfsSpChannelTemplateRegion();
            if (!string.IsNullOrEmpty(Request.Form["relationID"]) && Request.Form["relationID"] + "" != "0")
            {
                obj = channelService.GetRegionRelationInfoByCondition(int.Parse(Request.Form["relationID"]), "", "", "", "").FirstOrDefault();
            }
            else
            {
                //查询会场区块信息的关联内容--根据（会场编号--区域块ID--关联模块RelationType)
                obj = channelService.GetRegionRelationInfoByCondition(0, Request.Form["channelNO"], Request.Form["regionID"], "", Request.Form["TemplateNO"]).FirstOrDefault();
            }
            //获取活动列表
            IList<ActiveAndSpecial> list = channelService.GetActiveAndSpecialByPage(int.Parse(Request.Form["pageIndex"]), int.Parse(Request.Form["pageSize"]),
                Request.Form["activeNameAndNO"], Request.Form["webSource"], Request.Form["activeType"], Request.Form["activeStatus"],
                Request.Form["activeStartDate"], Request.Form["activeEndDate"], out totalCount);
            if (obj == null)
            {
                obj = new SWfsSpChannelTemplateRegion { RelationType = 5 };
            }
            #region 标记以选中项
            if (obj.RelationType >= 1 && obj.RelationType <= 4)
            {
                switch (obj.RelationType)
                {
                    case 1://尚品活动
                        if (list.Count(p => p.ActiveNO == obj.RelationContent && p.WebSource == 1 && p.ActiveType == 1) == 1)
                        {
                            list.Single(p => p.ActiveNO == obj.RelationContent && p.WebSource == 1 && p.ActiveType == 1).ActiveID = -1;
                        }
                        break;
                    case 2://奥莱活动
                        if (list.Count(p => p.ActiveNO == obj.RelationContent && p.WebSource == 2 && p.ActiveType == 1) == 1)
                        {
                            list.Single(p => p.ActiveNO == obj.RelationContent && p.WebSource == 2 && p.ActiveType == 1).ActiveID = -1;
                        }
                        break;
                    case 3://奥莱专题
                        if (list.Count(p => p.ActiveNO == obj.RelationContent && p.WebSource == 2 && p.ActiveType == 0) == 1)
                        {
                            list.Single(p => p.ActiveNO == obj.RelationContent && p.WebSource == 2 && p.ActiveType == 0).ActiveID = -1;
                        }
                        break;
                    case 4://尚品专题
                        if (list.Count(p => p.ActiveNO == obj.RelationContent && p.WebSource == 1 && p.ActiveType == 0) == 1)
                        {
                            list.Single(p => p.ActiveNO == obj.RelationContent && p.WebSource == 1 && p.ActiveType == 0).ActiveID = -1;
                        }
                        break;
                }
            }
            //修改时间格式
            for (int i = 0; i < list.Count; i++)
            {
                list[i].ActiveStartTime = list[i].StartTime.ToString("yyyy-MM-dd HH:mm:ss");
                list[i].ActiveEndTime = list[i].EndTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
            #endregion
            return "{\"relationobj\":" + json.Serialize(obj) + ",\"count\":" + Math.Ceiling(totalCount / double.Parse(Request.Form["pageSize"])) + ",\"list\":" + json.Serialize(list) + "}";
        }
        //根据模板生成HTML
        public ActionResult CreateHTMLByTemplate(string channelNO, string templateNO)
        {
            if (string.IsNullOrEmpty(templateNO) || channelNO == null)
            {
                return Content("0");
            }
            SWfsSpChannelTemplate obj = channelService.GetSwfsChannelTemplateObjByNO(templateNO);//获取模板
            if (obj == null)
            {
                return Content("0");
            }
            if (string.IsNullOrEmpty(obj.TemplatePath))
            {
                return Content("0");
            }
            if (!System.IO.File.Exists(Server.MapPath(obj.TemplatePath))) //验证模板是否存在
            {
                return Content("0");
            }
            //查询填充模板的数据列表
            IEnumerable<SWfsSpChannelTemplateRegion> Opratorlist = channelService.GetRegionRelationInfoByCondition(0, channelNO, "", "", templateNO);
            return View(obj.TemplatePath, Opratorlist);
        }
        //发布会场
        [HttpPost, ValidateInput(false)]
        public string PublishVenue(string venuehtml)
        {
            try
            {
                if (channelService.SaveVenueHtml(Request.Form["DetailID"], venuehtml))
                {
                    return "1";
                }
                return "0";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        #endregion

        #region 促销商品组管理
        //获取产品分组
        public ActionResult ChannelProductGroupManager(string channelno, int pageIndex = 1, int pageSize = 20)
        {
            int total = 0;
            IEnumerable<SWfsSpChannelProductGroup> list = channelService.GetChannelProductGroupList(Request.QueryString["groupname"], Request.QueryString["status"], Request.QueryString["starttime"], Request.QueryString["endtime"], channelno, pageIndex, pageSize, out total);
            ViewBag.totalCount = total;
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;
            return View(list);
        }
        //编辑产品分组
        public ActionResult EditeProductGroup(int id)
        {
            if (id == 0)
            {
                return View(new SWfsSpChannelProductGroup() { Status = 1, ChannelNO = Request.QueryString["channelno"] });
            }
            SWfsSpChannelProductGroup obj = channelService.GetSWfsSpChannelProductGroupObjByID(id);
            return View(obj);
        }
        [HttpPost]
        public ActionResult EditeProductGroup(SWfsSpChannelProductGroup obj)
        {
            obj.CreateDate = DateTime.Now;
            if (string.IsNullOrEmpty(obj.GroupName) || string.IsNullOrEmpty(Request.Form["StartTime"]) || string.IsNullOrEmpty(Request.Form["EndTime"]) || string.IsNullOrEmpty(obj.ChannelNO))
            {
                ViewData["tip"] = new HtmlString("<script>alert('数据不合法');</script>");
                return View(obj);
            }
            int result = channelService.AddSWfsSpChannelProductGroup(obj);
            if (result > 0)
            {
                ViewData["tip"] = new HtmlString("<script>alert('操作成功');location.href='/Shangpin/Channel/ChannelProductGroupManager?channelno=" + obj.ChannelNO + "';</script>");
                return View(obj);
            }
            else
            {
                ViewData["tip"] = new HtmlString("<script>alert('操作失败');</script>");
            }
            return View(obj);
        }
        //删除产品分组
        public ActionResult DeleteChannelProductGroup(int id, string channelno)
        {
            channelService.DeleteSWfsSpChannelProductGroupByID(id);
            return Redirect("/shangpin/channel/ChannelProductGroupManager?channelNo=" + channelno);
        }
        //获取分组内的产品列表
        public ActionResult ChannelProductList(int groupid, int pageIndex = 1, int pageSize = 10)
        {
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
            if (Request.QueryString["IsShelf"] != null)
            {
                ViewBag.IsShelf = Request.QueryString["IsShelf"];
            }
            IEnumerable<SkillProductExtends> list = channelService.GetSWfsSpChannelProductList(ViewBag.IsShelf, ViewBag.Gender, ViewBag.BrandNO, ViewBag.category, ViewBag.keyWord, groupid, pageIndex, pageSize, out total);
            list = list.GroupBy(p => p.ProductNo).Select(p => new SkillProductExtends() 
            {
                Id = p.ElementAt(0).Id,
                SkillGroupId = p.ElementAt(0).SkillGroupId,
                ProductNo = p.ElementAt(0).ProductNo,
                ProductFileNo = p.ElementAt(0).ProductFileNo,
                CreateTime = p.ElementAt(0).CreateTime,
                SortValue = p.ElementAt(0).SortValue,
                GoodsNo = p.ElementAt(0).GoodsNo,
                ProductModel = p.ElementAt(0).ProductModel,
                ProductName = p.ElementAt(0).ProductName,
                ProductPicFile = p.ElementAt(0).ProductPicFile,
                ProductShowPic = p.ElementAt(0).ProductShowPic,
                SellPrice = p.ElementAt(0).SellPrice,
                GoldPrice = p.ElementAt(0).GoldPrice,
                PlatinumPrice = p.ElementAt(0).PlatinumPrice,
                DiamondPrice = p.ElementAt(0).DiamondPrice,
                LimitedPrice = p.ElementAt(0).LimitedPrice,
                StandardPrice = p.ElementAt(0).StandardPrice,
                MarketPrice = p.ElementAt(0).MarketPrice,
                PromotionPrice = p.ElementAt(0).PromotionPrice,
                IsShelf = p.ElementAt(0).IsShelf,
                PcSaleState = p.ElementAt(0).PcSaleState,
                BrandCnName = p.ElementAt(0).BrandCnName,
                BrandEnName = p.ElementAt(0).BrandEnName,
                MarketPriceRegion = p.Min(a => a.MarketPrice) + "~" + p.Max(a => a.MarketPrice),
                StandardPriceRegion = p.Min(a => a.StandardPrice) + "~" + p.Max(a => a.StandardPrice),
                PlatinumPriceRegion = p.Min(a => a.PlatinumPrice) + "~" + p.Max(a => a.PlatinumPrice),
                DiamondPriceRegion = p.Min(a => a.DiamondPrice) + "~" + p.Max(a => a.DiamondPrice),
                PromotionPriceRegion = p.Min(a => a.PromotionPrice) + "~" + p.Max(a => a.PromotionPrice),
                GoldPriceRegion = p.Min(a => a.GoldPrice) + "~" + p.Max(a => a.GoldPrice)
            });
            ViewBag.totalCount = total;
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;
            return View(list);
        }
        //删除分组内产品
        public ActionResult DeleteProduct(int id)
        {
            channelService.DeleteSWfsSpChannelProductByID(id);
            return Redirect("/shangpin/channel/ChannelProductList?groupid=" + Request.QueryString["groupid"] + "&channelno=" + Request.QueryString["channelno"]);
        }
        //加载商品列表
        public ActionResult AboutProductList(int groupid, int pageIndex = 1, int pageSize = 10)
        {
            int totalCount = 0;
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
            if (Request.QueryString["IsShelf"] != null)
            {
                ViewBag.IsShelf = Request.QueryString["IsShelf"];
            }
            IList<ProductInfo> list = channelService.GetProductList(ViewBag.IsShelf, ViewBag.Gender, ViewBag.BrandNO, ViewBag.category, ViewBag.keyWord, pageIndex, pageSize, out totalCount);
            list = list.GroupBy(p => p.ProductNo).Select(p => new ProductInfo() 
            {
                ProductNo = p.ElementAt(0).ProductNo,
                GoodsNo = p.ElementAt(0).GoodsNo,
                ProductModel = p.ElementAt(0).ProductModel,
                IsShelf = p.ElementAt(0).IsShelf,
                PcSaleState = p.ElementAt(0).PcSaleState,
                ProductName = p.ElementAt(0).ProductName,
                ProductPicFile = p.ElementAt(0).ProductPicFile,
                ProductShowPic = p.ElementAt(0).ProductShowPic,
                SellPrice = p.ElementAt(0).SellPrice,
                GoldPrice = p.ElementAt(0).GoldPrice,
                PlatinumPrice = p.ElementAt(0).PlatinumPrice,
                DiamondPrice = p.ElementAt(0).DiamondPrice,
                LimitedPrice = p.ElementAt(0).LimitedPrice,
                StandardPrice = p.ElementAt(0).StandardPrice,
                MarketPrice = p.ElementAt(0).MarketPrice,
                PromotionPrice = p.ElementAt(0).PromotionPrice,
                BrandNo = p.ElementAt(0).BrandNo,
                BrandCnName = p.ElementAt(0).BrandCnName,
                BrandEnName = p.ElementAt(0).BrandEnName,
                MarketPriceRegion = p.Min(a => a.MarketPrice) + "~" + p.Max(a => a.MarketPrice),
                StandardPriceRegion = p.Min(a => a.StandardPrice) + "~" + p.Max(a => a.StandardPrice),
                PlatinumPriceRegion = p.Min(a => a.PlatinumPrice) + "~" + p.Max(a => a.PlatinumPrice),
                DiamondPriceRegion = p.Min(a => a.DiamondPrice) + "~" + p.Max(a => a.DiamondPrice),
                PromotionPriceRegion = p.Min(a => a.PromotionPrice) + "~" + p.Max(a => a.PromotionPrice),
                GoldPriceRegion = p.Min(a => a.GoldPrice) + "~" + p.Max(a => a.GoldPrice)
            }).ToList();
            //查询添加过的商品
            IEnumerable<string> oldidlist = channelService.GetProductNO(groupid);
            if (oldidlist != null)
            {
                if (oldidlist.Count() > 0)
                {
                    foreach (var item in oldidlist)
                    {
                        if (list.Count(p => p.ProductNo == item) > 0)
                        {
                            list.Where(p => p.ProductNo == item).First().Status = -2;
                        }
                    }
                }
            }
            ViewBag.totalCount = totalCount;
            return View(list);
        }
        //批量添加商品
        [HttpPost]
        public int AddProductNOList(int groupid, string productNO)
        {
            return channelService.AddProduct(groupid, productNO);
        }
        [HttpPost]
        public ActionResult AddProduct(int groupid)
        {
            channelService.AddProduct(groupid, Request.Form["productNO"]);
            return Redirect("/shangpin/channel/AboutProductList?groupid=" + groupid + "&pageindex=" + Request.QueryString["pageindex"] + "&channelno=" + Request.QueryString["channelno"]);
        }
        //保存排序
        public int SaveProductSort(int id, int sortValue)
        {
            return channelService.SaveSortProduct(id, sortValue);
        }
        //批量删除产品
        [HttpPost]
        public ActionResult DeleteProductByIdList()
        {
            channelService.DeleteProductByIdList(Request.Form["objID"]);
            return Redirect("/shangpin/channel/ChannelProductList?groupid=" + Request.QueryString["groupid"] + "&channelno=" + Request.QueryString["channelno"]);
        }
        //上传图片
        [HttpPost]
        public string UpSkillProductImg()
        {
            string imgNO = null;
            if (string.IsNullOrEmpty(Request.Form["skillId"]))
            {
                return "{\"status\" : 0, \"message\" : \"数据不存在\"}";
            }
            if (Request.Files["imgfile"] != null && Request.Files["imgfile"].ContentLength > 0)
            {
                imgNO = SaveImg(Request.Files["imgfile"], "width:120,Height:224,Length:1000");
                if (rsPic.Keys.Contains("error"))
                {
                    return "{\"status\" : 0, \"message\" : \"请上传尺寸为 120*224 的图片\"}";
                }
                if (channelService.AddProductImg(int.Parse(Request.Form["skillId"]), imgNO) > 0)
                {
                    return "{\"status\" : 1, \"message\" : \"" + imgNO + "\"}";
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(Request.Form["imgNO"]))
                {
                    return "{\"status\" : 1, \"message\" : \"" + Request.Form["imgNO"] + "\"}";
                }
            }
            return "{\"status\" : 0, \"message\" : \"图片不合法\"}";
        }
        #endregion

        #region 频道推荐品牌
        //获取频道品牌组列表
        public ActionResult ChannelBrandGroupManager(string channelno, int pageIndex = 1, int pageSize = 20)
        {
            int total = 0;
            IEnumerable<SWfsSpChannelBrandGroup> list = channelService.GetBrandGroupList(channelno, pageIndex, pageSize, out total);
            ViewBag.totalCount = total;
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;
            return View(list);
        }
        //删除品牌分组
        public ActionResult DeleteBrandGroup(int id)
        {
            channelService.DeleteBrandGroupByID(id);
            return Redirect("/shangpin/channel/ChannelBrandGroupManager?channelno=" + Request.QueryString["channelno"]);
        }
        //添加分组
        public ActionResult EditeBrandGroup(int id)
        {
            if (id == 0)
            {
                return View(new SWfsSpChannelBrandGroup() { Status = 0, ChannelNO = Request.QueryString["channelno"] });
            }
            SWfsSpChannelBrandGroup obj = channelService.GetSWfsSpChannelBrandGroupObjByID(id);
            return View(obj);
        }
        [HttpPost]
        public ActionResult EditeBrandGroup(SWfsSpChannelBrandGroup obj)
        {
            obj.CreateDate = DateTime.Now;
            if (string.IsNullOrEmpty(obj.GroupName) || string.IsNullOrEmpty(obj.ChannelNO))
            {
                ViewData["tip"] = new HtmlString("<script>alert('数据不合法');</script>");
                return View(obj);
            }
            int result = channelService.EditeBrandGroup(obj);
            if (result > 0)
            {
                ViewData["tip"] = new HtmlString("<script>alert('操作成功');location.href='/Shangpin/Channel/ChannelBrandGroupManager?channelno=" + obj.ChannelNO + "';</script>");
                return View(obj);
            }
            else
            {
                if (result == -1)
                {
                    ViewData["tip"] = new HtmlString("<script>alert('品牌组名称: " + obj.GroupName + "已存在');</script>");
                }
                else
                {
                    ViewData["tip"] = new HtmlString("<script>alert('操作失败');</script>");
                }

            }
            return View(obj);
        }
        //按ID获取品牌
        public string GetBrandByID(int id)
        {
            JavaScriptSerializer json = new JavaScriptSerializer();
            return json.Serialize(channelService.GetBrandByID(id));
        }
        //保存品牌
        public string EditeBrand(SWfsSpChannelBrand obj)
        {
            JavaScriptSerializer json = new JavaScriptSerializer();
            if (obj.GroupID == 0 || string.IsNullOrEmpty(obj.BrandName) || string.IsNullOrEmpty(obj.ImgLink))
            {
                return "{\"status\":0,\"message\":\"信息不完整\"}";
            }
            if (Request.Files["imgfile"] != null && Request.Files["imgfile"].ContentLength > 0)
            {
                obj.ImgNO = SaveImg(Request.Files["imgfile"], "width:155,Height:74,Length:300");
                if (rsPic.Keys.Contains("error"))
                {
                    return "{\"status\" : 0, \"message\" : \"请上传 宽155 高74 大小为 300kb 的图片\"}";
                }
            }
            else
            {
                if (string.IsNullOrEmpty(obj.ImgNO))
                {
                    return "{\"status\" : 0, \"message\" : \"请上传图片\"}";
                }
            }
            return "{\"status\":1,\"objid\":" + channelService.EditeBrand(obj) + ",\"imgno\":\"" + obj.ImgNO + "\"}";
        }
        //修改品牌状态(不用啦)
        //public int EditeBrandStatus(int id,int status)
        //{
        //    return channelService.EditeBrandStatus(id, status);
        //}

        /// <summary>
        /// 修改品牌状态
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="status"></param>
        /// <param name="channelno"></param>
        /// <returns></returns>
        public int EditeBrandStatus(int id, int status, string channelno)
        {
            return channelService.UpdateBrandGroupStatus(id, status, channelno);
        }
        #endregion

        #region 频道品牌轮播图
        //添加频道品牌轮播图分组
        public ActionResult EditeBrandAlterPicGroup(string channelNo, int GroupId = 0)
        {
            SWfsListAlterGroup obj = null;
            if (GroupId == 0)
            {
                obj = new SWfsListAlterGroup { Category = channelNo, GroupType = 2, Status = 0, Gender = channelNo.IndexOf("A01") >= 0 ? 1 : 0 };
                return View(obj);
            }
            obj = channelService.GetBrandAlterGroupObj(GroupId);
            return View(obj);
        }
        [HttpPost]
        public ActionResult EditeBrandAlterPicGroup(SWfsListAlterGroup obj)
        {
            obj.CreateDate = DateTime.Now;
            obj.GroupType = 2;
            if (string.IsNullOrEmpty(obj.GroupName))
            {
                ViewData["tip"] = new HtmlString("<script>alert('分组名称不能为空');</script>");
                return View(obj);
            }
            int result = channelService.EditeBrandAlerPicGroupObj(obj);
            if (result > 0)
            {
                ViewData["tip"] = new HtmlString("<script>alert('操作成功');location.href='/Shangpin/Channel/BrandAlterPicGroup?channelNO=" + obj.Category + "';</script>");
                return View(obj);
            }
            else
            {
                ViewData["tip"] = new HtmlString("<script>alert('操作失败');</script>");
            }
            return View(obj);
        }
        //删除品牌轮播分组
        public ActionResult DeleteBrandAlterPicGroup(int id, string channelno)
        {
            channelService.DeleteBrandAlterPicGroup(id);
            return Redirect("/shangpin/channel/BrandAlterPicGroup?channelNo=" + channelno);
        }
        //频道品牌轮播图分组
        public ActionResult BrandAlterPicGroup(int pageIndex = 1, int pageSize = 20)
        {
            int total = 0;
            IEnumerable<SWfsListAlterGroup> list = channelService.GetBrandAlterGroup(Request.QueryString["channelno"], Request.QueryString["groupname"], Request.QueryString["status"], pageIndex, pageSize, out total);
            ViewBag.totalCount = total;
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;
            return View(list);
        }
        //添加品牌分组内图片
        public JsonResult EditeBrandAlterPic(int id)
        {
            SWfsListAlterPic obj = channelService.GetBrandAlterPicObj(id);
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public string EditeBrandAlterPic(SWfsListAlterPic obj)
        {
            obj.CreateDate = DateTime.Now;
            if (string.IsNullOrEmpty(obj.LinkName))
            {
                return "{\"result\" : 0, \"message\" : \"名称不能为空\" }";
            }
            if (string.IsNullOrEmpty(obj.AlterAddress))
            {
                return "{\"result\" : 0, \"message\" : \"链接地址不能为空\" }";
            }
            if (obj.GroupId <= 0)
            {
                return "{\"result\" : 0, \"message\" : \"分组不存在\" }";
            }
            if (Request.Files["imgfile"] != null && Request.Files["imgfile"].ContentLength > 0)
            {
                obj.LargePicture = SaveImg(Request.Files["imgfile"], "width:316,Height:190,Length:150");
                if (rsPic.Keys.Contains("error"))
                {
                    return "{\"result\" : 0, \"message\" : \"请上传 316*190 大小为 150kb 的图片\" }";
                }
            }
            int result = channelService.EditeBrandAlterObj(obj);
            if (result > 0)
            {
                return "{\"result\" : 1, \"message\" : \"操作成功\" }";
            }
            else
            {
                return "{\"result\" : 0, \"message\" : \"操作失败\" }";
            }
        }
        //频道品牌图片列表
        public ActionResult BrandAlterPicList(int groupID)
        {
            return View(channelService.GetBrandAlterPicList(groupID));
        }
        //删除品牌轮播图片
        public ActionResult DeleteBrandAlterPicByID(int id)
        {
            channelService.DelteBrandAlterPic(id);
            return Redirect("/shangpin/channel/BrandAlterPicList?groupid=" + Request.QueryString["groupid"] + "&channelno=" + Request.QueryString["channelno"]);
        }
        #endregion

        #region 频道头图管理

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

        /// <summary>
        /// 查询头图列表 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult ChannelAdverList(string channelno, int pageIndex = 1, int pageSize = 10)
        {
            int count = 0;
            ViewBag.list = ChannelAdverCount(channelno, pageIndex, pageSize, out count);
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;
            ViewBag.TotalCount = count;
            return View();
        }
        //返回总条数
        public IEnumerable<SWfsSpChannelAdver> ChannelAdverCount(string channelno, int pageIndex, int pageSize, out int count)
        {
            var dic = new Dictionary<string, object>();
            string name = Request.QueryString["AdverName"];

            if (name != null && name != "")
            {
                dic.Add("AdverName", name);
                ViewBag.KeyWord = name;
            }
            else
            {
                dic.Add("AdverName", "");
                ViewBag.KeyWord = name;
            }
            IEnumerable<SWfsSpChannelAdver> list = DapperUtil.Query<SWfsSpChannelAdver>("ComBeziWfs_SWfsSpChannelAdver_List", dic, new { ChannelNo = channelno, AdverName = dic["AdverName"], pageIndex = pageIndex, pageSize = pageSize }).ToList();
            count = DapperUtil.Query<int>("ComBeziWfs_SWfsSpChannelAdver_Count", dic, new { ChannelNo = channelno, AdverName = dic["AdverName"], pageIndex = pageIndex, pageSize = pageSize }).First<int>();
            return list;
        }

        /// <summary>
        /// 添加头图
        /// </summary>
        /// <param name="adverId"></param>
        /// <returns></returns>
        public ActionResult ChannelAdverCreate(int adverId = 0)
        {
            var list = channelService.ChannelAdver_BYID(adverId);
            if (list == null)
            {
                return View(new SWfsSpChannelAdver { AdverID = adverId, ChannelNO = Request.QueryString["channelno"] });
            }
            return View(list);
        }
        [HttpPost]
        public ActionResult ChannelAdverCreate(SWfsSpChannelAdver obj)
        {
            string channelno = Request.Form["channelno"];
            obj.AdverName = Request.Form["AdverName"];
            obj.AdverLink = Request.Form["AdverLink"];
            obj.ChannelNO = channelno;
            obj.CreateDate = DateTime.Now;
            #region 添加图片
            if (Request.Files["imgfile"] != null && Request.Files["imgfile"].ContentLength > 0)
            {
                obj.ImgNO = SaveImg(Request.Files["imgfile"], "width:714,Height:50,Length:300");
                if (rsPic.Keys.Contains("error"))
                {
                    ViewData["tip"] = new HtmlString("<script>alert('" + rsPic["error"] + "')</script>");
                    return View(obj);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(obj.ImgNO))
                {
                    obj.ImgNO = "";
                }
            }
            #endregion
            if (obj.ImgNO == "")
            {
                return Content("<script>alert('图片不能为空'); window.location.href='ChannelAdverCreate.html?channelno=" + channelno + "'</script>");
            }
            if (obj.AdverID == 0)
            {
                if (channelService.ChannelAdverCreate(obj) > 0)
                {
                    //ViewData["tip"] = new HtmlString("<script>alert('添加成功'); window.location.href='ChannelAdverList.html" + CommonService.GetTimeStamp("?") + "'</script>");
                    ViewData["tip"] = new HtmlString("<script>alert('添加成功'); window.location.href='ChannelAdverList.html?channelno=" + channelno + "'</script>");
                }
                else
                {
                    ViewData["tip"] = new HtmlString("<script>alert('添加失败')</script>");

                }
            }
            else
            {
                if (channelService.ChannelAdverUpdate(obj))
                {
                    //ViewData["tip"] = new HtmlString("<script>alert('修改成功'); window.location.href='ChannelAdverList.html" + CommonService.GetTimeStamp("?") + "'</script>");
                    ViewData["tip"] = new HtmlString("<script>alert('修改成功'); window.location.href='ChannelAdverList.html?channelno=" + channelno + "'</script>");
                }
                else
                {
                    ViewData["tip"] = new HtmlString("<script>alert('修改失败')</script>");
                }
            }
            return View(obj);
        }

        /// <summary>
        /// 删除一条头图
        /// </summary>
        /// <param name="adverId"></param>
        /// <returns></returns>
        public ActionResult ChannelAdverDelete(int adverId, string channelno)
        {
            channelService.ChannelAdverDelete(adverId);
            return Redirect("ChannelAdverList.html?channelno=" + channelno);
        }

        /// <summary>
        /// 修改头图状态
        /// </summary>
        /// <param name="adverId"></param>
        /// <param name="status"></param>
        /// <param name="channelno"></param>
        /// <returns></returns>
        public ActionResult ChannelAdverStatus(int adverId, int status, string channelno)
        {
            channelService.ChannelAdverStatus(adverId, status, channelno);
            return Redirect("ChannelAdverList.html?channelno=" + channelno);
        }
        #endregion

        #region 频道轮播图



        /// <summary>
        /// 分组列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult ChannelAlterList(string channelno, int pageIndex = 1, int pageSize = 20)
        {
            int count = 0;
            if (Request.QueryString["Gender"] == "0")
            {
                string womenbags = AppSettingManager.AppSettings["WomenBags"];
                string womenclothing = AppSettingManager.AppSettings["WomenClothing"];
                string womenshoes = AppSettingManager.AppSettings["WomenShoes"];
                string womenaccessories = AppSettingManager.AppSettings["WomenAccessories"];
                string womenhomes = AppSettingManager.AppSettings["WomenHomes"];
                string WomenBeauty = AppSettingManager.AppSettings["WomenBeauty"];
                string WomenWatches = AppSettingManager.AppSettings["WomenWatches"];
            }
            string menbags = AppSettingManager.AppSettings["MenBags"];
            string menclothing = AppSettingManager.AppSettings["MenClothing"];
            string menshoes = AppSettingManager.AppSettings["MenShoes"];
            string menaccessories = AppSettingManager.AppSettings["MenAccessories"];
            string MenHomes = AppSettingManager.AppSettings["MenHomes"];
            string MenBeauty = AppSettingManager.AppSettings["MenBeauty"];
            string MenWatches = AppSettingManager.AppSettings["MenWatches"];
            List<string> categoryList = new List<string>() { 
                AppSettingManager.AppSettings["WomenBags"],
                AppSettingManager.AppSettings["WomenBags"],
                AppSettingManager.AppSettings["WomenBags"],
            };
            ViewBag.categoryList = categoryList;
            //ViewBag.list = brandService.GetSWfsListAlterGroupList(pageIndex, pageSize, out count);
            ViewBag.list = AlterLists(channelno, pageIndex, pageSize, out count);
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;
            ViewBag.TotalCount = count;
            return View();
        }
        //返回列表的总条数
        public IEnumerable<SWfsListAlterGroup> AlterLists(string channelno, int pageIndex, int pageSize, out int count)
        {
            //string channelno = Request.QueryString[""];
            var dic = new Dictionary<string, object>();
            IEnumerable<SWfsListAlterGroup> list = DapperUtil.Query<SWfsListAlterGroup>("ComBeziWfs_SWfsListAlterGroup_Select", dic, new { channelno = channelno, pageIndex = pageIndex, pageSize = pageSize }).ToList();
            count = DapperUtil.Query<int>("ComBeziWfs_SWfsListAlterGroup_Select_Count", dic, new { channelno = channelno, pageIndex = pageIndex, pageSize = pageSize }).First<int>();
            return list;
        }

        /// <summary>
        /// 添加分组
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult AlterCreate(int id = 0)
        {
            SWfsBrandService service = new SWfsBrandService();
            SWfsListAlterGroup obj = service.GetGroupbyID(id);
            if (obj == null)
            {
                return View(new SWfsListAlterGroup { GroupId = id });
            }
            return View(obj);
        }
        [HttpPost]
        public ActionResult AlterCreate(SWfsListAlterGroup obj)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            SWfsBrandService service = new SWfsBrandService();
            //int groupId = int.Parse(Request.Form["GroupId"]);
            //string category = Request.Form["Category"];
            string channelno = Request.Form["channelname"];
            obj.CreateDate = DateTime.Now;
            obj.GroupName = Request.Form["GroupName"];
            //obj.Gender = Request.Form["Gender"] != null ? int.Parse(Request.Form["Gender"]) : 0;
            obj.Status = Request.Form["Status"] != null ? int.Parse(Request.Form["Status"]) : 0;
            obj.Category = channelno;
            obj.GroupType = 1;

            if (obj.GroupId == 0)
            {
                if (service.GroupCreate(obj) > 0)
                {
                    ViewData["tip"] = new HtmlString("<script>alert('添加成功'); window.location.href='ChannelAlterList.html?channelno=" + channelno + "'</script>");
                }
                else
                {
                    ViewData["tip"] = new HtmlString("<script>alert('添加失败');</script>");
                }
            }
            else
            {
                if (service.GroupUpdate(obj))
                {
                    ViewData["tip"] = new HtmlString("<script>alert('修改成功'); window.location.href='ChannelAlterList.html?channelno=" + channelno + "'</script>");
                }
                else
                {
                    ViewData["tip"] = new HtmlString("<script>alert('修改失败');</script>");
                }
            }
            return View(obj);
        }

        /// <summary>
        /// 添加轮播图片
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public ActionResult AlterimageCreate(int groupId)
        {
            SWfsBrandService service = new SWfsBrandService();
            IEnumerable<SWfsListAlterPic> list = service.getSWfsListAlterGroupListID(groupId);
            return View(list);
        }
        [HttpPost]
        public string ImgAlterUpload(SWfsListAlterPic obj, string linkname, int groupId)
        {
            SWfsBrandService service = new SWfsBrandService();
            obj.CreateDate = DateTime.Now;
            if (string.IsNullOrEmpty(obj.AlterAddress))
            {
                obj.AlterAddress = "";
            }
            #region 添加图片
            if (Request.Files["alter1file"] != null && Request.Files["alter1file"].ContentLength > 0)
            {
                obj.LargePicture = SaveImg(Request.Files["alter1file"], "width:765,Height:335,Length:300");
                if (rsPic.Keys.Contains("error"))
                {
                    ViewData["tip"] = new HtmlString("<script>alert('" + rsPic["error"] + "')</script>");
                    return rsPic["error"];
                }
            }
            else
            {
                if (string.IsNullOrEmpty(obj.LargePicture))
                {
                    obj.LargePicture = "";
                }
            }
            #endregion
            if (obj.LargePicture == "")
            {
                return "3";
            }
            if (obj.AlterPicId == 0)
            {
                var ser = service.HeavyRow(linkname, groupId);
                if (ser.Count() > 0)
                {
                    if (ser.ElementAt(0).AlterPicId != obj.AlterPicId)
                    {
                        return "2";
                    }
                }
                //调用排重方法
                var row = service.HeavyRow(linkname, groupId);
                if (row.Count() > 0)
                {
                    return "2";
                }
                obj.LinkName = Request.Form["LinkName"];
                //添加
                return service.Alterinsert(obj) > 0 ? "1" : "0";

            }
            else
            {
                obj.LinkName = Request.Form["LinkName"];
                //修改
                return service.AlterUpdate(obj) ? "1" : "0";
            }
        }

        //二级联动
        public string CategoryByManOrWomanChange(int aa)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder("<option value='-1'>品类</option>");
            if (aa == 0)
            {
                //女士
                sb.Append("<option value='" + AppSettingManager.AppSettings["WomenBags"] + "'>女士箱包</option>");
                sb.Append("<option value='" + AppSettingManager.AppSettings["WomenClothing"] + "'>女士服饰</option>");
                sb.Append("<option value='" + AppSettingManager.AppSettings["WomenShoes"] + "'>女士鞋靴</option>");
                sb.Append("<option value='" + AppSettingManager.AppSettings["WomenAccessories"] + "'>女士配饰</option>");
                sb.Append("<option value='" + AppSettingManager.AppSettings["WomenHomes"] + "'>女士家居</option>");
                sb.Append("<option value='" + AppSettingManager.AppSettings["WomenBeauty"] + "'>女士美妆</option>");
                sb.Append("<option value='" + AppSettingManager.AppSettings["WomenWatches"] + "'>女士腕表</option>");
            }
            else if (aa == 1)
            {
                //男士
                sb.Append("<option value='" + AppSettingManager.AppSettings["MenBags"] + "'>男士箱包</option>");
                sb.Append("<option value='" + AppSettingManager.AppSettings["MenClothing"] + "'>男士服饰</option>");
                sb.Append("<option value='" + AppSettingManager.AppSettings["MenShoes"] + "'>男士鞋靴</option>");
                sb.Append("<option value='" + AppSettingManager.AppSettings["MenAccessories"] + "'>男士配饰</option>");
                sb.Append("<option value='" + AppSettingManager.AppSettings["MenHomes"] + "'>男士家居</option>");
                sb.Append("<option value='" + AppSettingManager.AppSettings["MenBeauty"] + "'>男士美妆</option>");
                sb.Append("<option value='" + AppSettingManager.AppSettings["MenWatches"] + "'>男士腕表</option>");
            }
            else
            {
                //sb.Append("<option value='" + AppSettingManager.AppSettings["WomenBags"] + "'>女士箱包</option>");
                //sb.Append("<option value='" + AppSettingManager.AppSettings["WomenClothing"] + "'>女士服装</option>");
                //sb.Append("<option value='" + AppSettingManager.AppSettings["WomenShoes"] + "'>女士鞋靴</option>");
                //sb.Append("<option value='" + AppSettingManager.AppSettings["WomenAccessories"] + "'>女士配饰</option>");
                //sb.Append("<option value='" + AppSettingManager.AppSettings["WomenHomes"] + "'>女士家居</option>");
                //sb.Append("<option value='" + AppSettingManager.AppSettings["WomenBeauty"] + "'>女士美妆</option>");
                //sb.Append("<option value='" + AppSettingManager.AppSettings["WomenWatches"] + "'>女士腕表</option>");
                //sb.Append("<option value='" + AppSettingManager.AppSettings["MenBags"] + "'>女士箱包</option>");
                //sb.Append("<option value='" + AppSettingManager.AppSettings["MenClothing"] + "'>男士服装</option>");
                //sb.Append("<option value='" + AppSettingManager.AppSettings["MenShoes"] + "'>男士鞋靴</option>");
                //sb.Append("<option value='" + AppSettingManager.AppSettings["MenAccessories"] + "'>男士配饰</option>");
                //sb.Append("<option value='" + AppSettingManager.AppSettings["MenHomes"] + "'>男士家居</option>");
                //sb.Append("<option value='" + AppSettingManager.AppSettings["MenBeauty"] + "'>男士美妆</option>");
                //sb.Append("<option value='" + AppSettingManager.AppSettings["MenWatches"] + "'>男士腕表</option>");
            }
            return sb.ToString();
        }

        //修改状态
        public int AlterlistStatus(int groupId, int status, string category)
        {
            SWfsChannelService bll = new SWfsChannelService();
            return bll.AlterStatus(groupId, status, category);
        }

        //删除分组同时删除图片
        public ActionResult AlterGropDelete(int groupId)
        {
            SWfsBrandService brandService = new SWfsBrandService();
            var dele = brandService.AlterDelete(groupId);
            return Redirect("ChannelAlterList.html?channelno=" + Request.QueryString["channelno"]);
        }
        #endregion

        #region 频道OCS分类推荐
        //查询推荐的分类
        public ActionResult RecommendCategory(string channelNO)
        {
            return View(channelService.GetRecommendCategory(channelNO));
        }
        //添加推荐分类
        public ActionResult AddRecommendCategory()
        {
            //添加推荐分类
            channelService.AddRecommendCategory(Request.Form["itemobj"], Request.Form["channelno"]);
            //取消推荐分类
            if (!string.IsNullOrEmpty(Request.Form["cancelCategory"]))
            {
                channelService.CancelRecommendCategory(Request.Form["cancelCategory"]);
            }
            return Redirect("/shangpin/channel/RecommendCategory?channelno=" + Request.Form["channelno"]);
        }
        //保存推荐分类的排序值
        public JsonResult SaveCategorySortValue(int id, int sortValue)
        {
            if (channelService.SaveCategorySortValue(id, sortValue))
            {
                return Json(new { result = 1, message = "操作成功" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { result = 0, message = "操作失败" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 频道推荐品牌
        //获取推荐的频道品牌列表
        public ActionResult RecommendBrandList(string channelNO)
        {
            return View(channelService.GetRecommendBrand(channelNO));
        }
        //添加推荐品牌
        [HttpPost]
        public ActionResult AddRecommendBrand()
        {
            SWfsSpChannelRecommendBrand obj = null;
            if (Request.Form["itemObj"] != null)
            {
                string[] itemList = Request.Form["itemobj"].Split(',');
                IList<ChannelRecommendBrandExtends> addRecommendList = channelService.GetAddRecommendBrand(Request.Form["channelno"]);
                for (int i = 0; i < itemList.Length; i++)
                {
                    if (addRecommendList.Count(p => p.BrandNO == itemList[i]) > 0)
                    {
                        //continue;
                    }
                    obj = new SWfsSpChannelRecommendBrand();
                    obj.ChannelNO = Request.Form["channelno"];
                    obj.BrandNO = itemList[i];
                    obj.CreateDate = DateTime.Now;
                    obj.SortValue = 999;
                    channelService.AddRecommendBrand(obj);
                }
            }
            //删除取消推荐的品牌
            if (!string.IsNullOrEmpty(Request.Form["cancelBrandNO"]))
            {
                channelService.CancelRecommendBrand(Request.Form["cancelBrandNO"]);
            }
            return Redirect("/shangpin/channel/RecommendBrandList?channelno=" + Request.Form["channelno"]);
        }
        //修改排序
        public JsonResult SaveBrandSortValue(int id, int sort)
        {
            if (channelService.SaveBrandSortValue(id, sort))
            {
                return Json(new { result = 1, message = "操作成功" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { result = 0, message = "操作失败" }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region 频道推荐标题管理
        /// <summary>
        /// 频道推荐标题管理
        /// </summary>
        /// <returns></returns>
        public ActionResult ChannelRecommendLink()
        {
            string channelNo = Rq.GetStringQueryString("channelno");
            IList<SWfsSpChannelRecommendLink> channelRecommendLinkList = null;
            if (!string.IsNullOrEmpty(channelNo))
            {
                channelRecommendLinkList = channelService.GetSWfsSpChannelRecommendLinkList(channelNo);
            }
            return View(channelRecommendLinkList);
        }

        /// <summary>
        /// 频道推荐标题管理更新或者添加信息
        /// </summary>
        /// <returns></returns>
        public ActionResult ChannelRecommendLinkAddOrUpdate()
        {
            string channelNo = Rq.GetStringForm("txtchannelNo");
            int parentIDValue = 0;
            for (int i = 0; i <= 6; i++)
            {

                SWfsSpChannelRecommendLink recommendLinkSingle = new SWfsSpChannelRecommendLink();
                string RecommendLinkID = Rq.GetStringForm("txtRecommendLinkIDUpdate" + i);
                if (!string.IsNullOrEmpty(RecommendLinkID) && RecommendLinkID != "0")
                {
                    recommendLinkSingle.RecommendLinkID = int.Parse(RecommendLinkID);
                }
                string RecommendLinkName = Rq.GetStringForm("txtRecommendLinkID" + i);
                string RecommendLinkUrl = Rq.GetStringForm("txtLinkID" + i);
                string RecommendLinkSortValue = Rq.GetStringForm("txtSortValue" + i);
                int RecommendLinkSortValueInt = 0;
                int.TryParse(RecommendLinkSortValue, out RecommendLinkSortValueInt);
                recommendLinkSingle.ChannelNO = channelNo;
                recommendLinkSingle.LinkName = RecommendLinkName == "最多5个汉字" ? "" : RecommendLinkName;
                recommendLinkSingle.LinkUrl = RecommendLinkUrl == "http://" ? "" : RecommendLinkUrl;
                recommendLinkSingle.SortValue = RecommendLinkSortValueInt;
                recommendLinkSingle.CreateDate = DateTime.Now;
                if (i == 0)
                {
                    recommendLinkSingle.ParentID = 0;
                    channelService.EditeSWfsSpChannelRecommendLink(recommendLinkSingle);
                    parentIDValue = recommendLinkSingle.RecommendLinkID;
                }
                else
                {
                    recommendLinkSingle.ParentID = parentIDValue;
                    channelService.EditeSWfsSpChannelRecommendLink(recommendLinkSingle);
                }
            }
            CommonHelp.Alert("保存成功", "/shangpin/channel/ChannelRecommendLink?channelno=" + channelNo);
            return View();
        }
        #endregion

        #region 清除Style缓存
        public JsonResult ClearStyleCache()
        {
            try
            {
                RedisCacheProvider.Instance.Remove("ShangPin_Channel_Style_Index_StyleIdData");
                RedisCacheProvider.Instance.Remove("Shangpin_Style_GetTagsList_All");
                return Json("清除成功", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("清除异常" + ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
    }
}
