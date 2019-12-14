using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Shangpin.Ocs.Service.Shangpin;
using Shangpin.Ocs.Entity.Extenstion.ShangPin;
using Shangpin.Entity.Wfs;

namespace Shangpin.Ocs.Web.Areas.Shangpin.Controllers
{
    public class SEOController : Controller
    {
        private SWfsCategoryBrandAliasService _SWfsCategoryBrandAliasService;
        private int pageSize = 15;
        private int count = 0;

        public SEOController()
        {
            _SWfsCategoryBrandAliasService = new SWfsCategoryBrandAliasService();
        }

        #region Brand
        /// <summary>
        /// CreateUser：zhangxuekun
        /// Description：品牌别名列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public ActionResult BrandAlias(int pageIndex = 1, string brandName = "", string aliasName="")
        {
            var list = _SWfsCategoryBrandAliasService.GetAllBrand(pageIndex, pageSize, brandName.Trim(),aliasName.Trim(), out count);
            ViewBag.PageIndex = pageIndex;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = count;
            ViewBag.BrandName = brandName;
            ViewBag.AliasName = aliasName;
            return View(list);
        }

        /// <summary>
        /// CreateUser：zhangxuekun
        /// Description：无品牌别名列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public ActionResult NoBdAlias(int pageIndex = 1)
        {
            var list = _SWfsCategoryBrandAliasService.GetNoBrandAlias(pageIndex, pageSize, out count);
            ViewBag.PageIndex = pageIndex;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = count;
            return View(list);
        }

        /// <summary>
        /// CreateUser：zhangxuekun
        /// Description：添加品牌别名
        /// </summary>
        /// <param name="brandNo">品牌编号</param>
        /// <param name="alias">别名</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AjaxSaveBrandAlias(string brandNo, string alias, short typeId, short gender)
        {
            SWfsCategoryBrandAlias brandAlias = new SWfsCategoryBrandAlias();
            brandAlias.TypeID = typeId;
            brandAlias.Gender = gender;
            brandAlias.ObjectNo = brandNo;
            brandAlias.ObjectAlias = alias;
            var flag = _SWfsCategoryBrandAliasService.SaveArias(brandAlias);
            return Json(new { result = flag, message = "AA" });
        }

        [HttpPost]
        public ActionResult AjaxDeleteBrandAlias(int id)
        {
            var flag = _SWfsCategoryBrandAliasService.DeleteArias(id);
            return Json(new { result = flag, message = "AA" });
        }

        #endregion

        #region Category
        /// <summary>
        /// CreateUser：zhangxuekun
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public ActionResult CategoryAlias(int pageIndex = 1, string categoryNo = "A01", string categoryName = "",string aliasName="" )
        {
            if (categoryNo.ToLower() != "a01" && categoryNo.ToLower() != "a02")
                categoryNo = "A01";
            ViewBag.Gender = categoryNo == "A01" ? 0 : 1;
            ViewBag.CategoryNo = categoryNo;
            var list = _SWfsCategoryBrandAliasService.GetAllCategory(pageIndex, pageSize, categoryNo, categoryName.Trim(), aliasName.Trim(), out count);
            ViewBag.PageIndex = pageIndex;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = count;
            ViewBag.CategoryName = categoryName;
            ViewBag.AliasName = aliasName;
            ViewBag.Title = categoryNo == "A01" ? "女士分类别名设置" : "男士分类别名设置";
            return View(list);
        }

        /// <summary>
        /// CreateUser：zhangxuekun
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public ActionResult NoCyAlias(int pageIndex = 1, string cNo = "A01")
        {
            if (cNo.ToLower() != "a01" && cNo.ToLower() != "a02")
                cNo = "A01";
            ViewBag.Gender = cNo == "A01" ? 0 : 1;
            ViewBag.CategoryNo = cNo;
            var list = _SWfsCategoryBrandAliasService.GetNoCategoryAlias(pageIndex, pageSize, ViewBag.Gender, out count);
            ViewBag.PageIndex = pageIndex;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = count;
            ViewBag.Title = cNo == "A01" ? "无女士分类别名列表" : "无男士分类别名列表";
            return View(list);
        }

        #endregion
    }
}
