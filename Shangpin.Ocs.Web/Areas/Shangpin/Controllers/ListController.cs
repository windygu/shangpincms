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
    public class ListController : Controller
    {
        //
        // GET: /Shangpin/List/
        SWfsBrandService brandService = new SWfsBrandService();
        private Dictionary<string, string> rsPic = new Dictionary<string, string>();
        public ActionResult Index()
        {
            return View();
        }
        //通用保存图片
        private string SaveImg(HttpPostedFileBase file, string imgInfo)
        {
            CommonService commonService = new CommonService();
            rsPic.Clear();
            //Dictionary<string, string> rsPic = new Dictionary<string, string>();
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
        #region 列表轮播图

        #region 列表
        public ActionResult AlterList(int pageIndex = 1, int pageSize = 20)
        {
            int count = 0;
            if (Request.QueryString["Gender"] == "0")
            {
                string womenbags = AppSettingManager.AppSettings["WomenBags"];
                string womenclothing = AppSettingManager.AppSettings["WomenClothing"];
                string womenshoes = AppSettingManager.AppSettings["WomenShoes"];
                string womenaccessories = AppSettingManager.AppSettings["WomenAccessories"];
                string womenhomes = AppSettingManager.AppSettings["WomenHomes"];
                string WomenBeauty = AppSettingManager.AppSettings["WomenBeauty"];
                string WomenWatches = AppSettingManager.AppSettings["WomenWatches"];
            }
            string menbags = AppSettingManager.AppSettings["MenBags"];
            string menclothing = AppSettingManager.AppSettings["MenClothing"];
            string menshoes = AppSettingManager.AppSettings["MenShoes"];
            string menaccessories = AppSettingManager.AppSettings["MenAccessories"];
            string MenHomes = AppSettingManager.AppSettings["MenHomes"];
            string MenBeauty = AppSettingManager.AppSettings["MenBeauty"];
            string MenWatches = AppSettingManager.AppSettings["MenWatches"];
            List<string> categoryList = new List<string>() { 
                AppSettingManager.AppSettings["WomenBags"],
                AppSettingManager.AppSettings["WomenBags"],
                AppSettingManager.AppSettings["WomenBags"],
            };
            ViewBag.categoryList = categoryList;
            //ViewBag.list = brandService.GetSWfsListAlterGroupList(pageIndex, pageSize, out count);
            ViewBag.list = AlterLists(pageIndex, pageSize, out count);
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;
            ViewBag.TotalCount = count;
            return View();
        }
        //返回列表的总条数
        public IEnumerable<SWfsListAlterGroup> AlterLists(int pageIndex, int pageSize, out int count)
        {
            var dic = new Dictionary<string, object>();
            if (Request.QueryString["Gender"] != "-1" && Request.QueryString["Gender"] != null && Request.QueryString["Gender"] != "")
            {
                dic.Add("Gender", int.Parse(Request.QueryString["Gender"]));
            }
            else
            {
                dic.Add("Gender", "");
            }
            if (Request.QueryString["Category"] != "-1" && Request.QueryString["Category"] != null && Request.QueryString["Category"] != "")
            {
                dic.Add("Category", Request.QueryString["Category"]);
            }
            else
            {
                dic.Add("Category", "");
            }
            IEnumerable<SWfsListAlterGroup> list = DapperUtil.Query<SWfsListAlterGroup>("ComBeziWfs_Brand_SWfsListAlterGroup_List", dic, new { Gender = dic["Gender"], Category = dic["Category"], pageIndex = pageIndex, pageSize = pageSize }).ToList();
            count = DapperUtil.Query<int>("ComBeziWfs_Brand_SWfsListAlterGroup_List_Count", dic, new { Gender = dic["Gender"], Category = dic["Category"], pageIndex = pageIndex, pageSize = pageSize }).First<int>();
            return list;
        }
        #endregion

        #region 添加
        public ActionResult AlterCreate(int id = 0)
        {
            SWfsBrandService service = new SWfsBrandService();
            SWfsListAlterGroup obj = service.GetGroupbyID(id);
            if (obj == null)
            {
                return View(new SWfsListAlterGroup { GroupId = id });
            }
            return View(obj);
        }
        [HttpPost]
        public ActionResult AlterCreate(SWfsListAlterGroup obj)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            SWfsBrandService service = new SWfsBrandService();
            obj.CreateDate = DateTime.Now;
            obj.GroupName = Request.Form["GroupName"];
            obj.Gender = Request.Form["Gender"] != null ? int.Parse(Request.Form["Gender"]) : 0;
            obj.Status = Request.Form["Status"] != null ? int.Parse(Request.Form["Status"]) : 0;
            obj.GroupType = 0;
            obj.Category = Request.Form["Category"];
            if (obj.GroupId == 0)
            {
                if (service.GroupCreate(obj) > 0)
                {
                    ViewData["tip"] = new HtmlString("<script>alert('添加成功'); window.location.href='AlterList.html'</script>");
                }
                else
                {
                    ViewData["tip"] = new HtmlString("<script>alert('添加失败');</script>");
                }
            }
            else
            {
                if (service.GroupUpdate(obj))
                {
                    ViewData["tip"] = new HtmlString("<script>alert('修改成功'); window.location.href='AlterList.html'</script>");
                }
                else
                {
                    ViewData["tip"] = new HtmlString("<script>alert('修改失败');</script>");
                }
            }
            return View(obj);
        }
        #endregion

        #region 图片添加
        public ActionResult AlterimageCreate(int groupId)
        {
            SWfsBrandService service = new SWfsBrandService();
            IEnumerable<SWfsListAlterPic> list = service.getSWfsListAlterGroupListID(groupId);
            return View(list);
        }
        [HttpPost]
        public string ImgAlterUpload(SWfsListAlterPic obj, string linkname, int groupId)
        {
            SWfsBrandService service = new SWfsBrandService();
            obj.CreateDate = DateTime.Now;
            if (string.IsNullOrEmpty(obj.AlterAddress))
            {
                obj.AlterAddress = "";
            }
            #region 添加图片
            if (Request.Files["alter1file"] != null && Request.Files["alter1file"].ContentLength > 0)
            {
                obj.LargePicture = SaveImg(Request.Files["alter1file"], "width:690,Height:290,Length:300");
                if (rsPic.Keys.Contains("error"))
                {
                    ViewData["tip"] = new HtmlString("<script>alert('" + rsPic["error"] + "')</script>");
                    return rsPic["error"];
                }
            }
            else
            {
                if (string.IsNullOrEmpty(obj.LargePicture))
                {
                    obj.LargePicture = "";
                }
            }
            
            #endregion
            if (obj.AlterPicId == 0)
            {
                var ser = service.HeavyRow(linkname, groupId);
                if (ser.Count() > 0)
                {
                    if (ser.ElementAt(0).AlterPicId != obj.AlterPicId)
                    {
                        return "2";
                    }
                }
                //调用排重方法
                var row = service.HeavyRow(linkname, groupId);
                if (row.Count() > 0)
                {
                    return "2";
                }
                obj.LinkName = Request.Form["LinkName"];
                //添加
                return service.Alterinsert(obj) > 0 ? "1" : "0";

            }
            else
            {
                obj.LinkName = Request.Form["LinkName"];
                //修改
                return service.AlterUpdate(obj) ? "1" : "0";
            }
        }
        #endregion

        //修改状态
        public int AlterlistStatus(int groupId, int status)
        {
            SWfsBrandService bll = new SWfsBrandService();
            return bll.AlterStatus(groupId, status);
        }

        public ActionResult Imagelist(int groupId)
        {
            IEnumerable<SWfsListAlterPic> lis = brandService.getSWfsListAlterGroupListID(groupId);
            return View(lis);
        }

        //二级联动
        public string CategoryByManOrWomanChange(int aa)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder("<option value='-1'>品类</option>");
            if (aa == 0)
            {
                //女士
                sb.Append("<option value='" + AppSettingManager.AppSettings["WomenBags"] + "'>女士箱包</option>");
                sb.Append("<option value='" + AppSettingManager.AppSettings["WomenClothing"] + "'>女士服饰</option>");
                sb.Append("<option value='" + AppSettingManager.AppSettings["WomenShoes"] + "'>女士鞋靴</option>");
                sb.Append("<option value='" + AppSettingManager.AppSettings["WomenAccessories"] + "'>女士配饰</option>");
                sb.Append("<option value='" + AppSettingManager.AppSettings["WomenHomes"] + "'>女士家居</option>");
                sb.Append("<option value='" + AppSettingManager.AppSettings["WomenBeauty"] + "'>女士美妆</option>");
                sb.Append("<option value='" + AppSettingManager.AppSettings["WomenWatches"] + "'>女士腕表</option>");
            }
            else if (aa == 1)
            {
                //男士
                sb.Append("<option value='" + AppSettingManager.AppSettings["MenBags"] + "'>男士箱包</option>");
                sb.Append("<option value='" + AppSettingManager.AppSettings["MenClothing"] + "'>男士服饰</option>");
                sb.Append("<option value='" + AppSettingManager.AppSettings["MenShoes"] + "'>男士鞋靴</option>");
                sb.Append("<option value='" + AppSettingManager.AppSettings["MenAccessories"] + "'>男士配饰</option>");
                sb.Append("<option value='" + AppSettingManager.AppSettings["MenHomes"] + "'>男士家居</option>");
                sb.Append("<option value='" + AppSettingManager.AppSettings["MenBeauty"] + "'>男士美妆</option>");
                sb.Append("<option value='" + AppSettingManager.AppSettings["MenWatches"] + "'>男士腕表</option>");
            }
            else
            {
                //sb.Append("<option value='" + AppSettingManager.AppSettings["WomenBags"] + "'>女士箱包</option>");
                //sb.Append("<option value='" + AppSettingManager.AppSettings["WomenClothing"] + "'>女士服装</option>");
                //sb.Append("<option value='" + AppSettingManager.AppSettings["WomenShoes"] + "'>女士鞋靴</option>");
                //sb.Append("<option value='" + AppSettingManager.AppSettings["WomenAccessories"] + "'>女士配饰</option>");
                //sb.Append("<option value='" + AppSettingManager.AppSettings["WomenHomes"] + "'>女士家居</option>");
                //sb.Append("<option value='" + AppSettingManager.AppSettings["WomenBeauty"] + "'>女士美妆</option>");
                //sb.Append("<option value='" + AppSettingManager.AppSettings["WomenWatches"] + "'>女士腕表</option>");
                //sb.Append("<option value='" + AppSettingManager.AppSettings["MenBags"] + "'>女士箱包</option>");
                //sb.Append("<option value='" + AppSettingManager.AppSettings["MenClothing"] + "'>男士服装</option>");
                //sb.Append("<option value='" + AppSettingManager.AppSettings["MenShoes"] + "'>男士鞋靴</option>");
                //sb.Append("<option value='" + AppSettingManager.AppSettings["MenAccessories"] + "'>男士配饰</option>");
                //sb.Append("<option value='" + AppSettingManager.AppSettings["MenHomes"] + "'>男士家居</option>");
                //sb.Append("<option value='" + AppSettingManager.AppSettings["MenBeauty"] + "'>男士美妆</option>");
                //sb.Append("<option value='" + AppSettingManager.AppSettings["MenWatches"] + "'>男士腕表</option>");
            }
            return sb.ToString();
        }

        //清除缓存
        [HttpPost]
        public int clearPicCach(string category)
        {
            try
            {
                RedisCacheProvider.Instance.Remove("SWfsListAlterPicListByCategory" + category);
                return 1;
            }
            catch (Exception)
            {

                return 0;
            }
        }

        //删除分组同时删除图片
        public ActionResult AlterGropDelete(int groupId)
        {
            var dele = brandService.AlterDelete(groupId);
            return Redirect("AlterList.html?Gender=" + Request.QueryString["Gender"] + "&Category=" + Request.QueryString["Category"]);
        }

        #endregion

    }
}
