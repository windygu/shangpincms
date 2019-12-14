using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Shangpin.Ocs.Service.Common;
using Shangpin.Entity.Wfs;
using Shangpin.Ocs.Service.Outlet;
using Shangpin.Ocs.Service;

namespace Shangpin.Ocs.Web.Areas.Outlet.Controllers
{

    [OCSAuthorization]
    public class GroupTimeController : Controller
    {
        //
        // GET: /Outlet/SubjectGroup/

        public ActionResult Index(string name = "", int pageIndex = 1)
        {
            name = name.Trim();
            int pageSize = 10;
            ViewBag.CurPage = pageIndex;
            ViewBag.PageSize = pageSize;
            IList<SWfsSubjectTimeGroup> list = new SubjectTimeGroupService().GetList(name, 2);//1网站 2移动端
            ViewBag.Count = list.Count();
            list = list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();//默认每页显示20条数据
            ViewBag.List = list;
            ViewBag.Name = name ?? "";
            return View();
        }

        /// <summary>
        /// 创建和修改的呈现
        /// </summary>
        /// <returns></returns>
        public ActionResult Manage(string act, int gid)
        {
            SWfsSubjectTimeGroup model = new SWfsSubjectTimeGroup();
            ViewBag.Act = act;
            SubjectTimeGroupService service = new SubjectTimeGroupService();
            switch (act)
            {
                case "del"://删除
                    service.Del(gid);
                    return RedirectToAction("Index");

                case "create"://新建
                    model.DateBegin = Convert.ToDateTime("1900-1-1");
                    model.DateEnd = Convert.ToDateTime("1900-1-1");
                    return View("/Areas/Outlet/Views/GroupTime/Manage.cshtml", model);

                case "edit"://修改

                    model = service.GetModelById(gid);
                    return View("/Areas/Outlet/Views/GroupTime/Manage.cshtml", model);
            }
            return View();
        }


        /// <summary>
        /// 创建或修改
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Manager(SWfsSubjectTimeGroup form)
        {
            
            form.GroupName = form.GroupName.Trim();
            if (string.IsNullOrWhiteSpace(form.GroupName))
            {
               return Json(new { reslut = "error", msg = "请填写分组名称" });
            }
            if (form.GroupName.Trim().Length>10)
            {
                return Json(new { reslut = "error", msg = "分组名称不能超过10个汉字" });
            }
            if (form.DateBegin == null)
            {
                return Json(new { reslut = "error", msg = "分组开始时间不能为空" });
            }
            if (form.DateEnd == null)
            {
                return Json(new { reslut = "error", msg = "分组结束时间不能为空" });
            }
            SubjectTimeGroupService service = new SubjectTimeGroupService();//验证数据
            var file = Request.Files["picFileNo"];
            
            Dictionary<string, string> rsPic;
            if (form.GID <= 0)//写入
            {

                if (file == null || file.ContentLength <= 0)
                {
                    return Json(new { reslut = "error", msg = "请上传分组图片" });
                }
                SWfsSubjectTimeGroup originNameModel = service.SelectByName(form.GroupName);//查询是否存在相同的分组名称
                if (originNameModel != null && originNameModel.GroupName == form.GroupName)
                {
                    return Json(new { reslut = "error", msg = string.Format("分组名称{0}已经存在", form.GroupName) });
                }
                SWfsSubjectTimeGroup originTimeModel = service.SelectByTime(form.DateBegin, form.DateEnd, form.GID);//查询是否存在相同的分组名称
                if (originTimeModel != null)
                {
                    return Json(new { reslut = "error", msg = string.Format("当前时间段有重复分组{0}—{1}", originTimeModel.DateBegin.ToShortTimeString(), originTimeModel.DateEnd.ToShortTimeString()) });
                }
                form.CreateUserId = PresentationHelper.GetPassport().UserName;
                form.DateCreate = DateTime.Now;
                form.DateUpdate = DateTime.Now;
                form.UpdateUserId = PresentationHelper.GetPassport().UserName;
                form.Status = 1;
                form.GroupPicLink = "";
                form.GroupPicType = 0;
                form.ShowType = 2;
                rsPic = fileNo(file);
                if (!rsPic.Keys.Contains("success"))
                {
                    return Json(new { reslut = "error", msg = "请上传符合要求的图片" });
                }
                form.GroupPicNo = rsPic["success"];
                try
                {
                    service.Add(form);
                }
                catch (Exception e)
                {
                    return Json(new { reslut = "error", msg = e.Message });
                }

            }
            else //修改
            {
                SWfsSubjectTimeGroup model = service.GetModelById(form.GID);
                model.GroupName = form.GroupName;
                if (file != null && file.ContentLength > 0)
                {
                    rsPic = fileNo(file);
                    if (!rsPic.Keys.Contains("success"))
                    {
                        return Json(new { reslut = "error", msg = "请上传符合要求的图片" });
                    }
                    model.GroupPicNo = rsPic["success"];
                }
                SWfsSubjectTimeGroup originNameModel = service.SelectByName(form.GroupName);//查询是否存在相同的分组名称
                if (originNameModel != null && originNameModel.GroupName == form.GroupName && originNameModel.GID != form.GID)
                {
                    return Json(new { reslut = "error", msg = string.Format("分组名称{0}已经存在", form.GroupName) });
                }
                SWfsSubjectTimeGroup originTimeModel = service.SelectByTime(form.DateBegin, form.DateEnd, form.GID);//查询是否存在相同的分组名称                
                if (originTimeModel != null)
                {
                    return Json(new { reslut = "error", msg = string.Format("当前时间段有重复分组{0}—{1}", originTimeModel.DateBegin.ToShortTimeString(), originTimeModel.DateEnd.ToShortTimeString()) });
                }
                try
                {
                    model.DateBegin = form.DateBegin;
                    model.DateEnd = form.DateEnd;
                    service.Update(model);
                }
                catch (Exception e)
                {
                    return Json(new { reslut = "error", msg = e.Message });
                }

            }
            return Json(new { reslut = "success", msg = "操作成功" });
        }

        private Dictionary<string, string> fileNo(HttpPostedFileBase file)
        {
            Dictionary<string, string> rsPic = new Dictionary<string, string>();
            if (file != null && file.ContentLength > 0)
            {
                string imgSize = AppSettingManager.AppSettings["AolaiSubjectGroupTime"];
                CommonService commonService = new CommonService();
                rsPic = commonService.PostImg(Request.Files["picFileNo"], imgSize);
                //if (rsPic.Keys.Contains("error"))
                //{
                //    return "error";
                //}
                //if (rsPic.Keys.Contains("success"))
                //{
                //   return rsPic["success"];
                //}
            }
            return rsPic;
        }
    }
}
