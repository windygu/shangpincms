using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Shangpin.Entity.Wfs;
using Shangpin.Framework.Common;
using Shangpin.Ocs.Service.Shangpin;
using Shangpin.Ocs.Service;
using Shangpin.Framework.Configuration;
using Shangpin.Framework.Common.Cache;

namespace Shangpin.Ocs.Web.Areas.Shangpin.Controllers
{
    public class CurtainController : Controller
    {
        //
        // GET: /Shangpin/Curtain/
        CurtainService curtain = new CurtainService();
        private Dictionary<string, string> rsPic = new Dictionary<string, string>();
        #region 列表
        public ActionResult CurtainList(int pageIndex=1, int pageSize=10)
        {
            int count = 0;
            ViewBag.list = CurtainLists(pageIndex, pageSize, out count);
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;
            ViewBag.TotalCount = count;
            return View();
        }
        public IEnumerable<SWfsCurtain> CurtainLists(int pageIndex, int pageSize, out int count)
        {
            var dic = new Dictionary<string, object>();
            if (Request.QueryString["CurtainTitle"] != "" && Request.QueryString["CurtainTitle"] != null)
            {
                dic.Add("CurtainTitle", Request.QueryString["CurtainTitle"]);
                ViewBag.KeyWord = Request.QueryString["CurtainTitle"];
            }
            else
            {
                dic.Add("CurtainTitle", "");
                ViewBag.KeyWord = Request.QueryString["CurtainTitle"];
            }
            if (Request.QueryString["CurtainStatus"] != "-1" && Request.QueryString["CurtainStatus"] != "" && Request.QueryString["CurtainStatus"] != null)
            {
                dic.Add("CurtainStatus", Request.QueryString["CurtainStatus"]);
            }
            else
            {
                dic.Add("CurtainStatus", "");
            }
            if (Request.QueryString["StartShowTime"] != "" && Request.QueryString["StartShowTime"] != null)
            {
                dic.Add("StartShowTime", Request.QueryString["StartShowTime"]);
                ViewBag.StartShowTime = Request.QueryString["StartShowTime"];
            }
            else
            {
                dic.Add("StartShowTime", "");
            }
            if (Request.QueryString["EndShowTime"] != "" && Request.QueryString["EndShowTime"] != null)
            {
                dic.Add("EndShowTime", Request.QueryString["EndShowTime"]);
                ViewBag.EndShowTime = Request.QueryString["EndShowTime"];
            }
            else
            {
                dic.Add("EndShowTime", "");
            }
            IEnumerable<SWfsCurtain> list = DapperUtil.Query<SWfsCurtain>("ComBeziWfs_WfsCmsContent_SWfsCurtainList", dic, new { CurtainTitle = Request.QueryString["CurtainTitle"], CurtainStatus = Request.QueryString["CurtainStatus"], StartShowTime = Request.QueryString["StartShowTime"], EndShowTime =string.IsNullOrEmpty(Request.QueryString["EndShowTime"])?DateTime.Now: DateTime.Parse(Request.QueryString["EndShowTime"]).AddDays(1), pageIndex = pageIndex, pageSize = pageSize });
            count = DapperUtil.Query<int>("ComBeziWfs_WfsCmsContent_SWfsCurtain_Count", dic, new { CurtainTitle = Request.QueryString["CurtainTitle"], CurtainStatus = Request.QueryString["CurtainStatus"], StartShowTime = Request.QueryString["StartShowTime"], EndShowTime = string.IsNullOrEmpty(Request.QueryString["EndShowTime"]) ? DateTime.Now : DateTime.Parse(Request.QueryString["EndShowTime"]).AddDays(1), pageIndex = pageIndex, pageSize = pageSize }).First<int>();
            return list;
        }
        #endregion
        //删除一数据
        public ActionResult CurtainDele(int curtainId)
        {
           curtain.CurtainDelete(curtainId);
           EnyimMemcachedClient.Instance.Remove("ComBeziWfs_SWfsCurtain_GetSWfsCurtain_GetCurtainAdver");
           return Redirect("CurtainList.html");
        }
        //修改状态
        public ActionResult CurtainStatus(int curtainId, int curtainStatus)
        {
            curtain.CurtainStatus(curtainId, curtainStatus);
            EnyimMemcachedClient.Instance.Remove("ComBeziWfs_SWfsCurtain_GetSWfsCurtain_GetCurtainAdver");
            return Redirect("CurtainList.html");
        }
        //添加
        public ActionResult CurtainCreate(int curtainId = 0)
        {
            if (curtainId == 0)
            {
                return View(new SWfsCurtain() { CurtainStatus = 0, StartShowTime = DateTime.Now, EndShowTime = DateTime.Now.AddDays(1) });
            }
            var bl = curtain.CurtainListId(curtainId);

            if (bl == null)
            {
                return View(new SWfsCurtain() { CurtainStatus = 0, StartShowTime = DateTime.Now, EndShowTime = DateTime.Now });
            }

            return View(bl);
        }
        [HttpPost]
        public ActionResult CurtainCreate(SWfsCurtain obj)
        {
            obj.CreateTime = DateTime.Now;
            obj.CurtainTitle = Request.Form["CurtainTitle"];
            obj.CurtainStatus = Request.Form["CurtainStatus"] != null ? int.Parse(Request.Form["CurtainStatus"]) : 0;
            obj.StartShowTime = DateTime.Parse(Request.Form["StartShowTime"]);
            obj.EndShowTime = DateTime.Parse(Request.Form["EndShowTime"]);
            obj.CurtainLinkAddress = Request.Form["CurtainLinkAddress"];
            #region 添加图片
            if (Request.Files["imgfile"] != null && Request.Files["imgfile"].ContentLength > 0)
            {
                obj.CurtainImage = SaveImg(Request.Files["imgfile"], "width:0,Height:0,Length:10000");
                if (rsPic.Keys.Contains("error"))
                {
                    ViewData["tip"] = new HtmlString("<script>alert('" + rsPic["error"] + "')</script>");
                    return View(obj);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(obj.CurtainImage))
                {
                    obj.CurtainImage = "";
                }
            }
            #endregion
            if (obj.CurtainId == 0)
            {
                if (curtain.CurtainCreate(obj) > 0)
                {
                    EnyimMemcachedClient.Instance.Remove("ComBeziWfs_SWfsCurtain_GetSWfsCurtain_GetCurtainAdver");
                    ViewData["tip"] = new HtmlString("<script>alert('添加成功'); window.location.href='CurtainList.html" + CommonService.GetTimeStamp("?") + "'</script>");
                }
                else
                {
                    ViewData["tip"] = new HtmlString("<script>alert('添加失败')</script>");

                }
            }
            else
            {
                if (curtain.SWfsCurtainUpdate(obj))
                {
                    EnyimMemcachedClient.Instance.Remove("ComBeziWfs_SWfsCurtain_GetSWfsCurtain_GetCurtainAdver");
                    ViewData["tip"] = new HtmlString("<script>alert('修改成功'); window.location.href='CurtainList.html" + CommonService.GetTimeStamp("?") + "'</script>");
                }
                else
                {
                    ViewData["tip"] = new HtmlString("<script>alert('修改失败')</script>");
                }

            }
            return View(obj);
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

        #region 多图片上传(当前属于会场模版图片上传，加在这里的原因是因为当前控制器无验证)

        [HttpPost]
        public JsonResult UploadImg(HttpPostedFileBase fileData)
        {
            if (fileData != null)
            {
                try
                {
                    var tempFile = Request.Files["Filedata"];
                    string tempFileName = tempFile.FileName;
                    int fileLocation = tempFileName.LastIndexOf("/"); //此块为因对不同浏览器图片路径可能不一样问题
                    if (fileLocation > -1)   //如果查到截取，如果没有引用原值
                    {
                        tempFileName = tempFileName.Substring(fileLocation + 1);
                    }
                    CommonService commonService = new CommonService();
                    string picType = AppSettingManager.AppSettings["VeuneImgType"].ToString();
                    Dictionary<string, string> dicRs = commonService.PostImg(tempFile, "width:0,Height:0,Length:0", picType);
                    string picNo = dicRs.Values.FirstOrDefault();
                    string picKey = dicRs.Keys.FirstOrDefault();
                    picNo = picNo + "#" + tempFileName;    // 组合原名与生成的名字
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
        #endregion
    }
}
