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
using System.Linq.Expressions;



namespace Shangpin.Ocs.Web.Areas.Shangpin.Controllers
{
    [OCSAuthorization]
    public class SubjectController : Controller
    {
        private Dictionary<string, string> rsPic = new Dictionary<string, string>();
        string[] ChannelNoName = new string[]{
                "Women",
				"Men"
              };

        string[] ChannelNoList =new string[]{
                "Bagsv",
				"Clothingv",
				"Shoesv",
				"Accessoriesv",
				"Homesv",
				"Watchesv",
				"Beautyv" 
              };
        private readonly SWfsNewSubjectService subject;

        public SubjectController()
        {
            subject = new SWfsNewSubjectService();
        }

        #region 商品管理
        /// <summary>
        /// 商品列表
        /// </summary>
        /// <param name="categoryNo"></param>
        /// <param name="brandNo"></param>
        /// <param name="status"></param>
        /// <param name="keyword"></param>
        /// <param name="pageindex"></param>
        /// <returns></returns>
        public ActionResult ProductLists_over(string subjectNo, string categoryNo, string brandNo, string isShelf, string genderStyle, string productNameOrNo, string isout,bool isbatch = false, int pageindex = 1)
        {
            SWfsNewSubjectService subject = new SWfsNewSubjectService(); 
            int pageSize = 1000;
            string scategoryNo = Request.QueryString["sCategoryNo"];
            genderStyle = Rq.GetStringQueryString("GenderStyle");
            string lpstart = Rq.GetStringQueryString("lpstart");
            string lpend = Rq.GetStringQueryString("lpend");
            decimal lpstartPrice = 0M;
            decimal lpendPrice = 0M;
            decimal.TryParse(lpstart,out lpstartPrice);
            decimal.TryParse(lpend, out lpendPrice);
            if (!string.IsNullOrEmpty(productNameOrNo))
                productNameOrNo = productNameOrNo.Trim();
            string categoryNo1 = string.Empty;
            string categoryNo2 = string.Empty;
            string categoryNo3 = string.Empty;
            string categoryNo4 = string.Empty;
            string brandName = string.Empty;

            isbatch = (Request["isbatch"] != null && Request["isbatch"] == "1") ? true : false;

            RecordPage<ProductInfoNew> productList = new RecordPage<ProductInfoNew>();
            int count = 0;
            if (!isbatch)
            {
                Session["IsBatch"] = false;

                categoryNo1 = Request.QueryString["CategoryNo1"];
                categoryNo2 = Request.QueryString["CategoryNo2"];
                categoryNo3 = Request.QueryString["CategoryNo3"];
                categoryNo4 = Request.QueryString["CategoryNo4"];
                brandName = Request.QueryString["BrandName"];
                if (!(string.IsNullOrEmpty(categoryNo1) & string.IsNullOrEmpty(categoryNo2) & string.IsNullOrEmpty(categoryNo3) & string.IsNullOrEmpty(categoryNo4) & string.IsNullOrEmpty(isShelf) & string.IsNullOrEmpty(brandNo) & string.IsNullOrEmpty(productNameOrNo)))
                {
                    productList = subject.GetSWfsSubjectProduct(categoryNo, scategoryNo, brandNo, isShelf,genderStyle, productNameOrNo,isout,lpstartPrice,lpendPrice, pageindex, pageSize);
                    //查询商品总条数 by tianxiuquan
                    count = subject.GetSWfsSubjectProductTotalCount(categoryNo, scategoryNo, brandNo, isShelf, productNameOrNo,genderStyle,isout, lpstartPrice, lpendPrice);
                }
            }
            else
            {
                string productNos = Request["productNos"].Replace("\r\n", ",").Replace("\n\r", ",").Replace("\n", ",").Replace("\r", ",").TrimEnd(',').Replace(" ", ",");
                Session["IsBatch"] = true;//记录下批量查询
                Session["ProductNos"] = productNos;//记录下批量查询产品编号

                productList = subject.GetSWfsSubjectProductList_over(categoryNo, scategoryNo, brandNo, isShelf,genderStyle, productNos,isout, pageindex, pageSize);
                count = subject.GetSWfsSubjectProductListTotalCount(categoryNo, scategoryNo, brandNo, isShelf,genderStyle, productNos,isout); 
            }
            ViewBag.CategoryNo = categoryNo1;
            ViewBag.CategoryNo2 = categoryNo2;
            ViewBag.CategoryNo3 = categoryNo3;
            ViewBag.CategoryNo4 = categoryNo4;

            ViewBag.IsShelf = isShelf;
            ViewBag.ProductNameOrNo = productNameOrNo;
            ViewBag.BrandName = brandName;
            ViewBag.BrandNo = brandNo;

            //所有的一级分类
            ViewBag.AllFirstCategory = subject.SelectErpCategoryByParentNo("ROOT");
            //该活动下的一级分类
            ViewBag.FirstCategory = subject.GetErpCategoryListBySubjectNo(subjectNo);

            ViewBag.Category2 = subject.SelectErpCategoryByParentNo(categoryNo1);
            ViewBag.Category3 = subject.SelectErpCategoryByParentNo(categoryNo2);
            ViewBag.Category4 = subject.SelectErpCategoryByParentNo(categoryNo3);

            ViewBag.SubjectNo = subjectNo;
            ViewBag.SCategoryNo = scategoryNo;
            ViewBag.CategoryName = subject.GetSubjectCategoryModel(scategoryNo).CategoryName;
            // ViewBag.ProductList = productList;
            ViewBag.pageIndex = pageindex;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = count;
            SubjectInfoNew smodel = subject.GetSubjectInfo(subjectNo);
            ViewBag.SubjectName = smodel.SubjectName;
            ViewBag.SubjectUrlPost = Request.Url.ToString();
            ViewBag.LimitedPriceStart = lpstartPrice;
            ViewBag.LimitedPriceEnd = lpendPrice;
            ViewBag.GenderStyle = genderStyle;
            return View(productList);
        }
        //添加页面的商品列表 temp1
        public ActionResult ProductList(int pageIndex=1,int pageSize=400) 
        {
            int total = 0;
            IEnumerable<ProductInfoNew> list = new List<ProductInfoNew>();
            if (string.IsNullOrEmpty(Request.QueryString["SCategoryNo"]) || string.IsNullOrEmpty(Request.QueryString["issearch"]))
            {
                ViewBag.totalCount = total;
                ViewBag.pageIndex = pageIndex;
                ViewBag.pageSize = pageSize;
                return View(list);
            }
            if (string.IsNullOrEmpty(Request.Form["productNos"]))
            {
                list = subject.GetSWfsSubjectProductList(Request.QueryString["CategoryNo"], Request.QueryString["SCategoryNo"],
                Request.QueryString["BrandNo"], Request.QueryString["IsShelf"], Request.QueryString["ProductSex"], Request.QueryString["keyWord"], Request.QueryString["isout"], "1", "", Request.QueryString["lpstart"],Request.QueryString["lpend"],pageIndex,pageSize, out total);
            }
            else//批量查询
            {
                list = subject.GetSWfsSubjectProductList(Request.Form["CategoryNo"], Request.Form["SCategoryNo"],
                Request.Form["BrandNo"], Request.Form["IsShelf"], Request.Form["ProductSex"], Request.Form["keyWord"], Request.Form["isout"], "1", Request.Form["productNos"], Request.QueryString["lpstart"], Request.QueryString["lpend"], pageIndex, pageSize, out total);
            }
            ViewBag.totalCount = total;
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;
            ViewBag.SubjectNo = Request.QueryString["SubjectNo"];
            ViewBag.SCategoryNo = Request.QueryString["SCategoryNo"];

            return View(list);
        }

        [HttpPost]
        public ActionResult AddSubjectProductRef_over(string subjectNo, string sCategoryNo, string productNoStr, string subjectUrlPost)
        {
            bool ProductNoStrBool = false;
            string str = productNoStr;
            string SubjectUrlPost = "/shangpin/subject/SubjectProductRef.html?SubjectNo=" + subjectNo + "&SCategoryNo=" + sCategoryNo + CommonService.GetTimeStamp("&");
            string isBatch = Request["isbatch"] ?? "0";
            if (isBatch.Equals("1")) //批量添加 
            {
                str = Request["productNos"];
                subjectNo = Request["subjectNo"];
                sCategoryNo = Request["SCategoryNo"]; 
                if (string.IsNullOrEmpty(subjectUrlPost))
                { SubjectUrlPost = Request["SubjectUrlPost"]; }
                else { SubjectUrlPost = subjectUrlPost; } 
            }
            else
            { 
                if (string.IsNullOrEmpty(subjectUrlPost)) 
                { SubjectUrlPost = Request["SubjectUrlPost"]; }
                else { SubjectUrlPost = subjectUrlPost; } 
            }

            bool addAllProducts = (Request["AddAllProducts"] != null && Request["AddAllProducts"] == "1") ? true : false;

            string[] ProductNoStr;

            SWfsNewSubjectService subject = new SWfsNewSubjectService();

            if (addAllProducts)
            {
                ProductNoStrBool = true;
                subjectNo = Request["SubjectNo"].ToString().Trim();
                string scategoryNo = Request["SCategoryNo"].ToString().Trim();
                string categoryNo1 = (Request["CategoryNo1"] == null || Request["CategoryNo1"] == "null") ? null : Request["CategoryNo1"];
                string categoryNo2 = (Request["CategoryNo2"] == null || Request["CategoryNo2"] == "null") ? null : Request["CategoryNo2"];
                string categoryNo3 = (Request["CategoryNo3"] == null || Request["CategoryNo3"] == "null") ? null : Request["CategoryNo3"];
                string categoryNo4 = (Request["CategoryNo4"] == null || Request["CategoryNo4"] == "null") ? null : Request["CategoryNo4"];
                string brandName = (Request["BrandName"] == null || Request["BrandName"] == "null") ? null : Request["BrandName"];
                string categoryNo = (Request["CategoryNo"] == null || Request["CategoryNo"] == "null") ? null : Request["CategoryNo"];
                string brandNo = (Request["BrandNo"] == null || Request["BrandNo"] == "null") ? null : Request["BrandNo"];
                string isShelf = (Request["IsShelf"] == null || Request["IsShelf"] == "null") ? null : Request["IsShelf"];
                string genderStyle = (Request["GenderStyle"] == null || Request["GenderStyle"] == "null") ? null : Request["GenderStyle"]; 
                string lpstart = (Request["lpstart"] == null || Request["lpstart"] == "null") ? null : Request["lpstart"];
                string lpend = (Request["lpend"] == null || Request["lpend"] == "null") ? null : Request["lpend"];
            
                decimal lpstartPrice = 0M;
                decimal lpendPrice = 0M;
                decimal.TryParse(lpstart, out lpstartPrice);
                decimal.TryParse(lpend, out lpendPrice);

                string productNameOrNo = (Request["ProductNameOrNo"] == null || Request["ProductNameOrNo"] == "null") ? null : Request["ProductNameOrNo"]; 
                IList<string> productList_V;
                if (Session["IsBatch"] != null && bool.Parse(Session["IsBatch"].ToString()) == true)
                {
                    string productNos = Session["ProductNos"].ToString();
                    productList_V = subject.GetSWfsSubjectAllProductList_LxyAdd(categoryNo, scategoryNo, brandNo, isShelf, productNos);
                }
                else
                {
                    productList_V = subject.GetSWfsSubjectAllProduct_LxyAdd(categoryNo, scategoryNo, brandNo, isShelf, productNameOrNo,genderStyle,lpstartPrice,lpendPrice);
                }
                ProductNoStr = productList_V.ToArray();
            }
            else
            {
                ProductNoStr = str.Replace("\r\n", ",").Replace("\n\r", ",").Replace("\n", ",").Replace("\r", ",").TrimEnd(',').Split(',');
            }

            //WfsProduct pdr;
            int addcount = 0;
            string existListStr = string.Empty;
            string existListStrNo = string.Empty;
            List<string> noneList = new List<string>();
            List<string> existList = new List<string>();
            StringBuilder sb = new StringBuilder();
            SWfsNewProductService proService = new SWfsNewProductService();

            //活动商品
            IList<string> splistSearchProductNoS = subject.SelectSubjectProductRef_LxyAdd(sCategoryNo, ProductNoStr);
            //查看该商品是否已添加到该活动
            sb.Append("");
            foreach (string pid in ProductNoStr)
            {
                if (string.IsNullOrEmpty(pid)) continue;
                int result = 1;
                //查看该商品是否存在
                if (splistSearchProductNoS != null)
                {
                    if (splistSearchProductNoS.Contains(pid))
                    {
                        existListStr += pid.Trim() + ",";
                        existList.Add(pid.Trim());
                        result = 0;
                        break;

                    }
                }
                if (!subject.IsExistProduct(pid.Trim()))
                {
                    existListStrNo += pid.Trim() + ",";
                    //sb.AppendFormat("商品：{0}不存在，请重新输入!\r\n", pid);
                    noneList.Add(pid.Trim());
                    continue;
                }
                //是否有库存
                int quantity = proService.GetInventoryByProductNo(pid).SumQuantity;
                if (quantity == 0)
                {
                    result = 0;
                    continue;
                }

                //添加商品到活动
                if (result == 1)
                {
                    addcount = addcount + 1; ;
                    SWfsNewSubjectProductRef spr = new SWfsNewSubjectProductRef();
                    spr.ProductNo = pid.Trim();
                    spr.DateCreate = DateTime.Now;
                    spr.CategoryNo = sCategoryNo;
                    spr.PropertyValue = 0;
                    spr.TopFlag = false;
                    spr.SortNo = 999;
                    spr.BuyType = "";
                    //spr.TypeFlag = 0;
                    spr.ShowTime = DateTime.Now;
                    spr.IsShow = 1;
                    DapperUtil.Insert<SWfsNewSubjectProductRef>(spr);
                }
            }
            //修改活动折扣类型（lxy 20131007标示作废）
            //if (ProductNoStr.Length > 0)
            //{
            //    #region 商品变动重新计算活动折扣数据
            //    subject.ExecuteDiscountTypeValue(subjectNo, sCategoryNo);
            //    #endregion
            //}


            SWfsSubjectOrChannelLog log = new SWfsSubjectOrChannelLog();
            SubjectInfoNew subj = subject.GetSubjectInfo(subjectNo);
            log.SubjectOrChannelNo = subjectNo;
            log.TypeValue = 0;
            log.DateOperator = DateTime.Now;
            log.OperatorContent = "向活动中添加商品";
            log.OperatorUserId = PresentationHelper.GetPassport().UserName;
            subject.InsertSubjectOrChannelLog(log);
            string existListT = string.Empty;
            int ts = 0;
            subject.GetSWfsNewSubject_UpdateProductCount(subjectNo);
            subject.ClearSubjectListRedisCache(subjectNo);
            if (ProductNoStrBool)
            {
                if (!string.IsNullOrEmpty(existListStr) || !string.IsNullOrEmpty(existListStrNo))
                { existListT = "活动已存在商品：" + existListStr + "以下商品不存在:" + existListStrNo; ts = 1; if (existListT.Length > 50) { existListT = existListT.Substring(0, 50); } }
                if (addcount > 0)
                {
                    return Json(new { Ok = 1, TS = ts, Msg = existListT, Count = addcount });
                }
                else
                { return Json(new { Ok = 0, TS = ts, Msg = existListT,Count = addcount }); }
            }
            else
            {
                CommonHelp.Alert("本次添加商品"+addcount+"个", SubjectUrlPost);
                return Json(new { Ok = 0, TS = ts, Msg = existListT, Count = addcount });
            }
        }
        //添加商品   temp2
        [HttpPost]
        public ActionResult AddSubjectProductRef() 
        {
            int AddProductCount = subject.AddProductToSubjectFloor(Request.Form["subjectNo"], Request.Form["sCategoryNo"], Request.Form["ProductNoCheckBox"]);
            TempData["addCount"] = "<script>alert('成功添加"+AddProductCount+"条商品')</script>";
            return Redirect("/shangpin/subject/SubjectProductRef.html?subjectNo=" + Request.Form["subjectNo"] + "&scategoryNo=" + Request.Form["sCategoryNo"]);
        }
        
        //查看活动已经添加的商品列表  temp3
        public ActionResult SubjectProductRef(int pageIndex=1,int pageSize=400) 
        {
            
            int total = 0;
            IEnumerable<SubjectProductRefNew> list = subject.GetSWfsSubjectAlreadyAddProductList(Request.QueryString["SubjectNo"], Request.QueryString["CategoryNo"],
                Request.QueryString["SCategoryNo"], Request.QueryString["BrandNo"], Request.QueryString["IsShelf"], Request.QueryString["ProductSex"],
                Request.QueryString["keyWord"], Request.QueryString["isout"], "0", "", pageIndex, pageSize, out total);
            ViewBag.totalCount = total;
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;
            ViewBag.SubjectNo = Request.QueryString["SubjectNo"];
            ViewBag.SCategoryNo = Request.QueryString["SCategoryNo"];
            if (!string.IsNullOrEmpty(Request.QueryString["upCount"]))
            {
                subject.UpdateSubjectProductCount(Request.QueryString["SubjectNo"]);
            }
            return View(list);
        }

        public ActionResult SubjectProductRef_over()
        {
            string categoryNo = Rq.GetStringQueryString("categoryNo");
            string scategoryNo = Rq.GetStringQueryString("scategoryNo");
            string brandNo = Rq.GetStringQueryString("brandNo");
            string isShelf = Rq.GetStringQueryString("isShelf");
            int pageindex = Rq.GetIntQueryString("pageindex");
             int pagesizeNew = Rq.GetIntQueryString("pagesize"); 
            if (pageindex <= 0)
            { pageindex = 1; }
            string productNameOrNo = Rq.GetStringQueryString("productNameOrNo");
            string brandName = Rq.GetStringQueryString("BrandName");
            string categoryNo1 = Rq.GetStringQueryString("CategoryNo1");
            string categoryNo2 = Rq.GetStringQueryString("CategoryNo2");
            string categoryNo3 = Rq.GetStringQueryString("CategoryNo3");
            string categoryNo4 = Rq.GetStringQueryString("CategoryNo4");
            string subjectNo = Rq.GetStringQueryString("subjectNo");
            int isout = string.IsNullOrEmpty(Request.QueryString["isout"]) ? 0 : int.Parse(Request.QueryString["isout"]);
            SWfsNewSubjectService subject = new SWfsNewSubjectService();
            int pageSize = int.Parse(AppSettingManager.AppSettings["ComonListPageNum"].ToString());
            if (pagesizeNew > 0 && pagesizeNew <= 100) { pageSize = pagesizeNew; }
            ViewBag.CategoryNo = categoryNo;
            ViewBag.SubjectNo = subjectNo;
            ViewBag.CategoryNo = categoryNo1;
            ViewBag.CategoryNo2 = categoryNo2;
            ViewBag.CategoryNo3 = categoryNo3;
            ViewBag.CategoryNo4 = categoryNo4;
            ViewBag.IsShelf = isShelf;
            ViewBag.ProductNameOrNo = productNameOrNo;
            ViewBag.BrandName = brandName;
            //所有的一级分类
            ViewBag.AllFirstCategory = subject.SelectErpCategoryByParentNo("ROOT");
            //该活动下的一级分类
            ViewBag.FirstCategory = subject.GetErpCategoryListBySubjectNo(subjectNo);

            ViewBag.Category2 = subject.SelectErpCategoryByParentNo(categoryNo1);
            ViewBag.Category3 = subject.SelectErpCategoryByParentNo(categoryNo2);
            ViewBag.Category4 = subject.SelectErpCategoryByParentNo(categoryNo3);

            ViewBag.SubjectNo = subjectNo;
            ViewBag.SCategoryNo = scategoryNo;
            ViewBag.CategoryName = subject.GetSubjectCategoryModel(scategoryNo).CategoryName;
            RecordPage<SubjectProductRefNew> list = subject.SelectSubjectProductRefList(categoryNo, scategoryNo, brandNo, isShelf, productNameOrNo,isout, pageindex, pageSize);
            string quantity = Request.Params["Quantity"];
            list.Items = list.Items.GroupBy(p => p.ProductNo).Select(p => new SubjectProductRefNew()
            {
                SubjectProductRefId = p.ElementAt(0).SubjectProductRefId,
                PropertyValue = p.ElementAt(0).PropertyValue,
                TopFlag = p.ElementAt(0).TopFlag,
                SortNo = p.ElementAt(0).SortNo,
                BuyType = p.ElementAt(0).BuyType,
                ShowTime = p.ElementAt(0).ShowTime,
                IsShow = p.ElementAt(0).IsShow,
                ProductNo = p.ElementAt(0).ProductNo,
                ProductName = p.ElementAt(0).ProductName,
                ProductPicFile = p.ElementAt(0).ProductPicFile,
                ProductShowPic = p.ElementAt(0).ProductShowPic,
                GoodsNo = p.ElementAt(0).GoodsNo,
                ProductModel = p.ElementAt(0).ProductModel,
                MarketPrice = p.ElementAt(0).MarketPrice,
                SellPrice = p.ElementAt(0).SellPrice,
                GoldPrice = p.ElementAt(0).GoldPrice,
                PlatinumPrice = p.ElementAt(0).PlatinumPrice,
                DiamondPrice = p.ElementAt(0).DiamondPrice,
                PromotionPrice = p.ElementAt(0).PromotionPrice,
                LimitedPrice = p.ElementAt(0).LimitedPrice,
                StandardPrice = p.ElementAt(0).StandardPrice,
                IsShelf = p.ElementAt(0).IsShelf,
                PcSaleState = p.ElementAt(0).PcSaleState,
                BrandNo = p.ElementAt(0).BrandNo,
                BrandCnName = p.ElementAt(0).BrandCnName,
                BrandEnName = p.ElementAt(0).BrandEnName,
                MarketPriceRegion = p.Min(a => a.MarketPrice) + "~" + p.Max(a => a.MarketPrice),
                StandardPriceRegion = p.Min(a => a.StandardPrice) + "~" + p.Max(a => a.StandardPrice),
                PlatinumPriceRegion = p.Min(a => a.PlatinumPrice) + "~" + p.Max(a => a.PlatinumPrice),
                DiamondPriceRegion = p.Min(a => a.DiamondPrice) + "~" + p.Max(a => a.DiamondPrice),
                PromotionPriceRegion = p.Min(a => a.PromotionPrice) + "~" + p.Max(a => a.PromotionPrice),
                GoldPriceRegion = p.Min(a => a.GoldPrice) + "~" + p.Max(a => a.GoldPrice),
                IsOutSide = p.ElementAt(0).IsOutSide
            }).ToList();
            IList<SubjectProductRefNew> productList = list.Items.ToList();
            IList<SubjectProductRefNew> result = new List<SubjectProductRefNew>();
            if (!string.IsNullOrEmpty(quantity))
            {
                SWfsNewProductService service = new SWfsNewProductService();
                if (quantity == "1")
                {
                    foreach (SubjectProductRefNew sp in productList)
                    {
                        ProductInventoryNew pinventory = service.GetInventoryByProductNo(sp.ProductNo);
                        if (pinventory.SumQuantity > 0)
                        {
                            result.Add(sp);
                        }
                    }
                }
                else
                {
                    foreach (SubjectProductRefNew sp in productList)
                    {
                        ProductInventoryNew pinventory = service.GetInventoryByProductNo(sp.ProductNo);
                        if (pinventory.SumQuantity == 0)
                        {
                            result.Add(sp);
                        }
                    }
                }
                list.Items = result;
            }

            //SubjectInfoNew smodel = subject.GetSubjectInfo(subjectNo);
            //ViewBag.SubjectName = smodel.SubjectName;
            subject.GetSWfsNewSubject_UpdateProductCount(subjectNo);
            return View(list);
        }


        public ActionResult AjaxCategory()
        {
            string parentNo = Rq.GetStringQueryString("parentNo");
            string categoryType = Rq.GetStringQueryString("categoryType");
            SWfsNewSubjectService subject = new SWfsNewSubjectService();
            ViewBag.CategoryType = categoryType;
            ViewBag.CategoryNo = parentNo;
            ViewBag.ErpCategory = subject.SelectErpCategoryByParentNo(parentNo);

            return View();
        }
        /// <summary>
        /// 删除活动中的商品
        /// </summary>
        /// <param name="subjectNo"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        
        public string DeleteSubjectProductRef(string categoryNo, string relationId)
        {
            SWfsNewSubjectService subject = new SWfsNewSubjectService();
            subject.DeleteSubjectProductRef(relationId);

            string subjectNo = Request.Params["SubjectNo"];
            SWfsSubjectOrChannelLog log = new SWfsSubjectOrChannelLog();
            log.SubjectOrChannelNo = subjectNo;
            log.TypeValue = 0;
            log.DateOperator = DateTime.Now;
            log.OperatorContent = "删除活动中的商品";
            log.OperatorUserId = PresentationHelper.GetPassport().UserName;
            subject.InsertSubjectOrChannelLog(log);
            subject.GetSWfsNewSubject_UpdateProductCount(subjectNo);
            //subject.ClearSubjectListRedisCache(subjectNo);
            #region 商品变动重新计算活动折扣数据  尚品计算作废
           // subject.ExecuteDiscountTypeValue(subjectNo, categoryNo);
            #endregion

            return "";
            //return Redirect("/Shangpin/subject/SubjectProductRef.html?SubjectNo=" + subjectNo + "&SCategoryNo=" + categoryNo + CommonService.GetTimeStamp("&"));
        }

        public ActionResult DeleteSubjectProductRefs(string categoryNo, string relationIds)
        {
            SWfsNewSubjectService subject = new SWfsNewSubjectService();
            if (!string.IsNullOrEmpty(relationIds))
            {
                string[] res = relationIds.Split(',');
                foreach (var reid in res)
                {
                    subject.DeleteSubjectProductRef(reid);
                }
            }

            string subjectNo = Request.Params["SubjectNo"];
            SWfsSubjectOrChannelLog log = new SWfsSubjectOrChannelLog();
            log.SubjectOrChannelNo = subjectNo;
            log.TypeValue = 0;
            log.DateOperator = DateTime.Now;
            log.OperatorContent = "删除活动中的商品";
            log.OperatorUserId = PresentationHelper.GetPassport().UserName;
            subject.InsertSubjectOrChannelLog(log);
            subject.GetSWfsNewSubject_UpdateProductCount(subjectNo);
            subject.ClearSubjectListRedisCache(subjectNo);
            #region 商品变动重新计算活动折扣数据 尚品作废，无计算折扣
            //subject.ExecuteDiscountTypeValue(subjectNo, categoryNo);
            #endregion
            return Redirect("/Shangpin/subject/SubjectProductRef.html?SubjectNo=" + subjectNo + "&SCategoryNo=" + categoryNo + CommonService.GetTimeStamp("&"));
        }

        //更改商品是都在活动中显示
        public ActionResult UpdateProductShow(int subjectproductrefId, bool isShow)
        {
            //SWfsProductService service = new SWfsProductService();
            SWfsNewSubjectService service = new SWfsNewSubjectService();
            SWfsNewSubjectProductRef model = service.GetSubjectProduct(subjectproductrefId);
            model.IsShow = isShow == true ? (short)1 : (short)0;
            bool flag = DapperUtil.Update(model);
            if (flag)
            {
                return Json(new { result = 1, message = "保存成功！" });
            }
            else
            {
                return Json(new { result = 0, message = "保存失败！" });
            }
        }
        //商品标签
        public ActionResult AjaxSaveNewSubjectCategoryNo(int refId, string sCategoryNo)
        {
            SWfsNewProductService service = new SWfsNewProductService();
            SWfsNewSubjectProductRef model = service.GetSubjectProduct(refId);
            model.CategoryNo = sCategoryNo;
            bool flag = DapperUtil.Update(model);
            if (flag)
            {
                return Json(new { result = 1, message = "保存成功！" });
            }
            else
            {
                return Json(new { result = 0, message = "保存失败！" });
            } 
        }
        //更改显示时间
        public ActionResult AjaxSaveShowTime(int refId, string showTime)
        {
            SWfsNewProductService service = new SWfsNewProductService();
            SWfsNewSubjectProductRef model = service.GetSubjectProduct(refId);
            model.ShowTime = Convert.ToDateTime(showTime);
            bool flag = DapperUtil.Update(model);
            if (flag)
            {
                return Json(new { result = 1, message = "保存成功！" });
            }
            else
            {
                return Json(new { result = 0, message = "保存失败！" });
            }
        }
        /// <summary>
        /// 更新排序标号
        /// </summary>
        /// <param name="refIdAndSortNo"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AjaxSaveSortNo(string refIdAndSortNo)
        {
            string[] strArr = refIdAndSortNo.Split('|');
            int refId = 0;
            int sortId = 0;
            int succedCount = 0;
            for (int i = 0; i < strArr.Length; i++)
            {
                refId = Convert.ToInt32(strArr[i].Split('-')[0]);
                sortId = Convert.ToInt32(strArr[i].Split('-')[1]);

                SWfsNewProductService service = new SWfsNewProductService();
                SWfsNewSubjectProductRef model = service.GetSubjectProduct(refId);
                model.SortNo = sortId;
                bool flag = DapperUtil.Update(model);
                if (flag)
                {
                    succedCount++;
                }
            }

            return Json(new { result = 1, message = "保存成功！", count = succedCount });
        }


        //更改商品是都在活动中显示
        public ActionResult UpdateSubjectProductRefProperty(int subjectProductRefPropertyID, string PropertyValue)
        {
            //SWfsProductService service = new SWfsProductService();
            SWfsNewSubjectService service = new SWfsNewSubjectService();
            SWfsNewSubjectProductRef model = service.GetSubjectProduct(subjectProductRefPropertyID);
            model.PropertyValue =short.Parse(PropertyValue);
            bool flag = DapperUtil.Update(model);
            if (flag)
            {
                return Json(new { result = 1, message = "保存成功！" });
            }
            else
            {
                return Json(new { result = 0, message = "保存失败！" });
            }
            return View();
        }

        //批量更改商品是都在活动中显示
        public ActionResult SetSubjectProductRefProperty()
        {
            string subjectNoV = Request.Params["SubjectNoV"];
            string categoryNoV = Request.Params["categoryNoV"];
            string relationIdsV = Request.Params["RelationIdsV"];
            string PropertyValueV= Request.Params["PropertyValueV"];
            if (string.IsNullOrEmpty(PropertyValueV)) { PropertyValueV = "3"; }

            SWfsNewSubjectService subject = new SWfsNewSubjectService();
            if (!string.IsNullOrEmpty(relationIdsV))
            {
                string[] res = relationIdsV.Split(',');
                foreach (var reid in res)
                {
                    if (!string.IsNullOrEmpty(reid))
                    { 
                    SWfsNewSubjectProductRef model = subject.GetSubjectProduct(int.Parse(reid));
                    model.PropertyValue = short.Parse(PropertyValueV);
                    bool flag = DapperUtil.Update(model); 
                    }
                }
            }
            return Redirect("/Shangpin/subject/SubjectProductRef.html?SubjectNo=" + subjectNoV + "&SCategoryNo=" + categoryNoV + CommonService.GetTimeStamp("&") + "&pageindex=" + Request.QueryString["pageindex"] + "&group=" + Request.QueryString["group"]);
 
        }

        #endregion

        #region 活动列表展示 lxy

        /// <summary>
        /// 活动列表 lxy
        /// </summary>
        public ActionResult Index()
        {
            PaginationInfoModel pageinfo = new PaginationInfoModel();
            List<SWfsNewSubject> list = null;
            int pageSize = int.Parse(AppSettingManager.AppSettings["ComonListPageNum"].ToString());
            string acUrl = AppSettingManager.AppSettings["ActiveHrefUrlShow"];
            string keyWord = Rq.GetStringQueryString("keyword");
            string status = Rq.GetStringQueryString("status");
            string channelType = Rq.GetStringQueryString("channelNo");
            string startTime = Rq.GetStringQueryString("StartTime");
            string endTime = Rq.GetStringQueryString("EndTime");
            string gender = Rq.GetStringQueryString("Gender");
            string channelSord= Rq.GetStringQueryString("ChannelSord");
            string createUserId = Rq.GetStringQueryString("createUserId");
            int currentPage = Rq.GetIntQueryString("pageindex");
            string productNo = Rq.GetStringQueryString("productNo");
            string startTimeValue = string.Empty, endTimeValue = string.Empty;
            #region 条件筛选
            startTimeValue = string.IsNullOrEmpty(startTime) ? "" : startTime + " 00:00:00";
            endTimeValue = string.IsNullOrEmpty(endTime) ? "" : endTime + " 23:59:59";
            pageinfo.CurrentPage = currentPage;
            pageinfo.Condition = " 1=1 ";
            pageinfo.Condition += (string.IsNullOrEmpty(keyWord) || keyWord == "活动名称/活动编号") ? "" : " and (SWfsNewSubject.SubjectName like '%" + keyWord + "%' or SWfsNewSubject.SubjectNo='" + keyWord + "')";
            pageinfo.Condition += (string.IsNullOrEmpty(status)) ? "" : "and SWfsNewSubject.Status =" + status + "";
            pageinfo.Condition += (string.IsNullOrEmpty(channelType)) ? "" : " and SWfsNewSubject.ChannelType like '%" + channelType + "%'";
            pageinfo.Condition += (string.IsNullOrEmpty(channelSord)) ? "" : " and SWfsNewSubject.SubjectCategoryNo like '%" + channelSord + "%'";
            pageinfo.Condition += (string.IsNullOrEmpty(gender)) ? "" : " and SWfsNewSubject.SubjectGender like '%" + gender + "%'";
            pageinfo.Condition += (string.IsNullOrEmpty(createUserId) || createUserId == "创建人") ? "" : " and SWfsNewSubject.CreateUserId like '%" + createUserId + "%'";
            pageinfo.Condition += (string.IsNullOrEmpty(startTimeValue)) ? "" : "  and SWfsNewSubject.DateEnd >='" + startTimeValue + "'";
            pageinfo.Condition += (string.IsNullOrEmpty(endTimeValue)) ? "" : "  and SWfsNewSubject.DateEnd <='" + endTimeValue + "'";
            if (!string.IsNullOrEmpty(startTimeValue) && !string.IsNullOrEmpty(endTimeValue))
            {
                pageinfo.Condition += " or  (SWfsNewSubject.DateBegin >='" + startTimeValue + "' and SWfsNewSubject.DateBegin <='" + endTimeValue + "')";
            }
            else
            {
                pageinfo.Condition += (string.IsNullOrEmpty(startTimeValue)) ? "" : "  or  SWfsNewSubject.DateBegin >='" + startTimeValue + "'";
                pageinfo.Condition += (string.IsNullOrEmpty(endTimeValue)) ? "" : "  or SWfsNewSubject.DateBegin <='" + endTimeValue + "' ";
            }
            pageinfo.Condition += (string.IsNullOrEmpty(productNo) || productNo == "商品编号") ? "" : "  and SWfsNewSubject.SubjectNo IN(SELECT sc.SubjectNo FROM dbo.SWfsNewSubjectProductRef AS spref , dbo.SWfsNewSubjectCategory AS sc WHERE spref.CategoryNo=sc.CategoryNo AND spref.ProductNo='" + productNo + "' )";
            #endregion
            try
            {
                list = subject.GetSubjectPageList(ref pageinfo);
            }
            catch { }
        
            IList<ErpCategory> erpCategoryList = subject.GetERPCategoryListByParentNo("ROOT");
            ViewBag.ErpCategoryList = erpCategoryList;
            ViewBag.SubjectGenderList = subject.GetSubjectGenderList();
            ViewBag.SWfsNewSubjectList = list;
            ViewBag.PaginationInfoSingel = pageinfo;
            ViewBag.KeyWord = keyWord;
            ViewBag.Status = status;
            ViewBag.ChannelNo = channelType;
            ViewBag.StartTime = startTime;
            ViewBag.EndTime = endTime;
            ViewBag.ProductNo = productNo;
            ViewBag.ActiveHrefUrlShow = acUrl;
            ViewBag.Gender = gender;
            ViewBag.ChannelSord = channelSord;
            ViewBag.CreateUserId = createUserId;
            return View();
        }
        //by tianxiuquan 改版
        public ActionResult IndexNew(int pageIndex=1,int pageSize=20) 
        {
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;
            int total = 0;
            IEnumerable<SWfsNewSubject> subjectList= subject.GetSubjectIndexList(Request.QueryString["subjectNameOrSubjectNo"], Request.QueryString["productNo"], Request.QueryString["createUserId"],
                Request.QueryString["subjectStatus"], Request.QueryString["subjectCategoryNo"], Request.QueryString["channelType"],
                Request.QueryString["subjectGender"], Request.QueryString["subjectDateEndStart"], Request.QueryString["subjectDateEndEnd"],pageIndex,pageSize,out total);
            ViewBag.totalCount = total;
            return View(subjectList);
        }

        
        //已废弃
        public ActionResult SubjectStatusModify_over(string subjectNo, string ScategoryNo, string status)
        {
            SWfsNewSubjectService service = new SWfsNewSubjectService();
            SWfsNewProductService proService = new SWfsNewProductService();
            SWfsNewSubject subject = service.GetNewSubjectInfo(subjectNo);
            IList<string> list = service.GetSWfsNewSubjectProductRefList(subjectNo);
            string tempStatue = string.Empty;
            bool isAllPic = true;
            if (status == "1")
            {
                if (list.Count() > 0)
                {
                    int quantity = 0;
                    foreach (var item in list)
                    {
                        quantity = proService.GetInventoryByProductNo(item).SumQuantity;
                        if (quantity == 0)
                        {
                            return Json(new { result = "0", message = "此活动中存在库存为0的商品商品，不能开启！" });
                        }
                    }
                    if (subject.SubjectTemplateTopPicNo != "")
                    {
                        tempStatue = status;
                    }
                    else
                    {
                        tempStatue = "0";
                        isAllPic = false;
                    }
                }
                else
                {
                    return Json(new { result = "0", message = "此活动中没有商品，不能开启！" });
                }
            }
            try
            {
                service.NewSubjectStatusModify(subjectNo, tempStatue);
                return Json(new { result = "1", message = isAllPic ? "活动状态修改成功！" : "由于此活动的图片上传不完全，所以不能开启该活动！" });
            }
            catch (Exception ex)
            {
                return Json(new { result = "0", message = "活动状态修改失败！" });
            }
        }
        /// <summary>
        /// 修改活动状态 by tianxiuquan
        /// </summary>
        /// <param name="subjectNo"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public JsonResult SubjectStatusModify(string subjectNo, string status)
        {
            SWfsNewSubjectService service = new SWfsNewSubjectService();
            SWfsNewProductService proService = new SWfsNewProductService();
            string pic = service.SubjectIsExistPic(subjectNo);
            if (status=="1")
            {
                #region 验证活动开启的条件
                //验证活动是否上传了活动头图
                string subjectPic = service.SubjectIsExistPic(subjectNo);
                if (string.IsNullOrEmpty(subjectPic))
                {
                    return Json(new
                    {
                        status = 0,
                        message = "开启失败！请上传活动头图"
                    }, JsonRequestBehavior.AllowGet);
                }
                //验证活动是否存在库存为0的商品
                IList<string> ProductNoList = service.GetSWfsNewSubjectProductRefList(subjectNo);
                if (ProductNoList.Count() > 0)
                {
                    for (int i = 0; i < ProductNoList.Count; i++)
                    {
                        if (proService.GetInventoryByProductNo(ProductNoList[i]).SumQuantity == 0) //查询每个商品的库存
                        {
                            return Json(new { result = 0, message = "此活动中存在库存为0的商品商品，不能开启！" });
                        }
                    }
                }
                else
                {
                    return Json(new
                    {
                        status = 0,
                        message = "开启失败！此活动没有商品"
                    }, JsonRequestBehavior.AllowGet);
                }
                //更新活动状态
                service.NewSubjectStatusModify(subjectNo, "1");
                return Json(new {status=1,message="开启成功" },JsonRequestBehavior.AllowGet);
                #endregion
            }
            else
            {
                service.NewSubjectStatusModify(subjectNo, "0");//关闭活动
                return Json(new { status = 1, message = "关闭成功" }, JsonRequestBehavior.AllowGet);
            }
        }

        //批量关闭活动
        public ActionResult CloseSubject(string subjectNos, string status)
        {
            string[] subjectNo = subjectNos.Split(',');
            SWfsNewSubjectService service = new SWfsNewSubjectService();
            foreach (string no in subjectNo)
            {
                service.NewSubjectStatusModify(no, "0");
            }
            return Json(new { result = "1", message = "活动状态修改成功！" });
        }
        #endregion

        #region 创建活动 编辑活动

        #region 价格标签
        /// <summary>
        /// 创建价格标签
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, string> GetPriceDic()
        {
            Dictionary<int, string> dic = new Dictionary<int, string>();
            dic.Add(0, "选择价格标签");
            dic.Add(1, "￥--起");
            dic.Add(2, "￥--封顶");
            dic.Add(3, "--折起");
            dic.Add(4, "--折封顶");
            dic.Add(5, "低至--折");
            dic.Add(6, "低至--元");
            dic.Add(7, "直降￥--");
            dic.Add(8, "最高降￥--");
            dic.Add(9, "全场￥--");
            dic.Add(10, "全场--折");
            dic.Add(11, "满减￥--");
            dic.Add(12, "全场狂欢");
            return dic;
        }
        /// <summary>
        /// 获取价格标签
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetPriceName(int key)
        {
            Dictionary<int, string> dic = GetPriceDic();
            if (dic.Keys.Contains(key))
            {
                return dic[key];
            }
            return null;
        }
        #endregion

        #region 创建活动视图
        /// <summary>
        /// 创建新活动视图
        /// </summary>
        /// <returns></returns>      
        public ActionResult CreateSubject()
        {
            ViewBag.PriceItem = GetPriceDic();
            SWfsNewSubjectService subject = new SWfsNewSubjectService();
            #region 频道页面选项
            List<ErpCategory> categoryList = new List<ErpCategory>();
            for (int i = 0; i < ChannelNoName.Count(); i++)
            {
                for (int n = 0; n < ChannelNoList.Count(); n++)
                {
                    ErpCategory cSingle = new ErpCategory();
                    string channelValue = ChannelNoName[i] + ChannelNoList[n];
                    string [] chanelSingle= AppSettingManager.AppSettings[channelValue].Split(',');
                    cSingle.CategoryNo = chanelSingle[0];
                    cSingle.CategoryName = chanelSingle[1];
                    categoryList.Add(cSingle);
                } 
            }  
            #endregion
            ViewBag.CList = subject.SelectErpCategoryByParentNo("ROOT");
            ViewBag.PriceTypeList = subject.GetSWfsNewSubjectPriceTypeList(1);
            ViewBag.ChanelCategoryList = categoryList;
            return View();
        }

        #endregion

        #region 编辑活动视图
        /// <summary>
        /// 编辑活动视图
        /// </summary>
        /// <returns></returns>
        public ActionResult EditSubject()
        {
            string subjectNo = Request.QueryString["subjectNo"];
            SWfsNewSubject mSWfsNewSubject = null;
            if (!string.IsNullOrEmpty(subjectNo))
            {
                SWfsNewSubjectService service = new SWfsNewSubjectService();
                mSWfsNewSubject = service.GetNewSubjectInfo(subjectNo);
                if (mSWfsNewSubject != null)
                {
                    ViewBag.subjectMsg = "获取信息正常";
                    ViewBag.subjectNo = subjectNo;
                    ViewBag.SWfsNewSubject = mSWfsNewSubject;
                }
                else
                {
                    ViewBag.subjectNo = string.Empty;
                    ViewBag.subjectMsg = "未获取到活动信息";
                }
            }
            else
            {
                ViewBag.subjectNo = string.Empty;
                ViewBag.subjectMsg = "未获取到正确标示信息";
            }
            SWfsNewSubjectService subject = new SWfsNewSubjectService();
            #region 频道页面选项
            List<ErpCategory> categoryList = new List<ErpCategory>();
            for (int i = 0; i < ChannelNoName.Count(); i++)
            {
                for (int n = 0; n < ChannelNoList.Count(); n++)
                {
                    ErpCategory cSingle = new ErpCategory();
                    string channelValue = ChannelNoName[i] + ChannelNoList[n];
                    string[] chanelSingle = AppSettingManager.AppSettings[channelValue].Split(',');
                    cSingle.CategoryNo = chanelSingle[0];
                    cSingle.CategoryName = chanelSingle[1];
                    categoryList.Add(cSingle);
                }
            }
            #endregion
            ViewBag.CList = subject.SelectErpCategoryByParentNo("ROOT");
            ViewBag.PriceItem = GetPriceDic();
            ViewBag.PriceTypeList = subject.GetSWfsNewSubjectPriceTypeList(1);
            ViewBag.ChanelCategoryList = categoryList;
            return View();
        }
        #endregion

        #region 创建和编辑活动
        /*原所有上传图片为必填项，活动背景图可以同步预热背景图，需判断活动预热是否开启,背景图是否同步预热背景图
         * 20130910需求更改，可以不上传活动背景图，取消同步预热背景图选项
         * 只需判断背景图编辑时候是否存在即可
         * 20130924添加品牌，取消预热副标题，预热描述，添加活动副标题
         * 20130926添加手机活动背景图
         * 20131003添加手机端入口 wap端入口 预热位1图片 折扣信息
         * 20131015修改价格标签需求 改为直接存入数字
         * 20131015增加关注基数 楼层右侧名称和链接
         * 20131111增加性别 品类 字符数
         */
        /// <summary>
        /// 创建和编辑活动
        /// </summary>
        /// <returns></returns>
        [HttpPost] 
        [ValidateInput(false)]
        public ActionResult CreateHandle()
        {
            //获取SubManage字段来判断是编辑还是添加
            string SubjectManage = Request.Params["SubjectManage"];
            SWfsNewSubjectService service = new SWfsNewSubjectService();
       
            #region 获取活动主键
            string SubjectNo = SubjectManage == "Create" ? CreateSubjectNO() : Request.Params["SubjectNo"];
            if (string.IsNullOrEmpty(SubjectNo))
            {
                //return Json(new { result = "-1", message = "主键信息有误" }, JsonRequestBehavior.AllowGet);
                return Content("{ \"result\" : \"-1\", \"message\" : \"主键信息有误\" }");
            }
            //清除缓存
            EnyimMemcachedClient.Instance.Remove("ShangpinSWfsNewSubjectSingleData" + SubjectNo);
            EnyimMemcachedClient.Instance.Remove("ShangpinSWfsNewSubjectVisitsByTopicNo" + SubjectNo); 
            #endregion

            #region 如果是编辑判断是否存在对象
            SWfsNewSubject mNewSubject = new SWfsNewSubject();
            if (SubjectManage == "Edit")
            {
                mNewSubject = service.GetNewSubjectInfo(SubjectNo);
                if (mNewSubject == null)
                {
                    //return Json(new { result = "-1", message = "主键信息有误" }, JsonRequestBehavior.AllowGet);
                    return Content("{ \"result\" : \"-1\", \"message\" : \"主键信息有误\" }");
                }
            }

            #endregion

            #region 获取参数信息 不包含上传图片

            string SubjectName = Request.Params["SubjectName"];
            string ChannelNo = Request.Params["ChannelNo"];
            string Status = Request.Params["Status"];
            string DateBegin = Request.Params["DateBegin"];
            string DateEnd = Request.Params["DateEnd"];
            string Temp = Request.Params["Temp"] ?? Request.Params["TempTypeValue"];
            string ShowColumn = Request.Params["ShowColumn"];
            string TopPic = Request.Params["TopPic"];
            string txtSubjectText = Request.Params["txtSubjectText"];
            string IPhoneText = Request.Params["IPhoneText"];
            string HotDateBegin = Request.Params["HotDateBegin"];
            string HotPrice = Request.Params["hotPrice"];
            string Coupon = Request.Params["Coupon"];
            string CouponText = Request.Params["CouponText"];
            string HotTwoUrl = Request.Params["HotTwoUrl"];
            string oneHotSelect = Request.Params["oneHotSelect"];//是优惠码或者预热图片
            string ForHotBackImg = Request.Params["ForHotBackImg"];//是否同预热模板;已取消该字段
            /*20130913需求变更新增*/
            string FlapHotLOGOUrl = Request.Params["FlapHotLOGOUrl"];
            string HotTitle = Request.Params["HotTitle"];
            string HotDes = Request.Params["HotDes"];
            /*20130924需求变更*/
            string CKBrand = Request.Params["ck_Brand"];
            //20131003需求新增
            string CommodityPItem = Request.Params["CommodityPItem"];
            string CommodityP = Request.Params["CommodityP"];
            //20131003新增关注基数
            string FocusNums = Request.Params["FocusNums"];
            string BrandName = Request.Params["BrandName"];
            string BrandUrl = Request.Params["BrandUrl"];
            //20131111新增性别分类
            string SexNo = Request.Params["SexNo"];
            string CategoryNo = Request.Params["CategoryNo"];
            //20131209新增预热位置一图片链接
            string PrivilegeValueOrUrl = Request.Params["PrivilegeValueOrUrl"];
            //20140303新增-- BU选项（BU1到BU5）必选 1：BU1  2：BU2 3：BU3 4：BU4 5：BU5
            string BUNo = Request.Params["BUNo"];
            //20140303新增----销售额预估
            string ForecastSalesAmountNums = Request.Params["ForecastSalesAmountNums"];
            //20140320新增--推荐频道
            string chanelCategoryNo = Request.Params["chanelCategoryNo"];
            //20140328设置请输入网页html代码、、请输入移动端html代码
            string txtTopTopImgCodePC = Request.Params["txtTopTopImgCodePC"];
            string txtTopTopImgCodeM = Request.Params["txtTopTopImgCodeM"];
            //20140415新增--进行中运营位1,2，领取优惠码（1预热可以领取，2，进行中可以领取），显示是否开启提醒
             string StartPrivilegeValueOrUrl = Request.Params["StartPrivilegeValueOrUrl"];
             string StartHotTwoUrl = Request.Params["StartHotTwoUrl"];
             string StartoneHotSelect = Request.Params["StartoneHotSelect"];//是优惠码或者预热图片
             string CouponNew = Request.Params["CouponNew"];
             string TempPreType = Request.Params["TempPreType"] ?? Request.Params["TempPreTypeValue"];
             string SubjectPreRemind = Request.Params["SubjectPreRemind"];

             //是否支持免运费(0:不支持 1：支持;默认不支持)  author;zhanggong  date:2014.11.13
             string freightFree = Request.Params["IsFreightFree"];

            #endregion

            #region 获取上传图片参数并且判断
            string imgerror = string.Empty;

            string FileTopTopImg =mNewSubject!=null?mNewSubject.SubjectTemplateTopPicNo:string.Empty;  //活动头图 
            string FileBackImg =mNewSubject!=null?mNewSubject.SubjectTemplateBackgroundPicNO:string.Empty;//活动背景图
            string IphoneTopImg = mNewSubject != null ? mNewSubject.SubjectIPhonePicNo: string.Empty;//20130926加手机头图
            string IphoneClientImg =mNewSubject!=null?mNewSubject.SubjectIphoneInletPicNo: string.Empty;  //20131003新增 手机端入口图 
            string IphoneWapImg =mNewSubject!=null?mNewSubject.SubjectWapInletPicNo: string.Empty;//20131003新增 手机Wap端入口图 
            IphoneClientImg = GetActiveImageName("IphoneClientImg", "IphoneClientImg", "手机(客户)端入口图", SubjectManage, mNewSubject.SubjectIphoneInletPicNo, out imgerror);
            if (!string.IsNullOrEmpty(imgerror))
            {
                //return Json(new { result = "-1", message = imgerror }, JsonRequestBehavior.AllowGet);
                return Content("{ \"result\" : \"-1\", \"message\" : \"" + imgerror + "\"}");
            }
            if ((!string.IsNullOrEmpty(HotDateBegin)&&TempPreType != "1") || Temp != "1")
            {
                if (TopPic == "1")
                {
                    FileTopTopImg = GetActiveImageName("FileTopTopImg", "ActiveTopImg", "活动顶部头图", SubjectManage, mNewSubject.SubjectTemplateTopPicNo, out imgerror);
                    if (!string.IsNullOrEmpty(imgerror))
                    {
                        return Content("{ \"result\" : \"-1\", \"message\" : \"" + imgerror + "\" }");
                    }
                }
                //活动背景图
                  FileBackImg = GetActiveImageName("FileBackImg", "ActiveBackImg", false, "活动背景图", SubjectManage, mNewSubject.SubjectTemplateBackgroundPicNO, true, out imgerror);
                if (!string.IsNullOrEmpty(imgerror))
                {
                    return Content("{\"result\" : \"-1\", \"message\" : " + imgerror + " }");
                }
                //20130926加手机头图
                IphoneTopImg = GetActiveImageName("IphoneTopImg", "IphoneActiveTopImg", "手机头图", SubjectManage, mNewSubject.SubjectIPhonePicNo, out imgerror);
                if (!string.IsNullOrEmpty(imgerror))
                {
                    return Content("{ \"result\" : \"-1\", \"message\" : \""+imgerror+"\" }");
                } 
                //20131003新增 手机Wap端入口图           
                IphoneWapImg = GetActiveImageName("IphoneWapImg", "IphoneWapImg", "WAP端入口图", SubjectManage, mNewSubject.SubjectWapInletPicNo, out imgerror);
                if (!string.IsNullOrEmpty(IphoneWapImg) && !string.IsNullOrEmpty(imgerror))
                {
                    return Content("{ \"result\" : \"-1\", \"message\" : \"" + imgerror + "\" }");
                }
            }
            string FileHotImg =mNewSubject!=null?mNewSubject.SubjectPreStartBackgroundPic:string.Empty;//预热背景图
            string FlapHotOne =mNewSubject!=null?mNewSubject.SubjectPreStartTemplatePicNoOne: string.Empty;//预热运营位1
            string FlapHotTwo = mNewSubject != null ? mNewSubject.SubjectPreStartTemplatePicNoTwo : string.Empty;//预热运营位2
            string FlapHotLOGO =mNewSubject != null ? mNewSubject.SubjectPreTopPicNo:string.Empty;//20130913新增,用于页面上传LOGO图片
            string IphoneFlapHotOne = mNewSubject != null ? mNewSubject.PrivilegeValuePicNo : string.Empty; ;//20131003新增手机端运营位1
            
            //预热模板为楼层模板的时候执行以下判断
            if (TempPreType == "1" || Temp=="1")
            {
                FileHotImg = GetActiveImageName("FileHotImg", "ActiveHotBackImg", false, "预热背景图", SubjectManage, mNewSubject.SubjectPreStartBackgroundPic, false, out imgerror);
                if (!string.IsNullOrEmpty(imgerror))
                {
                    return Content("{ \"result\" : \"-1\", \"message\" : \"" + imgerror + "\" }");
                }
                //预热运营位1
                FlapHotOne = GetActiveImageName("FlapHotOne", "ActiveHotOneImg", "预热运营位1图", SubjectManage, mNewSubject.SubjectPreStartTemplatePicNoOne, out imgerror);
                if (!string.IsNullOrEmpty(imgerror))
                {
                    return Content("{ \"result\" : \"-1\", \"message\" : \"" + imgerror + "\" }");
                }
                //20131003手机端预热运营位1
                IphoneFlapHotOne = GetActiveImageName("IphoneFlapHotOne", "IphoneFlapHotOne", "手机端预热运营位1图", SubjectManage, mNewSubject.PrivilegeValuePicNo, out imgerror);
                if (!string.IsNullOrEmpty(imgerror))
                {
                    return Content("{ \"result\" : \"-1\", \"message\" : \"" + imgerror + "\" }");
                }
                //预热运营位2
                FlapHotTwo = GetActiveImageName("FlapHotTwo", "ActiveHotTwoImg", "预热运营位2图", SubjectManage, mNewSubject.SubjectPreStartTemplatePicNoTwo, out imgerror);
                if (!string.IsNullOrEmpty(imgerror))
                {
                    return Content("{ \"result\" : \"-1\", \"message\" : \"" + imgerror + "\" }");
                }
                //运营图片3 logo图
                FlapHotLOGO = GetActiveImageName("FlapHotLOGO", "ActiveHotLogoImg", "Logo图片错误", SubjectManage, mNewSubject.SubjectPreTopPicNo, out imgerror);
                if (!string.IsNullOrEmpty(imgerror))
                {
                    return Content("{ \"result\" : \"-1\", \"message\" : \"" + imgerror + "\" }");
                }
            }


            //20140415新增--进行中运营位1,2，领取优惠码（1预热可以领取，2，进行中可以领取）
            //运营位1
            string StartFlapHotOne = string.Empty;//预热运营位1
            string StartFlapHotTwo = string.Empty;//预热运营位2
            string StartIphoneFlapHotOne = string.Empty;//20131003新增手机端运营位1
            StartFlapHotOne = GetActiveImageName("StartFlapHotOne", "StartActiveHotOneImg", "运营位1图", SubjectManage, mNewSubject.SubjectTemplatePicNoOne, out imgerror);
            if (!string.IsNullOrEmpty(StartFlapHotOne) && !string.IsNullOrEmpty(imgerror))
            {
                return Content("{ \"result\" : \"-1\", \"message\" : \"" + imgerror + "\" }");
            }
            //手机端运营位1
            StartIphoneFlapHotOne = GetActiveImageName("StartIphoneFlapHotOne", "StartIphoneFlapHotOne", "手机端运营位1图", SubjectManage, mNewSubject.SubjectTemplateIphonePicNoOne, out imgerror);
            if (!string.IsNullOrEmpty(StartIphoneFlapHotOne)&&!string.IsNullOrEmpty(imgerror))
            {
                return Content("{ \"result\" : \"-1\", \"message\" : \"" + imgerror + "\" }");
            }
            //运营位2
            StartFlapHotTwo = GetActiveImageName("StartFlapHotTwo", "StartActiveHotTwoImg", "运营位2图", SubjectManage, mNewSubject.SubjectTemplatePicNoTwo, out imgerror);
            if (!string.IsNullOrEmpty(StartFlapHotTwo) && !string.IsNullOrEmpty(imgerror))
            {
                return Content("{ \"result\" : \"-1\", \"message\" : \"" + imgerror + "\" }");
            }


            #endregion

            #region 判断所有参数信息

            #region 渠道非空判断
            if (string.IsNullOrEmpty(ChannelNo))
            {
                return Content("{ \"result\" : \"-1\", \"message\" : \"渠道不能为空\" }");
                //return Json(new { result = "-1", message = "渠道不能为空" }, JsonRequestBehavior.AllowGet);
            }
            #endregion

            #region 判断价格标签
            string PriceName = string.Empty;
            if (!string.IsNullOrEmpty(CommodityPItem) && Convert.ToInt32(CommodityPItem) != 0 && !string.IsNullOrEmpty(CommodityP))
            {
                //PriceName = GetPriceName(Convert.ToInt32(CommodityPItem)).Replace("--", CommodityP);
                PriceName = CommodityP;
            }
            #endregion

            #region 活动开始时间和结束时间判断
            //活动开始和结束时间判断           
            DateTime ActivityBeginDate = Convert.ToDateTime(DateBegin);
            DateTime ActivityEndDate = DateTime.MaxValue;
            if (!string.IsNullOrEmpty(DateEnd))
            {
                ActivityEndDate = Convert.ToDateTime(DateEnd);
                if (ActivityBeginDate > ActivityEndDate)
                {
                    return Content("{ \"result\" : \"-1\", \"message\" : \"活动开始时间不能大于结束时间\" }");
                    //return Json(new { result = "-1", message = "活动开始时间不能大于结束时间" }, JsonRequestBehavior.AllowGet);
                }
            }
            #endregion

            #region 活动预热时间和活动开始时间判断
            if (!string.IsNullOrEmpty(HotDateBegin))
            {
                if (ActivityBeginDate < Convert.ToDateTime(HotDateBegin))
                {
                    return Content("{ \"result\" : \"-1\", \"message\" : \"预热时间不可大于活动时间\" }");
                    //return Json(new { result = "-1", message = "预热时间不可大于活动时间" }, JsonRequestBehavior.AllowGet);
                }
            }
            #endregion
            #endregion

            #region 给对象赋值
            SWfsNewSubject mSWfsNewSubject = new SWfsNewSubject();
            mSWfsNewSubject.ChannelType = ChannelNo;
            mSWfsNewSubject.CreateUserId = PresentationHelper.GetPassport().UserName;
            mSWfsNewSubject.DateBegin = ActivityBeginDate;
            mSWfsNewSubject.DateCreate = DateTime.Now;
            mSWfsNewSubject.DateEnd = ActivityEndDate;
            mSWfsNewSubject.DatePre = string.IsNullOrEmpty(HotDateBegin) ? DateTime.MaxValue : Convert.ToDateTime(HotDateBegin);
            mSWfsNewSubject.Ip = WebUtilityX.GetUserIpAddress();
            mSWfsNewSubject.IphoneContent = IPhoneText == "30字以内" ? string.Empty : IPhoneText;
            mSWfsNewSubject.PriceType = Convert.ToInt16(HotPrice); 
            mSWfsNewSubject.PrivilegeValueOrUrl = PrivilegeValueOrUrl;
            mSWfsNewSubject.ShowColumn = Convert.ToInt32(ShowColumn);
            mSWfsNewSubject.Status = Convert.ToInt16(Status);
            mSWfsNewSubject.SubjectContent = txtSubjectText;
            mSWfsNewSubject.SubjectName = SubjectName;
            mSWfsNewSubject.SubjectNo = SubjectNo;
            mSWfsNewSubject.SubjectPreStartBackgroundPic = FileHotImg;
            mSWfsNewSubject.SubjectPreStartTemplatePicNoOne = FlapHotOne;
            mSWfsNewSubject.SubjectPreStartTemplatePicNoTwo = FlapHotTwo;
            mSWfsNewSubject.SubjectPreStartTemplatePicUrlTwo = HotTwoUrl;
            mSWfsNewSubject.SubjectPreStartTemplateType = Convert.ToInt16(oneHotSelect);
            mSWfsNewSubject.SubjectTemplateBackgroundPicNO = FileBackImg ?? string.Empty;
            mSWfsNewSubject.SubjectTemplateTopPicNo = FileTopTopImg.IsNullOrEmpty() == true ? "" : FileTopTopImg;
            mSWfsNewSubject.SubjectTemplateTopPicType = Convert.ToInt16(TopPic);
            mSWfsNewSubject.SubjectTemplateType = Convert.ToInt16(Temp);
            mSWfsNewSubject.SubjectPreName = HotTitle;
            mSWfsNewSubject.SubjectPreChildtName = string.IsNullOrEmpty(HotDes) == true ? "" : HotDes;
            mSWfsNewSubject.SubjectPreTopPicNo = FlapHotLOGO;
            mSWfsNewSubject.SubjectPreTopPicUrl = FlapHotLOGOUrl;
            mSWfsNewSubject.SubjectPara = string.IsNullOrEmpty(CKBrand) == true ? "" : CKBrand;
            mSWfsNewSubject.SubjectIPhonePicNo = IphoneTopImg ?? string.Empty;
            //20131003新增
            mSWfsNewSubject.SubjectInletPicNo = string.Empty;//备用
            mSWfsNewSubject.SubjectIphoneTopPicNo = string.Empty;//备用
            mSWfsNewSubject.SubjectIphoneInletPicNo = IphoneClientImg;
            mSWfsNewSubject.SubjectWapInletPicNo = IphoneWapImg;
            mSWfsNewSubject.PrivilegeValuePicNo = IphoneFlapHotOne;
            mSWfsNewSubject.SubjectPriceType = Convert.ToInt32(CommodityPItem);
            mSWfsNewSubject.PriceName = PriceName;
            //20131015新增
            mSWfsNewSubject.SubjectPv = mNewSubject.SubjectPv;
            mSWfsNewSubject.SubjectPvBase = Convert.ToInt32(FocusNums);
            mSWfsNewSubject.SubjectBrandName = string.IsNullOrEmpty(BrandName) ? string.Empty : BrandName;
            mSWfsNewSubject.SubjectBrandUrl = string.IsNullOrEmpty(BrandUrl) ? string.Empty : BrandUrl;
            //20131111新增
            mSWfsNewSubject.SubjectGender = SexNo;

            mSWfsNewSubject.SubjectCategoryNo = CategoryNo;
          
            //20140303新增-- BU选项（BU1到BU5）必选 1：BU1  2：BU2 3：BU3 4：BU4 5：BU5
            mSWfsNewSubject.SubjectBU =BUNo.IsNullOrEmpty()==true?0: int.Parse(BUNo);
            //20140303新增----销售额预估
            mSWfsNewSubject.ForecastSalesAmount = ForecastSalesAmountNums.IsNullOrEmpty() == true ? 0 : decimal.Parse(ForecastSalesAmountNums); 
            //20140320新增---推荐频道
            mSWfsNewSubject.ChannelNo = chanelCategoryNo;
            //20140328设置请输入网页html代码、、请输入移动端html代码 
            mSWfsNewSubject.SubjectTemplateTopIphoneMemo1 = txtTopTopImgCodeM.IsNullOrEmpty() == true || txtTopTopImgCodeM.Contains("请输入移动端html代码") == true ? "" : txtTopTopImgCodeM;
            mSWfsNewSubject.SubjectTemplateTopMemo1 = txtTopTopImgCodePC.IsNullOrEmpty() == true || txtTopTopImgCodePC.Contains("请输入网页html代码") == true ? "" : txtTopTopImgCodePC;
            //20140415新增--进行中运营位1,2，领取优惠码（1预热可以领取，2，进行中可以领取）
            mSWfsNewSubject.SubjectTemplatePicNoOne = StartFlapHotOne;
            mSWfsNewSubject.SubjectTemplatePicNoTwo = StartFlapHotTwo;
            mSWfsNewSubject.SubjectTemplateIphonePicNoOne = StartIphoneFlapHotOne;
            mSWfsNewSubject.SubjectTemplatePicNoOneUrl = StartPrivilegeValueOrUrl; ;
            mSWfsNewSubject.SubjectTemplatePicNoTwoUrl = StartHotTwoUrl;
            mSWfsNewSubject.SubjectStartTemplateType =Convert.ToInt16(StartoneHotSelect); 
            mSWfsNewSubject.SubjectCouponShowType =CouponNew.IsNullOrEmpty()==true?"":CouponNew;
            mSWfsNewSubject.SubjectCoupon = CouponText.IsNullOrEmpty()==true?"":CouponText;//优惠券值
            mSWfsNewSubject.SubjectPreTemplateType = TempPreType.IsNullOrEmpty() == true ? short.Parse("3") : short.Parse(TempPreType);
            mSWfsNewSubject.SubjectPreRemind =SubjectPreRemind.IsNullOrEmpty()==true?short.Parse("0"): short.Parse(SubjectPreRemind);

            //mSWfsNewSubject.PageTitle = ""; // author:zhanggong  date:2014.11.06

            //是否支持免运费(0:不支持 1：支持;默认不支持)  author:zhanggong  date:2014.11.13
            mSWfsNewSubject.IsFreightFree = freightFree.IsNullOrEmpty() ? 0 : int.Parse(freightFree);

            #endregion

            #region 判断是编辑还是添加信息

            if (SubjectManage == "Create")
            { 
               
                int ret = service.NewSubjectCreate(mSWfsNewSubject);
                if (ret == 0)
                {
                    bool createGroup = Temp == "2" ? true : CreateSystemGroup(SubjectNo, CreateGroupNO());
                    if (createGroup)
                    {
                        return Content("{ \"result\" : \"1\", \"message\" : \"新增活动成功\" }");
                        //return Json(new { result = "1", message = "新增活动成功" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Content("{ \"result\" : \"-1\", \"message\" : \"新增活动成功，新增系统分组失败，请记录信息联系管理员\" }");
                        //return Json(new { result = "-1", message = "新增活动成功，新增系统分组失败，请记录信息联系管理员" }, JsonRequestBehavior.AllowGet);
                    }

                }
                else
                {
                    return Content("{ \"result\" : \"-1\", \"message\" : \"添加失败\" }");
                    //return Json(new { result = "-1", message = "添加失败" }, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                bool retVal = service.NewSubjectEdit(mSWfsNewSubject);
                if (retVal)
                {
                    return Content("{ \"result\" : \"1\", \"message\" : \"活动编辑成功\" }");
                    //return Json(new { result = "1", message = "活动编辑成功" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Content("{ \"result\" : \"-1\", \"message\" : \"主键信息有误\" }");
                    //return Json(new { result = "-1", message = "主键信息有误" }, JsonRequestBehavior.AllowGet);
                }

            }
            #endregion

        }
        #endregion

        #region 判断图片 返回图片名称
        /// <summary>
        /// 判断图片 返回图片名称
        /// 必须为必选，固定宽高图片
        /// 不限可使用，100-200不可使用
        /// </summary>
        /// <param name="FormName">接受参数值web页面标签Name</param>
        /// <param name="ConfigName">对应配置文件中配置</param>
        /// <param name="Dir">错误描述信息</param>
        /// <param name="SubjectManage">是否是编辑</param>
        /// <param name="EditMsg">编辑中对应字段值</param>
        /// <param name="imgerror">返回错误信息</param>        
        /// <returns></returns>
        public string GetActiveImageName(string FormName, string ConfigName, string Dir, string SubjectManage, string EditMsg, out string imgerror)
        {
            bool FlagEmpty = false;
            bool ImageConfig = true;
            return GetActiveImageName(FormName, ConfigName, ImageConfig, Dir, SubjectManage, EditMsg, FlagEmpty, out  imgerror);
        }
        /// <summary>
        /// 判断图片 返回图片名称
        /// </summary>
        /// <param name="FormName">接受参数值web页面标签Name</param>
        /// <param name="ConfigName">对应配置文件中配置</param>
        /// <param name="ImageConfig">宽高是否固定</param>
        /// <param name="Dir">错误描述信息</param>
        /// <param name="SubjectManage">是否是编辑</param>
        /// <param name="EditMsg">编辑中对应字段值</param>
        /// <param name="imgerror">返回错误信息</param>
        /// <param name="FlagEmpty">图片是否可空</param>
        /// <returns></returns>
        public string GetActiveImageName(string FormName, string ConfigName, bool ImageConfig, string Dir, string SubjectManage, string EditMsg, bool FlagEmpty, out string imgerror)
        {
            imgerror = string.Empty;
            string ImageName = ImgNameGet(FormName, ConfigName, ImageConfig, out imgerror);
            if (string.IsNullOrEmpty(ImageName))
            {
                if (!string.IsNullOrEmpty(imgerror) && imgerror != "未添加图片")
                {
                    imgerror = string.Format("{0}错误，{1}", Dir, imgerror);
                }
                else if (SubjectManage == "Edit" && !string.IsNullOrEmpty(EditMsg))
                {
                    imgerror = string.Empty;
                    ImageName = EditMsg;
                }
                else
                {
                    if (FlagEmpty)
                    {
                        imgerror = string.Empty;
                        ImageName = EditMsg;
                    }
                    else
                    {
                        imgerror = string.Format("{0}错误，{1}", Dir, imgerror);
                    }
                }
            }
            return ImageName;
        }
        #endregion

        #region 生成活动编号
        /// <summary>
        /// 生成活动编号
        /// </summary>
        /// <returns></returns>
        private static string CreateSubjectNO()
        {
            CommonService cs = new CommonService();
            string subjectId = cs.GetNextCounterId("SubjectNo").ToString("000");
            string num = subjectId.Substring(subjectId.Length - 3, 3);
            string year = DateTime.Now.Year.ToString().Substring(3, 1);
            string month = DateTime.Now.ToString("MM");
            string day = DateTime.Now.ToString("dd");
            return string.Format("{0}{1}{2}{3}", year, month, day, num);

            //string year = DateTime.Now.Year.ToString().Substring(3, 1);
            //string month = DateTime.Now.ToString("MM");
            //string day = DateTime.Now.ToString("dd");
            //Random rand = new Random();
            //int num = rand.Next(100, 999);
            //string subjectNo = string.Format("{0}{1}{2}{3}", year, month, day, num);
            //SWfsNewSubjectService service = new SWfsNewSubjectService();
            //SWfsNewSubject mSWfsNewSubject = service.GetNewSubjectInfo(subjectNo);
            //if (mSWfsNewSubject != null)
            //{
            //    return CreateSubjectNO();
            //}
            //else
            //{
            //    return subjectNo;
            //}
        }
        #endregion

        #region 上传图片显示图片名称
        /// <summary>
        /// 上传图片返回名称
        /// </summary>
        /// <param name="ImgName"></param>
        /// <param name="CongifName"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        private string ImgNameGet(string ImgName, string CongifName, out string error)
        {
            error = string.Empty;
            bool flag = true;
            return ImgNameGet(ImgName, CongifName, flag, out error);
        }
        /// <summary>
        /// 上传图片显示图片名称
        /// </summary>
        /// <param name="ImgName"></param>
        /// <param name="CongifName"></param>
        /// <returns></returns>
        private string ImgNameGet(string ImgName, string CongifName, bool flag, out string error)
        {
            error = string.Empty;
            if (Request.Files[ImgName] == null || Request.Files[ImgName].ContentLength == 0)
            {
                error = "未添加图片";
                return string.Empty;
            }
            Dictionary<string, string> belongsSubjectPic = new CommonService().PostImg(Request.Files[ImgName], AppSettingManager.AppSettings[CongifName].ToString(), flag);
            string belongsSubjectPicNo = belongsSubjectPic.Values.FirstOrDefault();
            string belongsSubjectPicNokey = belongsSubjectPic.Keys.FirstOrDefault();
            if (!string.IsNullOrEmpty(belongsSubjectPicNo))
            {
                if (belongsSubjectPicNokey != "error")
                {
                    return belongsSubjectPicNo;
                }
                else
                {
                    error = belongsSubjectPicNo;
                    return string.Empty;
                }
            }
            else
            {
                error = "图片上传失败";
                return string.Empty;
            }
        }
        #endregion

        #endregion

        #region 活动商品可视化管理列表


        /// <summary>
        /// 活动商品可视化
        /// </summary>
        /// <param name="subjectNo"></param>
        /// <param name="SCategoryNo"></param>
        /// <returns></returns>
        public ActionResult ProductView()
        {
            SWfsNewProductService service = new SWfsNewProductService();
            string subjectNo = Rq.GetStringQueryString("subjectNo");
            string categoryNo = Rq.GetStringQueryString("scategoryNo");
            IList<SubjectNewProductRefModel> listProduct = null;

            if (!string.IsNullOrEmpty(categoryNo))
            {
                listProduct = subject.GetSWfsNewSubjectProductRefList(subjectNo, categoryNo);
            }
            else
            {
                listProduct = subject.GetSWfsNewSubjectProductRefListAll(subjectNo);
            }

            foreach (SubjectNewProductRefModel sp in listProduct)
            {
                ProductInventoryNew pinventory = service.GetInventoryByProductNo(sp.ProductNo); 
                sp.Quantity = pinventory.SumQuantity;  
            }
            ViewBag.SubjectNo = subjectNo;
            ViewBag.CategoryNo = categoryNo;
            return View(listProduct);
        }

        /// <summary>
        /// 保存商品排序
        /// </summary>
        /// <param name="subjectNo"></param>
        /// <param name="productNos"></param>
        /// <returns></returns>
        public ActionResult AjaxSaveProductSort()
        {
            string subjectNo = Rq.GetStringForm("subjectNo") ?? Rq.GetStringQueryString("subjectNo");
            string productNos = Rq.GetStringForm("productNos") ?? Rq.GetStringQueryString("productNos");
            string categoryNo = Rq.GetStringForm("scategoryNo") ?? Rq.GetStringQueryString("scategoryNo");
            IList<SubjectNewProductRefModel> listProduct = null;
            if (!string.IsNullOrEmpty(categoryNo))
            {
                listProduct = subject.GetSWfsNewSubjectProductRefList(subjectNo, categoryNo);
            }
            else
            {
                listProduct = subject.GetSWfsNewSubjectProductRefListAll(subjectNo);
            }


            if (listProduct == null || listProduct.Count() <= 0)
            {
                return Json(new { result = 0, message = "保存失败！" });
            }
            else
            {
                #region 更新排序规则
                string[] proNoslist = productNos.Split(',');
                for (int i = 0; i < proNoslist.Count(); i++)
                {
                    string productNo = proNoslist[i];
                    try
                    {
                        SubjectNewProductRefModel productRefSingle = listProduct.Where(p => p.ProductNo == productNo).FirstOrDefault();
                        if (productRefSingle != null)
                        {
                            subject.UpdateSWfsNewSubjectProductRefSort(i, productRefSingle.ProductNo, productRefSingle.CategoryNo);
                        }
                    }
                    catch (Exception ex)
                    {
                        return Json(new { result = 0, message = ex.Message });
                    }
                }
                #endregion

                return Json(new { result = 1, message = "保存成功！" });
            }
        }
        #endregion

        #region 分组管理

        #region 编辑分组
        /// <summary>
        /// 创建分组创建视图
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateGroup()
        {
            string subjectNo = Request.Params["subjectNo"];
            if (string.IsNullOrEmpty(subjectNo))
            {
                return Content("缺少活动标识");
            }

            SWfsNewSubjectService service = new SWfsNewSubjectService();
            SWfsNewSubject mSWfsNewSubject = service.GetNewSubjectInfo(subjectNo);
            if (mSWfsNewSubject == null)
            {
                return Content("活动标识错误");
            }
            return View(mSWfsNewSubject);
        }
        /// <summary>
        /// 创建分组编辑视图
        /// </summary>
        /// <returns></returns>
        public ActionResult EditGroup()
        {
            string categoryNo = Request.Params["categoryNo"];
            if (string.IsNullOrEmpty(categoryNo))
            {
                return Content("缺少分组标识");
            }
            SWfsNewSubjectService service = new SWfsNewSubjectService();
            SWfsNewSubjectCategory mNewGroup = service.GetNewGroupInfo(categoryNo);
            if (mNewGroup == null)
            {
                return Content("活动标识错误");
            }
            SWfsNewSubject mNewSubject = service.GetNewSubjectInfo(mNewGroup.SubjectNo);
            if (mNewSubject == null)
            {
                return Content("该活动信息已失效");
            }
            ViewBag.SubjectName = mNewSubject.SubjectName;
            return View(mNewGroup);
        }
        /// <summary>
        /// 分组操作
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult GroupHandle()
        {
            //IEnumerable<string> filelist = new List<string>();
            //StringBuilder sb = new StringBuilder();
            
            //Dictionary<string, string> belongsSubjectPic = new CommonService().PostImg(Request.Files[ImgName], AppSettingManager.AppSettings[CongifName].ToString(), flag);
            //if (Request.Files.AllKeys.Count()>0)
            //{
            //    filelist = Request.Files.AllKeys.Where(p => p == "PuzzlePic");                
            //}
            //for (int i = 1; i <= 10; i++)
            //{
            //   sb.Append(  new CommonService().PostImg(Request.Files["PuzzlePic" + i], "100");
            //}



            //获取SubManage字段来判断是编辑还是添加
            string GroupManage = Request.Params["GroupManage"];
            SWfsNewSubjectService service = new SWfsNewSubjectService();
            string PicType = Request.Params["PicType"];

            #region 获取分组主键
            string GroupNo = GroupManage == "Create" ? CreateGroupNO() : Request.Params["GroupNo"];
            if (string.IsNullOrEmpty(GroupNo))
            {
                return Json(new { result = "-1", message = "主键信息有误" }, JsonRequestBehavior.AllowGet);
            }
            #endregion

            #region 如果是编辑判断是否存在对象
            SWfsNewSubjectCategory mNewGroup = new SWfsNewSubjectCategory();
            if (GroupManage == "Edit")
            {
                mNewGroup = service.GetNewGroupInfo(GroupNo);
                if (mNewGroup == null)
                {
                    return Json(new { result = "-1", message = "主键信息有误" }, JsonRequestBehavior.AllowGet);
                }
            }

            #endregion

            #region 判断图片信息
            string imgError = string.Empty;
            string GroupPic = ImgNameGet("GroupImage", "GroupImage", out imgError);
            if (string.IsNullOrEmpty(GroupPic))
            {
                if (!string.IsNullOrEmpty(imgError) && imgError != "未添加图片")
                {
                    return Json(new { result = "-1", message = "新增分组失败，" + imgError }, JsonRequestBehavior.AllowGet);
                }
                else if (GroupManage == "Edit" && !string.IsNullOrEmpty(mNewGroup.PicNo))
                {
                    GroupPic = mNewGroup.PicNo; 
                }
                else
                {
                    GroupPic = string.Empty;
                    //return Json(new { result = "-1", message = "新增分组失败，" + imgError }, JsonRequestBehavior.AllowGet);
                }
            }

            string IphoneGroupImage = ImgNameGet("IphoneGroupImage", "IphoneGroupImg", out imgError);
            if (string.IsNullOrEmpty(IphoneGroupImage))
            {
                if (!string.IsNullOrEmpty(imgError) && imgError != "未添加图片")
                {
                    return Json(new { result = "-1", message = "新增分组失败，" + imgError + "(建议图片尺寸640*200)" }, JsonRequestBehavior.AllowGet);
                }
                else if (GroupManage == "Edit" && !string.IsNullOrEmpty(mNewGroup.IphonePicNo))
                {
                    IphoneGroupImage = mNewGroup.IphonePicNo;
                }
                else
                {
                    IphoneGroupImage = string.Empty;
                }
            }
            //string PuzzleIphonePic = SaveImg(Request.Files["PuzzleIphonePic"], AppSettingManager.AppSettings["aaa"]);
            string PuzzleIphonePic = ImgNameGet("PuzzleIphonePic", "IphoneImg", out imgError);
            if (string.IsNullOrEmpty(PuzzleIphonePic))
            {
                if (!string.IsNullOrEmpty(imgError) && imgError != "未添加图片")
                {
                    return Json(new { result = "-1", message = "新增分组失败，" + imgError }, JsonRequestBehavior.AllowGet);
                }
                else if (GroupManage == "Edit" && !string.IsNullOrEmpty(mNewGroup.PuzzleIphonePicNo))
                {
                    PuzzleIphonePic = mNewGroup.PuzzleIphonePicNo;
                }
                else
                {
                    PuzzleIphonePic = string.Empty;
                }
            }

            

            #endregion


            #region 给参数赋值
            //20140328设置请输入网页html代码、、请输入移动端html代码
            string txtTopTopImgCodePC = Request.Params["txtTopTopImgCodePC"];
            string txtTopTopImgCodeM = Request.Params["txtTopTopImgCodeM"]; 

            SWfsNewSubjectCategory mCategory = new SWfsNewSubjectCategory();
            mCategory.CategoryName = Request.Params["GroupName"];
            mCategory.ShowColumn = Convert.ToInt32(Request.Params["GroupClomn"]);
            mCategory.SubjectNo = Request.Params["subjectNo"];
            mCategory.CreateUserId = PresentationHelper.GetPassport().UserName;
            mCategory.PicNo = GroupPic;
            mCategory.DateCreate = DateTime.Now;
            mCategory.CategoryNo = GroupNo;
            mCategory.SortNo = Convert.ToInt32(Request.Params["SortNo"]);
            mCategory.PicUrl = Request.Params["PicUrl"];
            mCategory.IsDelete = 1;
            mCategory.IphonePicNo = IphoneGroupImage ?? string.Empty;
            mCategory.PuzzleIphonePicNo = PuzzleIphonePic ?? string.Empty;
            mCategory.PicType = short.Parse(PicType); 
            mCategory.IphonePicMemo = txtTopTopImgCodeM.IsNullOrEmpty() == true || txtTopTopImgCodeM.Contains("请输入移动端html代码") == true ? "" : txtTopTopImgCodeM;
            mCategory.TopPicMemo = txtTopTopImgCodePC.IsNullOrEmpty() == true || txtTopTopImgCodePC.Contains("请输入网页html代码") == true ? "" : txtTopTopImgCodePC; 
            #endregion

            #region 如果选择标准模板或者修改为标准模板 创建系统分组

            #endregion

            #region 判断是编辑还是添加信息

            if (GroupManage == "Create")
            {
                #region 添加图片
                StringBuilder sb = new StringBuilder();
                try
                {
                    for (int i = 1; i <= 10; i++)
                    {
                        if (Request.Files["PuzzlePic" + i] != null)
                        {
                            sb.Append(SaveImg(Request.Files["PuzzlePic" + i], AppSettingManager.AppSettings["PuzzlePic" + i]) + ",");
                            if (rsPic.Keys.Contains("error"))
                            {
                                #region 图片尺寸判断
                                //ViewData["tip"] = new HtmlString("<script>alert('" + rsPic["error"] + "')</script>");
                                if (i == 1)
                                {
                                    return Content("<script>alert('顶通图尺寸不符！！')</script>");
                                }
                                else
                                {
                                    sb.Append(SaveImg(Request.Files["PuzzlePic" + i], AppSettingManager.AppSettings["PuzzlePic" + i]));
                                }
                                if (i == 2)
                                {
                                    return Content("<script>alert('左大图尺寸不符！！')</script>");
                                }
                                else
                                {
                                    sb.Append(SaveImg(Request.Files["PuzzlePic" + i], AppSettingManager.AppSettings["PuzzlePic" + i]));
                                }
                                if (i == 3)
                                {
                                    return Content("<script>alert('居上大图尺寸不符！！')</script>");
                                }
                                else
                                {
                                    sb.Append(SaveImg(Request.Files["PuzzlePic" + i], AppSettingManager.AppSettings["PuzzlePic" + i]));
                                }
                                if (i == 4)
                                {
                                    return Content("<script>alert('居下大图尺寸不符！！')</script>");
                                }
                                else
                                {
                                    sb.Append(SaveImg(Request.Files["PuzzlePic" + i], AppSettingManager.AppSettings["PuzzlePic" + i]));
                                }
                                if (i == 5)
                                {
                                    return Content("<script>alert('右大图尺寸不符！！')</script>");
                                }
                                else
                                {
                                    sb.Append(SaveImg(Request.Files["PuzzlePic" + i], AppSettingManager.AppSettings["PuzzlePic" + i]));
                                }
                                if (i == 6)
                                {
                                    return Content("<script>alert('小图1尺寸不符！！')</script>");
                                }
                                else
                                {
                                    sb.Append(SaveImg(Request.Files["PuzzlePic" + i], AppSettingManager.AppSettings["PuzzlePic" + i]));
                                }
                                if (i == 7)
                                {
                                    return Content("<script>alert('小图2尺寸不符！！')</script>");
                                }
                                else
                                {
                                    sb.Append(SaveImg(Request.Files["PuzzlePic" + i], AppSettingManager.AppSettings["PuzzlePic" + i]));
                                }
                                if (i == 8)
                                {
                                    return Content("<script>alert('小图3尺寸不符！！')</script>");
                                }
                                else
                                {
                                    sb.Append(SaveImg(Request.Files["PuzzlePic" + i], AppSettingManager.AppSettings["PuzzlePic" + i]));
                                }
                                if (i == 9)
                                {
                                    return Content("<script>alert('小图4尺寸不符！！')</script>");
                                }
                                else
                                {
                                    sb.Append(SaveImg(Request.Files["PuzzlePic" + i], AppSettingManager.AppSettings["PuzzlePic" + i]));
                                }
                                if (i == 10)
                                {
                                    return Content("<script>alert('小图5尺寸不符！！')</script>");
                                }
                                else
                                {
                                    sb.Append(SaveImg(Request.Files["PuzzlePic" + i], AppSettingManager.AppSettings["PuzzlePic" + i]));
                                }
                                #endregion
                            }
                        }
                        if (i == 9)
                        {
                            sb.Append(SaveImg(Request.Files["PuzzlePic" + i], AppSettingManager.AppSettings["PuzzlePic" + i]));
                        }
                        else
                        {
                            sb.Append(SaveImg(Request.Files["PuzzlePic" + i], AppSettingManager.AppSettings["PuzzlePic" + i]));
                        }
                    }
                }
                catch { }
                mCategory.PuzzlePicNo = sb.ToString();
                #endregion

                #region 添加连接

                mCategory.PuzzlePicUrl = Request.Form["linktext"];
                if (string.IsNullOrEmpty(mCategory.PuzzlePicUrl))
                {
                    mCategory.PuzzlePicUrl = ",,,,,,,,,";
                }
                else
                {
                    if (mCategory.PuzzlePicUrl.Split(',').Length<10)
                    {
                        mCategory.PuzzlePicUrl = ",,,,,,,,,";
                    }
                }
                #endregion


                #region 添加最新分组模板
            
                #endregion

                int ret = service.NewGroupCreate(mCategory);
                if (ret == 0)
                {
                    return Json(new { result = "1", message = "新增分组成功" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { result = "-1", message = "新增分组失败" }, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                string[] picList= (Request.Form["picfilelist"]+"").Split(',');
                if (picList.Length < 10)
                {
                    picList = new string[10];
                }
                StringBuilder sb = new StringBuilder();
                try
                {
                    for (int i = 0; i < 10; i++)
                    {
                        if (Request.Files["PuzzlePic" + (i + 1)] != null && Request.Files["PuzzlePic" + (i + 1)].ContentLength > 0)
                        {
                            picList[i] = SaveImg(Request.Files["PuzzlePic" + (i + 1)], AppSettingManager.AppSettings["PuzzlePic" + (i + 1)]);
                            if (rsPic.Keys.Contains("error"))
                            {
                                //ViewData["tip"] = new HtmlString("<script>alert('" + rsPic["error"] + "')</script>");
                                #region 图片尺寸判断
                                //ViewData["tip"] = new HtmlString("<script>alert('" + rsPic["error"] + "')</script>");
                                if (i == 0)
                                {
                                    return Content("<script>alert('顶通图尺寸不符！！')</script>");
                                }
                                else
                                {
                                    sb.Append(SaveImg(Request.Files["PuzzlePic" + i], AppSettingManager.AppSettings["PuzzlePic" + i]));
                                }
                                if (i == 1)
                                {
                                    return Content("<script>alert('左大图尺寸不符！！')</script>");
                                }
                                else
                                {
                                    sb.Append(SaveImg(Request.Files["PuzzlePic" + i], AppSettingManager.AppSettings["PuzzlePic" + i]));
                                }
                                if (i == 2)
                                {
                                    return Content("<script>alert('居上大图尺寸不符！！')</script>");
                                }
                                else
                                {
                                    sb.Append(SaveImg(Request.Files["PuzzlePic" + i], AppSettingManager.AppSettings["PuzzlePic" + i]));
                                }
                                if (i == 3)
                                {
                                    return Content("<script>alert('居下大图尺寸不符！！')</script>");
                                }
                                else
                                {
                                    sb.Append(SaveImg(Request.Files["PuzzlePic" + i], AppSettingManager.AppSettings["PuzzlePic" + i]));
                                }
                                if (i == 4)
                                {
                                    return Content("<script>alert('右大图尺寸不符！！')</script>");
                                }
                                else
                                {
                                    sb.Append(SaveImg(Request.Files["PuzzlePic" + i], AppSettingManager.AppSettings["PuzzlePic" + i]));
                                }
                                if (i == 5)
                                {
                                    return Content("<script>alert('小图1尺寸不符！！')</script>");
                                }
                                else
                                {
                                    sb.Append(SaveImg(Request.Files["PuzzlePic" + i], AppSettingManager.AppSettings["PuzzlePic" + i]));
                                }
                                if (i == 6)
                                {
                                    return Content("<script>alert('小图2尺寸不符！！')</script>");
                                }
                                else
                                {
                                    sb.Append(SaveImg(Request.Files["PuzzlePic" + i], AppSettingManager.AppSettings["PuzzlePic" + i]));
                                }
                                if (i == 7)
                                {
                                    return Content("<script>alert('小图3尺寸不符！！')</script>");
                                }
                                else
                                {
                                    sb.Append(SaveImg(Request.Files["PuzzlePic" + i], AppSettingManager.AppSettings["PuzzlePic" + i]));
                                }
                                if (i == 8)
                                {
                                    return Content("<script>alert('小图4尺寸不符！！')</script>");
                                }
                                else
                                {
                                    sb.Append(SaveImg(Request.Files["PuzzlePic" + i], AppSettingManager.AppSettings["PuzzlePic" + i]));
                                }
                                if (i == 9)
                                {
                                    return Content("<script>alert('小图5尺寸不符！！')</script>");
                                }
                                else
                                {
                                    sb.Append(SaveImg(Request.Files["PuzzlePic" + i], AppSettingManager.AppSettings["PuzzlePic" + i]));
                                }
                                #endregion
                            }
                        }
                        if (i == 9)
                        {
                            sb.Append(picList[i]);
                        }
                        else
                        {
                            sb.Append(picList[i] + ",");
                        }

                    }
                }
                catch { }
                mCategory.PuzzlePicNo = sb.ToString();

                //链接地址
                sb.Length = 0;
                string[] str = Request.Form["linktext"].Split(',');
                for (int j = 0; j < str.Length; j++)
                {
                    if (j==9)
                    {
                        sb.Append(str[j]);
                        
                    }else
                    {
                        sb.Append(str[j] + ",");

                    }
                }
                mCategory.PuzzlePicUrl = sb.ToString();
                bool retVal = service.NewGroupEdit(mCategory);
                if (retVal)
                {
                    return Json(new { result = "1", message = "分组编辑成功" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { result = "-1", message = "分组信息有误" }, JsonRequestBehavior.AllowGet);
                }

            }
            #endregion

        }

        #region 创建主键编号
        /// <summary>
        /// 创建主键编号
        /// </summary>
        /// <returns></returns>
        private string CreateGroupNO()
        {
            return DateTime.Now.ToString("yyyyMMdd") + new CommonService().GetNextCounterId("CategoryNo").ToString("000");
        }
        #endregion


        #endregion

        #region 分组信息展示 //查看活动分组后的商品列表  temp4
        /// <summary>
        /// 查询所有分组信息
        /// </summary>
        /// <returns></returns>
        public ActionResult GroupList()
        {
            SWfsNewSubjectService service = new SWfsNewSubjectService();
            PaginationInfoModel pageinfo = new PaginationInfoModel();
            int pageSize = int.Parse(AppSettingManager.AppSettings["ComonListPageNum"].ToString());
            string CategoryNo = Rq.GetStringQueryString("categoryNo");
            string SubjectNo = Rq.GetStringQueryString("subjectNo");
            int currentPage = Rq.GetIntQueryString("pageindex");

            #region 条件筛选
            pageinfo.PageSize = pageSize;
            pageinfo.CurrentPage = currentPage;
            pageinfo.Condition = " 1=1 ";
            pageinfo.Condition += (string.IsNullOrEmpty(CategoryNo) || CategoryNo == "分组名称") ? "" : " and SWfsNewSubjectCategory.categoryName like '%" + CategoryNo + "%'";
            pageinfo.Condition += (string.IsNullOrEmpty(SubjectNo) || SubjectNo == "活动编号") ? "" : " and SWfsNewSubjectCategory.SubjectNo = '" + SubjectNo + "'";
            pageinfo.Condition += " And IsDelete=1 ";
            pageinfo.OrderBy = " SortNo ASC ";
            #endregion
            List<SWfsNewSubjectCategory> list = service.GetGroupList(ref pageinfo);
            SWfsNewSubject mSWfsNewSubject = service.GetNewSubjectInfo(SubjectNo);
            ViewBag.SWfsNewSubjectSingle = mSWfsNewSubject;
            ViewBag.SWfsNewSubjectCategory = list;
            ViewBag.PaginationInfoSingel = pageinfo;
            ViewBag.CategoryNo = CategoryNo;
            ViewBag.SubjectNo = SubjectNo;
            if (!string.IsNullOrEmpty(Request.QueryString["upCount"]))
            {
                subject.UpdateSubjectProductCount(Request.QueryString["SubjectNo"]);
            }
            //subject.GetSWfsNewSubject_UpdateProductCount(SubjectNo);
            return View();
        }
        #endregion

        #region 删除分组
        /// <summary>
        /// 删除分组信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RemoveGroup()
        {
            string CategoryNo = Request.Params["CategoryNo"];
            if (string.IsNullOrEmpty(CategoryNo))
            {
                return Json(new { result = "-1", message = "删除分组失败，未获取到分组编号" }, JsonRequestBehavior.AllowGet);
            }
            SWfsNewSubjectService service = new SWfsNewSubjectService();
            SWfsNewSubjectCategory mNewGroup = service.GetNewGroupInfo(CategoryNo);
            if (mNewGroup == null)
            {
                return Json(new { result = "-1", message = "删除分组失败，未获取到该分组信息" }, JsonRequestBehavior.AllowGet);
            }
            mNewGroup.IsDelete = 2;
            bool retVal = service.NewGroupEdit(mNewGroup);
            if (retVal)
            {
                return Json(new { result = "1", message = "该分组已删除" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { result = "-1", message = "删除分组失败，分组信息有误" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 创建系统分组
        /// <summary>
        /// 创建系统分组
        /// </summary>
        /// <param name="subjectNo">活动编号</param>
        /// <param name="GroupNo">分组编号</param>
        /// <returns></returns>
        private bool CreateSystemGroup(string subjectNo, string GroupNo)
        {
            SWfsNewSubjectService service = new SWfsNewSubjectService();
            SWfsNewSubjectCategory mCategory = new SWfsNewSubjectCategory();
            mCategory.CategoryName = "系统创建分组";
            mCategory.ShowColumn = 3;
            mCategory.SubjectNo = subjectNo;
            mCategory.CreateUserId = PresentationHelper.GetPassport().UserName;
            mCategory.PicNo = "";
            mCategory.DateCreate = DateTime.Now;
            mCategory.CategoryNo = GroupNo;
            mCategory.SortNo = 0;
            mCategory.IsDelete = 1;
            mCategory.PicUrl = string.Empty;
            mCategory.IphonePicNo = string.Empty;
            int ret = service.NewGroupCreate(mCategory);
            if (ret == 0)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region 创建楼层表 楼层运营位连接 楼层运营位名称
        /// <summary>
        /// 创建楼层表 楼层运营位连接 楼层运营位名称
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateGroupLC()
        {
            string message = string.Empty;
            SWfsNewSubjectService service = new SWfsNewSubjectService();
            string SubjectNoID = Request.Params["SubjectNoID"];
            string BrandName = Request.Params["BrandName"];
            string BrandUrl = Request.Params["BrandUrl"];

            SWfsNewSubject mSWfsNewSubject = service.GetNewSubjectInfo(SubjectNoID);
            if (mSWfsNewSubject != null)
            {
                mSWfsNewSubject.SubjectBrandName = BrandName;
                mSWfsNewSubject.SubjectBrandUrl = BrandUrl;
                bool retVal = service.NewSubjectEdit(mSWfsNewSubject);
                if (retVal)
                {
                    message = "导航运营位保存成功";
                }
                else
                {
                    message = "导航运营位保存失败";
                }
                //清除缓存
                EnyimMemcachedClient.Instance.Remove("ShangpinSWfsNewSubjectSingleData" + mSWfsNewSubject.SubjectNo);
            }
            CommonHelp.Alert(message, "/Shangpin/Subject/grouplist.html?subjectNo=" + SubjectNoID);
            return View();
        }
        #endregion
        #endregion

        #region 活动数据统计列表
        /// <summary>
        /// 活动数据统计列表
        /// </summary>
        /// <returns></returns>
        public ActionResult DataStatistics()
        {
            IList<SubjectProductStatistic> tempProductStatisticList = new List<SubjectProductStatistic>();
            string subjectNo = Rq.GetStringForm("subjectNo") ?? Rq.GetStringQueryString("subjectNo");
            string range = Rq.GetStringForm("range") ?? Rq.GetStringQueryString("range");
            string beginTime = Rq.GetStringForm("beginTime") ?? Rq.GetStringQueryString("beginTime");
            string endTime = Rq.GetStringForm("endTime") ?? Rq.GetStringQueryString("endTime");
            string toexcel = Rq.GetStringForm("toexcel") ?? Rq.GetStringQueryString("toexcel");
            string buNo = Rq.GetStringForm("buNo") ?? Rq.GetStringQueryString("buNo");
            string brandNo = Rq.GetStringForm("brandNo") ?? Rq.GetStringQueryString("brandNo");
            string categoryNo = Rq.GetStringForm("categoryNo") ?? Rq.GetStringQueryString("categoryNo");

            if (string.IsNullOrEmpty(range)) { range = "0"; }
            if (string.IsNullOrEmpty(beginTime) && range == "5") { beginTime = DateTime.Now.Date.ToString(); }
            ViewBag.subjectNo = subjectNo;
            ViewBag.range = range;
            ViewBag.beginTime = beginTime;
            ViewBag.endTime = endTime;
            ViewBag.BUNo = buNo;
            ViewBag.BrandNo = brandNo;
            ViewBag.CategoryNo = categoryNo;
            SWfsNewSubject subjectInfo = subject.GetSubjectInfoNew(subjectNo);
            if (subjectInfo != null)
            {
                ViewBag.SubjectName = subjectInfo.SubjectName;
            }
          IList<SubjectProductStatistic>  productStatistics = subject.GetSubjectProductStatisticList(subjectNo, range, beginTime, endTime);
          if (productStatistics != null && productStatistics.Count() > 0)
          {
              #region 条件筛选
              tempProductStatisticList = productStatistics;
              Expression<Func<SubjectProductStatistic, bool>> predicate = PredicateBuilder.True<SubjectProductStatistic>();
              if (!string.IsNullOrEmpty(buNo))
              {
                  predicate = predicate.And(a => a.BU.Equals(buNo));
              }
              if (!string.IsNullOrEmpty(brandNo))
              {
                  predicate = predicate.And(a => a.BrandNo.Equals(brandNo));
              }
              if (!string.IsNullOrEmpty(categoryNo))
              {
                  predicate = predicate.And(a => a.CategoryNo.Equals(categoryNo));
              }
              tempProductStatisticList = productStatistics.Where(predicate.Compile()).ToList();
              #endregion
          }
          SubjectSaleStatistic sale = subject.GetSubjectSaleStatistic(subjectNo, range, beginTime, endTime);
          SubjectVisitStatistic visitStatistic = subject.GetSubjectVisitStatistic(subjectNo, range, beginTime, endTime);
           IList<ORderToExcel>  orderSumList=subject.GetOrderByNewSubject(subjectNo.Trim());
          
            #region 各参数赋值 BU|品牌|品类|筛选数据
           if (orderSumList != null)
           { ViewBag.OrderNums = orderSumList.Count(); }
           else
           { ViewBag.OrderNums = 0; }
           ViewBag.subjectProductStatistics = tempProductStatisticList;
           ViewBag.subjectSaleStatistics = sale;
           ViewBag.subjectVisitStatistic = visitStatistic;

            Dictionary<string, string> buDic = new Dictionary<string, string>(); //BU
            Dictionary<string, string> brandDic = new Dictionary<string, string>(); //品牌
            Dictionary<string, string> categoryDic = new Dictionary<string, string>(); //品类
            if (productStatistics != null && productStatistics.Count() > 0)
            {
                subject.NewSubjectFilterStatisticData(productStatistics, ref buDic, ref brandDic, ref categoryDic);
            }
            ViewBag.BUStatisticsData = buDic; //BU
            ViewBag.BrandStatisticsData = brandDic; //品牌
            ViewBag.CategoryStatisticsData = categoryDic; //品类
            #endregion

            if (!string.IsNullOrEmpty(toexcel))
            {
                switch (toexcel)
                {
                    case "1":
                        subject.GetSubjectDataStatisticsToExcel(tempProductStatisticList, sale, visitStatistic, subjectInfo);
                        break;
                    case "2":
                        subject.GetSubjectProductToExcel(subjectInfo);
                        break;
                    case "3":
                        subject.GetSubjectOrderSumToExcel(orderSumList,subjectInfo);
                        break;
                }
            }
            return View();
        }
        #endregion 

        #region 获取品牌信息
        public ActionResult SubjectBrand(string ids = "")
        {
            List<string> list = null;
            if (!string.IsNullOrEmpty(ids))
            {
                if (ids.EndsWith(","))
                {
                    list = ids.Substring(0, ids.Length - 1).Split('|').ToList();
                }
                else
                {
                    list = ids.Split(',').ToList();
                }

            }
            List<BrandInfo> mBrandList = DapperUtil.Query<BrandInfo>("ComBeziWfs_WfsBrand_Select").OrderBy(x => x.BrandEnName).ToList();
            ViewBag.BrandList = list;
            return PartialView(mBrandList);
        }
        #endregion

        #region 清除缓存数据
        //批量清除数据活动 
        [HttpPost]
        public ActionResult AddSubjectListRedisCaches(string subjectNos, string status)
        {
            int count = 0;
            #region 清除缓存
            if (!string.IsNullOrEmpty(subjectNos))
            {
                string redisCache = AppSettingManager.AppSettings["RedisCacheProviderKeyShow"];
                string redisCacheTopic = AppSettingManager.AppSettings["RedisCacheNewSubjectTopicNo"];

                RedisCacheProvider.SetRedisCluster(redisCache);
                var instance = RedisCacheProvider.Instance;
                if (status == "1")
                {
                    var listKeys = instance.SearchKeys(redisCacheTopic);
                    if (listKeys != null)
                    {
                        foreach (var item in listKeys)
                        {
                            count += 1;
                            instance.Remove(item);
                            RedisCacheProvider.Instance.Remove("ShangpinSWfsNewSubjectProductListByTopicNo" + item);
                        }
                    }
                }
                else if (status == "2")
                {
                    string[] subjectNoList = subjectNos.Split(',');
                    foreach (var item in subjectNoList)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            string redisCacheTopicNo = redisCacheTopic + item;
                            if (instance.Get<object>(redisCacheTopicNo) != null)
                            {
                                instance.Remove(redisCacheTopicNo);
                                count = count + 1;
                            } 
                            EnyimMemcachedClient.Instance.Remove("ShangpinSWfsNewSubjectSingleData" + item);
                            EnyimMemcachedClient.Instance.Remove("ShangpinSWfsNewSubjectVisitsByTopicNo" + item); 
                            EnyimMemcachedClient.Instance.Remove("SWfsVActivityCodesRefByNewSubjectNoTopCount" + item);
                            RedisCacheProvider.Instance.Remove("ShangpinSWfsNewSubjectProductListByTopicNo"+item);
                            
                        }
                    }
                }
            }
            #endregion
            return Json(new { Ok = 1, Msg = count });

        }

        public ActionResult ClearSubjectCache(string subjectNos)
        {
            string[] subjectnoList=new string[0];
            if (!string.IsNullOrEmpty(subjectNos))
            {
                subjectnoList = subjectNos.Split(',');
                for (int i = 0; i < subjectnoList.Length; i++)
                {
                    EnyimMemcachedClient.Instance.Remove("ShangpinSWfsNewSubjectSingleData" + subjectnoList[i]);
                    EnyimMemcachedClient.Instance.Remove("ShangpinSWfsNewSubjectVisitsByTopicNo" + subjectnoList[i]);
                    EnyimMemcachedClient.Instance.Remove("SWfsVActivityCodesRefByNewSubjectNoTopCount" + subjectnoList[i]);
                    RedisCacheProvider.Instance.Remove("ShangpinSWfsNewSubjectProductListByTopicNo" + subjectnoList[i]);
                    RedisCacheProvider.Instance.Remove("SPNewTopicProducts" + subjectnoList[i]);
                    RedisCacheProvider.Instance.Remove("ComBeziWfs_SWfsNewSubjectCategory_SelectAllBySubjectNo_" + subjectnoList[i]);
                }
            }
            return Json(new { Ok = 1, Msg = subjectnoList.Length },JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region 导出商品excel信息
        [HttpPost]
        public ActionResult ToExcewlSubjectList(string subjectNos, string status)
        { 
            return Json(new { Ok = 1, Msg = "" });
        }
        #endregion

        //通用保存图片
        private string SaveImg(HttpPostedFileBase file, string imgInfo)
        {
            CommonService commonService = new CommonService();
            rsPic.Clear();
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

        #region 添加价格类型数据
             /// <summary>
        /// 向活动中添加商品
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddNewSubjectPriceType(string priceTag)
        {
            int count = 0;
            if (subject.SelectSingleSWfsNewSubjectPriceType(priceTag) <= 0)
            {
                SWfsNewSubjectPriceType typeSingle = new SWfsNewSubjectPriceType();
                typeSingle.PriceTag = priceTag;
                typeSingle.PriceType = 1;
                typeSingle.IsDelete = 0;
                count = subject.CreateSWfsNewSubjectPriceType(typeSingle);
                if (count > 0)
                {
                    return Json(new { Ok = 1, TS = priceTag, Count = count });
                }
                else
                { return Json(new { Ok = 0, TS = priceTag, Count = count }); }
            }
            else
            { return Json(new { Ok = -1, TS = priceTag, Count = count }); }
        }
        #endregion



        #region 活动数据统计列表最新需求20140228 添加 lxy
        /// <summary>
        /// 活动数据统计列表最新需求20140228 添加 lxy
        /// </summary>
        /// <returns></returns>
        public ActionResult DataStatisticsNew()
        {
            string subjectNos = string.Empty;
            SubjectNewStatisticInfo newsInfo = new SubjectNewStatisticInfo();
            SubjectSaleStatistic saleSum = new SubjectSaleStatistic();
            SubjectVisitStatistic visitSum = new SubjectVisitStatistic();
            string txtSubjectNo = Rq.GetStringForm("subjectNo") ?? Rq.GetStringQueryString("subjectNo");
            string range = Rq.GetStringForm("range") ?? Rq.GetIntQueryString("range").ToString();
            string beginTime = Rq.GetStringForm("beginTime") ?? Rq.GetStringQueryString("beginTime");
            string endTime = Rq.GetStringForm("endTime") ?? Rq.GetStringQueryString("endTime");
            string beginTimeSubject = Rq.GetStringForm("beginTimeSubject") ?? Rq.GetStringQueryString("beginTimeSubject");
            string endTimeSubject= Rq.GetStringForm("endTimeSubject") ?? Rq.GetStringQueryString("endTimeSubject");
            string toexcel = Rq.GetStringQueryString("toexcel");
            if (!string.IsNullOrEmpty(txtSubjectNo))
            {
                  subjectNos = txtSubjectNo.Replace("\r\n", ",").Replace("\n\r", ",").Replace("\n", ",").Replace("\r", ",").TrimEnd(',').Replace(" ", ",");
            }
            List<SubjectNewStatistic> subjectNS = new List<SubjectNewStatistic>();
            IList<SWfsNewSubject> subjectList = subject.SelectSubjectList(beginTimeSubject, endTimeSubject, subjectNos, range);
            IList<ErpCategory> erpCategoryList = subject.GetERPCategoryListByParentNo("ROOT");

            if (subjectList != null)
            {
                foreach (var item in subjectList)
                {
                    SubjectNewStatistic statisticNew = new SubjectNewStatistic();
                    SubjectSaleStatistic sale = subject.GetSubjectSaleStatisticNew(item.SubjectNo, range, beginTime, endTime);
                    SubjectVisitStatistic visitStatistic = subject.GetSubjectVisitStatisticNew(item.SubjectNo, range, beginTime, endTime);
                    statisticNew.SaleStatistic = sale ?? new SubjectSaleStatistic();
                    statisticNew.VisitStatistic = visitStatistic ?? new SubjectVisitStatistic();
                    statisticNew.NewSubject = item;
                    subjectNS.Add(statisticNew);
                    if (sale != null)
                    {
                        saleSum.Amount = saleSum.Amount + sale.Amount;
                        saleSum.OrderNums = saleSum.OrderNums + sale.OrderNums;
                        saleSum.CostAmount = saleSum.CostAmount + sale.CostAmount;
                        saleSum.StockCount = saleSum.StockCount + sale.StockCount;
                        saleSum.SKUCount = saleSum.SKUCount + sale.SKUCount;
                        saleSum.SaleCount = saleSum.SaleCount + sale.SaleCount;
                        saleSum.SaledSKUCount = saleSum.SaledSKUCount + sale.SaledSKUCount;
                        saleSum.StockTotalAmount = saleSum.StockTotalAmount + sale.StockTotalAmount;
                    }
                    if (visitStatistic != null)
                    {
                        visitSum.PV = visitSum.PV + visitStatistic.PV;
                        visitSum.UV = visitSum.UV + visitStatistic.UV;
                        visitSum.VIP = visitSum.VIP + visitStatistic.VIP;
                        visitSum.GoldenVIP = visitSum.GoldenVIP + visitStatistic.GoldenVIP;
                        visitSum.PlatinaVIP = visitSum.PlatinaVIP + visitStatistic.PlatinaVIP;
                        visitSum.DiamondVIP = visitSum.DiamondVIP + visitStatistic.DiamondVIP;
                        visitSum.OrderNums = visitSum.OrderNums + visitStatistic.OrderNums;
                    }
                }

                newsInfo.SubjectNewStatisticList = subjectNS;
                newsInfo.SaleStatistic = saleSum;
                newsInfo.VisitStatistic = visitSum;
                newsInfo.ErpCategoryList = erpCategoryList;
                newsInfo.SubjectCount = subjectList.Count();
            }
            newsInfo.SubjectNos = subjectNos;
            newsInfo.Range = range;
            newsInfo.BeginTime = beginTime;
            newsInfo.EndTime = endTime;
            newsInfo.BeginTimeSubject = beginTimeSubject;
            newsInfo.EndTimeSubject = endTimeSubject;
                if (!string.IsNullOrEmpty(toexcel))
                {
                    switch (toexcel)
                    {
                        case "1":
                            subject.GetSubjectDataStatisticsToExcelNew(newsInfo);
                            break;
                    }
                } 
            return View(newsInfo); 
        }
        #endregion


        #region 删除无库存的商品信息
        /// <summary>
        /// 删除无库存的商品信息
        /// </summary>
        /// <param name="subjectNo"></param>
        /// <returns></returns>
        public JsonResult SubjectDeleteProductByQuantity()
        {
            string subjectNo = Request.Form["subjectNoD"];
            if (string.IsNullOrEmpty(subjectNo)) { return Json(new { result = "0", message = "请按对应的活动清除！" }); };
            SWfsNewSubjectService service = new SWfsNewSubjectService();
            SWfsNewProductService proService = new SWfsNewProductService();
            IList<string> list = service.GetSWfsNewSubjectProductRefListByDelete(subjectNo);
            int indexcount = 0;
            try
            { 
            if (list.Count() > 0)
            {
                int quantity = 0;
                foreach (var item in list)
                {
                    string [] strProduct = item.Split(',');
                    quantity = proService.GetInventoryByProductNo(strProduct[0].Trim()).SumQuantity;
                    if (quantity == 0)
                    {
                        string subjectProductRefId = strProduct[1].Trim();
                        service.DeleteSubjectProductRef(subjectProductRefId);
                        indexcount = indexcount + 1;
                    }
                } 
            }
            if (indexcount > 0)
            {
                return Json(new { result = "1", message = "清除成功（清除了" + indexcount + "个商品）" },JsonRequestBehavior.AllowGet);
            }
            else {
                return Json(new { result = "1", message = "不存在无库存商品" },JsonRequestBehavior.AllowGet);
            }
            }
            catch (Exception ex)
            {
                return Json(new { result = "0", message = "清除失败！" },JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
    }
}
