using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Shangpin.Ocs.Service.Outlet;
using Shangpin.Entity.Wfs;
using Shangpin.Ocs.Entity.Extenstion.Outlet;
using Shangpin.Ocs.Service.Common;
using Shangpin.Ocs.Service;
using System.Text;

namespace Shangpin.Ocs.Web.Areas.Outlet.Controllers
{
    [OCSAuthorization]
    public class IndexFouctPicController : Controller
    {
        //
        // GET: /Outlet/IndexFouctPic/

        public ActionResult Index(string subjectNoName = "", string startTime = "", string endTime = "", int pageIndex = 1, int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(subjectNoName) || subjectNoName == "活动名称/活动编号")
            {
                subjectNoName = "";
            }
            SWfsSubjectFocusAreaService service = new SWfsSubjectFocusAreaService();
            int totalCount = 0;
            IList<SWfsSubjectFocusUIModel> list = service.GetList(subjectNoName, startTime, endTime, pageIndex, pageSize, out totalCount);

            ViewBag.subjectNoName = subjectNoName;
            ViewBag.startTime = startTime;
            ViewBag.endTime = endTime;

            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;
            ViewBag.totalCount = totalCount;
            ViewBag.DataList = list;
            return View();
        }

        [HttpGet]
        public ActionResult Add()
        {
            string Id = Request.QueryString["id"];
            string act = Request.QueryString["act"];
            SWfsSubjectFocusArea model = new SWfsSubjectFocusArea();
            if (!string.IsNullOrEmpty(Id) && !string.IsNullOrEmpty(act) && act.Equals("edit"))
            {
                SWfsSubjectFocusAreaService service = new SWfsSubjectFocusAreaService();
                model = service.GetModel(Id);
            }
            if (!string.IsNullOrEmpty(Id) && !string.IsNullOrEmpty(act) && act.Equals("del"))
            {
                SWfsSubjectFocusAreaService service = new SWfsSubjectFocusAreaService();//删除
                service.DelById(Id);
                return RedirectToAction("Index", new { rd = new Random().Next(0, 100) });
            }
            return View(model);

        }

        [HttpPost]
        public ActionResult Add(DateTime showDate, string subjectNoes)
        {
            List<string> existNoes = new List<string>();
            SWfsSubjectFocusAreaService service = new SWfsSubjectFocusAreaService();
            List<SWfsSubjectFocusArea> existList = new List<SWfsSubjectFocusArea>();
            string[] tempNoArry = null;
            string type = Request.Form["Type"];
            string webUrl = Request.Form["WebUrl"];
            string mobileUrl = Request.Form["MobileUrl"];
            //判断是修改还是写入
            int ID = 0;
            if (Request.Form["ID"] != null)
            {
                ID = int.Parse(Request.Form["ID"]);
            }

            #region 写入前数据验证
            if (showDate == null || showDate == new DateTime())
            {
                return Json(new { rs = "error", message = "请选择日期" });
            }
            if (showDate > new DateTime(9999, 12, 31) || showDate < new DateTime(1700, 01, 01))
            {
                return Json(new { rs = "error", message = "选择日期非法" });
            }
            #region 专题活动类型
            if (type == "1")
            {
                if (string.IsNullOrWhiteSpace(subjectNoes))
                {
                    return Json(new { rs = "error", message = "请填写活动编号" });
                }
                string[] subjectNoArry = subjectNoes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < subjectNoArry.Length; i++)
                {
                    subjectNoArry[i] = subjectNoArry[i].Trim();
                }

                //if (subjectNoArry.Length > 3)
                //{
                // return Json(new { rs = "error", message = "请不要填写超过三个活动编号" });
                //}

                tempNoArry = subjectNoArry.Distinct().ToArray();
                if (tempNoArry.Length != subjectNoArry.Length)
                {
                    return Json(new { rs = "error", message = "请不要填写重复活动编号" });
                }
                existList = service.GetSWfsSubjectFocusAreaList(showDate);//当前日期下已存在的活动编号
                existNoes = (from b in existList select b.SubjectNo).ToList();
                //if (existList.Count() + tempNoArry.Length > 3)
                //{
                // return Json(new { rs = "error", message = string.Format("当前显示日期已经添加了{0}个活动,只能再添加{1}个活动", existList.Count(), 3 - existList.Count()) });
                //}
            }
            #endregion
            else
            {
                if (Request.Form["ID"] == "0")
                {
                    if (Request.Files["WebPic"] == null || Request.Files["WebPic"].ContentLength == 0)
                    {
                        return Json(new { rs = "error", message = "请上传网站图片" });
                    }
                    if (Request.Files["MobilePic"] == null || Request.Files["MobilePic"].ContentLength == 0)
                    {
                        return Json(new { rs = "error", message = "请上传移动端图片" });
                    }
                }
            }
            #endregion
            #region  修改
            if (ID > 0)//修改时，只能有一个活动编号
            {
                if (type == "1")
                {
                    if (tempNoArry.Length > 1)
                    {
                        return Json(new { rs = "error", message = string.Format("修改状态下只能填写一个活动编号") });
                    }
                    SWfsSubjectFocusArea tempList = existList.Where(r => r.ShowDate.Equals(showDate)).Where(r => r.SubjectNo.Equals(tempNoArry[0])).FirstOrDefault();
                    if (tempList != null && tempList.ID != ID)
                    {
                        return Json(new { rs = "error", message = string.Format("当前日期下活动{0}已经存在", tempNoArry[0]) });
                    }
                    string msg = ValidSubject(tempNoArry);
                    if (!string.IsNullOrEmpty(msg))
                    {
                        return Json(new { rs = "error", message = msg });
                    }

                }
                if (string.IsNullOrWhiteSpace(Request.Form["sort"]))
                {
                    return Json(new { rs = "error", message = string.Format("排序值不能为空") });
                }
                try
                {
                    int.Parse(Request.Form["sort"]);
                }
                catch
                {
                    return Json(new { rs = "error", message = "排序号错误" });
                }
                if (int.Parse(Request.Form["sort"]) <= 0)
                {
                    return Json(new { rs = "error", message = "排序号请填写大于0的正整数" });
                }
                SWfsSubjectFocusArea model = service.GetModel(ID.ToString());
                model.ShowDate = showDate;
                model.Sort = int.Parse(Request.Form["sort"].ToString());
                model.Type = Convert.ToInt16(type);
                if (type == "1")
                {
                    model.SubjectNo = subjectNoes;
                    model.WebUrl = "";
                    model.WebPic = "";
                    model.MobileUrl = "";
                    model.MobilePic = "";
                }
                else
                {
                    if (Request.Files["WebPic"] == null || Request.Files["WebPic"].ContentLength == 0)
                    {
                        if (string.IsNullOrEmpty(Request.Form["WebPicNo"].ToString()))
                        {
                            return Json(new { rs = "error", message = "请上传网站图片！" }, "text/plain", Encoding.UTF8, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            model.WebPic = model.WebPic;
                        }
                    }
                    else
                    {
                        Dictionary<string, string> webPic = new CommonService().PostImg(Request.Files["WebPic"], AppSettingManager.AppSettings["ToppicFlapPic"].ToString(), ".jpg/.gif");
                        string webPicNo = webPic.Values.FirstOrDefault();
                        string webPicNokey = webPic.Keys.FirstOrDefault();
                        if (webPicNokey != "error")
                        {
                            model.WebPic = webPicNo;
                        }
                        else
                        {
                            return Json(new { rs = "error", message = webPicNo }, "text/plain", Encoding.UTF8, JsonRequestBehavior.AllowGet);
                        }
                    }
                    if (Request.Files["MobilePic"] == null || Request.Files["MobilePic"].ContentLength == 0)
                    {
                        if (string.IsNullOrEmpty(Request.Form["MobilePicNo"].ToString()))
                        {
                            return Json(new { rs = "error", message = "请上传移动端图片！" }, "text/plain", Encoding.UTF8, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            model.MobilePic = model.MobilePic;
                        }
                    }
                    else
                    {
                        Dictionary<string, string> mobilePic = new CommonService().PostImg(Request.Files["MobilePic"], AppSettingManager.AppSettings["ToppicIPhonePicFileNo"].ToString(), ".jpg/.png");
                        string mobilePicNo = mobilePic.Values.FirstOrDefault();
                        string mobilePicNokey = mobilePic.Keys.FirstOrDefault();
                        if (mobilePicNokey != "error")
                        {
                            model.MobilePic = mobilePicNo;
                        }
                        else
                        {
                            return Json(new { rs = "error", message = mobilePicNo }, "text/plain", Encoding.UTF8, JsonRequestBehavior.AllowGet);
                        }
                    }
                    model.SubjectNo = "";
                    model.WebUrl = webUrl;
                    model.MobileUrl = mobileUrl;
                }
                model.DateUpdate = DateTime.Now;
                model.UpdateUserId = PresentationHelper.GetPassport().UserName;
                try
                {
                    service.Update(model);
                }
                catch (Exception e)
                {
                    return Json(new { rs = "error", message = e.Message }, "text/plain", Encoding.UTF8, JsonRequestBehavior.AllowGet);
                }
            }
            #endregion
            else //新增
            {
                if (type == "1")
                {
                    foreach (var item in tempNoArry)
                    {
                        if (existNoes.Contains(item))
                        {
                            return Json(new { rs = "error", message = string.Format("当前日期下活动{0}已经存在", item) });
                        }
                    }
                    #region 循环写入
                    string tmpmsg = ValidSubject(tempNoArry);
                    if (!string.IsNullOrWhiteSpace(tmpmsg))
                    {
                        return Json(new { rs = "error", message = tmpmsg }, "text/plain", Encoding.UTF8, JsonRequestBehavior.AllowGet);
                    }
                    for (int i = 0; i < tempNoArry.Length; i++)
                    {
                        SWfsSubjectFocusArea model = new SWfsSubjectFocusArea
                        {
                            SubjectNo = tempNoArry[i],
                            Sort = i + 1,
                            ShowDate = showDate,
                            CreateUserId = PresentationHelper.GetPassport().UserName,
                            DateCreate = DateTime.Now,
                            DateUpdate = DateTime.Now,
                            UpdateUserId = PresentationHelper.GetPassport().UserName,
                            Status = 1,
                            Type = Convert.ToInt16(type)
                        };
                        try
                        {
                            service.Insert(model);
                        }
                        catch (Exception e)
                        {
                            return Json(new { rs = "error", message = e.Message }, "text/plain", Encoding.UTF8, JsonRequestBehavior.AllowGet);
                        }
                    }
                    #endregion
                }
                else //自定义链接20141106 by lijia
                {
                    Dictionary<string, string> webPic = new CommonService().PostImg(Request.Files["WebPic"], AppSettingManager.AppSettings["ToppicFlapPic"].ToString(), ".jpg/.gif");
                    string webPicNo = webPic.Values.FirstOrDefault();
                    string webPicNokey = webPic.Keys.FirstOrDefault();
                    Dictionary<string, string> mobilePic = new CommonService().PostImg(Request.Files["MobilePic"], AppSettingManager.AppSettings["ToppicIPhonePicFileNo"].ToString(), ".jpg/.png");
                    string mobilePicNo = mobilePic.Values.FirstOrDefault();
                    string mobilePicNokey = mobilePic.Keys.FirstOrDefault();
                    SWfsSubjectFocusArea model = new SWfsSubjectFocusArea();
                    model.ShowDate = showDate;
                    model.Sort = 1;
                    model.SubjectNo = "";
                    model.Type = Convert.ToInt16(type);
                    model.WebUrl = webUrl;
                    if (webPicNokey != "error")
                    {
                        model.WebPic = webPicNo;
                    }
                    else
                    {
                        return Json(new { rs = "error", message = webPicNo }, "text/plain", Encoding.UTF8, JsonRequestBehavior.AllowGet);
                    }
                    if (mobilePicNokey != "error")
                    {
                        model.MobilePic = mobilePicNo;
                    }
                    else
                    {
                        return Json(new { rs = "error", message = mobilePicNo }, "text/plain", Encoding.UTF8, JsonRequestBehavior.AllowGet);
                    }
                    model.MobileUrl = mobileUrl;
                    model.DateUpdate = DateTime.Now;
                    model.UpdateUserId = PresentationHelper.GetPassport().UserName;
                    model.CreateUserId = PresentationHelper.GetPassport().UserName;
                    model.DateCreate = DateTime.Now;
                    model.Status = 1;
                    try
                    {
                        service.Insert(model);
                    }
                    catch (Exception e)
                    {
                        return Json(new { rs = "error", message = e.Message }, "text/plain", Encoding.UTF8, JsonRequestBehavior.AllowGet);
                    }

                }
            }
            return Json(new { rs = "ok", message = "" }, "text/plain", Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }

        private string ValidSubject(string[] tempNoArry)
        {
            SWfsSubjectService subjectService = new SWfsSubjectService();
            SubjectInfo subjectModel = null;
            foreach (var item in tempNoArry)
            {
                subjectModel = subjectService.GetSubjectInfo(item);
                if (subjectModel == null)
                {
                    return string.Format("活动编号{0}错误", item);
                }
                if (subjectModel.SubjectTemplate != 4)
                {
                    return string.Format("活动编号{0}类型错误", item);
                }
                if (subjectModel.Status != 1)
                {
                    return string.Format("活动编号{0}状态未开启", item);
                }
                if (subjectModel.IsAudited != 1)
                {
                    return string.Format("活动编号{0}未审核", item);
                }
                if (subjectModel.DateEnd <= DateTime.Now)
                {
                    return string.Format("活动编号{0}已经结束", item);
                }
            }
            return string.Empty;
        }

        #region 修改排序字段
        public int UpdateSort(int id, int sort)
        {
            SWfsSubjectFocusAreaService service = new SWfsSubjectFocusAreaService();
            bool flag = service.SortUpdate(id, sort);
            if (flag)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        #endregion
    }
}
