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
using System.Threading;
using System.Text.RegularExpressions;

namespace Shangpin.Ocs.Web.Areas.Outlet.Controllers
{
    //[OCSAuthorization]
    public class NoticeController : BaseController
    {   
        /// <summary>
        /// 预告页列表
        /// </summary>
        /// <param name="title"></param>
        /// <param name="datecreate"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public ActionResult Index(string title, string datecreate, int pageIndex = 1,int islimitedOutlet=2)
        {
            int pageSize = int.Parse(AppSettingManager.AppSettings["ComonListPageNum"].ToString());
            ViewBag.CurPage = pageIndex;
            ViewBag.PageSize = pageSize;
            int totalCount = 0;
            IList<SWfsAolaiAdvancePage> list = new SWfsAolaiAdvancePageService().GetList(title, datecreate, pageIndex, pageSize, islimitedOutlet, out totalCount);
            ViewBag.Count = totalCount;
            //list = list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();//默认每页显示20条数据
            ViewBag.List = list;
            ViewBag.Name = title ?? "";
            ViewBag.Time = datecreate ?? "";
            ViewBag.islimitedOutlet = islimitedOutlet;
            return View();
        }

        /// <summary>
        /// 预告页操作的页面展现
        /// </summary>
        /// <param name="id"></param>
        /// <param name="act"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ActionResult Manage(string id, string act, int value = 1)
        {
            SWfsAolaiAdvancePageService service = new SWfsAolaiAdvancePageService();
            SWfsAolaiAdvancePage modle = new SWfsAolaiAdvancePage();
            switch (act)
            {
                case "create"://新建
                    modle.SWfsAolaiNoticePageId = "0";
                    return View("/Areas/Outlet/Views/Notice/Manage.cshtml", modle);

                case "edit"://修改
                    modle = service.GetModel(id);
                    return View("/Areas/Outlet/Views/Notice/Manage.cshtml", modle);

                case "del":
                    service.Del(id);
                    return Redirect("/outlet/notice/index?islimitedOutlet=" + Request.QueryString["islimitedOutlet"]);

            }
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Manage(FormCollection formCol)
        {
            string tmpId = formCol.Get("SWfsAolaiNoticePageId");
            string pageName = formCol.Get("PageName");
            string pageDes = formCol.Get("PageDes");
            string pageDesKey = formCol.Get("PageDesKey");
            string pageStaticContent = formCol.Get("PageStaticContent");
            string islimitedOutlet = formCol.Get("islimitedOutlet");
            if (string.IsNullOrEmpty(islimitedOutlet))
            {
                return Json(new { reslut = "error", msg = "islimitedOutlet数据有误 网站所属不存在" });
            }

            SWfsAolaiAdvancePageService service = new SWfsAolaiAdvancePageService();
            SWfsAolaiAdvancePage model = new SWfsAolaiAdvancePage();

            if (!string.IsNullOrEmpty(tmpId) && !tmpId.Equals("0")) //修改
            {
                model = service.GetModel(tmpId);
            }
            else //新增
            {
                model.DateBegin = Convert.ToDateTime("1900-1-1");//暂时未用
                model.DateEnd = Convert.ToDateTime("1900-1-1");//暂时未用
                model.DateCreate = DateTime.Now;//暂时未用
                model.SWfsAolaiNoticePageId = new CommonService().GetNextCounterId("SWfsAolaiNoticePageId").ToString("00000");
            }
            model.BackUpField = string.IsNullOrWhiteSpace(formCol.Get("CreateType")) ? "1" : formCol.Get("CreateType");//1图片创建 2代码创建
            model.OperatorUserId = PresentationHelper.GetPassport().UserName;
            model.PageDes = pageDes;
            model.PageDesKey = pageDesKey;
            model.PageName = pageName;
            model.PageStaticContent = pageStaticContent;
            model.IsLimitedOutlet = int.Parse(islimitedOutlet);

            if (!string.IsNullOrEmpty(tmpId) && !tmpId.Equals("0")) //修改
            {
                try
                {
                    service.Update(model);
                    return Json(new { reslut = "success", msg = "修改成功" });
                }
                catch (Exception e)
                {
                    return Json(new { reslut = "error", msg = e.Message });
                }
            }
            else
            {
                try
                {
                    service.Add(model);
                    return Json(new { reslut = "success", msg = "添加成功" });
                }
                catch (Exception e)
                {
                    return Json(new { reslut = "error", msg = e.Message });
                }
            }
        }

        public ActionResult PicIndex(string id, int pageIndex = 1)
        {
            int pageSize = int.Parse(AppSettingManager.AppSettings["ComonListPageNum"].ToString());
            ViewBag.CurPage = pageIndex;
            ViewBag.PageSize = pageSize;

            SWfsAolaiAdvancePageService service = new SWfsAolaiAdvancePageService();
            SWfsAolaiAdvancePage model = service.GetModel(id);
            ViewBag.PageName = model.PageName;
            ViewBag.SwfsAolaiNoticePageId = model.SWfsAolaiNoticePageId;
            IList<SWfsAolaiAdvancePagePic> list = service.GetPicList(id);
            ViewBag.Count = list.Count();
            list = list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();//默认每页显示20条数据
            ViewBag.List = list;
            return View();
        }

        [HttpPost]
        public JsonResult UploadImg(HttpPostedFileBase fileData)
        {
            if (fileData != null)
            {
                try
                {
                   CommonService commonService = new CommonService();
                   Dictionary<string,string> dicRs  = commonService.PostImg(Request.Files["Filedata"], "width:0,Height:0,Length:0");
                   string picNo = dicRs.Values.FirstOrDefault();
                   string picKey = dicRs.Keys.FirstOrDefault();
                   if (!string.IsNullOrEmpty(picNo) && picKey != "error")
                   {
                       return Json(new { result = true, message = picNo });
                   }
                   return Json(new { result = false, message = "上传保存错误" });
                }
                catch (Exception ex)
                {
                    return Json(new { result = false, Message = ex.Message });
                }
            }
            else
            {
                return Json(new { result = false, Message = "请选择要上传的文件！" });
            }
                 
        }

        /// <summary>
        /// 预告页 页内图片操作的页面展现
        /// </summary>
        /// <param name="id"></param>
        /// <param name="act"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ActionResult PicManage(string id, string act, string picId)
        {
            SWfsAolaiAdvancePageService service = new SWfsAolaiAdvancePageService();
            SWfsAolaiAdvancePagePic modle = new SWfsAolaiAdvancePagePic();
            switch (act)
            {
                case "create"://新建
                    SWfsAolaiAdvancePage model = service.GetModel(id);
                    modle.SWfsAolaiNoticePagPicId = "0";
                    modle.SwfsAolaiNoticePageId = id;
                    ViewBag.PageName = model.PageName;
                    return View("/Areas/Outlet/Views/Notice/PicManage.cshtml", modle);

                case "edit"://修改
                    modle = service.GetPicModel(picId);
                    return View("/Areas/Outlet/Views/Notice/PicManage.cshtml", modle);

                case "del":
                    service.DelPic(picId);
                    return Redirect("/outlet/notice/PicIndex?id=" + id + CommonService.GetTimeStamp("&"));
            }
            return View();
        }

        [HttpPost]
        public ActionResult PicManage(FormCollection formCol)
        {
            string tmpPicId = formCol.Get("SWfsAolaiNoticePagPicId");
            string tmpPageId = formCol.Get("SwfsAolaiNoticePageId");
            string pictureName = formCol.Get("PictureName");
            string pictureLink = formCol.Get("PictureLink");
            string sort = formCol.Get("Sort");

            SWfsAolaiAdvancePageService service = new SWfsAolaiAdvancePageService();
            SWfsAolaiAdvancePagePic model = new SWfsAolaiAdvancePagePic();

            if (!string.IsNullOrEmpty(tmpPicId) && !tmpPicId.Equals("0")) //修改
            {
                model = service.GetPicModel(tmpPicId);
            }
            else //新增
            {
                model.BackUpField = string.Empty;
                model.DateCreate = DateTime.Now;//暂时未用
                model.SWfsAolaiNoticePagPicId = new CommonService().GetNextCounterId("SWfsAolaiNoticePagPicId").ToString("00000");
            }
            model.PictureName = pictureName;
            model.PictureLink = pictureLink;
            model.Sort = int.Parse(sort);
            model.SwfsAolaiNoticePageId = tmpPageId;
            CommonService commonService = new CommonService();
            Dictionary<string, string> rsPic = new Dictionary<string, string>();

            if (null != Request.Files["PictureFileNo"] && Request.Files["PictureFileNo"].ContentLength > 0)
            {
                rsPic = commonService.PostImg(Request.Files["PictureFileNo"], "width:0,Height:0,Length:300");
                if (rsPic.Keys.Contains("error"))
                {
                    return Json(new { reslut = "error", msg = rsPic["error"] });
                }
                if (rsPic.Keys.Contains("success"))
                {
                    model.PictureFileNo = rsPic["success"];
                }
            }


            if (!string.IsNullOrEmpty(tmpPicId) && !tmpPicId.Equals("0")) //修改
            {
                try
                {
                    service.UpdatePagePic(model);
                    return Json(new { reslut = "success", msg = "修改成功" });
                }
                catch (Exception e)
                {
                    return Json(new { reslut = "error", msg = e.Message });
                }
            }
            else
            {
                try
                {
                    service.AddPic(model);
                    return Json(new { reslut = "success", msg = "添加成功" });
                }
                catch (Exception e)
                {
                    return Json(new { reslut = "error", msg = e.Message });
                }
            }
        }

        /// <summary>
        /// 图片上传部分视图页面
        /// </summary>
        /// <param name="id"></param>
        /// <param name="act"></param>
        /// <returns></returns>
        public ActionResult PartUpoladPicPage(string id, string act,int pageIndex=1,int pageSize=100)
        {
            SWfsAolaiAdvancePageService service = new SWfsAolaiAdvancePageService();
            SWfsAolaiAdvancePage model;
            List<string> piclist = new List<string>();
            switch (act)
            {
                case "edit"://修改
                    model = service.GetModel(id);
                    //如果里面有lasy 且值为图片信息 则读取真是图片的链接生成  20140423164035918507
                    string pattern = @"\d{20}";
                    Regex regObj = new Regex(pattern);
                    MatchCollection results = regObj.Matches(model.PageStaticContent);
                    foreach (Match item in results)
                    {
                        piclist.Add(item.Value);//图片编号
                    }
                    break;
                default:
                    model = new SWfsAolaiAdvancePage();
                    break;
            }
            ViewBag.Current = pageIndex;
            ViewBag.PageSize = pageSize;
            ViewBag.Total = piclist.Count;

            piclist = piclist.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.PicList = piclist;
            if (!string.IsNullOrEmpty(Request.QueryString["flag"])&&Request.QueryString["flag"].Equals("js"))
            {
                return Json(new { rs = true, content = RenderPartialViewToString("/Areas/Outlet/Views/Notice/PartUpoladPicPage.cshtml", model) }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return PartialView(model);
            }
        }

        /// <summary>
        /// 预告页列表
        /// </summary>
        /// <param name="title"></param>
        /// <param name="datecreate"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetList(string title = "", string datecreate = "", int pageIndex = 1,int islimitedOutlet=2)
        {
            //Thread.Sleep(2000);
            int pageSize = int.Parse(AppSettingManager.AppSettings["ComonListPageNum"].ToString());
            ViewBag.CurPage = pageIndex;
            ViewBag.PageSize = pageSize;
            int totalCount = 0;
            IList<SWfsAolaiAdvancePage> list = new SWfsAolaiAdvancePageService().GetList(title, datecreate, pageIndex, pageSize, islimitedOutlet, out totalCount);
            ViewBag.Count = totalCount;
            list = list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();//默认每页显示20条数据

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 预热页面列表 by zhangwei 20140321
        /// </summary>
        /// <param name="key">预热页名称</param>
        /// <returns></returns>
        public ActionResult SelectPrdheatHtml(string id = "", string key = "", int pageIndex = 1, int pageSize = 12, int islimitedOutlet=2)
        {
            ViewBag.CurPage = pageIndex;
            ViewBag.PageSize = 12;
            ViewBag.PrdheatID = id;
            int totalCount = 0;
            key = string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(key) ? key : "";
            IList<SWfsAolaiAdvancePage> list = new SWfsAolaiAdvancePageService().GetList(key, "", pageIndex, pageSize, islimitedOutlet, out totalCount);
            ViewBag.Count = totalCount;
            list = list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();//默认每页显示20条数据
            ViewBag.PrdheadList = list;
            return View();
        }
    }
}
