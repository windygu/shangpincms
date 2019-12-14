using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Shangpin.Ocs.Entity.Extenstion.ProductFlat;
using Shangpin.Ocs.Service.Shangpin.ProductSort;
using Shangpin.Entity.Wfs;
using Shangpin.Ocs.Service.ProductSort;
using Shangpin.Ocs.Entity.Extenstion.Login;
using Shangpin.Ocs.Service.Common;
using System.Text;
using Shangpin.Ocs.Entity.Extenstion.ShangPin.ProductFlat;

namespace Shangpin.Ocs.Web.Areas.ProductFlat.Controllers
{
    [OCSAuthorization]
    public class ProductController : Controller
    {

        #region 商品列表页搜索 加入排序
        //列表查询
        [ValidateInput(false)]
        public ActionResult ProductList(int pageIndex = 1, int pageSize = 10)
        {
            Parameters param = CreateParam(pageIndex, pageSize);
            if (Request.QueryString["IsRecord"] != null && Request.QueryString["IsRecord"] == "1")//说明是查询并保存
            {
                SWfsSortHistoryService sshsDal = new SWfsSortHistoryService();
                Passport passport = PresentationHelper.GetPassport();
                if (passport != null && !string.IsNullOrEmpty(passport.UserName))
                {
                    string thisUrl = Request.Url.ToString().Replace("http://" + Request.Url.Host, "").Replace("IsRecord", "IsRecoaddrd");
                    if (!string.IsNullOrEmpty(Request.QueryString["ProductName"]) && Request.QueryString["ProductName"] != "")
                    {
                        string ProductName = Server.UrlDecode(Request.QueryString["ProductName"]);
                        ProductName = ProductName.Replace(" ", "+");
                        thisUrl = thisUrl.Replace(ProductName, Server.UrlEncode(Request.QueryString["ProductName"]));
                    }
                    sshsDal.InsertHistory(param, thisUrl, passport.UserName);
                    return Redirect(thisUrl);
                }
            }

            SearchSortService dal = new SearchSortService();
            XMLReturnClassLists model = dal.GetLists(param);//接口查询 商品列表

            //按品牌编号查询修改热度的商品列表
            if (model.ListProducts.Count() > 0)
            {
                IEnumerable<ProductHot> hotProductList = dal.GetProductListByProductNoList(model.ListProducts.Select(p => p.ProductNo));
                for (int i = 0; i < model.ListProducts.Count; i++)
                {
                    if (hotProductList.Count(p => p.ProductNo == model.ListProducts[i].ProductNo) > 0)
                    {
                        model.ListProducts[i].EditeHot = model.ListProducts[i].Hot + hotProductList.Where(p => p.ProductNo == model.ListProducts[i].ProductNo).ElementAt(0).ProductHotValue;
                        model.ListProducts[i].EditeSevenHot = model.ListProducts[i].SevenHot + hotProductList.Where(p => p.ProductNo == model.ListProducts[i].ProductNo).ElementAt(0).ProductSevenHotValue;
                    }
                    else
                    {
                        model.ListProducts[i].EditeHot = model.ListProducts[i].Hot;
                        model.ListProducts[i].EditeSevenHot = model.ListProducts[i].SevenHot;
                    }
                }
            }

            ProductSortService tian = new ProductSortService();
            string categoryNo = string.Empty;
            if (!string.IsNullOrEmpty(param.categoryNO))
            {
                categoryNo = param.categoryNO;
            }
            else
            {
                categoryNo = param.brandNO;
            }
            IEnumerable<SWfsSortProduct> saveProductList = tian.GetSortProductList(categoryNo);
            if (saveProductList.Count() > 0)
            {
                for (int i = 0; i < model.ListProducts.Count; i++)//对比商品是否已经加入排序池
                {
                    if (saveProductList.Count(q => q.ProductNo == model.ListProducts.ElementAt(i).ProductNo) > 0)
                    {
                        model.ListProducts.ElementAt(i).IsSelected = 1;
                    }
                }
            }
            List<SearchResultCategorys> result = new List<SearchResultCategorys>();
            //分类递归
            if (!string.IsNullOrEmpty(Request.QueryString["categoryNO"]) && Request.QueryString["categoryNO"].Length >= 3)
            {
                List<SearchResultCategorys> first = model.ListCategorys.Where(p => p.CategoryNo.Contains(Request.QueryString["categoryNO"]) && p.CategoryNo.Length == (Request.QueryString["categoryNO"].Length + 3)).ToList();
                List<SearchResultCategorys> second;
                List<SearchResultCategorys> third;
                for (int i = 0; i < first.Count; i++)
                {
                    result.Add(first[i]);
                    second = model.ListCategorys.Where(p => p.PrentNo == first[i].CategoryNo).ToList();
                    for (int j = 0; j < second.Count; j++)
                    {
                        second[j].CateGoryName = "|-" + second[j].CateGoryName;
                        result.Add(second[j]);
                        third = model.ListCategorys.Where(p => p.PrentNo == second[j].CategoryNo).ToList();
                        for (int k = 0; k < third.Count; k++)
                        {
                            third[k].CateGoryName = "|-|-" + third[k].CateGoryName;
                            result.Add(third[k]);
                        }
                    }
                }
                first = null;
                second = null;
                third = null;
            }
            else
            {
                List<SearchResultCategorys> first = model.ListCategorys.Where(p => p.CateGoryLevel == 2).ToList();
                List<SearchResultCategorys> second;
                List<SearchResultCategorys> third;
                for (int i = 0; i < first.Count; i++)
                {
                    result.Add(first[i]);
                    second = model.ListCategorys.Where(p => p.PrentNo == first[i].CategoryNo).ToList();
                    for (int j = 0; j < second.Count; j++)
                    {
                        second[j].CateGoryName = "|-" + second[j].CateGoryName;
                        result.Add(second[j]);
                        third = model.ListCategorys.Where(p => p.PrentNo == second[j].CategoryNo).ToList();
                        for (int k = 0; k < third.Count; k++)
                        {
                            third[k].CateGoryName = "|-|-" + third[k].CateGoryName;
                            result.Add(third[k]);
                        }
                    }
                }
                first = null;
                second = null;
                third = null;
            }
            model.ListCategorys = result;
            model.SaveProductCount = saveProductList.Count();
            SWfsSortHistoryService HistoryDal = new SWfsSortHistoryService();
            IList<SWfsSortHistory> listHistory = HistoryDal.SelectHistory();
            ViewBag.listHistory = listHistory;
            return View(model);
        }
        //根据搜索条件生成搜索参数
        private Parameters CreateParam(int pageIndex, int pageSize)
        {
            Parameters p = new Parameters();
            if (Request.QueryString["childCategoryNO"] != null && Request.QueryString["childCategoryNO"] != "0")
            {
                p.categoryNO = Request.QueryString["childCategoryNO"].ToUpper();
                p.OCSCategoryName = Server.UrlDecode(Request.QueryString["childCategoryname"]);
                //p.OCSCategoryName = Request.QueryString["childCategoryname"];
            }
            else
            {
                if (Request.QueryString["categoryno"] != null)
                {
                    p.categoryNO = Request.QueryString["categoryno"].ToUpper();
                    p.OCSCategoryName = Server.UrlDecode(Request.QueryString["childCategoryname"]);
                    //p.OCSCategoryName = Request.QueryString["ocscategoryname"];
                }
            }
            p.start = ((pageIndex - 1) * pageSize + 1) + "";
            p.end = (pageIndex * pageSize) + "";
            if (!string.IsNullOrEmpty(Request.QueryString["ProductNo"]))
            {
                p.productNO = Request.QueryString["ProductNo"];
            }
            if (!string.IsNullOrEmpty(Request.QueryString["brandNO"]) && Request.QueryString["brandNO"] != "0")
            {
                p.brandNO = Request.QueryString["brandNO"].ToUpper();
                p.BrandName = Server.UrlDecode(Request.QueryString["brandName"]);
                //p.BrandName = Request.QueryString["brandName"];
            }
            if (!string.IsNullOrEmpty(Request.QueryString["colorId"]) && Request.QueryString["colorId"] != "0")
            {
                p.colorId = Request.QueryString["colorId"].ToUpper();
                p.ColorName = Server.UrlDecode(Request.QueryString["colorName"]);
                //p.ColorName = Request.QueryString["colorName"];
            }
            if (!string.IsNullOrEmpty(Request.QueryString["ProductName"]))
            {
                p.productName = Server.UrlEncode(Request.QueryString["ProductName"]);
                //p.productName = Server.UrlDecode(Request.QueryString["ProductName"]);
                //p.productName = Url.Encode(Request.QueryString["ProductName"]);
            }
            if (Request.QueryString["shelfType"] == "0")
            {
                p.shelfType = "0";
                if (Request.QueryString["categoryType"] == "1")
                {
                    ProductRulesService prs = new ProductRulesService();
                    SWfsSortOcsCategory ocsCategory = prs.IsRuleCategory(Request.QueryString["categoryno"]);//用分类的ID查询
                    if (ocsCategory != null && ocsCategory.DateUpdate.ToString("yyyy-MM-dd") != "1900-01-01")
                    {
                        p.shelfDate = ocsCategory.DateUpdate.ToString("yyyy-MM-dd HH:mm:ss") + "~" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                }
                else if (Request.QueryString["categoryType"] == "2")
                {
                    ProductRulesService prs = new ProductRulesService();
                    SWfsSortOcsCategory ocsCategory = prs.IsRuleCategory(Request.QueryString["BrandNO"]);//用品牌ID查询
                    if (ocsCategory != null && ocsCategory.DateUpdate.ToString("yyyy-MM-dd") != "1900-01-01")
                    {
                        p.shelfDate = ocsCategory.DateUpdate.ToString("yyyy-MM-dd HH:mm:ss") + "~" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                }

            }
            else if (Request.QueryString["shelfType"] == "1")
            {
                if (!string.IsNullOrEmpty(Request.QueryString["shelfDateStart"]) && !string.IsNullOrEmpty(Request.QueryString["shelfDateEnd"]))
                {
                    p.shelfType = "1";
                    p.shelfDate = Request.QueryString["shelfDateStart"] + "~" + Request.QueryString["shelfDateEnd"];
                    p.StartShelfDate = Request.QueryString["shelfDateStart"];
                    p.EndShelfDate = Request.QueryString["shelfDateEnd"];
                }
            }
            if (!string.IsNullOrWhiteSpace(Request.QueryString["postArea"]))
            {
                p.postArea = Request.QueryString["postArea"];
            }
            if (!string.IsNullOrEmpty(Request.QueryString["MinPrice"]))
            {
                p.price = Request.QueryString["MinPrice"] + "~" + (string.IsNullOrEmpty(Request.QueryString["MaxPrice"]) ? "10000000" : Request.QueryString["MaxPrice"]);
                p.StartPrice = Request.QueryString["MinPrice"];
                p.EndPrice = string.IsNullOrEmpty(Request.QueryString["MaxPrice"]) ? "10000000" : Request.QueryString["MaxPrice"];
            }
            else if (!string.IsNullOrEmpty(Request.QueryString["MaxPrice"]))
            {
                p.price = "0" + "~" + Request.QueryString["MaxPrice"];
                p.StartPrice = "0";
                p.EndPrice = Request.QueryString["MaxPrice"];
            }
            if (!string.IsNullOrEmpty(Request.QueryString["MinStock"]))
            {
                p.stock = Request.QueryString["MinStock"] + "~" + (string.IsNullOrEmpty(Request.QueryString["MaxStock"]) ? "100000" : Request.QueryString["MaxStock"]);
                p.StartStock = Request.QueryString["MinStock"];
                p.EndStock = string.IsNullOrEmpty(Request.QueryString["MaxStock"]) ? "100000" : Request.QueryString["MaxStock"];
            }
            else if (!string.IsNullOrEmpty(Request.QueryString["MaxStock"]))
            {
                p.stock = "0" + "~" + Request.QueryString["MaxStock"];
                p.StartStock = "0";
                p.EndStock = Request.QueryString["MaxStock"];
            }
            if (!string.IsNullOrEmpty(Request.QueryString["MinDiscountRate"]))
            {
                p.discountRate = Request.QueryString["MinDiscountRate"] + "~" + (string.IsNullOrEmpty(Request.QueryString["MaxDiscountRate"]) ? "100000" : Request.QueryString["MaxDiscountRate"]);
                p.StartDiscountRate = Request.QueryString["MinDiscountRate"];
                p.EndDiscountRate = string.IsNullOrEmpty(Request.QueryString["MaxDiscountRate"]) ? "100000" : Request.QueryString["MaxDiscountRate"];
            }
            else if (!string.IsNullOrEmpty(Request.QueryString["MaxDiscountRate"]))
            {
                p.discountRate = "0" + "~" + Request.QueryString["MaxDiscountRate"];
                p.StartDiscountRate = "0";
                p.EndDiscountRate = Request.QueryString["MaxDiscountRate"];
            }

            if (Request.QueryString["hotType"] == "0")
            {
                if (!string.IsNullOrEmpty(Request.QueryString["minHot"]))
                {
                    p.hot = Request.QueryString["minHot"] + "~" + (string.IsNullOrEmpty(Request.QueryString["maxHot"]) ? "10000000" : Request.QueryString["maxHot"]);
                }
                else if (!string.IsNullOrEmpty(Request.QueryString["maxHot"]))
                {
                    p.hot = "0"+ "~" + Request.QueryString["maxHot"];
                }
            }
            

            if (Request.QueryString["hotType"] == "1")
            {
                if (!string.IsNullOrEmpty(Request.QueryString["minHot"]))
                {
                    p.sevenHot = Request.QueryString["minHot"] + "~" + (string.IsNullOrEmpty(Request.QueryString["maxHot"]) ? "10000000" : Request.QueryString["maxHot"]);
                }
                else if (!string.IsNullOrEmpty(Request.QueryString["maxHot"]))
                {
                    p.sevenHot = "0" + "~" + Request.QueryString["maxHot"];
                }
            }

            return p;
        }
        //加入排序
        [HttpPost]
        public ActionResult ProductList()
        {
            //数据验证
            Parameters param = new Parameters();
            param.productNO = Request.Form["productNo"];
            if (string.IsNullOrEmpty(Request.Form["categoryNo"]))
            {
                TempData["tip"] = "<script>alert('商品分类不存在')</script>";
                return Redirect("/productflat/product/ProductList?categoryNO=" + Request.QueryString["categoryNO"] + "&ProductNo=" + Request.QueryString["ProductNo"] + "&ProductName=" + Request.QueryString["ProductName"] + "&brandNO=" + Request.QueryString["brandNO"] + "&colorId=" + Request.QueryString["colorId"] + "&shelfType=" + Request.QueryString["shelfType"] + "&ShelfDate=" + Request.QueryString["ShelfDate"] + "&MinPrice=" + Request.QueryString["MinPrice"] + "&MaxPrice=" + Request.QueryString["MaxPrice"] + "&MinStock=" + Request.QueryString["MinStock"] + "&MaxStock=" + Request.QueryString["MaxStock"] + "&MinDiscountRate=" + Request.QueryString["MinDiscountRate"] + "&MaxDiscountRate=" + Request.QueryString["MaxDiscountRate"]);
            }
            if (string.IsNullOrEmpty(param.productNO))
            {
                TempData["tip"] = "<script>alert('请选择加入排序的商品')</script>";
                return Redirect("/productflat/product/ProductList?categoryNO=" + Request.QueryString["categoryNO"] + "&ProductNo=" + Request.QueryString["ProductNo"] + "&ProductName=" + Request.QueryString["ProductName"] + "&brandNO=" + Request.QueryString["brandNO"] + "&colorId=" + Request.QueryString["colorId"] + "&shelfType=" + Request.QueryString["shelfType"] + "&ShelfDate=" + Request.QueryString["ShelfDate"] + "&MinPrice=" + Request.QueryString["MinPrice"] + "&MaxPrice=" + Request.QueryString["MaxPrice"] + "&MinStock=" + Request.QueryString["MinStock"] + "&MaxStock=" + Request.QueryString["MaxStock"] + "&MinDiscountRate=" + Request.QueryString["MinDiscountRate"] + "&MaxDiscountRate=" + Request.QueryString["MaxDiscountRate"]);
            }
            ProductSortService tian = new ProductSortService();
            List<SortProduct> searchProductList = tian.GetXMLProductByProductNo(param);
            ProductRulesService zhao = new ProductRulesService();
            for (int i = 0; i < searchProductList.Count; i++)
            {
                //searchProductList.ElementAt(i).RuleType
                zhao.AddProductToSort(Request.Form["categoryNo"], "", Request.QueryString["categoryType"], searchProductList.ElementAt(i));
                //zhao.AddProductToSort(Request.Form["categoryNo"], Request.Form["categoryName"], searchProductList.ElementAt(i));
            }
            TempData["tip"] = "<script>alert('操作成功')</script>";
            return Redirect("/productflat/product/ProductList?categoryNO=" + Request.QueryString["categoryNO"] + "&ProductNo=" + Request.QueryString["ProductNo"] + "&ProductName=" + Request.QueryString["ProductName"] + "&brandNO=" + Request.QueryString["brandNO"] + "&colorId=" + Request.QueryString["colorId"] + "&shelfType=" + Request.QueryString["shelfType"] + "&ShelfDate=" + Request.QueryString["ShelfDate"] + "&MinPrice=" + Request.QueryString["MinPrice"] + "&MaxPrice=" + Request.QueryString["MaxPrice"] + "&MinStock=" + Request.QueryString["MinStock"] + "&MaxStock=" + Request.QueryString["MaxStock"] + "&MinDiscountRate=" + Request.QueryString["MinDiscountRate"] + "&MaxDiscountRate=" + Request.QueryString["MaxDiscountRate"] + "&pageindex=" + Request.QueryString["pageindex"] + "&categoryType=" + Request.QueryString["categoryType"] + "&isLast=" + Request.QueryString["isLast"] + "&CategoryPath=" + Request.QueryString["CategoryPath"]);
        }
        //验证是否已经加入
        public ActionResult AddProductAjax()
        {
            Parameters param = new Parameters();
            param.productNO = Request.Form["productNo"];
            if (string.IsNullOrEmpty(Request.Form["categoryNo"]))
            {
                return Json(new
                {
                    error = 1,
                    msg = "商品分类不存在"
                });
            }
            if (string.IsNullOrEmpty(param.productNO))
            {
                return Json(new
                {
                    error = 1,
                    msg = "商品编号不存在"
                });
            }
            if (string.IsNullOrEmpty(Request.Form["categoryType"]))
            {
                return Json(new
                {
                    error = 1,
                    msg = "请选择左侧的分类或则品牌"
                });
            }
            ProductSortService tian = new ProductSortService();
            List<SortProduct> searchProductList = tian.GetXMLProductByProductNo(param);
            SWfsSortProduct saveProductObj = tian.GetSortProductByProductNo(Request.Form["productNo"], Request.Form["categoryNo"]);
            if (saveProductObj != null)
            {
                if (searchProductList.Count(p => p.ProductNo == saveProductObj.ProductNo) > 0)
                {
                    return Json(new
                    {
                        error = 1,
                        msg = "该商品已加入排序"
                    });
                }
            }
            ProductRulesService zhao = new ProductRulesService();
            for (int i = 0; i < searchProductList.Count; i++)
            {
                zhao.AddProductToSort(Request.Form["categoryNo"].ToUpper(), "", Request.Form["categoryType"], searchProductList.ElementAt(i));
            }
            return Json(new
            {
                error = 0,
                msg = "加入成功"
            });
        }
        //分类树形结构
        public ActionResult GetErpCategoryChildList(string ParentNo)
        {
            SWfsCategoryService dal = new SWfsCategoryService();
            IList<OCSInfo> CategoryList = dal.SelectCategoryByParentNo(ParentNo);
            return Json(CategoryList);
        }
        #endregion

        #region 可视化排序
        public ViewResult ViewPageInfo(string productStr)
        {
            return View();
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="ProductListView"></param>
        /// <returns></returns>
        [HttpPost]
        public string SaveProductView(List<ProductView> ProductListView)
        {
            string result = "0";
            Session["ProductListView"] = ProductListView;
            result = "1";
            return result;
        }

        //排序页面
        public ActionResult SortProductIndex(string allCount)
        {
            string categoryNo = "";
            if (Request.QueryString["categoryNO"] != null && Request.QueryString["categoryNO"] != "")
            {
                categoryNo = Request.QueryString["categoryNO"].ToUpper();
            }
            else
            {
                if (Request.QueryString["BrandNO"] != null && Request.QueryString["BrandNO"] != "")
                {
                    categoryNo = Request.QueryString["BrandNO"].ToUpper();
                }
            }
            ProductSortService SortService = new ProductSortService();
            SearchProductInfo PageData = SortService.GetSortResult(categoryNo, Request.QueryString["categoryType"]);//先用模拟数据
            ViewBag.CategoryNo = categoryNo;
            ViewBag.AllCount = allCount;
            return View(PageData);

        }
        //保存排序
        [HttpPost]
        public string SaveProductSort(PageDateInfo PageDateList)
        {
            PageDateList.CategoryNo = PageDateList.CategoryNo;
            ProductRulesService PsortService = new ProductRulesService();
            string result = "0";
            //没有执行规则
            int sortInt = 0;
            if (PageDateList.ProductList != null)
            {
                sortInt = sortInt + PageDateList.ProductList.Count();
            }
            if (PageDateList.RuleList != null)
            {
                for (int i = 0; i < PageDateList.RuleList.Count(); i++)
                {
                    if (PageDateList.RuleList[i].ProductList != null)
                    {
                        sortInt = sortInt + PageDateList.RuleList[i].ProductList.Count();
                    }
                    if (PageDateList.RuleList[i].RuleList != null)
                    {
                        for (int k = 0; k < PageDateList.RuleList[i].RuleList.Count(); k++)
                        {
                            if (PageDateList.RuleList[i].RuleList[k].ProductList != null)
                            {
                                sortInt = sortInt + PageDateList.RuleList[i].RuleList[k].ProductList.Count();
                            }
                        }

                    }

                }
            }

            if (PageDateList.IsRule == "0")
            {
                PsortService.DelSortRuleByCategoryNo(PageDateList.CategoryNo);
                PsortService.DelProductByCategoryNo(PageDateList.CategoryNo);
                PsortService.UpdateCateogryTime(PageDateList.CategoryNo, PageDateList.IsLast);
                if (PageDateList.ProductList != null)
                {
                    for (int i = 0; i < PageDateList.ProductList.Count(); i++)
                    {
                        SWfsSortProduct pdto = new SWfsSortProduct();
                        pdto.Sort = sortInt;
                        pdto.RuleId = -1;
                        pdto.ProductNo = PageDateList.ProductList[i].ProductNo;
                        pdto.DateCreate = DateTime.Now;
                        pdto.OcsCategoryNo = PageDateList.CategoryNo;// PageDateList.productlist[i].ocscategoryno;
                        PsortService.AddProductToSortSingle(pdto);
                        sortInt = sortInt - 1;
                    }
                }
            }
            else
            {
                PsortService.DelSortRuleByCategoryNo(PageDateList.CategoryNo);
                PsortService.DelProductByCategoryNo(PageDateList.CategoryNo);
                PsortService.UpdateCateogryTime(PageDateList.CategoryNo, PageDateList.IsLast);
                string parentid = "0";
                if (PageDateList.RuleList != null)
                {
                    for (int i = 0; i < PageDateList.RuleList.Count(); i++)
                    {
                        SWfsSortRule pruledto = new SWfsSortRule();
                        pruledto.OcsCategoryNo = PageDateList.CategoryNo;
                        pruledto.ParentId = 0;
                        pruledto.RuleObjectName = PageDateList.RuleList[i].RuleName;
                        pruledto.RuleObjectNo = PageDateList.RuleList[i].RuleNo;
                        pruledto.RuleType = Convert.ToInt16(PageDateList.RuleList[i].RuleType);
                        pruledto.Sort = PageDateList.RuleList.Count() - i;
                        parentid = PsortService.AddRuleToSort(pruledto);
                        if (PageDateList.RuleList[i].RuleList != null)
                        {
                            for (int j = 0; j < PageDateList.RuleList[i].RuleList.Count(); j++)
                            {
                                SWfsSortRule ruledto = new SWfsSortRule();
                                ruledto.OcsCategoryNo = PageDateList.CategoryNo;// PageDateList.categoryno;
                                ruledto.ParentId = Convert.ToInt32(parentid);
                                ruledto.RuleObjectName = PageDateList.RuleList[i].RuleList[j].RuleName;
                                ruledto.RuleObjectNo = PageDateList.RuleList[i].RuleList[j].RuleNo;
                                ruledto.RuleType = Convert.ToInt16(PageDateList.RuleList[i].RuleList[j].RuleType);
                                ruledto.Sort = PageDateList.RuleList[i].RuleList.Count() - j;
                                string ruleid = PsortService.AddRuleToSort(ruledto);

                                if (PageDateList.RuleList[i].RuleList[j].ProductList != null)
                                {
                                    for (int k = 0; k < PageDateList.RuleList[i].RuleList[j].ProductList.Count(); k++)
                                    {
                                        SWfsSortProduct ptdto = new SWfsSortProduct();
                                        ptdto.Sort = sortInt;
                                        ptdto.RuleId = Convert.ToInt32(ruleid);
                                        ptdto.ProductNo = PageDateList.RuleList[i].RuleList[j].ProductList[k].ProductNo;
                                        ptdto.DateCreate = DateTime.Now;
                                        ptdto.OcsCategoryNo = PageDateList.CategoryNo;// PageDateList.rulelist[i].rulelist[k].productlist[k].ocscategoryno;
                                        PsortService.AddProductToSortSingle(ptdto);
                                        sortInt = sortInt - 1;
                                    }
                                }
                            }
                        }
                        if (PageDateList.RuleList[i].ProductList != null)
                        {
                            for (int l = 0; l < PageDateList.RuleList[i].ProductList.Count(); l++)
                            {
                                SWfsSortProduct podto = new SWfsSortProduct();
                                podto.Sort = sortInt;
                                podto.RuleId = Convert.ToInt32(parentid);
                                podto.ProductNo = PageDateList.RuleList[i].ProductList[l].ProductNo;
                                podto.OcsCategoryNo = PageDateList.CategoryNo;// PageDateList.rulelist[i].productlist[l].ocscategoryno;
                                podto.DateCreate = DateTime.Now;
                                PsortService.AddProductToSortSingle(podto);
                                sortInt = sortInt - 1;
                            }
                        }
                        if (PageDateList.ProductList != null)
                        {
                            for (int m = 0; m < PageDateList.ProductList.Count(); m++)
                            {
                                SWfsSortProduct dto = new SWfsSortProduct();
                                dto.Sort = sortInt;
                                dto.RuleId = 0;
                                dto.ProductNo = PageDateList.ProductList[m].ProductNo;
                                dto.OcsCategoryNo = PageDateList.CategoryNo;// PageDateList.productlist[m].ocscategoryno;
                                dto.DateCreate = DateTime.Now;
                                PsortService.AddProductToSortSingle(dto);
                                sortInt = sortInt - 1;
                            }
                        }
                    }
                }
            }
            return result;

        }

        /// <summary>
        /// 执行清空所有商品
        /// </summary>
        /// <param name="CategoryNo"></param>
        /// <returns></returns>
        [HttpGet]
        public string ClearProductListByCategory(string CategoryNo)
        {
            string result = "";
            ProductRulesService SortService = new ProductRulesService();
            result = SortService.DelProductByCategoryNo(CategoryNo) + "";
            result = SortService.DelSortRuleByCategoryNo(CategoryNo) + "";
            return result;
        }

        //清除历史
        public void ClearSortHistory()
        {
            Passport passport = PresentationHelper.GetPassport();
            if (passport != null)
            {
                new SWfsSortHistoryService().ClearHistory(passport.UserName);
            }
        }
        #endregion

        #region App商品热度推荐
        public ActionResult AppReCommendProduct(int pageIndex=1,int pageSize=10) 
        {
            Parameters param = CreateParam(pageIndex, pageSize);
            if (Request.QueryString["IsCheckProduct"] != null && Request.QueryString["IsCheckProduct"] == "1")//剔除下架商品
            {
                
            }
            SearchSortService dal = new SearchSortService();
            XMLReturnClassLists model = dal.GetLists(param);//接口查询 商品列表

            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;
            return View(model);
        }
        #endregion

        #region TopShop热度排序
        [ValidateInput(false)]
        public ActionResult TopShopProductList(int pageIndex = 1, int pageSize = 10)
        {
            Parameters param = CreateParam(pageIndex, pageSize);
            if (Request.QueryString["IsRecord"] != null && Request.QueryString["IsRecord"] == "1")//说明是查询并保存
            {
                SWfsSortHistoryService sshsDal = new SWfsSortHistoryService();
                Passport passport = PresentationHelper.GetPassport();
                if (passport != null && !string.IsNullOrEmpty(passport.UserName))
                {
                    string thisUrl = Request.Url.ToString().Replace("http://" + Request.Url.Host, "").Replace("IsRecord", "IsRecoaddrd");
                    if (!string.IsNullOrEmpty(Request.QueryString["ProductName"]) && Request.QueryString["ProductName"] != "")
                    {
                        string ProductName = Server.UrlDecode(Request.QueryString["ProductName"]);
                        ProductName = ProductName.Replace(" ", "+");
                        thisUrl = thisUrl.Replace(ProductName, Server.UrlEncode(Request.QueryString["ProductName"]));
                    }
                    sshsDal.InsertHistory(param, thisUrl, passport.UserName);
                    return Redirect(thisUrl);
                }
            }

            SearchSortService dal = new SearchSortService();
            //param.brandNO = "B1885";
            XMLReturnClassLists model = dal.GetTopShopLists(param);//接口查询 商品列表

            //按品牌编号查询修改热度的商品列表
            if (model.ListProducts.Count()>0)
            {
                IEnumerable<ProductHot> hotProductList= dal.GetProductListByProductNoList(model.ListProducts.Select(p=>p.ProductNo));
                for (int i = 0; i < model.ListProducts.Count; i++)
                {
                    if (hotProductList.Count(p=>p.ProductNo==model.ListProducts[i].ProductNo)>0)
                    {
                        model.ListProducts[i].EditeHot = model.ListProducts[i].Hot + hotProductList.Where(p => p.ProductNo == model.ListProducts[i].ProductNo).ElementAt(0).ProductHotValue;
                        model.ListProducts[i].EditeSevenHot = model.ListProducts[i].SevenHot + hotProductList.Where(p => p.ProductNo == model.ListProducts[i].ProductNo).ElementAt(0).ProductSevenHotValue;
                    }
                    else
                    {
                        model.ListProducts[i].EditeHot = model.ListProducts[i].Hot ;
                        model.ListProducts[i].EditeSevenHot = model.ListProducts[i].SevenHot ;
                    }
                }
            }

            ProductSortService tian = new ProductSortService();
            string categoryNo = string.Empty;
            if (!string.IsNullOrEmpty(param.categoryNO))
            {
                categoryNo = param.categoryNO;
            }
            else
            {
                categoryNo = param.brandNO;
            }
            //IEnumerable<SWfsSortProduct> saveProductList = tian.GetSortProductList(categoryNo);
            //if (saveProductList.Count() > 0)
            //{
            //    for (int i = 0; i < model.ListProducts.Count; i++)//对比商品是否已经加入排序池
            //    {
            //        if (saveProductList.Count(q => q.ProductNo == model.ListProducts.ElementAt(i).ProductNo) > 0)
            //        {
            //            model.ListProducts.ElementAt(i).IsSelected = 1;
            //        }
            //    }
            //}
            List<SearchResultCategorys> result = new List<SearchResultCategorys>();
            //分类递归
            if (!string.IsNullOrEmpty(Request.QueryString["categoryNO"]) && Request.QueryString["categoryNO"].Length >= 3)
            {
                List<SearchResultCategorys> first = model.ListCategorys.Where(p => p.CategoryNo.Contains(Request.QueryString["categoryNO"]) && p.CategoryNo.Length == (Request.QueryString["categoryNO"].Length + 3)).ToList();
                List<SearchResultCategorys> second;
                List<SearchResultCategorys> third;
                for (int i = 0; i < first.Count; i++)
                {
                    result.Add(first[i]);
                    second = model.ListCategorys.Where(p => p.PrentNo == first[i].CategoryNo).ToList();
                    for (int j = 0; j < second.Count; j++)
                    {
                        second[j].CateGoryName = "|-" + second[j].CateGoryName;
                        result.Add(second[j]);
                        third = model.ListCategorys.Where(p => p.PrentNo == second[j].CategoryNo).ToList();
                        for (int k = 0; k < third.Count; k++)
                        {
                            third[k].CateGoryName = "|-|-" + third[k].CateGoryName;
                            result.Add(third[k]);
                        }
                    }
                }
                first = null;
                second = null;
                third = null;
            }
            else
            {
                List<SearchResultCategorys> first = model.ListCategorys.Where(p => p.CateGoryLevel == 2).ToList();
                List<SearchResultCategorys> second;
                List<SearchResultCategorys> third;
                for (int i = 0; i < first.Count; i++)
                {
                    result.Add(first[i]);
                    second = model.ListCategorys.Where(p => p.PrentNo == first[i].CategoryNo).ToList();
                    for (int j = 0; j < second.Count; j++)
                    {
                        second[j].CateGoryName = "|-" + second[j].CateGoryName;
                        result.Add(second[j]);
                        third = model.ListCategorys.Where(p => p.PrentNo == second[j].CategoryNo).ToList();
                        for (int k = 0; k < third.Count; k++)
                        {
                            third[k].CateGoryName = "|-|-" + third[k].CateGoryName;
                            result.Add(third[k]);
                        }
                    }
                }
                first = null;
                second = null;
                third = null;
            }
            model.ListCategorys = result;
            //model.SaveProductCount = saveProductList.Count();
            SWfsSortHistoryService HistoryDal = new SWfsSortHistoryService();
            IList<SWfsSortHistory> listHistory = HistoryDal.SelectHistory();
            ViewBag.listHistory = listHistory;
            return View(model);
        }
        //修改热度
        public int EditeHotValue(string productNo, int hotValue, int sevenHotValue, int type)
        {
            SearchSortService dal = new SearchSortService();
            return dal.EditeHotValue(productNo, hotValue, sevenHotValue, type);
        }
        #endregion
    }
}



// GET: /ProductFlat/Product/

//#region  解析客户端请求参数
///// <summary>
///// 解析客户端请求的加入排序参数
///// </summary>
///// <param name="form"></param>
///// <returns></returns>
//protected SortProduct[] ParseSortProduct(FormCollection form)
//{
//    string productNo = form["productNo"].ToString();
//    string brandNo = form["brandNo"].ToString();
//    string colorNo = form["colorNo"].ToString();
//    string categoryNo = form["categoryNo"].ToString();
//    string limitedPrice = form["limitedPrice"].ToString();
//    string[] arrProductNo = productNo.Split(',');
//    string[] arrBrandNo = brandNo.Split(',');
//    string[] arrColorNo = colorNo.Split(',');
//    string[] arrCategoryNo = categoryNo.Split(',');
//    string[] arrLimitedPrice = limitedPrice.Split(',');
//    int count = arrProductNo.Length;
//    SortProduct[] list = new SortProduct[count];
//    for (int i = 0; i < list.Length; i++)
//    {
//        list[i] = new SortProduct
//        {
//            ProductNo = arrProductNo[i],
//            BrandNo = arrBrandNo[i],
//            LimitedPrice = Convert.ToDecimal(arrLimitedPrice[i]),
//            //如果为空，则用空值代替
//            ProductPrimaryColorNO = arrColorNo[i] == SortProductEmpty ? "" : arrColorNo[i],
//            CategoryNo = arrCategoryNo[i] == SortProductEmpty ? "" : arrCategoryNo[i]
//        };
//    }
//    return list;
//}

///// <summary>
///// 解析客户端请求的查询参数
///// </summary>
///// <returns></returns>
//protected Parameters ParseSearchPara()
//{
//    Parameters p = new Parameters();
//    if (!string.IsNullOrEmpty(Request.QueryString["categoryno"]))
//    {
//        p.categoryNO = Request.QueryString["categoryno"].ToString();
//    }
//    if (!string.IsNullOrEmpty(Request.QueryString["OCSCategoryName"]))
//    {
//        p.OCSCategoryName = Request.QueryString["OCSCategoryName"].ToString();
//    }
//    if (!string.IsNullOrEmpty(Request.QueryString["BrandNo"]))
//    {
//        p.brandNO = Request.QueryString["BrandNo"].ToString();
//    }
//    if (!string.IsNullOrEmpty(Request.QueryString["SubType"]))
//    {
//        p.categoryNO = Request.QueryString["SubType"].ToString();
//    }
//    if (!string.IsNullOrEmpty(Request.QueryString["ColorNo"]))
//    {
//        p.colorId = Request.QueryString["ColorNo"].ToString();
//    }
//    if (!string.IsNullOrEmpty(Request.QueryString["DateShelf"]))
//    {
//        p.shelfDate = Request.QueryString["DateShelf"].ToString();
//    }
//    if (!string.IsNullOrEmpty(Request.QueryString["Price"]))
//    {
//        p.price = Request.QueryString["Price"].ToString();
//    }
//    if (!string.IsNullOrEmpty(Request.QueryString["Stock"]))
//    {
//        p.stock = Request.QueryString["Stock"].ToString();
//    }
//    if (!string.IsNullOrEmpty(Request.QueryString["Rate"]))
//    {
//        p.discountRate = Request.QueryString["Rate"].ToString();
//    }
//    if (!string.IsNullOrEmpty(Request.QueryString["pageindex"]))
//    {
//        int index = Convert.ToInt32(Request.QueryString["pageindex"].ToString());
//        int start = (index - 1) * SearchPageSize + 1;
//        p.start = start.ToString();
//        p.end = (start + SearchPageSize - 1).ToString();
//    }

//    if (string.IsNullOrEmpty(p.categoryNO))
//    {
//        p.categoryNO = "A01";
//        p.OCSCategoryName = "女性";
//    }
//    if (string.IsNullOrEmpty(p.start))
//    {
//        p.start = "1";
//        p.end = SearchPageSize.ToString();
//    }
//    return p;
//}
//#endregion

//public ActionResult Index()
//{
//    Parameters p = ParseSearchPara();
//    SearchSortService dal = new SearchSortService();
//    XMLReturnClassLists model = dal.GetLists(p);
//    List<SWfsSortProduct> sortedProduct = dal.GetSortedProduct(p.categoryNO);//查找已排序产品信息
//    model.SortedProductInfo = dal.ListToJsonString(sortedProduct);//设置已排序产品编号
//    return View(model);
//}

///// <summary>
///// 产品加入排序
///// </summary>
///// <param name="form"></param>
///// <returns></returns>
//[HttpPost]
//public ActionResult DoJoinToSort(FormCollection form)
//{
//    string ocsCategoryNo = form["OCSCategoryNo"].ToString();
//    string ocsCategoryName = form["OCSCategoryName"].ToString();
//    SortProduct[] list = ParseSortProduct(form);
//    ProductRulesService prs = new ProductRulesService();
//    SearchSortService searchDal = new SearchSortService();
//    string result = "0";
//    string message = "加入排序成功";
//    for (int i = 0; i < list.Length; i++)
//    {
//        //检查重复
//        if (searchDal.GetSortProductCount(ocsCategoryNo, list[i].ProductNo) == 0)
//        {
//            int n = prs.AddProductToSort(ocsCategoryNo, ocsCategoryName, list[i]);
//            if (n == 0)
//            {
//                result = "1";
//                message = "加入排序失败";
//            }
//        }
//        else
//        {
//            result = "1";
//            message = "加入排序失败,该产品已加入排序";
//        }
//    }
//    return Json(new { result = result, message = message });
//}
