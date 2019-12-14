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
using Shangpin.Ocs.Service.Shangpin;
using Shangpin.Ocs.Entity.Extenstion.ShangPin;

namespace Shangpin.Ocs.Web.Areas.Shangpin.Controllers
{
    [OCSAuthorization]
    public class AppRecommendProductMangerController : Controller
    {
        //
        // GET: /Shangpin/AppRecommendProductManger/


        //列表查询
        [ValidateInput(false)]
        public ActionResult Index(int pageIndex = 1, int pageSize = 10)
        {
            Parameters param = CreateParam(pageIndex, pageSize);

            AppRecommendService dal = new AppRecommendService();
            XMLReturnClassLists model = dal.GetLists(param);//接口查询 商品列表

            AppRecommendService zhao = new AppRecommendService();
            string categoryNo = string.Empty;
            if (!string.IsNullOrEmpty(param.categoryNO))
            {
                categoryNo = param.categoryNO;
            }
            else
            {
                categoryNo = param.brandNO;
            }
            List<AppRecommendProductModle> saveProductList = zhao.GetAppRecommendProductList();
            if (saveProductList.Count() > 0)
            {
                for (int i = 0; i < model.ListProducts.Count; i++)//对比商品是否已经加入排序池
                {
                    if (saveProductList.Count(q => q.ProductNo == model.ListProducts.ElementAt(i).ProductNo) > 0)
                    {
                        //库存为零时删除
                        //if (model.ListProducts.Where(p => p.ProductNo == saveProductList[i].ProductNo && p.stock < 1).Count() > 0)
                        //{
                        //    zhao.DelAppRecommendProductById(saveProductList[i].Id);
                        //}
                        model.ListProducts.ElementAt(i).IsSelected = 1;
                    }
                }
            }
            List<SearchResultCategorys> result = new List<SearchResultCategorys>();

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

            model.ListCategorys = result;
            model.SaveProductCount = saveProductList.Count();

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
            if (Server.UrlDecode(Request.QueryString["MinDiscountRate"]) != null && Server.UrlDecode(Request.QueryString["MinDiscountRate"]) != "")
            {
                p.discountRate = Request.QueryString["MinDiscountRate"] + "~" + (string.IsNullOrEmpty(Request.QueryString["MaxDiscountRate"]) ? "100000" : Request.QueryString["MaxDiscountRate"]);
                p.StartDiscountRate = Request.QueryString["MinDiscountRate"];
                p.EndDiscountRate = string.IsNullOrEmpty(Request.QueryString["MaxDiscountRate"]) ? "100000" : Request.QueryString["MaxDiscountRate"];
            }
            else if (Server.UrlDecode(Request.QueryString["MaxDiscountRate"]) != null && Server.UrlDecode(Request.QueryString["MaxDiscountRate"]) != "")
            {
                p.discountRate = "0" + "~" + Request.QueryString["MaxDiscountRate"];
                p.StartDiscountRate = "0";
                p.EndDiscountRate = Request.QueryString["MaxDiscountRate"];
            }
            if (!string.IsNullOrEmpty(Request.QueryString["MinHot"]) && !string.IsNullOrEmpty(Request.QueryString["MaxHot"]))
            {
                if (Request.QueryString["HotType"] == "0")
                {
                    p.hot = Request.QueryString["MinHot"] + "~" + Request.QueryString["MaxHot"];
                }
                else if (Request.QueryString["HotType"] == "1")
                {
                    p.sevenHot = Request.QueryString["MinHot"] + "~" + Request.QueryString["MaxHot"];
                }
            }

            return p;
        }

        //把商品加入推荐池
        public ActionResult AddProductToAppRecommondAjax()
        {

            string productNO = Request.Form["productNo"];
            if (string.IsNullOrEmpty(productNO))
            {
                TempData["tip"] = "<script>alert('添加失败')</script>";
                return Redirect("/Shangpin/AppRecommendProductManger/Index?categoryNO=" + Request.QueryString["categoryNO"] + "&ProductNo=" + Request.QueryString["ProductNo"] + "&ProductName=" + Request.QueryString["ProductName"] + "&brandNO=" + Request.QueryString["brandNO"] + "&colorId=" + Request.QueryString["colorId"] + "&shelfType=" + Request.QueryString["shelfType"] + "&ShelfDate=" + Request.QueryString["ShelfDate"] + "&MinPrice=" + Request.QueryString["MinPrice"] + "&MaxPrice=" + Request.QueryString["MaxPrice"] + "&MinStock=" + Request.QueryString["MinStock"] + "&MaxStock=" + Request.QueryString["MaxStock"] + "&MinDiscountRate=" + Request.QueryString["MinDiscountRate"] + "&MaxDiscountRate=" + Request.QueryString["MaxDiscountRate"] + "&hot=" + Request.QueryString["hot"] + "&sevenHot=" + Request.QueryString["sevenHot"]);
            }
            SWfsSortHistoryService sshsDal = new SWfsSortHistoryService();
            Passport passport = PresentationHelper.GetPassport();

            AppRecommendService zhao = new AppRecommendService();
            List<AppRecommendProductModle> saveProductList = zhao.GetAppRecommendProductList();
            if ((saveProductList.Count + productNO.Split(',').Length) > 50)
            {

                int remoteIndex = 0;
                remoteIndex = (saveProductList.Count + productNO.Split(',').Length) - 50;
                for (int i = 0; i < saveProductList.Count; i++)
                {
                    if (i < remoteIndex)
                    {
                        zhao.DelAppRecommendProductById(saveProductList[i].ProductNo);
                    }
                }

            }
            int result = 0;
            for (int i = 0; i < productNO.Split(',').Length; i++)
            {
                AppRecommendProductModle DTO = new AppRecommendProductModle()
                {
                    ProductNo = productNO.Split(',')[i],
                    Creator = passport.UserName
                };
                result = zhao.AddAppRecommendProduct(DTO);
            }

            sshsDal = null;
            TempData["tip"] = "<script>alert('添加成功')</script>";
            return Redirect("/Shangpin/AppRecommendProductManger/Index?categoryNO=" + Request.QueryString["categoryNO"] + "&ProductNo=" + Request.QueryString["ProductNo"] + "&ProductName=" + Request.QueryString["ProductName"] + "&brandNO=" + Request.QueryString["brandNO"] + "&colorId=" + Request.QueryString["colorId"] + "&shelfType=" + Request.QueryString["shelfType"] + "&ShelfDate=" + Request.QueryString["ShelfDate"] + "&MinPrice=" + Request.QueryString["MinPrice"] + "&MaxPrice=" + Request.QueryString["MaxPrice"] + "&MinStock=" + Request.QueryString["MinStock"] + "&MaxStock=" + Request.QueryString["MaxStock"] + "&MinDiscountRate=" + Request.QueryString["MinDiscountRate"] + "&MaxDiscountRate=" + Request.QueryString["MaxDiscountRate"] + "&hot=" + Request.QueryString["hot"] + "&sevenHot=" + Request.QueryString["sevenHot"]);
        }

        //移除推荐产品通过Id
        public string DelProductById(string productNo)
        {
            string result = "";
            if (productNo + "" != "")
            {
                for (int i = 0; i < productNo.Split(',').Length; i++)
                {
                    AppRecommendService zhao = new AppRecommendService();
                    result = zhao.DelAppRecommendProductById(productNo.Split(',')[i]) + "";
                }


            }
            return result;
        }


        public ActionResult AppRecommendProductList()
        {
            XMLReturnClassLists result = new XMLReturnClassLists();
            Parameters param = new Parameters();
            AppRecommendService zhao = new AppRecommendService();
            List<AppRecommendProductModle> saveProductList = zhao.GetAppRecommendProductList();

            for (int i = 0; i < saveProductList.Count; i = i + 10)
            {
                string productNoStr = "";
                for (int j = 0; j < 10; j++)
                {
                    if (i + j < saveProductList.Count)
                        productNoStr = productNoStr + saveProductList[i + j].ProductNo + ",";
                }

                if (productNoStr != "")
                {
                    productNoStr.TrimEnd(',');
                }
                param.productNO = productNoStr;
                XMLReturnClassLists model = zhao.GetLists(param);//接口查询 商品列表
                foreach (InterfaceProductInfo item in model.ListProducts)
                {
                    if (item.stock == 0)
                    {
                        zhao.DelAppRecommendProductById(item.ProductNo);
                    }
                    else
                    {
                        result.ListProducts.Add(item);
                    }
                }

                for (int j = 0; j < productNoStr.Split(',').Length; j++)
                {
                    if (model.ListProducts.Where(p => p.ProductNo == productNoStr.Split(',')[j]).Count() == 0)
                    {
                        zhao.DelAppRecommendProductById(productNoStr.Split(',')[j]);
                    }
                }

            }

            return View(result);
        }
    }
}
