using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Shangpin.Ocs.Service.Shangpin;
using Shangpin.Ocs.Service;
using Shangpin.Entity.Wfs;
using Shangpin.Framework.WebMvc;
using Shangpin.Framework.Common.Cache;
using Shangpin.Ocs.Entity.Extenstion.ShangPin;
using System.Text;
using Shangpin.Ocs.Service.Common;
using Shangpin.Ocs.Entity.Extenstion.Login;

namespace Shangpin.Ocs.Web.Areas.Shangpin.Controllers
{
    [OCSAuthorization]
    public class ProductController : Controller
    {
        //
        // GET: /Shangpin/ProductRef/

        private readonly SWfsProductRefService productService;
        private Dictionary<string, string> rsPic = new Dictionary<string, string>();
        private static readonly string[] StructRegion = AppSettingManager.AppSettings["StructRegion"].Split(',');
        public ProductController()
        {
            productService = new SWfsProductRefService();
        }

        #region 商品列表
        public ActionResult ProductManager(int pageIndex = 1, int pageSize = 20)
        {
            Passport passport = PresentationHelper.GetPassport();
            System.Text.StringBuilder sb = new System.Text.StringBuilder(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + passport.UserName + " ");
            int total = 0;
            if (Request.QueryString["editePeople"] != null)
            {
                sb.Append("编辑人:" + Request.QueryString["editePeople"] + " ");
            }
            if (Request.QueryString["publishPeople"] != null)
            {
                sb.Append("发布人:" + Request.QueryString["editePeople"] + " ");
            }
            if (Request.QueryString["keyWord"] != null)
            {
                sb.Append("关键字:" + Request.QueryString["keyWord"] + " ");
            }
            if (Request.QueryString["productNo"] != null)
            {
                sb.Append("商品编号:" + Request.QueryString["productNo"] + " ");
            }
            if (Request.QueryString["CategoryNo"] != null)
            {
                sb.Append("商品分类:" + Request.QueryString["CategoryName"] + " ");
            }
            if (Request.QueryString["brandNO"] != null)
            {
                sb.Append("品牌:" + Request.QueryString["BrandName"] + " ");
            }
            if (Request.QueryString["isnewShelf"] == "0")
            {
                sb.Append("新上架: ");
            }

            if (Request.QueryString["timeType"] == "1")
            {
                sb.Append(">编辑时间 :" + Request.QueryString["startTime"] + "到" + Request.QueryString["endTime"] + " ");
            }
            if (Request.QueryString["timeType"] == "2")
            {
                sb.Append(">发布时间 :" + Request.QueryString["startTime"] + "到" + Request.QueryString["endTime"] + " ");
            }
            if (Request.QueryString["timeType"] == "3")
            {
                sb.Append(">上架时间 :" + Request.QueryString["startTime"] + "到" + Request.QueryString["endTime"] + " ");
            }
            if (Request.QueryString["priceStart"] != null || Request.QueryString["priceEnd"] != null)
            {
                sb.Append("商品价格:" + (Request.QueryString["priceStart"] == null ? "0" : Request.QueryString["priceStart"]) + "到" + (Request.QueryString["priceEnd"] == null ? "最大" : Request.QueryString["priceEnd"]) + " ");
            }
            if (Request.QueryString["rateStart"] != null || Request.QueryString["rateEnd"] != null)
            {
                sb.Append("商品折扣:" + (Request.QueryString["rateStart"] == null ? "0" : Request.QueryString["rateStart"]) + "到" + (Request.QueryString["rateEnd"] == null ? "最大" : Request.QueryString["rateEnd"]) + " ");
            }

            if (Request.QueryString["IsRecord"] == "1" && string.IsNullOrEmpty(Request.QueryString["addProductListText"]))
            {
                SWfsProductSearchHistory obj = new SWfsProductSearchHistory()
                {
                    SearchName = sb.ToString(),
                    SearchUser = passport.UserName,
                    SearchUrl = Request.Url.ToString().Replace("http://" + Request.Url.Host, "").Replace("IsRecord", "IsRecordAdd"),
                    LogType = 1
                };
                productService.AddSearchHistory(obj);
                return Redirect(obj.SearchUrl);
            }
            //加载历史查询数据
            ViewBag.HistoryList = productService.GetSearchHistory(10);
            IEnumerable<SkillProductExtends> list = productService.GetSWfsProductList(Request.QueryString["editePeople"], Request.QueryString["publishPeople"], Request.QueryString["keyWord"],
                Request.QueryString["productNo"], Request.QueryString["CategoryNo"], Request.QueryString["brandNO"], Request.QueryString["isnewShelf"], Request.QueryString["timeType"],
                Request.QueryString["startTime"], Request.QueryString["endTime"], Request.QueryString["priceStart"], Request.QueryString["priceEnd"], Request.QueryString["rateStart"],
                Request.QueryString["rateEnd"], Request.QueryString["addProductListText"], Request.QueryString["IsEdite"], Request.QueryString["IsPublish"], pageIndex, pageSize,Request.QueryString["isout"], out total);
            ViewBag.totalCount = total;
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;

            list = list.GroupBy(p => p.ProductNo).Select(p => new SkillProductExtends 
            {
                ProductNo = p.ElementAt(0).ProductNo,
                LimitedPrice = p.ElementAt(0).LimitedPrice,
                GoodsNo = p.ElementAt(0).GoodsNo,
                ProductModel = p.ElementAt(0).ProductModel,
                ProductName = p.ElementAt(0).ProductName,
                ProductPicFile = p.ElementAt(0).ProductPicFile,
                ProductShowPic = p.ElementAt(0).ProductShowPic,
                BrandCnName = p.ElementAt(0).BrandCnName,
                BrandEnName = p.ElementAt(0).BrandEnName,
                Id = p.ElementAt(0).Id,
                IsPublish = p.ElementAt(0).IsPublish,
                EditeDate = p.ElementAt(0).EditeDate,
                PublishTime = p.ElementAt(0).PublishTime,
                PublishPeople = p.ElementAt(0).PublishPeople,
                EditePeople = p.ElementAt(0).EditePeople,
                IsEdite = p.ElementAt(0).IsEdite,
                DiscountShangpin=p.ElementAt(0).DiscountShangpin,
                MarketPriceRegion = p.Min(a => a.MarketPrice) + "~" + p.Max(a => a.MarketPrice),
                StandardPriceRegion = p.Min(a => a.StandardPrice) + "~" + p.Max(a => a.StandardPrice),
                PlatinumPriceRegion = p.Min(a => a.PlatinumPrice) + "~" + p.Max(a => a.PlatinumPrice),
                DiamondPriceRegion = p.Min(a => a.DiamondPrice) + "~" + p.Max(a => a.DiamondPrice),
                PromotionPriceRegion = p.Min(a => a.PromotionPrice) + "~" + p.Max(a => a.PromotionPrice),
                GoldPriceRegion = p.Min(a => a.GoldPrice) + "~" + p.Max(a => a.GoldPrice),
                DiscountShangpinRegion = p.Min(a => a.DiscountRate) + "~" + p.Max(a => a.DiscountRate),
                IsOutSide=p.ElementAt(0).IsOutSide
            });
            return View(list);
        }
        #endregion

        #region 清除缓存数据
        //批量清除数据活动 
        public JsonResult ProductRedisCacheClear(string ProductNos)
        {
            int count = 0;
            #region 清除缓存
            if (!string.IsNullOrEmpty(ProductNos))
            {
                foreach (var item in ProductNos.Split(','))
                {
                    string key = "GetDetailBigImgTemplateDetail_" + item;
                    MemcachedProvider.Instance.Delete("GetDetailBigImgTemplateDetail_" + item);
                    count += 1;
                }
            }
            #endregion
            return Json(new { count = count }, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region 商品编辑块
        //加载商品编辑块
        public ActionResult ProductRefContent(string productNO = "000000")
        {
            SWfsProductRef obj = productService.GetProductRefByProductNO(productNO);
            if (obj == null)
            {
                productService.AddProductRefContent(new SWfsProductRef
                {
                    ProductNO = productNO
                });
                obj = productService.GetProductRefByProductNO(productNO);
            }
            return View(obj);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult ProductRefContent(SWfsProductRef obj)
        {
            if (productService.EditeProductRefContent(obj) > 0)
            {
                ViewData["tip"] = new HtmlString("<script>alert('保存成功')</script>");
            }
            else
            {
                ViewData["tip"] = new HtmlString("<script>alert('保存失败')</script>");
            }
            return View(obj);
        }
        //切换模板编号
        public int EditeTemplateNo(int refID, string tempNO)
        {
            return productService.EditeProductTemplateNO(refID, tempNO);
        }
        //上传图片
        [HttpPost]
        public string SaveImg(int imgWidth = 0, int imgHeight = 0, int imgLength = 1000)
        {
            HttpPostedFileBase file = Request.Files["imgfile"];
            CommonService commonService = new CommonService();
            rsPic.Clear();
            if (file.ContentLength > 0 && file != null)
            {
                rsPic = commonService.PostImg(file, "width:" + imgWidth + ",Height:" + imgHeight + ",Length:" + imgLength);
                if (rsPic.Keys.Contains("success"))
                {
                    return "{\"status\":0,\"message\":\"" + ServicePic.ResolveUGCImage("2", rsPic["success"], imgWidth, imgHeight) + "\"}";
                }
                if (rsPic.Keys.Contains("error"))
                {
                    return "{\"status\":1,\"message\":\"" + rsPic["error"] + "\"}";
                }
            }
            return "{\"status\":1,\"message\":\"上传图片异常\"}";
        }
        //选择模板列表
        public ActionResult SelectMeetingTemplate(int pageIndex = 1, int pageSize = 3)
        {
            int total = 0;
            IEnumerable<SWfsProductTemplate> list = productService.GetTemplateList(pageIndex,
                pageSize, Request.QueryString["templateno"], Request.QueryString["templatename"], out total);

            ViewBag.totalCount = total;
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;
            return View(list);
        }
        //发布代码
        public int PublishProductContent(int publishID)
        {
            return productService.PublishProductContent(publishID);
        }
        //清除缓存
        public string ClearProductDetailCache(string producNO)
        {
            try
            {
                EnyimMemcachedClient.Instance.Remove("GetDetailBigImgTemplateDetail_" + producNO);
                return "清除成功";
            }
            catch (Exception ex)
            {

                return "清除异常" + ex.Message;
            }
        }
        #endregion

        #region 模板
        //模板列表
        public ActionResult TemplateList(int pageIndex = 1, int pageSize = 20)
        {
            int total = 0;
            IEnumerable<SWfsProductTemplate> list = productService.GetTemplateList(pageIndex,
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
                return View(new SWfsProductTemplate() { TemplateNO = DateTime.Now.ToString("yyyyMMddHHmmss") });
            }
            SWfsProductTemplate obj = productService.GetTemplateObj(id);
            if (obj == null)
            {
                return View(new SWfsProductTemplate() { TemplateNO = DateTime.Now.ToString("yyyyMMddHHmmss") });
            }
            return View(obj);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult EditeTemplate(SWfsProductTemplate obj)
        {
            int result = productService.EditeProductTemplate(obj);
            if (result > 0)
            {
                ViewData["tip"] = new HtmlString("<script>alert('操作成功');location.href='/Shangpin/product/TemplateList';</script>");
                return View(obj);
            }
            else
            {
                if (result == -1)
                {
                    ViewData["tip"] = new HtmlString("<script>alert('已存在该模板');</script>");
                }
                else if (result == -2)
                {
                    ViewData["tip"] = new HtmlString("<script>alert('该模板名称已存在');</script>");
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
            productService.DeleteTemplate(id);
            return Redirect("/shangpin/Product/TemplateList");
        }
        #endregion

        #region 商品模板组合编辑
        //加载商品的编辑内容
        public ActionResult GetStructContent(string productNo)
        {
            //查询商品title
            ViewBag.ProductTitle = productService.GetProductTitle(productNo);
            //加载商品信息
            ViewBag.ProductInfo = productService.GetProductInfoByProductNo(productNo);
            //加载所有模板列表
            ViewBag.TemplateList = productService.GetTemplateList();

            ViewBag.UnUseStruct = productService.GetStructList();//所有结构父类


            //加载正在使用的父类 子类结构
            SWfsProductRefTemplate obj = productService.GetUseParentStructList(productNo);
            IEnumerable<SWfsProductRefTemplate> childStructList = childStructList = productService.GetStructChild(obj.ProductRefTemplateID);
            obj.ProductNo = productNo;
            ViewBag.UseStruct = obj;//使用结构
            ViewBag.ChildStructList = childStructList;
            return View();
        }
        //保存编辑的内容
        [HttpPost]
        public JsonResult SaveStructContent(string structName, IEnumerable<SWfsProductRefTemplate> childStructList, string productNo, int structId)
        {
            if (string.IsNullOrEmpty(productNo))
            {
                return Json(new { satatus = 0, msg = "商品编号不存在" }, JsonRequestBehavior.AllowGet);
            }
            if (structId != 0)//修改
            {
                productService.EditeStructParent(structId, structName, productNo);//修改父类
                productService.EditeStructChild(childStructList);//根据父类 修改子类

            }
            else//增加
            {
                int parentId = productService.SaveStructParent(structName, productNo, 1);//添加父类
                productService.SaveStructChild(childStructList, parentId);//根据父类 添加子类
            }
            //重置Html代码
            StringBuilder sball = new StringBuilder();
            StringBuilder sballMobile = new StringBuilder();
            for (int i = 0; i < childStructList.Count(); i++)
            {
                sball.Append(childStructList.ElementAt(i).TemplateHtmlCode.Trim());
                sballMobile.Append(childStructList.ElementAt(i).TemplateHtmlCodeMobile.Trim());
            }
            SWfsProductRef productObj = productService.GetProductRefByProductNO(productNo);
            if (productObj == null)
            {
                productObj = new SWfsProductRef();
                productObj.ProductNO = productNo;
                productObj.HTMLCode = sball.ToString();
                productObj.HTMLCodeMobile = sballMobile.ToString();
                productService.AddProductRefContent(productObj);
            }
            else
            {
                productObj.HTMLCode = sball.ToString();
                productObj.HTMLCodeMobile = sballMobile.ToString();
                productService.EditeProductRefContent(productObj);
            }
            return Json(new { satatus = 0, msg = "操作成功" }, JsonRequestBehavior.AllowGet);
        }
        //保存商品title
        public JsonResult SaveProductTitle(string productNo, string productTitle)
        {
            if (string.IsNullOrEmpty(productNo.Trim()))
            {
                return Json(new { status = 0, msg = "商品编号不存在" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrEmpty(productTitle.Trim()))
            {
                return Json(new { status = 0, msg = "商品标题不能为空" }, JsonRequestBehavior.AllowGet);
            }
            if (productService.SaveProductTitle(productNo, productTitle) > 0)
            {
                return Json(new { status = 1, msg = "操作成功" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = 0, msg = "操作失败" }, JsonRequestBehavior.AllowGet);
            }
        }
        //保存结构
        public JsonResult SaveProductStruct(string structName, IEnumerable<SWfsProductRefTemplate> childStructList)
        {
            if (string.IsNullOrEmpty(structName.Trim()))
            {
                return Json(new { status = 0, msg = "结构名称不能为空" });
            }
            //数据验证
            if (productService.IsExistsStructName(structName) > 0) //验证结构名称是否存在
            {
                return Json(new { status = 0, msg = "结构名称已存在" });
            }
            //增加结构
            int parentId = productService.SaveStructParent(structName, "", 0);//添加父类
            productService.SaveStructChild(childStructList, parentId);//根据父类 添加子类
            return Json(new { status = 1, msg = "保存成功" }, JsonRequestBehavior.AllowGet);
        }
        //使用结构
        public JsonResult UseStruct(string productNo, int usestructId, int structId)
        {
            if (usestructId == 0)
            {
                return Json(new { satatus = 0, msg = "商品编号不存在" }, JsonRequestBehavior.AllowGet);
            }
            if (usestructId == 0)
            {
                return Json(new { satatus = 0, msg = "结构编号不存在" }, JsonRequestBehavior.AllowGet);
            }
            //获取结构的父类
            SWfsProductRefTemplate obj = productService.GetStructById(usestructId);
            if (obj == null)
            {
                return Json(new { satatus = 0, msg = "结构不存在" }, JsonRequestBehavior.AllowGet);
            }
            StringBuilder sb = new StringBuilder();
            StringBuilder sbMobile = new StringBuilder();
            string[] templateNoList = null;
            //获取子类
            IEnumerable<SWfsProductRefTemplate> useChildStructList = productService.GetStructChild(usestructId).OrderBy(p => p.Sort);
            //获取结构的组合模板代码
            for (int i = 0; i < useChildStructList.Count(); i++)
            {
                if (useChildStructList.ElementAt(i).ModuleStatus == 1 && !string.IsNullOrEmpty(useChildStructList.ElementAt(i).TemplateNo))
                {
                    //获取模板html
                    IEnumerable<SWfsProductTemplate> templateList = productService.GetTemplateContentByTemplateNoList(useChildStructList.ElementAt(i).TemplateNo);
                    templateNoList = useChildStructList.ElementAt(i).TemplateNo.Split(',');
                    for (int j = 0; j < templateNoList.Length; j++)
                    {
                        if (templateList.Count(p => p.TemplateNO == templateNoList[j]) == 1)
                        {
                            sb.Append(templateList.Single(p => p.TemplateNO == templateNoList[j]).TemplateCode);
                            sbMobile.Append(templateList.Single(p => p.TemplateNO == templateNoList[j]).TemplateCodeMobile);
                        }
                    }
                    useChildStructList.ElementAt(i).TemplateHtmlCode = sb.ToString();
                    useChildStructList.ElementAt(i).TemplateHtmlCodeMobile = sbMobile.ToString();
                }
                else
                {
                    useChildStructList.ElementAt(i).TemplateHtmlCode = "";
                    useChildStructList.ElementAt(i).TemplateHtmlCodeMobile = "";
                }
                useChildStructList.ElementAt(i).ProductNo = productNo;
                sb.Length = 0;
            }
            if (structId != 0)
            {
                //重置正在使用的父类
                productService.EditeStructParent(structId, obj.ModuleName, productNo);
                //重置编辑的html代码
                IEnumerable<SWfsProductRefTemplate> ChildStructList = productService.GetStructChild(structId).OrderBy(p => p.Sort);
                for (int i = 0; i < ChildStructList.Count() && i < useChildStructList.Count(); i++)
                {
                    ChildStructList.ElementAt(i).ModuleName = useChildStructList.ElementAt(i).ModuleName;
                    ChildStructList.ElementAt(i).ModuleStatus = useChildStructList.ElementAt(i).ModuleStatus;
                    ChildStructList.ElementAt(i).TemplateNo = useChildStructList.ElementAt(i).TemplateNo;
                    ChildStructList.ElementAt(i).TemplateHtmlCode = useChildStructList.ElementAt(i).TemplateHtmlCode.Trim();
                    ChildStructList.ElementAt(i).TemplateHtmlCodeMobile = useChildStructList.ElementAt(i).TemplateHtmlCodeMobile.Trim();
                    ChildStructList.ElementAt(i).ProductNo = productNo;
                }
                productService.EditeStructChild(ChildStructList);
            }
            else
            {
                //插入父类
                int parentId = productService.SaveStructParent(obj.ModuleName, productNo, 1);//添加父类
                productService.SaveStructChild(useChildStructList, parentId);//根据父类 添加子类
            }

            return Json(new { satatus = 1, msg = "操作成功" }, JsonRequestBehavior.AllowGet);
        }
        //根据模板编号获取模板代码组合
        public JsonResult GetTemplateListHtml(string templateNoListString)
        {
            if (string.IsNullOrEmpty(templateNoListString))
            {
                return Json(new { status = 0, msg = "", msgMobile = "" }, JsonRequestBehavior.AllowGet);
            }
            StringBuilder sb = new StringBuilder();
            StringBuilder sbMobile = new StringBuilder();
            string[] templateNoList = null;
            IEnumerable<SWfsProductTemplate> templateList = productService.GetTemplateContentByTemplateNoList(templateNoListString);
            templateNoList = templateNoListString.Split(',');
            for (int j = 0; j < templateNoList.Length; j++)
            {
                if (templateList.Count(p => p.TemplateNO == templateNoList[j]) == 1)
                {
                    sb.Append(templateList.Single(p => p.TemplateNO == templateNoList[j]).TemplateCode);
                    sbMobile.Append(templateList.Single(p => p.TemplateNO == templateNoList[j]).TemplateCodeMobile);
                }
            }
            return Json(new { status = 1, msg = sb.ToString(), msgMobile = sbMobile.ToString() }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 历史记录
        public void ClearLabelHistory()
        {
            Passport passport = PresentationHelper.GetPassport();
            if (passport != null)
            {
                productService.ClearLabelHistory(passport.UserName);
            }
        }
        #endregion

        #region 预览页加载
        public ActionResult PreviewWebDetail(string productNo, int type)
        {
            ViewBag.PicList = productService.GetProductPicList(productNo);
            ViewBag.DetailHTML = productService.GetProductDetailHtml(productNo, type);
            ViewBag.ProductInfo = productService.GetProductDetailByProductNo(productNo);
            if (type == 1)
            {
                return View("PreviewWebDetail");
            }
            else
            {
                return View("PreviewWebDetailMobile");
            }
        }
        #endregion
    }
}
