using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using Shangpin.Ocs.Service.Shangpin;
using Shangpin.Ocs.Service;
using Shangpin.Entity.Wfs;
using Shangpin.Ocs.Entity.Extenstion.ShangPin;

namespace Shangpin.Ocs.Web.Areas.Shangpin.Controllers
{
    public class AppIndexMamagerController : Controller
    {
        //
        // GET: /Shangpin/AppIndexMamager/

        private readonly AppIndexService appIndexService;
        private Dictionary<string, string> rsPic = new Dictionary<string, string>();
        public AppIndexMamagerController()
        {
            appIndexService = new AppIndexService();
        }

        //首页
        public ActionResult Index()
        {
            AppIndexParams model = new AppIndexParams();
            IEnumerable<SWfsAppShopCategory> shopCategorylist = appIndexService.GetAppShopCategoryList();
            SWfsAppOperatingPosition operatingPositionOne = appIndexService.GetAppOperatingPositionOne();
            model.ShopCategoryList = shopCategorylist;
            model.OperatingPositionOne = operatingPositionOne;
            model.HomeAlterPic = appIndexService.GetHomeAlterPic();
            return View(model);
        }

        #region 轮播图管理
        public ActionResult AlterPicEdit(string id, string AlterType = "0")
        {

            SWfsAppAlterPic entity;
            if (string.IsNullOrEmpty(id))
            {
                entity = new SWfsAppAlterPic();
            }
            else 
            {
                entity = appIndexService.GetAppAlterPicById(id);
            }
            if (entity==null)
            {
                entity = new SWfsAppAlterPic();
            }
            else
            {
                if (entity.RefType == 2)//查询品牌
                {
                    ViewBag.BrandName = appIndexService.GetBrandName(entity.RefContent);
                }
                if (entity.RefType==0)//查询活动
                {
                    ViewBag.SubjectObj = appIndexService.GetSubjectByNo(entity.RefContent);
                }
            }

            return View(entity);
        }
        [HttpPost]
        public ActionResult AlterPicEdit(FormCollection f)
        {
            int id = Convert.ToInt32(f["AppSlterPicId"]);
            SWfsAppAlterPic entity = new SWfsAppAlterPic();
            HttpPostedFileBase file = Request.Files["picFile"];
            entity.Name = f["Name"];

            entity.AlterType = Convert.ToInt16(f["AlterType"]);
            entity.Sort = Convert.ToInt32(f["Sort"]);
            entity.StartTime = DateTime.Parse(f["StartTime"]);
            entity.RefType = Convert.ToInt16(f["RefType"]);
            entity.RefContent = f["RefContent"];
            entity.CreateDate = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
            int sameTimeID = appIndexService.IsExistSameTimeAlter(entity.StartTime.ToString("yyyy-MM-dd HH:mm"), entity.Sort);
            CommonService commonService = new CommonService();
            rsPic.Clear();
            if (file != null && file.ContentLength > 0)
            {
                rsPic = commonService.PostImg(file, "width:0 ,Height:0,Length:100");
                if (rsPic.Keys.Contains("error"))
                {
                    ViewData["tip"] = "<script>alert('" + rsPic["error"] + "')</script>";
                    entity.PicNo = Request.Form["PicNo"];
                    return View(entity);
                }
                entity.PicNo = rsPic["success"];
            }
            else
            {
                if (!string.IsNullOrEmpty(Request.Form["PicNo"]))
                {
                    entity.PicNo = Request.Form["PicNo"];
                }
            }
            if (id > 0)
            {
                entity.AppSlterPicId = id;
                //验证是否重复时间的轮播图
                if (sameTimeID > 0 && sameTimeID != entity.AppSlterPicId)
                {
                    ViewData["tip"] = "<script>alert('已经存在开始时间相同的轮播图')</script>";
                    return View(entity);
                }
            }
            else 
            {
                if (sameTimeID > 0)
                {
                    ViewData["tip"] = "<script>alert('已经存在开始时间相同的轮播图')</script>";
                    return View(entity);
                }
            }
            
            

            if (string.IsNullOrEmpty(entity.PicNo))
            {
                entity.PicNo = "";
            }
            
            int result = id > 0 ? (appIndexService.UpdateAppAlterPic(entity) ? 1 : 0) : appIndexService.InsertAppAlterPic(entity);
            return Redirect("/shangpin/AppIndexMamager/AlterPicList?AlterType=" + Request.Form["AlterType"]);
        }

        public JsonResult GetSWfsNewSubjectById(string id)
        {
            SWfsNewSubject swf = appIndexService.GetSWfsNewSubjectById(id);
            return Json(new
            {
                id = swf.SubjectNo,
                name = swf.SubjectName,
                status = swf.Status == 1 ? "开启" : "关闭",
                startTime = swf.DateBegin.ToString("yyyy-MM-dd hh:mm:ss"),
                endTime = swf.DateEnd.ToString("yyyy-MM-dd hh:mm:ss"),
                imgID = swf.SubjectTemplateTopPicNo

            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AlterPicList(string AlterType, string Name, string beginTime = "", string endTime = "", int RefType = -1, int Sort = -1, int pageIndex = 1, int pageSize = 20)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic["Name"] = string.IsNullOrEmpty(Name) ? "" : Name;
            dic["RefType"] = RefType;
            dic["Sort"] = Sort;
            dic["beginTime"] = string.IsNullOrEmpty(beginTime) ? "" : beginTime;
            dic["endTime"] = string.IsNullOrEmpty(endTime)?"":DateTime.Parse(endTime).AddDays(1).ToShortDateString();
            dic["AlterType"] = Convert.ToInt16(AlterType);
            int count;
            IEnumerable<SWfsAppAlterPic> list = appIndexService.AlterLists(pageIndex, pageSize, dic, out count);
            ViewBag.list = list.ToList();
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;
            ViewBag.TotalCount = count;
            return View();
        }
        public JsonResult DelAlterPicById(string id)
        {
            return Json(new { status = appIndexService.DelAppAlterPic(id) }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 榜单管理

        #endregion

        #region 搜索页商城分类  运营位管理
        /// <summary>
        /// 得到 搜索页商城分类 和 运营位管理 的数据
        /// </summary>
        /// <returns></returns>
        public ActionResult ShopCategory()
        {
            AppIndexParams model = new AppIndexParams();
            IEnumerable<SWfsAppShopCategory> shopCategorylist = appIndexService.GetAppShopCategoryList();
            //IEnumerable<SWfsAppOperatingPosition> operatingPositionlist = appIndexService.GetAppOperatingPositionList();
            model.ShopCategoryList = shopCategorylist;
            //model.OperatingPositionList = operatingPositionlist;

            return View(model);
        }


        /// <summary>
        /// 保存搜索页商城分类
        /// </summary>
        /// <param name="shopCategorySort"></param>
        /// <param name="shopCategoryNo"></param>
        /// <param name="picno"></param>
        /// <param name="imgWidth"></param>
        /// <param name="imgHeight"></param>
        /// <param name="imgLength"></param>
        /// <returns></returns>
        [HttpPost]
        public string AddShopCategory(int shopCategorySort, string shopCategoryNo, string picno, int imgWidth, int imgHeight, int imgLength)
        {
            HttpPostedFileBase file = Request.Files["shopCategoryImgfile"];
            if (file != null && file.ContentLength > 0)
            {
                SaveImg(file, imgWidth, imgHeight, imgLength);
                if (rsPic.Keys.Contains("error"))
                {
                    return "{\"status\":1,\"message\":\"" + rsPic["error"] + "\"}";
                }
                else
                {
                    picno = rsPic["success"];
                }
            }

            if (picno.Length > 0)//上传成功或已有图
            {
                SWfsAppShopCategory obj = new SWfsAppShopCategory();
                obj.Sort = shopCategorySort;
                obj.CategoryNo = shopCategoryNo;
                obj.PicNo = picno;
                obj.CreateDate = DateTime.Now;
                appIndexService.UpdateAppShopCategory(obj);

                return "{\"status\":0,\"message\":\"" + picno + "\"}";
            }
            else
            {
                return "{\"status\":1,\"message\":\"图片上传失败\"}";
            }
        }

        public string AddShopCategory2(int categoryCount, string picno, int imgWidth, int imgHeight, int imgLength)
        {
            string errorInfo = string.Empty;
            HttpPostedFileBase file = Request.Files["shopCategoryImgfile"];
            if (file != null && file.ContentLength > 0)
            {
                SaveImg(file, imgWidth, imgHeight, 100);
                if (rsPic.Keys.Contains("error")) 
                {
                    return "{\"status\":1,\"message\":\"" + rsPic["error"] + "\"}";
                }
                else
                {
                    picno = rsPic["success"];
                }
            }

            if (picno.Length > 0)//上传成功或已有图
            {
                for (int i = 1; i <= categoryCount; i++)
                {
                    SWfsAppShopCategory obj = new SWfsAppShopCategory();
                    obj.Sort = i;
                    obj.CategoryNo = Request.Form["shopCategoryNo" + i.ToString()];
                    obj.PicNo = picno;
                    obj.CreateDate = DateTime.Now;
                    appIndexService.UpdateAppShopCategory(obj);
                }
                if (categoryCount == 6)
                {
                    appIndexService.DeleteAppShopCategoryBySorts(7, 8, 9);
                }

                return "{\"status\":0,\"message\":\"" + picno + "\"}";
            }
            else
            {
                return "{\"status\":1,\"message\":\"图片上传失败\"}";
            }
        }

        /// <summary>
        /// 保存运营位管理（1张图1个链接）
        /// </summary>
        /// <param name="operatingPositionSort"></param>
        /// <param name="operatingPositionLinkUrl"></param>
        /// <param name="picno"></param>
        /// <param name="imgWidth"></param>
        /// <param name="imgHeight"></param>
        /// <param name="imgLength"></param>
        /// <returns></returns>
        [HttpPost]
        public string AddOperatingPosition(int operatingPositionSort=0, string operatingPositionLinkUrl="", string picno="", int imgWidth=0, int imgHeight=0, int imgLength=0)
        {
            SWfsAppOperatingPosition obj = new SWfsAppOperatingPosition();
            HttpPostedFileBase file = Request.Files["operatingPositionImgfile"];
            if (file != null && file.ContentLength > 0)
            {
                SaveImg(file, imgWidth, imgHeight, 100);
                if (rsPic.Keys.Contains("error"))
                {
                    return "{\"status\":1,\"message\":\"" + rsPic["error"] + "\"}";
                }
                else
                {
                    obj.PicNo = rsPic["success"];
                }
            }
            else
            {
                obj.PicNo = picno;
            }
            if (string.IsNullOrEmpty(obj.PicNo))
            {
                obj.PicNo = "";
            }
            obj.OperatingPositionId = operatingPositionSort;
            obj.Sort = 0;
            obj.LinkUrl = operatingPositionLinkUrl;
            obj.CreateDate = DateTime.Now;
            appIndexService.UpdateAppOperatingPosition(obj);
            return "{\"status\":0,\"message\":\"操作成功\"}";
            
        }

        public ActionResult DeleteOperatingPositionBySort(int sort)
        {
            return Json(new { status = appIndexService.DeleteAppOperatingPositionBySorts(sort) }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult OperatingPositionList(int pageIndex = 1, int pageSize = 20)
        {
            int count;
            IEnumerable<SWfsAppOperatingPosition> list = appIndexService.GetAppOperatingPositionList(pageIndex, pageSize, out count);
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;
            ViewBag.TotalCount = count;
            return View(list);
        }

        /// <summary>
        /// 根据父类ID得到（状态为有效的）类别串
        /// </summary>
        /// <param name="parentNo">父类ID</param>
        /// <param name="selectedNo">选定的项编号</param>
        /// <returns></returns>
        public string GetCategoryListString(string parentNo, string selectedNo)
        {
            IEnumerable<Category> list = appIndexService.GetCategoryList(parentNo);
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            if (list != null)
            {
                foreach (var item in list)
                {
                    if (item.CategoryNo == selectedNo)
                    {
                        sb.Append("<option value='" + item.CategoryNo + "' selected='selected'>" + item.CategoryName + "</option>");
                    }
                    else
                    {
                        sb.Append("<option value='" + item.CategoryNo + "'>" + item.CategoryName + "</option>");
                    }
                }
            }
            return sb.ToString();
        }

        //修改排序值
        public int EditePositionSort(int id,int sort)
        {
            return appIndexService.EditePositionSort(id,sort);
        }
        #endregion

        #region 上传图片，检查图片格式  公共方法
        public string SaveImg(HttpPostedFileBase file, int imgWidth = 0, int imgHeight = 0, int imgLength = 100)
        {
            CommonService commonService = new CommonService();
            rsPic.Clear();
            if (file != null && file.ContentLength > 0)
            {
                rsPic = commonService.PostImg(file, "width:" + imgWidth + ",Height:" + imgHeight + ",Length:" + imgLength);
                if (rsPic.Keys.Contains("success"))
                {
                    return rsPic["success"];
                }
                if (rsPic.Keys.Contains("error"))
                {
                    return "";
                }
            }
            return "";
        }
        #endregion

        #region 根据热度商品推荐管理
        //根据搜索条件生成搜索参数


        
        #endregion
    }

    public class AppIndexParams
    {
        public IEnumerable<SWfsAppShopCategory> ShopCategoryList { get; set; }

        public SWfsAppOperatingPosition OperatingPositionOne { get; set; }

        public List<string> HomeAlterPic { get; set; }
    }
}
