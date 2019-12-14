using Shangpin.Entity.Wfs;
using Shangpin.Framework.Common;
using Shangpin.Framework.Common.Cache;
using Shangpin.Ocs.Entity.Extenstion.Login;
using Shangpin.Ocs.Entity.Extenstion.Outlet;
using Shangpin.Ocs.Entity.Extenstion.ShangPin;
using Shangpin.Ocs.Service.Common;
using Shangpin.Ocs.Service.Outlet;
using Shangpin.Ocs.Service.Shangpin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace Shangpin.Ocs.Web.Areas.Shangpin.Controllers
{

    //最新上架
    public class NewCommingController : Controller
    {
        private readonly NewCommingProductService productService;

        private readonly SWfsIndexNewArrivalService sWfsIndexNewArrivalService;

        private readonly SWfsIndexNewArrivalProductListService sWfsIndexNewArrivalProductListService;

        private Dictionary<string, string> rsPic = new Dictionary<string, string>();
        public NewCommingController()
        {
            productService = new NewCommingProductService();
            sWfsIndexNewArrivalService = new SWfsIndexNewArrivalService();
            sWfsIndexNewArrivalProductListService = new SWfsIndexNewArrivalProductListService();
        }

        #region 列表页
        public ActionResult NewCommingIndex(int pageIndex = 1, int pageSize = 10)
        {
            int total = 0;
            var model = sWfsIndexNewArrivalService.SelAllSWfsIndexNewArrival(pageIndex, pageSize, out total);
            foreach (var item in model)
            {
                var img = sWfsIndexNewArrivalService.SelAllSWfsIndexNewArrivalProduct(item.NewArrivalId);
                item.ProductNo = new List<string>();
                item.ProductPicFile = new List<string>();
                foreach (var imgitem in img)
                {
                    if (imgitem.ProductNo != null)
                    {
                        item.ProductNo.Add(imgitem.ProductNo);
                        item.ProductPicFile.Add(imgitem.ProductPicFile);
                    }
                }
            }

            if (Request.QueryString["titname"] != "" && Request.QueryString["titname"] != null)
            {
                model = model.Where(m => m.NewArrivalTitle.Contains(Request.QueryString["titname"])).ToList();
            }
            if (Request.QueryString["startdate"] != null && Request.QueryString["startdate"] != "")
            {
                model = model.Where(m => m.StartDate.CompareTo(DateTime.Parse(Request.QueryString["startdate"])) > 0).ToList();
            }
            if (Request.QueryString["enddate"] != null && Request.QueryString["enddate"] != "")
            {
                model = model.Where(m => m.StartDate.CompareTo(DateTime.Parse(Request.QueryString["enddate"])) < 0).ToList();
            }

            ViewBag.totalCount = total;// model.Count();
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;
            ViewBag.Num = model.Count();
            return View(model);
        }
        #endregion

        #region 添加上新
        public ActionResult AddNewComming()
        {
            string titlename = "";
            string xingqi = "";
            string detailtime = "";
            string path = AppDomain.CurrentDomain.BaseDirectory;
            path = string.Format("{0}ConfigFileCollection/App/NewCommingDate.xml", CommonHelper.GetParentPath(path, 2));

            //以下是循环读取xml文件中节点的值  
            XmlDocument xmlDoc = new XmlDocument();
            //xmlDoc.Load(Server.MapPath("~/ConfigFileCollection/App/NewCommingDate.xml").Replace("Shangpin.Ocs.Web", ""));//加载xml文件，文件
            xmlDoc.Load(path);
            XmlNodeList NodeList = xmlDoc.SelectNodes("/newcommingdate/date"); //xml节点的路径  
            //循环遍历节点  
            for (int i = 0; i < NodeList.Count; i++)
            {
                titlename = NodeList[i].ChildNodes[0].InnerText;    //获取第一个Student节点的StuName  
                xingqi = NodeList[i].ChildNodes[1].InnerText;     //获取第一个Student节点的StuSex  
                detailtime = NodeList[i].ChildNodes[2].InnerText;     //获取第一个Student节点的StuAge  
                //循环读取xml节点信息  
            }

            ViewBag.CommingTitle = titlename;
            ViewBag.CheckedAll = xingqi;
            ViewBag.DetailTime = detailtime;

            return View();
        }
        #endregion

        #region 添加商品
        public ActionResult AddGoods(int pageIndex = 1, int pageSize = 20)
        {
            if (Request.QueryString["GoodsId"] != null)
            {
                ViewBag.GoodsId = Request.QueryString["GoodsId"];
            }

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
            if (Request.QueryString["starttime"] != null && Request.QueryString["starttime"] != "")
            {
                ViewBag.StartDateShelf = Request.QueryString["starttime"];
            }
            if (Request.QueryString["endtime"] != null && Request.QueryString["endtime"] != "")
            {
                ViewBag.EndDateShelf = Request.QueryString["endtime"];
            }

            ViewBag.produclist = Request.Cookies["MyCook"] == null ? "" : Request.Cookies["MyCook"].Value + "";

            IEnumerable<ProductInfo> list = productService.GetSWfsProductList(ViewBag.Gender, ViewBag.BrandNO, ViewBag.category, ViewBag.keyWord, ViewBag.StartDateShelf, ViewBag.EndDateShelf, pageIndex, pageSize, out total);
            SWfsProductService service = new SWfsProductService();
            foreach (var item in list)
            {
                //商品下的Skulist
                IEnumerable<SpfSkuExtendInfo> skuList = productService.GetSkuListByProductNo(item.ProductNo);
                if (skuList.Any())
                {
                    //价格，上下架状态 
                    item.MarketPriceRegion = skuList.Select(n => n.MarketPrice).Min().ToString() + "~" + skuList.Select(n => n.MarketPrice).Max().ToString();
                    item.StandardPriceRegion = skuList.Select(n => n.StandardPrice).Min().ToString() + "~" + skuList.Select(n => n.StandardPrice).Max().ToString();
                    item.GoldPriceRegion = skuList.Select(n => n.GoldPrice).Min().ToString() + "~" + skuList.Select(n => n.GoldPrice).Max().ToString();
                    item.PlatinumPriceRegion = skuList.Select(n => n.PlatinumPrice).Min().ToString() + "~" + skuList.Select(n => n.PlatinumPrice).Max().ToString();
                    item.DiamondPriceRegion = skuList.Select(n => n.DiamondPrice).Min().ToString() + "~" + skuList.Select(n => n.DiamondPrice).Max().ToString();
                    item.OutletPriceRegion = skuList.Select(n => n.OutletPrice).Min().ToString() + "~" + skuList.Select(n => n.OutletPrice).Max().ToString();
                    item.PromotionPriceRegion = skuList.Select(n => n.PromotionPrice).Min().ToString() + "~" + skuList.Select(n => n.PromotionPrice).Max().ToString();
                    item.IsShelf = skuList.Select(n => n.IsShelf = 2).FirstOrDefault();
                    item.PcShowState = skuList.Select(n => n.PcShowState = 1).FirstOrDefault();
                    item.DisabledState = skuList.Select(n => n.DisabledState = 0).FirstOrDefault();
                }
                ProductInventory inventory = service.GetInventoryByProductNo(item.ProductNo);
                item.Quantity = inventory.SumQuantity;
                item.LockQuantity = inventory.SumLockQuantity;
            }

            ViewBag.totalCount = total;
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;

            return View(list);
        }
        #endregion

        #region 设置上新时间
        public ActionResult AddNewCommingDate()
        {
            string titlename = "";
            string xingqi = "";
            string detailtime = "";

            string path = AppDomain.CurrentDomain.BaseDirectory;
            path = string.Format("{0}ConfigFileCollection/App/NewCommingDate.xml", CommonHelper.GetParentPath(path, 2));
            //以下是循环读取xml文件中节点的值  
            XmlDocument xmlDoc = new XmlDocument();
            //xmlDoc.Load(Server.MapPath("~/ConfigFileCollection/App/NewCommingDate.xml").Replace("Shangpin.Ocs.Web", ""));//加载xml文件，文件
            xmlDoc.Load(path);
            XmlNodeList NodeList = xmlDoc.SelectNodes("/newcommingdate/date"); //xml节点的路径  
            //循环遍历节点  
            for (int i = 0; i < NodeList.Count; i++)
            {
                titlename = NodeList[i].ChildNodes[0].InnerText;    //获取第一个Student节点的StuName  
                xingqi = NodeList[i].ChildNodes[1].InnerText;     //获取第一个Student节点的StuSex  
                detailtime = NodeList[i].ChildNodes[2].InnerText;     //获取第一个Student节点的StuAge  
                //循环读取xml节点信息  
            }

            ViewBag.CommingTitle = titlename;
            ViewBag.CheckedAll = xingqi;
            ViewBag.DetailTime = detailtime;
            return View();
        }
        #endregion

        #region 保存上新时间
        [HttpPost]
        public ActionResult SetNewCommingDate()
        {
            //上新标题
            string title = Request.Form["Title"].ToString();
            //上新的商品
            string xinqi = Request.Form["WeekDay"].ToString();
            //上新的日期
            string detailtime = Request.Form["CreateTime"].ToString();

            string path = AppDomain.CurrentDomain.BaseDirectory;
            path = string.Format("{0}ConfigFileCollection/App/NewCommingDate.xml", CommonHelper.GetParentPath(path, 2));

            XmlDocument xmlDocx = new XmlDocument();
            //xmlDocx.Load(Server.MapPath("~/ConfigFileCollection/App/NewCommingDate.xml").Replace("Shangpin.Ocs.Web", ""));//加载xml文件，文件
            xmlDocx.Load(path);
            XmlNode xns = xmlDocx.SelectSingleNode("newcommingdate");//查找要修改的节点
            XmlNodeList xnl = xns.ChildNodes;//取出所有的子节点
            foreach (XmlNode xn in xnl)
            {
                XmlElement xe = (XmlElement)xn;//将节点转换一下类型
                if (xe.GetAttribute("genre") == "NewComming")//判断该子节点是否是要查找的节点
                {
                    XmlNodeList xnl2 = xe.ChildNodes;//取出该子节点下面的所有元素
                    foreach (XmlNode xn2 in xnl2)
                    {
                        XmlElement xe2 = (XmlElement)xn2;//转换类型
                        if (xe2.Name == "title")//判断是否是要查找的元素
                        {
                            xe2.InnerText = title;
                        }
                        if (xe2.Name == "xingqi")//判断是否是要查找的元素
                        {
                            xe2.InnerText = xinqi;
                        }
                        if (xe2.Name == "time")//判断是否是要查找的元素
                        {
                            xe2.InnerText = detailtime;
                        }
                    }
                }
            }
            //xmlDocx.Save(Server.MapPath("~/ConfigFileCollection/App/NewCommingDate.xml").Replace("Shangpin.Ocs.Web", ""));//再一次强调 ，一定要记得保存的该XML文件
            xmlDocx.Save(path);
            int num = productService.UpdateSWfsGlobalConfigByFunctionNo(title);
            if (num > 0)
            {
                //清除缓存
                EnyimMemcachedClient.Instance.Remove("ComBeziWfs_SWfsGlobalConfig_GetNewProductTime_GetNewProductTime");
                return Content("保存成功！");
            }
            else
            {
                return Content("保存失败！");
            }
        }
        #endregion

        #region 添加上新
        [HttpPost]
        public ActionResult AddNewCommingManager()
        {

            //上新标题
            string title = Request.Form["Title"].ToString();
            //上新的商品
            string goods = Request.Form["CommingGoods"].ToString();
            //上新的日期
            string date = Request.Form["CreateTime"].ToString();

            //上新实体
            SWfsIndexNewArrival sWfsIndexNewArrival = new SWfsIndexNewArrival();
            sWfsIndexNewArrival.NewArrivalTitle = title;
            sWfsIndexNewArrival.StartDate = DateTime.Parse(date);
            sWfsIndexNewArrival.OperateUserId = "admin";
            sWfsIndexNewArrival.WebSiteNo = "shangpin.com";
            sWfsIndexNewArrival.PageNo = "index";
            sWfsIndexNewArrival.PagePositionNo = "INDEX_NA";
            sWfsIndexNewArrival.PagePositionName = "首页上新";
            sWfsIndexNewArrival.CreateDate = DateTime.Now;
            sWfsIndexNewArrival.UpdateDate = DateTime.Now;
            sWfsIndexNewArrival.EndDate = DateTime.Parse(date);
            sWfsIndexNewArrival.DataState = 1;
            sWfsIndexNewArrival.Status = 1;

            //获取当前用户
            Passport passport = PresentationHelper.GetPassport();
            sWfsIndexNewArrival.OperateUserId = passport.UserName;

            int Start = sWfsIndexNewArrivalService.SelSWfsIndexNewArrivalDatailDate(sWfsIndexNewArrival.StartDate, 0);
            if (Start > 0)
            {
                return Content("-1");
            }
            else
            {
                //执行添加上新返回主键编号以便于添加该上新下边的商品
                int Pkid = sWfsIndexNewArrivalService.AddSWfsIndexNewArrival(sWfsIndexNewArrival);
                int sort = 50;
                int num = 0;

                if (Pkid != 0)
                {
                    string[] arr = goods.Split(',');
                    for (int i = 0; i < arr.Length; i++)
                    {
                        string[] arr1 = arr[i].Split('-');
                        SWfsIndexNewArrivalProductList sWfsIndexNewArrivalProductList = new SWfsIndexNewArrivalProductList();
                        sWfsIndexNewArrivalProductList.ProductNo = arr1[0];
                        sWfsIndexNewArrivalProductList.NewArrivalId = Pkid;
                        sWfsIndexNewArrivalProductList.SortValue = sort--;
                        sWfsIndexNewArrivalProductList.OperateUserId = passport.UserName;
                        num += sWfsIndexNewArrivalProductListService.AddSWfsIndexNewArrivalProductList(sWfsIndexNewArrivalProductList);
                    }
                }

                if (num == 0)
                {
                    EnyimMemcachedClient.Instance.Remove("ComBeziWfs_NewArrivalProductInfo_GetNewArrivalProductInfoByStartDate_getNewArrivalInfo");
                    return Content("添加失败！");
                }
                else
                {
                    return Content("添加成功！");
                }
            }
        }
        #endregion

        #region 管理商品
        public ActionResult UpdateGoods(int pageIndex = 1, int pageSize = 20)
        {

            string id = Request.QueryString["GoodsId"].ToString();
            int total = 0;
            IEnumerable<ProductInfo> list = productService.GetUpdateGoods(int.Parse(id));
            
            SWfsProductService service = new SWfsProductService();
            foreach (var item in list)
            {
                //商品下的Skulist
                var skuList = productService.GetSkuListByProductNo(item.ProductNo);
                if (skuList.Any())
                {
                    //价格，上下架状态 
                    item.MarketPriceRegion = skuList.Select(n => n.MarketPrice).Min().ToString() + "~" + skuList.Select(n => n.MarketPrice).Max().ToString();
                    item.StandardPriceRegion = skuList.Select(n => n.StandardPrice).Min().ToString() + "~" + skuList.Select(n => n.StandardPrice).Max().ToString();
                    item.GoldPriceRegion = skuList.Select(n => n.GoldPrice).Min().ToString() + "~" + skuList.Select(n => n.GoldPrice).Max().ToString();
                    item.PlatinumPriceRegion = skuList.Select(n => n.PlatinumPrice).Min().ToString() + "~" + skuList.Select(n => n.PlatinumPrice).Max().ToString();
                    item.DiamondPriceRegion = skuList.Select(n => n.DiamondPrice).Min().ToString() + "~" + skuList.Select(n => n.DiamondPrice).Max().ToString();
                    item.OutletPriceRegion = skuList.Select(n => n.OutletPrice).Min().ToString() + "~" + skuList.Select(n => n.OutletPrice).Max().ToString();
                    item.PromotionPriceRegion = skuList.Select(n => n.PromotionPrice).Min().ToString() + "~" + skuList.Select(n => n.PromotionPrice).Max().ToString();
                    item.IsShelf = skuList.Select(n => n.IsShelf = 2).FirstOrDefault();
                    item.PcShowState = skuList.Select(n => n.PcShowState = 1).FirstOrDefault();
                    item.DisabledState = skuList.Select(n => n.DisabledState = 0).FirstOrDefault();
                }
                //库存
                ProductInventory inventory = service.GetInventoryByProductNo(item.ProductNo);
                item.Quantity = inventory.SumQuantity;
                item.LockQuantity = inventory.SumLockQuantity;
            }

            ViewBag.totalCount = total;
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;
            var list1 = list.OrderByDescending(e => e.sortvalue);
            return View(list1);
        }
        #endregion

        #region 删除选中的商品
        [HttpPost]
        public ActionResult DeleteNewGoodsList()
        {

            //要删除的商品编号
            string goodsidlist = Request.Form["GoodsNo"].ToString();
            //要删除的商品所属的上新编号
            string newcommingid = Request.Form["NewCommingId"].ToString();

            string[] arr = goodsidlist.Split('-');
            int num = 0;
            for (int i = 0; i < arr.Length - 1; i++)
            {
                num += sWfsIndexNewArrivalProductListService.DelSWfsIndexNewArrivalProductListGoods(arr[i], newcommingid);
            }

            if (num == arr.Length - 1)
            {
                EnyimMemcachedClient.Instance.Remove("ComBeziWfs_NewArrivalProductInfo_GetNewArrivalProductInfoByStartDate_getNewArrivalInfo");
                return Content("删除成功！");
            }
            else
            {
                return Content("删除失败！");
            }

        }
        #endregion

        #region 单个上新的商品管理时的商品添加
        public ActionResult AddNewGoodsListByNewComming()
        {
            //要追加的商品编号
            string goodsidlist = Request.Form["GoodsNo"].ToString();
            //要追加的商品所属的上新编号
            string newcommingid = Request.Form["NewCommingId"].ToString();

            string[] arr = goodsidlist.Split(',');
            int sort = 30;
            int num = 0;
            for (int i = 0; i < arr.Length - 1; i++)
            {
                var arr1 = arr[i].Split('-');

                int count = sWfsIndexNewArrivalProductListService.AddSWfsIndexNewArrivalProductListGoods(arr1[0], newcommingid);
                if (count <= 0)
                {
                    SWfsIndexNewArrivalProductList sWfsIndexNewArrivalProductList = new SWfsIndexNewArrivalProductList();
                    sWfsIndexNewArrivalProductList.ProductNo = arr1[0];
                    sWfsIndexNewArrivalProductList.NewArrivalId = int.Parse(newcommingid);
                    sWfsIndexNewArrivalProductList.SortValue = sort--;
                    //获取当前用户
                    Passport passport = PresentationHelper.GetPassport();
                    sWfsIndexNewArrivalProductList.OperateUserId = passport.UserName;

                    num += sWfsIndexNewArrivalProductListService.AddSWfsIndexNewArrivalProductList(sWfsIndexNewArrivalProductList);
                }
            }

            if (num > 0)
            {
                EnyimMemcachedClient.Instance.Remove("ComBeziWfs_NewArrivalProductInfo_GetNewArrivalProductInfoByStartDate_getNewArrivalInfo");
                return Content("添加成功！");
            }
            else
            {
                return Content("添加失败！");
            }
        }
        #endregion

        #region 保存排序
        public ActionResult SortNewGoodsList()
        {
            //要排序的商品编号
            string goodsidlist = Request.Form["GoodsNo"].ToString();
            //要排序的商品所属的上新编号
            string newcommingid = Request.Form["NewCommingId"].ToString();

            string[] arr = goodsidlist.Split('-');
            int sort = 50;
            int num = 0;
            for (int i = 0; i < arr.Length - 1; i++)
            {
                num += sWfsIndexNewArrivalProductListService.UpdateSortSWfsIndexNewArrivalProductListGoods(arr[i], newcommingid, sort);
                sort--;
            }

            if (num > 0)
            {
                EnyimMemcachedClient.Instance.Remove("ComBeziWfs_NewArrivalProductInfo_GetNewArrivalProductInfoByStartDate_getNewArrivalInfo");
                return Content("排序成功！");
            }
            else
            {
                return Content("排序失败！");
            }
        }
        #endregion

        #region 编辑上新
        public ActionResult UpdateNewCommingManager()
        {
            //上新编号
            int pkid = int.Parse(Request.QueryString["ArivalId"].ToString());

            SWfsIndexNewArrivalExt model = new SWfsIndexNewArrivalExt();

            var newcomming = sWfsIndexNewArrivalService.SelSWfsIndexNewArrivalProductByIdentity(pkid);
            var img = sWfsIndexNewArrivalService.SelAllSWfsIndexNewArrivalProduct(pkid);

            model.NewArrivalId = newcomming.NewArrivalId;
            model.NewArrivalTitle = newcomming.NewArrivalTitle;
            model.StartDate = newcomming.StartDate;
            model.ProductNo = new List<string>();
            model.ProductPicFile = new List<string>();
            foreach (var imgitem in img)
            {
                if (imgitem.ProductNo != null)
                {
                    model.ProductNo.Add(imgitem.ProductNo);
                    model.ProductPicFile.Add(imgitem.ProductPicFile);
                }
            }

            string titlename = "";
            string xingqi = "";
            string detailtime = "";

            string path = AppDomain.CurrentDomain.BaseDirectory;
            path = string.Format("{0}ConfigFileCollection/App/NewCommingDate.xml", CommonHelper.GetParentPath(path, 2));
            //以下是循环读取xml文件中节点的值  
            XmlDocument xmlDoc = new XmlDocument();
            //xmlDoc.Load(Server.MapPath("~/ConfigFileCollection/App/NewCommingDate.xml").Replace("Shangpin.Ocs.Web", ""));//加载xml文件，文件
            xmlDoc.Load(path);
            XmlNodeList NodeList = xmlDoc.SelectNodes("/newcommingdate/date"); //xml节点的路径  
            //循环遍历节点  
            for (int i = 0; i < NodeList.Count; i++)
            {
                titlename = NodeList[i].ChildNodes[0].InnerText;    //获取第一个Student节点的StuName  
                xingqi = NodeList[i].ChildNodes[1].InnerText;     //获取第一个Student节点的StuSex  
                detailtime = NodeList[i].ChildNodes[2].InnerText;     //获取第一个Student节点的StuAge  
                //循环读取xml节点信息  
            }

            ViewBag.CommingTitle = titlename;
            ViewBag.CheckedAll = xingqi;
            ViewBag.DetailTime = detailtime;

            return View(model);
        }
        #endregion

        #region 上新编辑
        public ActionResult UpdateNewCommingManagerById()
        {
            //上新标题
            string title = Request.Form["Title"].ToString();
            //上新的编号
            string id = Request.Form["CommingId"].ToString();
            //上新的日期
            string date = Request.Form["CreateTime"].ToString();

            int Start = sWfsIndexNewArrivalService.SelSWfsIndexNewArrivalDatailDate(DateTime.Parse(date), int.Parse(id));
            if (Start > 0)
            {
                return Content("-1");
            }

            //上新实体
            SWfsIndexNewArrival sWfsIndexNewArrival = new SWfsIndexNewArrival();
            sWfsIndexNewArrival.NewArrivalId = int.Parse(id);
            sWfsIndexNewArrival.NewArrivalTitle = title;
            sWfsIndexNewArrival.StartDate = DateTime.Parse(date);
            sWfsIndexNewArrival.UpdateOperateUserId = "admin";

            int num = sWfsIndexNewArrivalService.UpdateSWfsIndexNewArrivalManager(sWfsIndexNewArrival);

            if (num == 0)
            {
                EnyimMemcachedClient.Instance.Remove("ComBeziWfs_NewArrivalProductInfo_GetNewArrivalProductInfoByStartDate_getNewArrivalInfo");
                return Content("修改失败！");
            }
            else
            {
                return Content("修改成功！");
            }
        }
        #endregion

        #region 删除上新
        public ActionResult DelNewComming()
        {
            string id = Request.Form["CommingId"].ToString();

            int num = sWfsIndexNewArrivalService.DelSWfsIndexNewArrivalStatus(int.Parse(id));

            if (num == 0)
            {
                EnyimMemcachedClient.Instance.Remove("ComBeziWfs_NewArrivalProductInfo_GetNewArrivalProductInfoByStartDate_getNewArrivalInfo");
                return Content("删除失败！");
            }
            else
            {
                return Content("删除成功！");
            }
        }
        #endregion

        #region 用于临时保存用户筛选的商品信息
        public string AddNewGoodsList(string GoodsNo)
        {
            HttpCookie cookies = Request.Cookies["MyCook"];
            string cookiesValue = string.Empty;
            if (cookies != null)
            {
                cookiesValue = cookies.Value;
            }

            HttpCookie cookie = new HttpCookie("MyCook");
            cookie.Value = cookiesValue + GoodsNo;
            Response.Cookies.Add(cookie);
            return "";
        }
        #endregion

        #region 用于删除用户临时保存用户筛选的商品信息
        public string UpdatreNewGoodsList(string GoodsNo)
        {
            HttpCookie cookies = Request.Cookies["MyCook"];

            HttpCookie cookie = new HttpCookie("MyCook");
            cookie.Value = GoodsNo;
            Response.Cookies.Add(cookie);
            return "";
        }
        #endregion

    }
}