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
using System.Xml;
using Shangpin.Framework.Common;

namespace Shangpin.Ocs.Web.Areas.Shangpin.Controllers
{
    public class BoutiqueController : Controller
    {
        //
        // GET: /Shangpin/Boutique/

        public ActionResult Index()
        {
            return View();
        }

        #region 最新上架管理

        #region 最新上架管理页面方法
        //最新上架管理页面
        public ActionResult NewShelfList(string startDate, string endDate, string pageIndex, int pageSize = 20, string brandNo = "B0626")
        {
            pageIndex = pageIndex == null || pageIndex == "" ? "1" : pageIndex;
            int count = 0;
            //NewShelfBrandProductService service = new NewShelfBrandProductService();
            //int count = 0;
            //Dictionary<string, List<ProductInfoNew>> dic = service.SelectNewBrandWeekDaysProduct(Convert.ToInt32(pageIndex), pageSize, brandNo, startDate, endDate, out count);
            //dic.Add("aaa_bbb", new List<ProductInfoNew>());
            SwfsFlagShipNewArrivalProductService service = new SwfsFlagShipNewArrivalProductService();
            SwfsFlagShipGloalConfig config = service.NewDateManageBrandNo(brandNo);
            ViewBag.DetailTime = config == null ? "" : config.ConfigTime;
            startDate = startDate == null ? "" : startDate;
            endDate = endDate == null ? "" : endDate;
            List<NewShelfListModel> list = service.SelectNewShelfList(brandNo, startDate, endDate, Convert.ToInt32(pageIndex), pageSize, out count);
            ViewBag.page = pageIndex;
            ViewBag.pageSize = pageSize;
            ViewBag.totalCount = count;
            ViewBag.brandNo = brandNo;
            string[] valuearrt = new string[] { "1", "2", "3", "4", "5", "6", "0" };
            string disabledDays = "";
            if (config != null)
            {
                foreach (var item in valuearrt)
                {
                    if (!config.ConfigValue.Contains(item))
                    {
                        disabledDays += item + ",";
                    }
                }
            }
            else
            {
                ViewBag.type = "请先进行上架时间设置";
            }
            ViewBag.disabledDays = disabledDays;

            return View(list);
        }

        /// <summary>
        /// ajax操作上新时间
        /// </summary>
        /// <param name="NewArrivalId"></param>
        /// <param name="BrandNo"></param>
        /// <param name="NewShelfDate"></param>
        /// <returns></returns>
        public ActionResult OperationNewShelf(string NewArrivalId, string BrandNo, string NewShelfDate)
        {
            Passport passport = PresentationHelper.GetPassport();//用户
            SwfsFlagShipNewArrival arrival = new SwfsFlagShipNewArrival();
            NewArrivalId = NewArrivalId == null || NewArrivalId == "" ? "0" : NewArrivalId;
            SwfsFlagShipNewArrivalProductService service = new SwfsFlagShipNewArrivalProductService();
            SwfsFlagShipNewArrival selectarrival = service.SelectBrandNoNewShelfDate(BrandNo, NewShelfDate);//查找某个品牌下的当前上新时间是否有数据
            if (selectarrival != null)
                return Json(new { message = "该上新时间已经存在" });
            arrival.UpdateOperateUserId = passport.UserName;
            arrival.BrandNo = BrandNo;
            arrival.NewShelfDate = Convert.ToDateTime(NewShelfDate);

            try
            {
                service.OperationNewShelf(arrival, Convert.ToInt32(NewArrivalId), passport.UserName);
                return Json(new { message = "操作成功" });
            }
            catch (Exception e)
            {
                return Json(new { message = "操作失败" });
            }
        }


        public ActionResult DeleteProductList(string NewArrivalId)
        {
            SwfsFlagShipNewArrivalProductService service = new SwfsFlagShipNewArrivalProductService();
            try
            {
                service.DeleteNewShelfListNewArrivalId(NewArrivalId);
                return Json(new { message = "删除成功" });
            }
            catch (Exception e)
            {
                return Json(new { message = e.ToString() });
            }

        }
        #endregion

        #region 管理商品页面方法
        //管理商品页面
        public ActionResult NewShelfProductListManage(string ArrivalId, string isOneRow)
        {
            isOneRow = isOneRow == null || isOneRow == "" ? "1" : isOneRow;
            SwfsFlagShipNewArrivalProductService service = new SwfsFlagShipNewArrivalProductService();
            List<ProductInfoNew> list = service.SelectNewArrivalId(ArrivalId, isOneRow);
            return View(list);
        }

        /// <summary>
        /// ajax修改商品排序值
        /// </summary>
        /// <param name="idStr"></param>
        /// <param name="sortStr"></param>
        /// <returns></returns>
        public ActionResult UpdateProductSort(string idStr, string sortStr, string memcache_key)
        {
            SwfsFlagShipNewArrivalProductService service = new SwfsFlagShipNewArrivalProductService();
            try
            {
                EnyimMemcachedClient.Instance.Remove(memcache_key);
                service.UpdateProductSort(idStr, sortStr);
                return Json(new { message = "修改成功" });
            }
            catch (Exception e)
            {
                return Json(new { message = "修改失败" + e.ToString() });
            }

        }
        //删除商品
        public ActionResult DeleteProduct(string idStr)
        {
            SwfsFlagShipNewArrivalProductService service = new SwfsFlagShipNewArrivalProductService();
            try
            {
                service.DeleteProduct(idStr);
                return Json(new { message = "删除成功" });
            }
            catch (Exception)
            {
                return Json(new { message = "删除失败" });
            }
        }

        #endregion

        #region 添加关联商品页面方法
        //添加关联商品页面
        public ActionResult AddNewShelfProductList(string ArrivalId, string DateShelf, string IsOneRow, string pageIndex, int pageSize = 20, string brandNo = "B0626")
        {
            if (IsOneRow == null || IsOneRow == "")
            {
                IsOneRow = "1";
            }
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
            SwfsFlagShipNewArrivalProductService service = new SwfsFlagShipNewArrivalProductService();
            int total = 0;
            List<ProductInfoNew> list = service.BrandNewShelfProductList(brandNo, Convert.ToInt32(pageIndex), pageSize, DateShelf, ViewBag.keyword, ViewBag.categoryNo, ViewBag.Gender, out total);
            ViewBag.page = pageIndex;
            ViewBag.pageSize = pageSize;
            ViewBag.totalCount = total;
            ViewBag.NewArrivalId = ArrivalId;
            ViewBag.IsOneRow = IsOneRow;
            List<ProductInfoNew> listp = service.SelectNewArrivalId(ArrivalId, IsOneRow);
            ViewBag.PCunt = listp.Count();
            return View(list);
        }

        /// <summary>
        /// ajax添加商品
        /// </summary>
        /// <param name="ProductNoStr"></param>
        /// <param name="NewArrivalId"></param>
        /// <param name="IsOneRow"></param>
        /// <returns></returns>
        public ActionResult AddShelfProduct(string ProductNoStr, string ArrivalId, string IsOneRow)
        {
            SwfsFlagShipNewArrivalProductService service = new SwfsFlagShipNewArrivalProductService();
            Passport passport = PresentationHelper.GetPassport();//用户
            try
            {
                service.AddShelfProduct(ProductNoStr, passport.UserName, ArrivalId, IsOneRow);
                return Json(new { message = "添加成功" });
            }
            catch (Exception)
            {
                return Json(new { message = "添加失败" });
                throw;
            }


        }
        #endregion


        #region 添加商品展示图页面方法
        /// <summary>
        /// 添加商品展示图页面
        /// </summary>
        /// <param name="brandNo">商品编号</param>
        /// <param name="DateShelf">上新时间</param>
        /// <returns></returns>
        public ActionResult AddProductImage(string ProductListId, string ArrivalId)
        {
            SwfsFlagShipNewArrivalProductService service = new SwfsFlagShipNewArrivalProductService();
            SwfsFlagShipNewArrivalProductList Product = service.SelectProductListId(ProductListId);
            ProductInfoNew model = service.SelectProductInfoNewProductListId(ProductListId);
            ViewBag.ProductFirstPicNo = Product == null ? "" : Product.ProductFirstPicNo;
            ViewBag.ProductSecondPicNo = Product == null ? "" : Product.ProductSecondPicNo;
            ViewBag.ArrivalId = ArrivalId;
            return View(model);
        }
        [HttpPost]
        public ActionResult UpdateProductPicNo()
        {
            string ProductListId = Request.Form["ProductListId"].Trim();
            SwfsFlagShipNewArrivalProductService service = new SwfsFlagShipNewArrivalProductService();
            SwfsFlagShipNewArrivalProductList Product = service.SelectProductListId(ProductListId);
            string ProductFirstPicNo = "";//第一张图片
            string ProductSecondPicNo = "";//第二张图片
            string aaa = Request.Files["FlapHotOne"].FileName;
            if (Request.Files["FlapHotOne"] == null || string.IsNullOrEmpty(Request.Files["FlapHotOne"].FileName))
            {
                return Json(new { result = "error", message = "请上传图片！" });
            }
            if (Request.Files["FlapHotTwo"] == null || string.IsNullOrEmpty(Request.Files["FlapHotTwo"].FileName))
            {
                return Json(new { result = "error", message = "请上传图片！" });
            }
            CommonService commonService = new CommonService();
            Dictionary<string, string> rsPic = commonService.PostImg(Request.Files["FlapHotOne"], "width:160,Height:420,Length:150");
            if (rsPic.Keys.Contains("error"))
            {
                return Json(new { result = "error", message = rsPic["error"] });
            }
            if (rsPic.Keys.Contains("success"))
            {
                ProductFirstPicNo = rsPic["success"];
            }
            Dictionary<string, string> rsPic2 = commonService.PostImg(Request.Files["FlapHotTwo"], "width:160,Height:420,Length:150");
            if (rsPic2.Keys.Contains("error"))
            {
                return Json(new { result = "error", message = rsPic["error"] });
            }
            if (rsPic2.Keys.Contains("success"))
            {
                ProductSecondPicNo = rsPic2["success"];
            }
            try
            {
                Product.ProductFirstPicNo = ProductFirstPicNo;
                Product.ProductSecondPicNo = ProductSecondPicNo;
                service.SelectProductListId(Product);
                return Json(new { result = "success", message = "修改成功。" });
            }
            catch (Exception ex)
            {
                return Json(new { result = "error", message = ex.Message });
            }
        }
        #endregion

        #region 上新时间管理页面方法
        /// <summary>
        /// 上新时间管理页面
        /// </summary>
        /// <returns></returns>
        public ActionResult NewDateManage(string brandNo = "B0626")
        {
            SwfsFlagShipNewArrivalProductService service = new SwfsFlagShipNewArrivalProductService();
            SwfsFlagShipGloalConfig Config = service.NewDateManageBrandNo(brandNo);
            return View(Config);
        }

        //修改上新时间
        public ActionResult UpdateNewDateManage(string brandNo, string configValue, string configName, string configTime, string memcache_key)
        {

            SwfsFlagShipGloalConfig config = new SwfsFlagShipGloalConfig();
            config.BrandNo = brandNo;
            config.ConfigValue = configValue.TrimEnd(',');
            config.ConfigName = configName;
            config.ConfigTime = configTime;
            Passport passport = PresentationHelper.GetPassport();//用户
            config.OperateUserId = passport.UserName;
            SwfsFlagShipNewArrivalProductService service = new SwfsFlagShipNewArrivalProductService();
            int count = service.ConfigNewDate(config);
            if (count == 0)
            {
                return Json(new { message = "修改失败" });
            }
            else
            {
                EnyimMemcachedClient.Instance.Remove(memcache_key);
                return Json(new { message = "修改成功" });
            }

        }
        #endregion

        #endregion
    }
}
