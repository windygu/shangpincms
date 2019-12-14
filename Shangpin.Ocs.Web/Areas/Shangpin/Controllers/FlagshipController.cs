using Shangpin.Entity.Wfs;
using Shangpin.Framework.Common;
using Shangpin.Framework.Common.Cache;
using Shangpin.Ocs.Entity.Extenstion.ShangPin;
using Shangpin.Ocs.Service;
using Shangpin.Ocs.Service.Shangpin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shangpin.Ocs.Web.Areas.Shangpin.Controllers
{
    public class FlagshipController : Controller
    {

        public ActionResult FlagshipList(int pageSize = 10, int pageIndex = 1)
        {
            SWfsBrandService bll = new SWfsBrandService();
            int count = 0;
            //IList<SWfsBrandExtendList> list = bll.AIIBrandsList(pageIndex, pageSize, out count);
            ViewBag.list = AllBrandsQueryList(pageIndex, pageSize, out count);
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;
            ViewBag.TotalCount = count;
            return View();
        }
        public IEnumerable<SWfsBrandExtendList> AllBrandsQueryList(int pageIndex, int pageSize, out int count)
        {
            var dic = new Dictionary<string, object>();
            //string aa = Request.Form["NaviTypeId"];
            ViewBag.KeyWord = Request.QueryString["BrandCnName"] == null ? "" : Request.QueryString["BrandCnName"].Trim();
            dic.Add("BrandCnName", Request.QueryString["BrandCnName"] == null ? "" : Request.QueryString["BrandCnName"].Trim());
            dic.Add("NaviTypeId", 1);
            dic.Add("BrandTypeId", 2);
            if (Request.QueryString["storestatus"] != "-1" && Request.QueryString["storestatus"] != null)
            {
                dic.Add("storestatus", int.Parse(Request.QueryString["storestatus"]));
            }
            else
            {
                dic.Add("storestatus", "");
            }

            if (Request.QueryString["Status"] != "" && Request.QueryString["Status"] != null)
            {
                dic.Add("Status", int.Parse(Request.QueryString["Status"]));
            }
            else
            {
                dic.Add("Status", "");
            }
            dic.Add("BrandEnName", Request.QueryString["enName"] != null ? Request.QueryString["enName"] : "");
            dic.Add("ZeroToNine", Request.QueryString["ZeroToNine"] != null ? Request.QueryString["ZeroToNine"] : "");
            dic.Add("pageIndex", pageIndex);
            dic.Add("pageSize", pageSize);
            //查询分页集合数据
            IEnumerable<SWfsBrandExtendList> query = DapperUtil.Query<SWfsBrandExtendList>("ComBeziWfs_SWfsBrandExtend_AllBrands_List", dic, new { BrandCnName = dic["BrandCnName"], NaviTypeId = dic["NaviTypeId"], BrandTypeId = dic["BrandTypeId"], Status = dic["Status"], BrandEnName = dic["BrandEnName"], ZeroToNine = dic["ZeroToNine"], pageIndex = pageIndex, pageSize = pageSize, storestatus = dic["storestatus"] });
            //查询总条数
            int totalCount = DapperUtil.Query<int>("ComBeziWfs_SWfsBrandExtend_AllBrands_Select", dic, new { BrandCnName = dic["BrandCnName"], NaviTypeId = dic["NaviTypeId"], BrandTypeId = dic["BrandTypeId"], Status = dic["Status"], BrandEnName = dic["BrandEnName"], ZeroToNine = dic["ZeroToNine"], storestatus = dic["storestatus"] }).First<int>();
            count = totalCount;
            return query;
        }


        SwfsFlagShipLogoService _logo = SwfsFlagShipLogoService.instrance();
        SwfsFlagShipFlashService _flash = SwfsFlagShipFlashService.instrance();
        CommonService _com = new CommonService();
        SWfsFlagShipOperationPictureService _pic = SWfsFlagShipOperationPictureService.instance();
        private Dictionary<string, string> rsPic = new Dictionary<string, string>();
        public ActionResult Index(string brandNo)
        {
            if (string.IsNullOrEmpty(brandNo))
                Response.Redirect("/Shangpin/Brand/AIIBrandsSelect");
            List<SwfsFlagShipModule> modules = _SwfsFlagShipModuleService.GetOrInitialBrandModules(brandNo);
            ViewBag.Modules = modules;
            ViewBag.ModuleLinkDic = _SwfsFlagShipModuleLinkService.GetFloorLinkDic(modules.Select(a => a.ModuleId).ToArray());
            var g01 = _pic.GetEntityByBrandNoAndIndex(brandNo, 0);
            var g02 = _pic.GetEntityByBrandNoAndIndex(brandNo, 1);
            var g03 = _pic.GetEntityByBrandNoAndIndex(brandNo, 2);
            ViewBag.picur1 = g01 == null ? "/Areas/Shangpin/Images/topshop/test_03.jpg" : "/ReadPic/GetPic.ashx?width=0&height=0&pictureFileNo=" + g01.PictureFileNo + "&type=2";
            ViewBag.picur2 = g02 == null ? "/Areas/Shangpin/Images/topshop/test_03.jpg" : "/ReadPic/GetPic.ashx?width=0&height=0&pictureFileNo=" + g02.PictureFileNo + "&type=2";
            ViewBag.picur3 = g03 == null ? "/Areas/Shangpin/Images/topshop/test_03.jpg" : "/ReadPic/GetPic.ashx?width=0&height=0&pictureFileNo=" + g03.PictureFileNo + "&type=2";
            SwfsFlagShipLogo entity = _logo.GetEntityByBrandNo(brandNo);
            ViewBag.logourl = entity == null ? "" : "/ReadPic/GetPic.ashx?width=0&height=0&pictureFileNo=" + entity.LogoNo + "&type=2";
            var flash_ = _flash.GetEntityByBrandNo(brandNo).FirstOrDefault();
            if (flash_ != null)
                ViewBag.flash_url = "/ReadPic/GetPic.ashx?width=0&height=0&pictureFileNo=" + flash_.PictureNo + "&type=2";
            //获取此品牌导航信息 用于导航链接
            ViewBag.BrandInfo = new SWfsBrandService().GetSWfsBrandExtendListByBrandNo(brandNo) ?? new SWfsBrandExtendList();
            return View();
        }
        #region 品牌Logo管理
        public ActionResult FlagShipLogo(string brandNo)
        {
            if (string.IsNullOrEmpty(brandNo))
                Response.Redirect("/Shangpin/Brand/AIIBrandsSelect");
            SwfsFlagShipLogo entity = _logo.GetEntityByBrandNo(brandNo);
            if (entity == null)
            {
                entity = new SwfsFlagShipLogo();
                entity.CreateDate = DateTime.Now;
                entity.BrandNo = brandNo;
            }
            return View(entity);
        }
        [HttpPost]
        public JsonResult FlagShipLogo(FormCollection form)
        {

            ResultBase_form result = new ResultBase_form();
            SwfsFlagShipLogo entity = new SwfsFlagShipLogo();
            TryUpdateModel<SwfsFlagShipLogo>(entity, form);
            HttpPostedFileBase file = Request.Files["picFile"];
            rsPic.Clear();
            if (file != null && file.ContentLength > 0)
            {
                string width = form["Width"];
                string height = form["Height"];
                string length = form["Length"];
                rsPic = _SwfsFlagShipModuleLinkService.SaveImg(Request.Files["picFile"], "width:" + width + " ,Height:" + height + ",Length:" + length + "");
                if (rsPic.Keys.Contains("error"))
                {
                    result.status = 0;
                    result.msg = rsPic["error"];
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    entity.LogoNo = rsPic["success"];
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(Request.Form["LogoNo"]))
                {
                    entity.LogoNo = Request.Form["LogoNo"];
                }
            }
            if (entity.LogoId == 0)
                result.status = _logo.Insert(entity);
            else
                result.status = _logo.Update(entity) ? 1 : 0;
            result.msg = result.status == 0 ? "操作失败" : "操作成功";
            result.content = entity;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region 轮播图
        public ActionResult AlterPicEdit(string id, string brandNo)
        {
            SwfsFlagShipFlash entity;
            if (string.IsNullOrEmpty(id))
            {
                entity = new SwfsFlagShipFlash();
                entity.DataCreate = DateTime.Now;
                entity.PictureIndex = 0;
                entity.State = 1;
                entity.DataState = 1;
            }
            else
                entity = _flash.GetEntityByIdAndBrandNo(Convert.ToInt32(id), brandNo);
            return View(entity);
        }

        [HttpPost]
        public JsonResult AlterPicEdit(FormCollection form)
        {
            ResultBase_form result = new ResultBase_form();
            SwfsFlagShipFlash entity = new SwfsFlagShipFlash();
            TryUpdateModel<SwfsFlagShipFlash>(entity, form);
            HttpPostedFileBase file = Request.Files["picFile"];
            rsPic.Clear();
            if (file != null && file.ContentLength > 0)
            {
                rsPic = _SwfsFlagShipModuleLinkService.SaveImg(Request.Files["picFile"], "width:960 ,Height:470,Length:500");
                //rsPic = _com.PostImg(file, "width:0 ,Height:0,Length:100");
                if (rsPic.Keys.Contains("error"))
                {
                    result.status = 0;
                    result.msg = rsPic["error"];
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    entity.PictureNo = rsPic["success"];
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(Request.Form["LogoNo"]))
                {
                    entity.PictureNo = Request.Form["LogoNo"];
                }
            }
            if (entity.FlashId == 0)
                result.status = _flash.Insert(entity);
            else
                result.status = _flash.Update(entity) ? 1 : 0;
            result.msg = result.status == 0 ? "操作失败" : "操作成功";
            result.content = entity;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CheckTime(string brandNo, DateTime BeginDateTime)
        {
            IEnumerable<SwfsFlagShipFlash> list = _flash.GetEntityByBranNoAndBeginTime(brandNo, BeginDateTime);
            if (list.Count() > 0)
            {
                return Json(list.FirstOrDefault().BrandNo == brandNo, JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }



        public ActionResult AlterPicList(string BrandNo, string PictureName, int State = -1, int PictureIndex = -1, string beginTime = "", string endTime = "", int pageIndex = 1, int pageSize = 20)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic["BrandNo"] = BrandNo;
            dic["PictureName"] = string.IsNullOrEmpty(PictureName) ? "" : PictureName;
            dic["State"] = Convert.ToInt16(State);
            dic["PictureIndex"] = Convert.ToInt32(PictureIndex);
            dic["beginTime"] = string.IsNullOrEmpty(beginTime) ? "" : DateTime.Parse(beginTime).ToShortDateString();
            dic["endTime"] = string.IsNullOrEmpty(endTime) ? "" : DateTime.Parse(endTime).AddDays(1).ToShortDateString();
            int count;
            IEnumerable<SwfsFlagShipFlash> list = _flash.AlterLists(pageIndex, pageSize, dic, out count);
            ViewBag.list = list.ToList();
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;
            ViewBag.TotalCount = count;
            return View();
        }
        public JsonResult DelAlterPicById(string id)
        {
            return Json(new { status = _flash.Delete(id) }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region 广告位

        public ActionResult OperationPicture(string brandNo, int index)
        {
            if (string.IsNullOrEmpty(brandNo))
                Response.Redirect("/Shangpin/Brand/AIIBrandsSelect");
            SWfsFlagShipOperationPicture entity = _pic.GetEntityByBrandNoAndIndex(brandNo, index);
            if (entity == null)
            {
                string strChnNames = "一二三四五六七八九";
                entity = new SWfsFlagShipOperationPicture();
                entity.PageNo = "index";
                entity.PictureIndex = Convert.ToInt16(index);
                entity.PagePositionNo = "bottom";
                entity.DateBegin = entity.DateEnd = entity.DateCreate = DateTime.Now;
                entity.DataState = 1;
                entity.PictureName = entity.PagePositionName = "底部广告位(" + strChnNames[index] + ")";
                entity.SortId = 0;
                entity.LinkAddress = "http://";
            }
            return View(entity);
        }
        [HttpPost]
        public JsonResult OperationPicture(FormCollection form)
        {
            ResultBase_form result = new ResultBase_form();
            SWfsFlagShipOperationPicture entity = new SWfsFlagShipOperationPicture();
            TryUpdateModel<SWfsFlagShipOperationPicture>(entity, form);
            HttpPostedFileBase file = Request.Files["picFile"];
            rsPic.Clear();
            if (file != null && file.ContentLength > 0)
            {
                rsPic = _SwfsFlagShipModuleLinkService.SaveImg(Request.Files["picFile"], "width:310 ,Height:200,Length:150");
                //rsPic = _com.PostImg(file, "width:0 ,Height:0,Length:100");
                if (rsPic.Keys.Contains("error"))
                {
                    result.status = 0;
                    result.msg = rsPic["error"];
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    entity.PictureFileNo = rsPic["success"];
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(Request.Form["LogoNo"]))
                {
                    entity.PictureFileNo = Request.Form["PictureFileNo"];
                }
            }
            if (entity.PictureManageId == 0)
                result.status = _pic.Insert(entity);
            else
                result.status = _pic.Update(entity) ? 1 : 0;
            result.msg = result.status == 0 ? "操作失败" : "操作成功";
            result.content = entity;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region 楼层 -hbq
        SwfsFlagShipModuleService _SwfsFlagShipModuleService = new SwfsFlagShipModuleService();
        SwfsFlagShipModuleLinkService _SwfsFlagShipModuleLinkService = new SwfsFlagShipModuleLinkService();
        CommonService _CommonService = new CommonService();
        #region 楼层图片
        public ActionResult EditFloorPic(int LinkId, int ModuleId, int Index)
        {
            SwfsFlagShipModuleLink link = _SwfsFlagShipModuleLinkService.GetSwfsFlagShipModuleLinkById(LinkId) ?? new SwfsFlagShipModuleLink
            {
                LinkId = LinkId,
                ModuleId = ModuleId,
                PicIndex = (short)Index
            };
            Dictionary<string, string> dic = _SwfsFlagShipModuleLinkService.GetPicSizeAndTip();
            ViewBag.Tip = dic.ContainsKey("pic" + (link.PicIndex + 1) + "Tip") ? dic["pic" + (link.PicIndex + 1) + "Tip"] : "";
            ViewBag.Size = dic.ContainsKey("pic" + (link.PicIndex + 1)) ? dic["pic" + (link.PicIndex + 1)] : "";
            ViewBag.Show = dic.ContainsKey("pic" + (link.PicIndex + 1) + "Show") ? dic["pic" + (link.PicIndex + 1) + "Show"] : "";
            return View(link);
        }

        [HttpPost]
        public string SaveFloorPic(FormCollection F)
        {
            int LinkId = Convert.ToInt32(F["LinkId"]);
            int ModuleId = Convert.ToInt32(F["ModuleId"]);
            int Index = Convert.ToInt16(F["Index"]);
            string picSizeStr = _SwfsFlagShipModuleLinkService.GetPicSizeAndTip()["pic" + (Index + 1) + "Check"];
            SwfsFlagShipModuleLink link = _SwfsFlagShipModuleLinkService.CreateEmptySwfsFlagShipModuleLink();
            SwfsFlagShipModule module = _SwfsFlagShipModuleService.GetSwfsFlagShipModuleById(ModuleId);
            string picOldFileId = null;
            link.LinkId = LinkId;
            if (link.LinkId > 0)
            {
                link = _SwfsFlagShipModuleLinkService.GetSwfsFlagShipModuleLinkById(LinkId);
                picOldFileId = link.PictureNo;
            }
            else
            {
                List<SwfsFlagShipModuleLink> links = _SwfsFlagShipModuleLinkService.GetAllSwfsFlagShipModuleLinkByModuleId(ModuleId);
                if (links.Any(a => a.PicIndex == link.PicIndex && link.ShowType == 1))
                {
                    return "{\"status\" : 0, \"message\" : \"此楼层已存在同位置图片,请返回管理页面\"}";
                }
            }
            if (Request.Files["picFile"] != null && Request.Files["picFile"].ContentLength > 0)
            {
                Dictionary<string, string> dic = _SwfsFlagShipModuleLinkService.SaveImg(Request.Files["picFile"], picSizeStr);
                if (dic.Keys.Contains("error"))
                {
                    return "{\"status\" : 0, \"message\" : \"图片不合法" + dic["error"] + "\"}";
                }
                else if (dic.Keys.Contains("success")) { link.PictureNo = dic["success"]; }
                if (!string.IsNullOrEmpty(picOldFileId))//删除旧图片
                {
                    _CommonService.DeleteImg(picOldFileId);
                }
            }
            link.ModuleId = ModuleId;
            link.PicIndex = (short)Index;
            link.ModuleType = module.ModuleType;
            link.ShowType = 1;//0是产品的，1是有图片的
            link.LinkUrl = F["LinkUrl"];
            _SwfsFlagShipModuleLinkService.InsertOrUpdateSwfsFlagShipModuleLink(link);
            EnyimMemcachedClient.Instance.Remove("FlagShip_SWfsFlagShipModule_BrandFlagShipModuleList_Memcached"+module.BrandNo);//清除楼层缓存
            return "{\"status\" : 1, \"message\" : \"保存成功\"}";
        }
        #endregion
        #region 楼层关联OCS分类
        [HttpGet]
        public ActionResult FloorRefOCS(int ModuleId)
        {
            ViewBag.ModuleId = ModuleId;
            SWfsBrandService dal = new SWfsBrandService();
            SwfsFlagShipModule module = _SwfsFlagShipModuleService.GetSwfsFlagShipModuleById(ModuleId);
            List<SwfsFlagShipModuleLink> links = _SwfsFlagShipModuleLinkService.GetAllSwfsFlagShipModuleLinkByModuleId(ModuleId).Where(a => a.ShowType == 0 && a.State == 1).ToList();
            ViewBag.ModuleType = module.ModuleType;
            ViewBag.brandNo = module.BrandNo;
            string CategoryNo = "";
            if (links != null && links.Any())
            {
                CategoryNo = links[0].CategoryNo;
            }
            else
            {
                links = new List<SwfsFlagShipModuleLink> { new SwfsFlagShipModuleLink() };
            }
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            IList<OCSInfo> list = dal.GetOCSListByBrandNO(module.BrandNo, CategoryNo, sb);
            //获取已经添加过的
            ViewBag.RightAdd = new HtmlString(sb.ToString());
            sb.Length = 0;
            dal.GetOCSTree(list, "ROOT", sb);
            ViewBag.TreeHTML = new HtmlString(sb.ToString());
            sb.Length = 0;

            return View(links[0]);
        }
        [HttpPost]
        public ActionResult FloorRefOCS(SwfsFlagShipModuleLink link)
        {
            try
            {
                SwfsFlagShipModuleLink temp = _SwfsFlagShipModuleLinkService.CreateEmptySwfsFlagShipModuleLink();
                SwfsFlagShipModule module = _SwfsFlagShipModuleService.GetSwfsFlagShipModuleById(link.ModuleId);
                if (link.LinkId > 0)
                {
                    temp = _SwfsFlagShipModuleLinkService.GetSwfsFlagShipModuleLinkById(link.LinkId);
                }
                else
                {
                    List<SwfsFlagShipModuleLink> links = _SwfsFlagShipModuleLinkService.GetAllSwfsFlagShipModuleLinkByModuleId(module.ModuleId);
                    if ((module.ModuleType != 2 && links.Count(a => a.ShowType == 0) >= 1) || (module.ModuleType == 2 && links.Count(a => a.ShowType == 0) >= 2))
                    {
                        ViewData["tip"] = new HtmlString("<script>alert('操作失败,分类数量超过限制');location.reload();</script>");
                    }
                }
                temp.ShowType = 0;//0是产品的，1是有图片的
                temp.CategoryNo = link.CategoryNo;
                temp.ModuleId = link.ModuleId;
                temp.LinkUrl = link.LinkUrl;
                temp.ModuleType = module.ModuleType;
                _SwfsFlagShipModuleLinkService.InsertOrUpdateSwfsFlagShipModuleLink(temp);
                EnyimMemcachedClient.Instance.Remove("FlagShip_SWfsFlagShipModule_BrandFlagShipModuleList_Memcached" + module.BrandNo);//清除楼层缓存
                ViewData["tip"] = new HtmlString("<script>alert('操作成功');location.href='/Shangpin/flagship/index?brandNo=" + module.BrandNo + "';</script>");
            }
            catch (Exception e)
            {
                ViewData["tip"] = new HtmlString("<script>alert('操作失败 错误信息:'" + e.Message + ");</script>");
            }
            return FloorRefOCS(link.ModuleId);
        }

        #endregion
        /// <summary>
        /// 修改楼层名称
        /// </summary>
        /// <param name="ModuleId"></param>
        /// <param name="ModuleName"></param>
        /// <returns></returns>
        public ActionResult AlterFloorName(int ModuleId, string ModuleName)
        {
            try
            {
                SwfsFlagShipModule module = _SwfsFlagShipModuleService.GetSwfsFlagShipModuleById(ModuleId);
                module.ModuleName = ModuleName;
                _SwfsFlagShipModuleService.InsertOrUpdateSwfsFlagShipModule(module);
                EnyimMemcachedClient.Instance.Remove("FlagShip_SWfsFlagShipModule_BrandFlagShipModuleList_Memcached" + module.BrandNo);//清除楼层缓存
                return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { status = 0, e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// 修改楼层状态
        /// </summary>
        /// <param name="ModuleId"></param>
        /// <returns></returns>
        public ActionResult AlterFloorState(int ModuleId, short value)
        {
            try
            {
                SwfsFlagShipModule module = _SwfsFlagShipModuleService.GetSwfsFlagShipModuleById(ModuleId);
                module.State = value;
                _SwfsFlagShipModuleService.InsertOrUpdateSwfsFlagShipModule(module);
                EnyimMemcachedClient.Instance.Remove("FlagShip_SWfsFlagShipModule_BrandFlagShipModuleList_Memcached"+module.BrandNo);//清除楼层缓存
                return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { status = 0, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// 修改楼层排序
        /// </summary>
        /// <param name="ModuleId"></param>
        /// <param name="Sort"></param>
        /// <returns></returns>
        public ActionResult AlterFloorSort(int ModuleId, int Sort)
        {
            try
            {
                SwfsFlagShipModule module = _SwfsFlagShipModuleService.GetSwfsFlagShipModuleById(ModuleId);
                List<SwfsFlagShipModule> moduleAll = _SwfsFlagShipModuleService.GetAllModulesByBrandNo(module.BrandNo);
                SwfsFlagShipModule module2 = moduleAll.First(a => a.SortId == Sort);
                module2.SortId = module.SortId;
                module.SortId = Sort;
                if (module.SortId > 0 && module.SortId < 4 && module2.SortId > 0 && module2.SortId < 4)
                {
                    _SwfsFlagShipModuleService.InsertOrUpdateSwfsFlagShipModule(module);
                    _SwfsFlagShipModuleService.InsertOrUpdateSwfsFlagShipModule(module2);
                }
                EnyimMemcachedClient.Instance.Remove("FlagShip_SWfsFlagShipModule_BrandFlagShipModuleList_Memcached"+module.BrandNo);//清除楼层缓存 移动位置不改变
                return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { status = 0, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        #region  旗舰店状态切换
        public JsonResult ChangeFlagShopStatus(string BrandNo, short value)
        {
            try
            {
                if (!string.IsNullOrEmpty(BrandNo))
                {
                    SwfsFlagShipGloalConfig config = _SwfsFlagShipModuleService.GetGlobalConfigByBrandNo("Flagship_" + BrandNo);
                    config.ConfigValue = value.ToString();
                    _SwfsFlagShipModuleService.InsertOrUpdateSwfsFlagShipGloalConfig(config);
                }
                EnyimMemcachedClient.Instance.Remove("FlagShip_SWfsFlagShipModule_BrandFlagShipModuleList_Memcached"+BrandNo);//清除楼层缓存
            }
            catch (Exception e)
            {
                return Json(new { status = -1, e.Message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="BrandNo"></param>
        /// <returns></returns>
        public JsonResult GetFlagShopStatus(string BrandNo)
        {
            try
            {
                SwfsFlagShipGloalConfig config = null;
                if (!string.IsNullOrEmpty(BrandNo))
                    config = _SwfsFlagShipModuleService.GetOrInitSwfsFlagShipGloalConfigbyBrandNo("Flagship_" + BrandNo) ?? _SwfsFlagShipModuleService.GetOrInitSwfsFlagShipGloalConfigbyBrandNo("Flagship_" + BrandNo);
                return Json(new { status = 1, data = new { name = config.ConfigName, value = config.ConfigValue } }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { status = -1, e.Message }, JsonRequestBehavior.AllowGet);
            }

        }
        #endregion
        #endregion


    }
}
