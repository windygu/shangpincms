using Shangpin.Ocs.Entity.Extenstion.ShangPin;
using Shangpin.Ocs.Service.Shangpin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Shangpin.Ocs.Service.Shangpin;
using Shangpin.Entity.Wfs;
using Shangpin.Ocs.Service.Common;
using Shangpin.Ocs.Entity.Extenstion.Login;

namespace Shangpin.Ocs.Web.Areas.Shangpin.Controllers
{
    [OCSAuthorization]
    public class ProductLabelController : Controller
    {
        private readonly ProductLabelService productLabelService;
        private Dictionary<string, string> rsPic = new Dictionary<string, string>();
        public ProductLabelController()
        {
            productLabelService = new ProductLabelService();
        }
        
        #region 标签管理
        public ActionResult ProductLabelManager()
        {
            ProductLabelService labelDal = new ProductLabelService();
            IEnumerable<SWfsProductLabel> LabelList = labelDal.GetLabelList();
            if (!string.IsNullOrEmpty(Request.QueryString["labelName"]))
            {
                string labelName = Request.QueryString["labelName"].ToString();
                //筛选出包含搜索标签名的标签数据
                var result1 = from p in LabelList
                              where (p.LabelName.IndexOf(labelName, 0, StringComparison.OrdinalIgnoreCase) > -1)
                              select p;
                //筛选出要返回客户端的标签数据
                var result2 = from p in LabelList
                              where result1.Count(i => i.LabelNo == p.LabelNo | p.ParentNo == i.LabelNo |
                                  p.LabelNo == i.ParentNo | (p.ParentNo == i.ParentNo && p.ParentNo != "Root")) > 0
                              select p;
                return View(result2);
            }
            return View(LabelList);
        }
        //按ID获取标签
        public ActionResult GetLabelById(int id)
        {
            ProductLabelService labelDal = new ProductLabelService();
            SWfsProductLabel obj = labelDal.GetLabelById(id);
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        //修改标签
        public ActionResult EditeLabel(int labelId, string labelName, string parentNo, short labelType, string labelNiceName)
        {
            int result = 0;
            labelName = Server.UrlDecode(labelName);
            ProductLabelService labelDal = new ProductLabelService();
            if (string.IsNullOrEmpty(parentNo))
            {
                return Json(new
                {
                    code = 1,
                    msg = "父类不存在"
                }, JsonRequestBehavior.AllowGet);
            }
            if (parentNo.ToLower() == "root")
            {
                if (string.IsNullOrEmpty(labelNiceName))
                {
                    return Json(new
                    {
                        code = 1,
                        msg = "标签别名不能为空"
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            //验证是否重复
            if (labelDal.IsExistSameLabel(parentNo, labelName, labelId) > 0)
            {
                return Json(new
                {
                    code = 1,
                    msg = "该标签已存在"
                }, JsonRequestBehavior.AllowGet);
            }
            int n = labelDal.EditeLabel(new SWfsProductLabel { LabelId = labelId, LabelName = labelName, ParentNo = parentNo, LabelType = labelType,LabelNickName=labelNiceName });
            return Json(new
            {
                code = n > 0 ? 0 : 1,
                msg = n > 0 ? "修改成功" : "修改失败"
            }, JsonRequestBehavior.AllowGet);
        }
        //增加标签
        public ActionResult AddLabel(string parentNo, string labelName,short labelType,string labelNiceName)
        {
            int result = 0;
            labelName = Server.UrlDecode(labelName);
            ProductLabelService labelDal = new ProductLabelService();
            if (string.IsNullOrEmpty(parentNo))
            {
                return Json(new
                {
                    code = 1,
                    msg = "父类不存在"
                }, JsonRequestBehavior.AllowGet);
            }
            if (parentNo.ToLower()=="root")
            {
                if (string.IsNullOrEmpty(labelNiceName))
                {
                    return Json(new
                    {
                        code = 1,
                        msg = "标签别名不能为空"
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            //验证是否重复
            if (labelDal.IsExistSameLabel(parentNo, labelName) > 0)
            {
                return Json(new
                {
                    code = 1,
                    msg = "该标签已存在"
                }, JsonRequestBehavior.AllowGet);
            }
            int n = labelDal.AddProductLabel(new SWfsProductLabel { ParentNo = parentNo, LabelName = labelName, LabelType = labelType,LabelNickName=labelNiceName });
            return Json(new
            {
                code = n > 0 ? 0 : 1,
                msg = n > 0 ? "添加成功" : "添加失败"
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 商品 关联 标签管理
        public ActionResult ProductList(int pageIndex = 1, int pageSize = 20)
        {
            Passport passport = PresentationHelper.GetPassport();
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            if (passport!=null&&!string.IsNullOrEmpty(passport.UserName))
            {
                sb.Append(DateTime.Now + " "+passport.UserName+" ");
            }
            int total = 0;
            if (Request.QueryString["keyWord"] != null)
            {
                ViewBag.keyWord = Request.QueryString["keyWord"];
                if (sb.Length > 0 && !string.IsNullOrEmpty(Request.QueryString["keyWord"]))
                {
                    sb.Append("关键字:" + Request.QueryString["keyWord"]+" ");
                }
            }
            if (Request.QueryString["CategoryNo"] != null)
            {
                ViewBag.category = Request.QueryString["CategoryNo"];
                if (sb.Length > 0 && !string.IsNullOrEmpty(Request.QueryString["categoryLevel"]))
                {
                    sb.Append(Request.QueryString["categoryLevel"] + " ");
                }
            }
            if (Request.QueryString["Gender"] != null)
            {
                ViewBag.Gender = Request.QueryString["Gender"];
                if (sb.Length > 0 && !string.IsNullOrEmpty(Request.QueryString["Gender"]))
                {
                    sb.Append("性别:"+(Request.QueryString["Gender"]=="0"?"女":"男") + " ");
                }
            }
            if (Request.QueryString["brandNO"] != null)
            {
                ViewBag.BrandNO = Request.QueryString["brandNO"];
                if (sb.Length > 0 && !string.IsNullOrEmpty(Request.QueryString["brandNO"]))
                {
                    sb.Append("品牌:" + Request.QueryString["BrandName"] + " ");
                }
            }
            if (Request.QueryString["IsShelf"] != null)
            {
                ViewBag.IsShelf = Request.QueryString["IsShelf"];
                if (sb.Length > 0)
                {
                    switch (Request.QueryString["IsShelf"])
                    {
                        case"1":
                            sb.Append("未上架 ");
                            break;
                        case "2":
                            sb.Append("已上架  ");
                            break;
                        case "3":
                            sb.Append("已下架  ");
                            break;
                    }
                    
                }
            }
            if (Request.QueryString["IsPublish"] != null)
            {
                ViewBag.IsPublish = Request.QueryString["IsPublish"];
            }
            if (Request.QueryString["TemplateName"] != null)
            {
                ViewBag.TemplateName = Request.QueryString["TemplateName"];
            }
            if (Request.QueryString["StartDateShelf"] != null)
            {
                ViewBag.StartDateShelf = Request.QueryString["StartDateShelf"];
                if (sb.Length > 0 && !string.IsNullOrEmpty(Request.QueryString["StartDateShelf"]))
                {
                    sb.Append("上架时间:" + Request.QueryString["StartDateShelf"] + "--" + Request.QueryString["EndDateShelf"]+" ");
                }
            }
            if (Request.QueryString["EndDateShelf"] != null)
            {
                ViewBag.EndDateShelf = Request.QueryString["EndDateShelf"];
            }
            
            if (Request.QueryString["IsRecord"]=="1")
            {
                SWfsProductLabelSearchHistory obj = new SWfsProductLabelSearchHistory() 
                {
                    SearchName = sb.ToString(),
                    SearchUser = passport.UserName,
                    SearchUrl = Request.Url.ToString().Replace("http://" + Request.Url.Host, "").Replace("IsRecord", "IsRecordAdd") 
                };
                productLabelService.AddSearchHistory(obj);
                return Redirect(obj.SearchUrl);
            }
            IEnumerable<ProductListForLabelSelect> list = productLabelService.GetSWfsProductList(ViewBag.StartDateShelf, ViewBag.EndDateShelf, ViewBag.IsShelf, ViewBag.Gender, ViewBag.BrandNO, ViewBag.category, ViewBag.keyWord, ViewBag.IsPublish, ViewBag.TemplateName, pageIndex, pageSize,Request.QueryString["isout"], out total);
            list = list.GroupBy(p => p.ProductNo).Select(p => new ProductListForLabelSelect
            {
                ProductNo = p.ElementAt(0).ProductNo,
                DateShelf = p.ElementAt(0).DateShelf,
                GoodsNo = p.ElementAt(0).GoodsNo,
                ProductModel = p.ElementAt(0).ProductModel,
                ProductName = p.ElementAt(0).ProductName,
                ProductPicFile = p.ElementAt(0).ProductPicFile,
                ProductShowPic = p.ElementAt(0).ProductShowPic,
                IsShelf = p.ElementAt(0).IsShelf,
                PcSaleState = p.ElementAt(0).PcSaleState,
                PcShowState = p.ElementAt(0).PcShowState,
                BrandCnName = p.ElementAt(0).BrandCnName,
                BrandEnName = p.ElementAt(0).BrandEnName,
                IsPublish = p.ElementAt(0).IsPublish,
                TemplateName = p.ElementAt(0).TemplateName,
                MarketPriceRegion = p.Min(a => a.MarketPrice) + "~" + p.Max(a => a.MarketPrice),
                StandardPriceRegion = p.Min(a => a.StandardPrice) + "~" + p.Max(a => a.StandardPrice),
                PlatinumPriceRegion = p.Min(a => a.PlatinumPrice) + "~" + p.Max(a => a.PlatinumPrice),
                DiamondPriceRegion = p.Min(a => a.DiamondPrice) + "~" + p.Max(a => a.DiamondPrice),
                PromotionPriceRegion = p.Min(a => a.PromotionPrice) + "~" + p.Max(a => a.PromotionPrice),
                GoldPriceRegion = p.Min(a => a.GoldPrice) + "~" + p.Max(a => a.GoldPrice),
                IsOutSide=p.ElementAt(0).IsOutSide
            }).ToList();
            
            ViewBag.totalCount = total;
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;
            ViewBag.HistoryList = productLabelService.GetSearchHistory(10);
            return View(list);
        }

        public ActionResult ProductRefLabel(string productNo = "")
        {
            ViewData["ProductNo"] = productNo;
            ViewData["allLabels"] = productLabelService.GetAllLabel();
            ViewData["productLabels"] = productLabelService.GetProductLabel(productNo);
            ViewBag.PicList = productLabelService.GetProductPicList(productNo);
            return View();
        }
        public JsonResult SaveProductRefLabel(string productNo, string ids, string pids)
        {
            //productNo = "0100620";
            //ids = "a,b,c,d,c#e,f,g,t,y";
            //pids = "z,x,c,v";
            string msg = "";
            try
            {
                if (string.IsNullOrEmpty(productNo))
                {
                    msg = "没有关联商品";
                }
                else
                {
                    productLabelService.DeleteProductLabel(productNo);//删除商品关联标签数据
                    if (ids.Length > 0)
                    {
                        string[] idStrArr = ids.Split('#');
                        string[] pidAtrr = pids.Split(',');
                        for (int i = 0; i < idStrArr.Length; i++)
                        {
                            if (idStrArr[i].Length == 0) continue;
                            string[] idsArr = idStrArr[i].Split(',');
                            for (int j = 0; j < idsArr.Length; j++)
                            {
                                SWfsProductRefLabel reflabel = new SWfsProductRefLabel
                                {
                                    ProductNo = productNo,
                                    Weight = short.Parse(j.ToString()),
                                    RefLabelNo = idsArr[j],
                                    RefLabelParentNo = pidAtrr[i]
                                };
                                productLabelService.InsertProductLabel(reflabel);
                            }
                        }
                    }
                    msg = "保存成功";
                }
            }
            catch (Exception)
            {
                return Json(new { msg = "保存商标关联标签时出现异常,请稍后再试!", data = "0" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { msg = msg, data = "1" }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 历史记录
        public void ClearLabelHistory() 
        {
            Passport passport = PresentationHelper.GetPassport();
            if (passport!=null)
            {
                productLabelService.ClearLabelHistory(passport.UserName); 
            }
        }
        #endregion
    }
}


//// 删除指定标签编号的标签,将关联的删除下级标签
//        public ActionResult DeleteLabel(string labelNo)
//        {
//            int result = 0;
//            string msg = "删除成功";
//            int n = productLabelService.DeleteLabelBtLabelNo(labelNo);
//            if (n == 0)
//            {
//                result = 1;
//                msg = "删除失败";
//            }
//            return Json(new { code = result, msg = msg }, JsonRequestBehavior.AllowGet);
//        }