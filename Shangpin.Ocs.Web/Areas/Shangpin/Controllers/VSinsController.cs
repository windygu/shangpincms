using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Shangpin.Entity.Wfs;
using Shangpin.Ocs.Service;
using Shangpin.Framework.Common;
using Shangpin.Ocs.Service.Shangpin;
using Shangpin.Ocs.Entity.Extenstion.ShangPin;
namespace Shangpin.Ocs.Web.Areas.Shangpin.Controllers
{
    public class VSinsController : Controller
    {
        //
        // GET: /Shangpin/VSins/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult VSinsList( int pageIndex = 1, int pageSize = 10)
        {
            VsinsService vsins = new VsinsService();
            Dictionary<string, object> dicStr = new Dictionary<string, object>();
            string productNo = Rq.GetStringForm("productNo");
            string time = Rq.GetStringForm("datetime");
            if (!string.IsNullOrEmpty(productNo))
            {
                dicStr.Add("ProductNo", productNo);
            }
            else
            {
                dicStr.Add("ProductNo", "");
            }

            if (!string.IsNullOrEmpty(time))
            {
                dicStr.Add("SelectTime", time);
            }
            else
            {
                dicStr.Add("SelectTime", "");
            }
            List<VsinsHotProduct> list = vsins.SelectHotProducts(dicStr, pageIndex, pageSize).ToList();
            int count=DapperUtil.Query<int>("ComBeziWfs_SWfsHotProduct_SelectAllcount",dicStr,new{ProductNo = dicStr["ProductNo"].ToString(), SelectTime = dicStr["SelectTime"].ToString()}).FirstOrDefault();
            ViewBag.PageIndex = pageIndex;
            ViewBag.totalcount = count;
            ViewBag.ProductNo = productNo;
            ViewBag.Time = time;
            return View(list);
        }
        public ActionResult CreateHotProduct(string hotpId)
        {
            if (string.IsNullOrEmpty(hotpId))
            {
                return View(new SWfsHotProduct() { CreateDate=DateTime.Now,StartDate=DateTime.Now,EndDate=DateTime.Now, Description="", Status=1, ProductNo="" });
            }
            else
            {
                return View(DapperUtil.QueryByIdentity<SWfsHotProduct>(hotpId));
            }
        }
        public string SaveVsinsHotProducts()
        {
            string hotProductId = Request.Form["hotProductId"];
            string productNo = Request.Form["productNo"];
            string status = Rq.GetStringForm("status");
            string beginTime = Rq.GetStringForm("dateBegin");
            string endTime = Rq.GetStringForm("dateEnd");
            string description = Rq.GetStringForm("description");
            string imgNo = string.Empty;
            Dictionary<string, string> picResDic = new Dictionary<string, string>();

            if (Request.Files["imgfile"] != null && Request.Files["imgfile"].ContentLength > 0)
            {
                picResDic = new CommonService().PostImg(Request.Files["imgfile"], "width:640,Height:0,Length:200");
                if (picResDic.Keys.Contains("error"))
                {
                    return "{ \"result\":\"error\", \"msg\":\""+picResDic["error"]+"\" }";
                }
                if (picResDic.Keys.Contains("success"))
                {
                    imgNo = picResDic["success"];
                }
            }
            else
            {
                imgNo = Rq.GetStringForm("imgNo");
            }
            
            SWfsHotProduct hotproduct = DapperUtil.QueryByIdentity<SWfsHotProduct>(hotProductId);
            if (hotproduct == null)
            {
                hotproduct = new SWfsHotProduct();
                hotproduct.CreateDate = DateTime.Now;
            }
            hotproduct.Description = description;
            hotproduct.EndDate = Convert.ToDateTime(endTime);
            hotproduct.PicFileNo = imgNo;
            hotproduct.ProductNo = productNo;
            hotproduct.StartDate = Convert.ToDateTime(beginTime);
            hotproduct.Status = short.Parse(status);
            hotproduct.Type = 1;
            if (hotproduct.HotProductId != 0)
            {
                DapperUtil.Update<SWfsHotProduct>(hotproduct);
            }
            else
            {
                DapperUtil.Insert<SWfsHotProduct>(hotproduct);
            }
            return "{\"result\":\"success\"}";
        }

        public JsonResult DeleHotProductById(string hotproductId)
        {
            if (!string.IsNullOrEmpty(hotproductId))
            {
                DapperUtil.Execute("ComBeziWfs_SWfsHotProduct_DeleteById", new { HotProductId=hotproductId });
            }
            return Json(new { result = "success" });
        }

        public JsonResult UpdateHotProductStatus(string hotproductId)
        {
            var hotproduct=DapperUtil.QueryByIdentity<SWfsHotProduct>(hotproductId);
            hotproduct.Status=hotproduct.Status == short.Parse("1") ? short.Parse("0") : short.Parse("1");
            DapperUtil.Update<SWfsHotProduct>(hotproduct);
            return Json(new { result = "success" });
        }
    }
}
