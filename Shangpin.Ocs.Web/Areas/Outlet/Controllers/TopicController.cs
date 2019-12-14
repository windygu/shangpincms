using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Shangpin.Ocs.Service.Common;
using Shangpin.Ocs.Service.Outlet;
using Shangpin.Entity.Wfs;
using Shangpin.Ocs.Service;
using Shangpin.Ocs.Entity.Extenstion.Outlet;
using Shangpin.Ocs.Entity.Extenstion.ShangPin;
using Shangpin.Ocs.Service.Shangpin;

namespace Shangpin.Ocs.Web.Areas.Outlet.Controllers
{
    /// <summary>
    /// 专题管理
    /// </summary>

    [OCSAuthorization]
    public class TopicController : Controller
    {
        #region 专题列表
        /// <summary>
        /// 专题列表页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(string titleOrNo, string headTitle, string status, string gender, int pageIndex = 1)
        {

            titleOrNo = titleOrNo ?? "";
            headTitle = headTitle ?? "";
            SWfsTopicService service = new SWfsTopicService();
            int readCount = 0;
            IList<SWfsTopics> list = service.GetTopicList(titleOrNo.Trim(), headTitle.Trim(), status, gender, pageIndex, 20, out readCount);
            ViewBag.TopicList = list;
            ViewBag.CurPageIndex = pageIndex;
            ViewBag.ReadCount = readCount;
            ViewBag.TitleOrNo = titleOrNo;
            ViewBag.HeadTitle = headTitle;
            ViewBag.Status = status;
            ViewBag.Gender = gender ?? "-1";

            return View(list);
        }
        #endregion

        #region 创建和修改跳转逻辑
        /// <summary>
        /// 创建和修改页面的展现
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public ActionResult Manage(string act, string topicNo = "")
        {
            ViewBag.FromUrl = (null != HttpContext.Request.UrlReferrer) ? HttpContext.Request.UrlReferrer.ToString() : "/outlet/topic/index";
            SWfsTopics model = new SWfsTopics();
            ViewBag.Act = act;

            switch (act)
            {

                case "create"://新建
                    model.DateBegin = Convert.ToDateTime("1900-1-1");
                    model.DateEnd = Convert.ToDateTime("1900-1-1");
                    return View("/Areas/Outlet/Views/topic/Manage.cshtml", model);
                //break;

                case "edit"://修改
                    //ViewBag.Title = "专题修改|尚品OCS管理系统";
                    SWfsTopicService topicServ = new SWfsTopicService();
                    model = topicServ.GetSWfsTopics(topicNo);
                    return View("/Areas/Outlet/Views/topic/Manage.cshtml", model);
                //break;
            }
            return View();
        }

        #endregion

        #region 专题创建和修改
        /// <summary>
        /// 专题创建或修改
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Manager()
        {
            string topicNo = Request.Form["TopicNo"];
            CommonService commonService = new CommonService();
            SWfsTopicService topicService = new SWfsTopicService();
            Dictionary<string, string> rsPic = new Dictionary<string, string>();
            SWfsTopics model = new SWfsTopics();
            //加上修改的逻辑前的读取
            if (!string.IsNullOrEmpty(topicNo))
            {
                model = topicService.GetSWfsTopics(topicNo);
                if (null == model)
                {
                    return Json(new { reslut = "error", msg = "专题不存在" });
                }
            }
            model.DateBegin = Convert.ToDateTime(string.IsNullOrEmpty(Request.Form["DateBegin"]) ? "1900-1-1" : Request.Form["DateBegin"]);
            model.DateCreate = DateTime.Now;
            model.DateEnd = Convert.ToDateTime(string.IsNullOrEmpty(Request.Form["DateEnd"]) ? "1900-1-1" : Request.Form["DateEnd"]);
            if (string.IsNullOrEmpty(topicNo)) //添加时写入这些字段
            {
                model.DateTop = new DateTime(1900, 1, 1);
                model.IsTop = false;
                model.Status = 0;//默认刚添加的专题为关闭状态
                model.CreaterUserId = PresentationHelper.GetPassport().UserName;
            }
            model.DepartmentName = "";
            model.DepartmentNo = "";
            model.Gender = Convert.ToInt16(Request.Form["ChannelNos"]);
            model.PushDescripton = Request.Form["PushDescripton"];

            model.SubHeading = Request.Form["SubHeading"];
            model.TopicName = Request.Form["TopicName"];
            model.TopicTotalPicNo = "";
            model.TopicURL = "";
            model.Types = 0;
            model.WebSite = 2;


            if (null != Request.Files["TopicPicFile"] && Request.Files["TopicPicFile"].ContentLength > 0)
            {
                rsPic = commonService.PostImg(Request.Files["TopicPicFile"], "width:1680,Height:390,Length:500");
                if (rsPic.Keys.Contains("error"))
                {
                    return Json(new { reslut = "error", msg = rsPic["error"] });
                }
                if (rsPic.Keys.Contains("success"))
                {
                    model.TopicPic = rsPic["success"];
                }
            }
            if (null != Request.Files["WapPicFileNo"] && Request.Files["WapPicFileNo"].ContentLength > 0)
            {
                rsPic = commonService.PostImg(Request.Files["WapPicFileNo"], "width:640,Height:320,Length:150");
                if (rsPic.Keys.Contains("error"))
                {
                    return Json(new { reslut = "error", msg = rsPic["error"] });
                }
                if (rsPic.Keys.Contains("success"))
                {
                    model.WapPicFileNo = rsPic["success"];
                }
            }
            if (null != Request.Files["IPhonePicFileNo"] && Request.Files["IPhonePicFileNo"].ContentLength > 0)
            {
                rsPic = commonService.PostImg(Request.Files["IPhonePicFileNo"], "width:640,Height:0,Length:150");
                if (rsPic.Keys.Contains("error"))
                {
                    return Json(new { reslut = "error", msg = rsPic["error"] });
                }
                if (rsPic.Keys.Contains("success"))
                {
                    model.IPhonePicNo = rsPic["success"];
                }
            }

            model.ThumbnailFileNo = "";
            model.FillPictureFileNo = "";
            if (null != Request.Files["ThumbnailFileNo"] && Request.Files["ThumbnailFileNo"].ContentLength > 0)
            {
                rsPic = commonService.PostImg(Request.Files["ThumbnailFileNo"], "width:78,Height:63,Length:5");
                if (rsPic.Keys.Contains("error"))
                {
                    return Json(new { reslut = "error", msg = rsPic["error"] });
                }
                if (rsPic.Keys.Contains("success"))
                {
                    model.ThumbnailFileNo = rsPic["success"];
                }
            }
            if (null != Request.Files["FillPictureFileNo"] && Request.Files["FillPictureFileNo"].ContentLength > 0)
            {
                rsPic = commonService.PostImg(Request.Files["FillPictureFileNo"], "width:6,Height:390,Length:10");
                if (rsPic.Keys.Contains("error"))
                {
                    return Json(new { reslut = "error", msg = rsPic["error"] });
                }
                if (rsPic.Keys.Contains("success"))
                {
                    model.FillPictureFileNo = rsPic["success"];
                }
            }

            ///放到最后处理
            if (string.IsNullOrEmpty(topicNo))//新增
            {
                topicNo = DateTime.Now.ToString("yyyyMMdd");
                string topicId = new CommonService().GetNextCounterId("TopicNo").ToString("000");
                topicNo += topicId.Substring(topicId.Length - 3, 3);
                model.TopicNo = topicNo;
                try
                {
                    topicService.AddSWfsTopics(model);
                    return Json(new { reslut = "success", msg = "添加成功" });
                }
                catch (Exception e)
                {
                    return Json(new { reslut = "error", msg = e.Message });
                }
            }
            else //修改
            {
                model.TopicNo = topicNo;
                bool rs = topicService.UpateSWfsTopics(model);
                return Json(new { reslut = rs ? "success" : "error", msg = rs ? "修改成功" : "修改失败" });
            }
        }
        #endregion

        #region 专题删除 开启 置顶操作
        /// <summary>
        /// 专题删除 开启 置顶 操作
        /// </summary>
        /// <param name="topicNo"></param>
        /// <param name="action"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public ActionResult Edit(string topicNo, string action, string status = "1")
        {

            SWfsTopicService topicServ = new SWfsTopicService();
            int rs = topicServ.Edit(topicNo, action, status);
            return Json(new { rs = rs });
        }

        #endregion

        #region 商品列表
        /// <summary>
        /// 商品列表--查询添加
        /// </summary>
        /// <param name="topicNo"></param>
        /// <param name="categoryNo"></param>
        /// <param name="brandNo"></param>
        /// <param name="productNameOrNo"></param>
        /// <param name="pageindex"></param>
        /// <returns></returns>
        public ActionResult AddProduct(string topicNo, int pageindex = 1)
        {
            int pageSize = 10;
            string categoryNo1 = Request["CategoryNo1"];
            string categoryNo2 = Request["CategoryNo2"];
            string categoryNo3 = Request["CategoryNo3"];
            string categoryNo4 = Request["CategoryNo4"];
            string brandNo = Request["BrandNo"];
            string productNoOrName = Request["ProductNameOrNo"];
            string gender = Request["Gender"];
            if (!string.IsNullOrEmpty(productNoOrName) && productNoOrName.Equals("商品编号/商品名称"))
            {
                productNoOrName = "";
            }
            /////////////////////////分页保留查询数据用///////////////////////////////
            ViewBag.CategoryNo = categoryNo1 ?? "";
            ViewBag.CategoryNo2 = categoryNo2 ?? "";
            ViewBag.CategoryNo3 = categoryNo3 ?? "";
            ViewBag.CategoryNo4 = categoryNo4 ?? "";
            ViewBag.ProductNameOrNo = productNoOrName ?? "";
            ViewBag.BrandName = Request.QueryString["BrandName"] ?? "";
            ViewBag.Gender = gender ?? "";
            ///////////////////////////分页保留查询数据用/////////////////////////////

            string gCategroyNo = categoryNo1;

            if (!string.IsNullOrEmpty(categoryNo2))
                gCategroyNo = categoryNo2;
            if (!string.IsNullOrEmpty(categoryNo3))
                gCategroyNo = categoryNo3;
            if (!string.IsNullOrEmpty(categoryNo4))
                gCategroyNo = categoryNo4;


            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("brandNo", brandNo ?? "");
            dic.Add("productNoOrName", productNoOrName ?? "");
            dic.Add("gender", gender ?? "");
            dic.Add("gCategroyNo", gCategroyNo ?? "");

            SWfsTopicService topicServ = new SWfsTopicService();
            var productList = topicServ.GetProductList(dic, pageindex, pageSize);

            SWfsProductService productService = new SWfsProductService();
            //ProductInventory pin = new ProductInventory();
            ProductInventoryNew pin = new ProductInventoryNew();
            SWfsNewProductService service = new SWfsNewProductService();
            decimal bid = 0;

            foreach (var product in productList.Items)
            {
                //pin = productService.GetInventoryByProductNo(product.ProductNo);
                pin = service.GetInventoryByProductNo(product.ProductNo);
                product.Quantity = pin.SumQuantity;
                product.LockQuantity = pin.SumLockQuantity;


                bid = productService.GetProductBuyPriceByProductNo(product.ProductNo);

                product.sellBid = product.SellPrice - bid;
                product.marketBid = product.MarketPrice - bid;
                product.platinumBid = product.PlatinumPrice - bid;
                product.diamondBid = product.DiamondPrice - bid;
                product.limitedBid = product.LimitedPrice - bid;
                product.limitedVipBid = product.LimitedVipPrice - bid;

                product.selBidRate = product.SellPrice.Equals(0) ? 100 : Math.Round((product.sellBid / product.SellPrice) * 100, 2);
                product.marketBidRate = product.MarketPrice.Equals(0) ? 100 : Math.Round((product.marketBid / product.MarketPrice) * 100, 2);// Convert.ToDecimal(Math.Round((Convert.ToDecimal(marketBid) / Convert.ToDecimal(marketPrice)) * 100, 2)).ToString() + "%";
                product.platinumBidRate = product.PlatinumPrice.Equals(0) ? 100 : Math.Round((product.platinumBid / product.PlatinumPrice) * 100, 2);// Convert.ToDecimal(Math.Round((Convert.ToDecimal(platinumBid) / Convert.ToDecimal(platinumPrice)) * 100, 2)).ToString() + "%";
                product.diamondBidRate = product.DiamondPrice.Equals(0) ? 100 : Math.Round((product.diamondBid / product.DiamondPrice) * 100, 2);//Convert.ToDecimal(Math.Round((Convert.ToDecimal(diamondBid) / Convert.ToDecimal(diamondPrice)) * 100, 2)).ToString() + "%";
                product.limitedBidRate = product.LimitedPrice.Equals(0) ? 100 : Math.Round((product.limitedBid / product.LimitedPrice) * 100, 2);// Convert.ToDecimal(Math.Round((Convert.ToDecimal(limitedBid) / Convert.ToDecimal(limitedPrice)) * 100, 2)).ToString() + "%";
                product.limitedVipBidRate = product.LimitedVipPrice.Equals(0) ? 100 : Math.Round((product.limitedVipBid / product.LimitedVipPrice) * 100, 2);// Convert.ToDecimal(Math.Round((Convert.ToDecimal(limitedVipBid) / Convert.ToDecimal(limitedVipPrice)) * 100, 2)).ToString() + "%";
            }

            //所有的一级分类
            SWfsSubjectService subject = new SWfsSubjectService();
            ViewBag.AllFirstCategory = subject.SelectErpCategoryByParentNo("ROOT");
            //该活动下的一级分类
            //ViewBag.FirstCategory = subject.GetErpCategoryListBySubjectNo(subjectNo);

            ViewBag.Category2 = subject.SelectErpCategoryByParentNo(categoryNo1);
            ViewBag.Category3 = subject.SelectErpCategoryByParentNo(categoryNo2);
            ViewBag.Category4 = subject.SelectErpCategoryByParentNo(categoryNo3);

            ViewBag.TopicNo = topicNo;

            SWfsTopics topicModel = topicServ.GetSWfsTopics(topicNo);
            ViewBag.TopicName = (null != topicModel) ? topicModel.TopicName : "";

            ViewBag.SCategoryNo = gCategroyNo;

            return View(productList);
        }
        #endregion

        #region 添加商品
        /// <summary>
        /// 添加商品
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddProduct()
        {

            string productNoes = Request["productNoes"];
            string topicNo = Request["topicId"];
            string flag = Request["flag"];
            //需要根据具体的需求进行改造
            if (string.IsNullOrEmpty(productNoes))
            {
                return Json(new { rs = "Error", msg = "没有要添加的商品" });
            }
            if (string.IsNullOrEmpty(topicNo))
            {
                return Json(new { rs = "Error", msg = "专题编号错误" });
            }
            if (flag == "pl")//批量添加的商品 以回车符号分隔的，这个需要处理，手动添加的以逗号分割的则不用处理
            {
                //处理成以逗号分隔就OK
                productNoes = productNoes.Replace("\n\r", ",").Replace("\r\n", ",").Replace("\r", ",").Replace("\n", ",");
            }



            //判断添加商品是否存在，存在则返回错误 不存在则添加
            SWfsTopicService topicServ = new SWfsTopicService();
            List<SWfsTopicProductRef> rs = topicServ.GetSWfsTopicProductList(productNoes, topicNo);
            if (null != rs && rs.Count > 0)
            {
                string tmp = string.Empty;
                foreach (var item in rs)
                {
                    tmp += item.ProductNo + ",";
                }
                return Json(new { rs = "Error", msg = "商品:" + tmp + "已经存在" });
            }
            else
            {
                List<string> pNoes = productNoes.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (null == pNoes || pNoes.Count <= 0)
                {
                    return Json(new { rs = "Error", msg = "没有要添加的商品" });
                }
                SWfsTopicProductRef model;
                foreach (var item in pNoes)
                {
                    model = new SWfsTopicProductRef();
                    model.ProductNo = item;
                    model.TopicNo = topicNo;
                    model.OrderFlag = 999;
                    model.TopicProductNo = topicNo + item;
                    topicServ.AddProductNo(model);
                }
                return Json(new { rs = "Success", msg = "添加成功!" });

            }
        }

        #endregion

        #region 专题商品管理-商品列表
        /// <summary>
        /// 专题页的商品管理
        /// </summary>
        /// <returns></returns>
        public ActionResult ManageProduct(string topicNo, int pageindex = 1)
        {
            int pageSize = 10;
            string categoryNo = Request["CategoryNo"];
            string brandNo = Request["BrandNo"];
            string productNoOrName = Request["ProductNameOrNo"];
            if (!string.IsNullOrEmpty(productNoOrName) && productNoOrName.Equals("商品编号/商品名称"))
            {
                productNoOrName = "";
            }
            /////////////////////////分页保留查询数据用///////////////////////////////
            ViewBag.CategoryNo = categoryNo ?? "";
            ViewBag.ProductNameOrNo = productNoOrName ?? "";
            ViewBag.BrandName = Request.QueryString["BrandName"] ?? "";
            ViewBag.BrandNo = brandNo ?? "";

            ///////////////////////////分页保留查询数据用/////////////////////////////

            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("brandNo", brandNo ?? "");
            dic.Add("productNoOrName", productNoOrName ?? "");
            dic.Add("gCategroyNo", categoryNo ?? "");
            dic.Add("topicId", topicNo ?? "");

            SWfsTopicService topicServ = new SWfsTopicService();
            var productList = topicServ.GetProductListByTopicNo(dic, pageindex, pageSize);
            SWfsProductService productService = new SWfsProductService();
            ProductInventory pin = new ProductInventory();
            foreach (var product in productList.Items)
            {
                pin = productService.GetInventoryByProductNo(product.ProductNo);
                product.Quantity = pin.SumQuantity;
                product.LockQuantity = pin.SumLockQuantity;
            }
            //所有的一级分类
            SWfsSubjectService subject = new SWfsSubjectService();
            ViewBag.AllFirstCategory = subject.SelectErpCategoryByParentNo("ROOT");
            ViewBag.TopicNo = topicNo;

            SWfsTopics topicModel = topicServ.GetSWfsTopics(topicNo);
            ViewBag.TopicName = (null != topicModel) ? topicModel.TopicName : "";
            return View(productList);
        }

        #endregion

        #region 专题商品管理-操作管理
        /// <summary>
        /// 专题商品管理
        /// </summary>
        /// <returns></returns>
        public ActionResult EditProduct()
        {
            string act = Request["act"];//标识 del 删除 update 是更新排序值
            if (string.IsNullOrEmpty(act))
            {
                return Json(new { rs = "Error", msg = "操作产品参数错误" });
            }

            SWfsTopicService topicServ = new SWfsTopicService();
            if (act.Equals("del"))
            {
                #region 删除操作
                string productStr = Request["productNoes"];
                string topicNo = Request["topicNo"];
                if (string.IsNullOrEmpty(productStr) || string.IsNullOrEmpty(topicNo))
                {
                    return Json(new { rs = "Error", msg = "操作删除产品参数错误" });
                }
                int rs = topicServ.DelTopicProduct(topicNo, productStr);
                if (rs > 0)
                {
                    return Json(new { rs = "Success", msg = "删除成功" });
                }
                return Json(new { rs = "Error", msg = "删除失败" });
                #endregion             
            }
            if (act.Equals("update"))
            {
                #region 编辑操作
                string topicProductNo = Request["topicProductNo"];
                string orderFlagValStr = Request["orderFlagValStr"];
                if (string.IsNullOrEmpty(topicProductNo) || string.IsNullOrEmpty(orderFlagValStr))
                {
                    return Json(new { rs = "Error", msg = "保持排序值参数错误" });
                }
                string[] arrTopicPNOes = topicProductNo.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                string[] arrorderFlagVals = orderFlagValStr.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (arrTopicPNOes.Length != arrorderFlagVals.Length)
                {
                    return Json(new { rs = "Error", msg = "保持排序值参数错误" });
                }
                for (int i = 0; i < arrTopicPNOes.Length; i++)
                {
                    topicServ.UpdateTopicProductOrderFlag(arrTopicPNOes[i], arrorderFlagVals[i]);
                }
                return Json(new { rs = "Success", msg = "保存成功" });
                #endregion              
            }
            return Json(new { rs = "Error", msg = "参数错误" });
        }
        #endregion

    }

}
