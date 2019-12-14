using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Shangpin.Ocs.Service.Common;
using Shangpin.Entity.Wfs;
using Shangpin.Framework.Common;
using Shangpin.Ocs.Service.Shangpin;
using Shangpin.Ocs.Service;
using Shangpin.Framework.Common.Cache;
using Shangpin.Ocs.Entity.Extenstion.ShangPin;

namespace Shangpin.Ocs.Web.Areas.Shangpin.Controllers
{
    public class PromotionController : Controller
    {
        //
        // GET: /Shangpin/Promotion/
        SWfsBrandService brandService = new SWfsBrandService();
        private Dictionary<string, string> rsPic = new Dictionary<string, string>();
        public ActionResult Index()
        {
            return View();
        }
        #region 促销提示
        #region 列表
        public ActionResult PromotionsList(int pageIndex = 1, int pageSize = 10)
        {
            int count = 0;
            ViewBag.list = PromotionsLists(pageIndex, pageSize, out count);
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;
            ViewBag.TotalCount = count;
            return View();
        }
        public IEnumerable<SWfsPromotionsList> PromotionsLists(int pageIndex, int pageSize, out int count)
        {
            SWfsPromotionsList obj = new SWfsPromotionsList();
            var dic = new Dictionary<string, object>();
            if (Request.QueryString["StartDate"] != "" && Request.QueryString["StartDate"] != null)
            {
                //obj.EndDate = DateTime.Now;
                dic.Add("StartDate", Request.QueryString["StartDate"]);
                ViewBag.StartDate = Request.QueryString["StartDate"];
            }
            else
            {
                dic.Add("StartDate", "");
                ViewBag.StartDate = Request.QueryString["StartDate"];
            }
            if (Request.QueryString["EndDate"] != "" && Request.QueryString["EndDate"] != null)
            {
                dic.Add("EndDate", Request.QueryString["EndDate"]);
                ViewBag.EndDate = Request.QueryString["EndDate"];
            }
            else
            {
                dic.Add("EndDate", "");
                ViewBag.EndDate = Request.QueryString["EndDate"];
            }
            if (Request.QueryString["Status"] != "-1" && Request.QueryString["Status"] != null && Request.QueryString["Status"] != "")
            {
                dic.Add("Status", Request.QueryString["Status"]);
            }
            else
            {
                dic.Add("Status", "");
            }
            if (Request.QueryString["PromotionInfoName"] != "" && Request.QueryString["PromotionInfoName"] != null)
            {
                dic.Add("PromotionInfoName", Request.QueryString["PromotionInfoName"]);
                ViewBag.KeyWord = Request.QueryString["PromotionInfoName"];
            }
            else
            {
                dic.Add("PromotionInfoName", "");
                ViewBag.KeyWord = Request.QueryString["PromotionInfoName"];
            }
            if (Request.QueryString["TipContent"] != "" && Request.QueryString["TipContent"] != null)
            {
                dic.Add("TipContent", Request.QueryString["TipContent"]);
                ViewBag.Prompt = Request.QueryString["TipContent"];
            }
            else
            {
                dic.Add("TipContent", "");
                ViewBag.Prompt = Request.QueryString["TipContent"];
            }
            if (Request.QueryString["CreateUsterId"] != "" && Request.QueryString["CreateUsterId"] != null)
            {
                dic.Add("CreateUsterId", Request.QueryString["CreateUsterId"]);
                ViewBag.Created = Request.QueryString["CreateUsterId"];
            }
            else
            {
                dic.Add("CreateUsterId", "");
                ViewBag.Created = Request.QueryString["CreateUsterId"];
            }
            if (Request.QueryString["ProductNo"] != "" && Request.QueryString["ProductNo"] != null)
            {
                dic.Add("ProductNo", Request.QueryString["ProductNo"]);
                ViewBag.ProductNo = Request.QueryString["ProductNo"];
            }
            else
            {
                dic.Add("ProductNo", "");
                ViewBag.ProductNo = Request.QueryString["ProductNo"];
            }
            if (Request.QueryString["CreateDate"] != "" && Request.QueryString["CreateDate"] != null)
            {
                dic.Add("CreateDate", Request.QueryString["CreateDate"]);
                ViewBag.CreateDate = Request.QueryString["CreateDate"];
            }
            else
            {
                dic.Add("CreateDate", "");
                ViewBag.CreateDate = Request.QueryString["CreateDate"];
            }
            if (Request.QueryString["EndCreateDate"] != "" && Request.QueryString["EndCreateDate"] != null)
            {
                dic.Add("EndCreateDate", Request.QueryString["EndCreateDate"]);
                ViewBag.EndCreateDate = Request.QueryString["EndCreateDate"];
            }
            else
            {
                dic.Add("EndCreateDate", "");
                ViewBag.EndCreateDate = Request.QueryString["EndCreateDate"];
            }
            IEnumerable<SWfsPromotionsList> list = DapperUtil.Query<SWfsPromotionsList>("ComBeziWfs_Brand_SWfsProductPromotionTip_List", dic, new { StartDate = Request.QueryString["StartDate"], EndDate = Request.QueryString["EndDate"], Status = Request.QueryString["Status"], PromotionInfoName = Request.QueryString["PromotionInfoName"], TipContent = Request.QueryString["TipContent"], CreateUsterId = Request.QueryString["CreateUsterId"], ProductNo = Request.QueryString["ProductNo"], CreateDate = Request.QueryString["CreateDate"], EndCreateDate = Request.QueryString["EndCreateDate"], pageIndex = pageIndex, pageSize = pageSize });
            count = DapperUtil.Query<int>("ComBeziWfs_Brand_SWfsProductPromotionTip_Count", dic, new { StartDate = Request.QueryString["StartDate"], EndDate = Request.QueryString["EndDate"], Status = Request.QueryString["Status"], PromotionInfoName = Request.QueryString["PromotionInfoName"], TipContent = Request.QueryString["TipContent"], CreateUsterId = Request.QueryString["CreateUsterId"], ProductNo = Request.QueryString["ProductNo"], CreateDate = Request.QueryString["CreateDate"], EndCreateDate = Request.QueryString["EndCreateDate"], pageIndex = pageIndex, pageSize = pageSize }).First<int>();
            return list;
        }
        #endregion

        #region 修改状态
        public ActionResult PromotionsStatus_Update(int promotioninfoId, int status)
        {
            brandService.UpdatePromotionsStatus(promotioninfoId, status);
            return Redirect("PromotionsList.html");
        }
        #endregion

        #region 删除一条数据
        public ActionResult DeletePromotions(int promotioninfoId)
        {
            brandService.PromotionsDelete(promotioninfoId);
            return Redirect("PromotionsList.html");
        }
        #endregion

        #region 添加
        public ActionResult PromotionsInsert(int promotioninfoId = 0)
        {
            if (promotioninfoId == 0)
            {
                return View(new SWfsProductPromotionTip() { Status = 0, StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(1), IsAllProduct = true });
            }
            var bl = brandService.PromotionsListId(promotioninfoId);

            if (bl == null)
            {
                return View(new SWfsProductPromotionTip() { Status = 0, StartDate = DateTime.Now, EndDate = DateTime.Now });
            }

            return View(bl);
        }
        [HttpPost]
        public ActionResult PromotionsInsert(SWfsProductPromotionTip obj, int promotionInfoId)
        {
            if (PresentationHelper.GetPassport().UserName == null)
            {
                obj.CreateUsterId = "";
            }
            else
            {
                obj.CreateUsterId = PresentationHelper.GetPassport().UserName;
            }
            obj.IsAllProduct = Request.Form["IsAllProduct"] == "1" ? true : false;
            string tt = Request.Form["ProductNo"];
            string set = Request.Form["MemberSet"];
            if (set == "0")
            {
                obj.MemberSet = set;
            }
            else
            {
                obj.MemberSet = ',' + set + ',';
            }
            string[] tx = tt.Split(',');
            tt = "";
            obj.CreateDate = DateTime.Now;
            if (obj.PromotionInfoId == 0)
            {
                if (brandService.PromotionsCreate(obj) > 0)
                {
                    for (int i = 0; i < tx.Length; i++)
                    {
                        //循环添加商品编号
                        brandService.PromotionTipRefCreate(new SWfsProductPromotionTipRef { PromotionInfoId = obj.PromotionInfoId, ProductNo = tx[i] });
                    }
                    ViewData["tip"] = new HtmlString("<script>alert('添加成功'); window.location.href='PromotionsList.html'</script>");
                }
                else
                {
                    ViewData["tip"] = new HtmlString("<script>alert('添加失败')</script>");
                }
            }
            else
            {
                if (brandService.PromotionsUpdate(obj))
                {
                    //修改时，先删除以前的商品编号
                    brandService.PromotionTipRefDelete(promotionInfoId = obj.PromotionInfoId);
                    for (int i = 0; i < tx.Length; i++)
                    {
                        //循环添加商品编号
                        brandService.PromotionTipRefCreate(new SWfsProductPromotionTipRef { PromotionInfoId = obj.PromotionInfoId, ProductNo = tx[i] });
                    }
                    ViewData["tip"] = new HtmlString("<script>alert('修改成功'); window.location.href='PromotionsList.html'</script>");
                }
                else
                {
                    ViewData["tip"] = new HtmlString("<script>alert('修改失败')</script>");
                }
            }

            return View(obj);
        }
        #endregion
        //添加关联品牌
        public ActionResult PromotionTipRefInsert(int promotionInfoId)
        {
            if (promotionInfoId == 0)
            {
                return View(new SWfsProductPromotionTipRef());
            }
            var dd = brandService.PromotionTipRefID(promotionInfoId);
            if (dd == null)
            {
                return View(new SWfsProductPromotionTipRef());
            }
            return View(dd);
        }
        [HttpPost]
        public ActionResult PromotionTipRefInsert(SWfsProductPromotionTipRef obj)
        {
            if (obj.ProductPromotionRefID == 0)
            {
                if (brandService.PromotionTipRefCreate(obj) > 0)
                {
                    return Redirect("PromotionsInsert.html");
                }
                else
                {
                    ViewData["tip"] = new HtmlString("<script>alert('关联编号添加失败')</script>");
                }
            }
            return View();
        }
        #endregion

    }
}
