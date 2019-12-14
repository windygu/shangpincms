using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Shangpin.Ocs.Service.Common;
using Shangpin.Ocs.Entity.Extenstion.Outlet;
using Shangpin.Entity.Wfs;
using Shangpin.Ocs.Service.Outlet;
using Shangpin.Entity.Common;

namespace Shangpin.Ocs.Web.Areas.Outlet.Controllers
{
    /// <summary>
    /// 奥莱市场运营----网络推广管理
    /// </summary>
    [OCSAuthorization]
    public class MarketOptionController : Controller
    {
        //
        // GET: /Outlet/MarketOption/

        public ActionResult Index(MarketOptionSearchParm parm, int pageIndex = 1)
        {
            MarketOptionService service = new MarketOptionService();
            RecordPage<SubjectInfo> list = service.AddApplyList(parm, pageIndex, 10);

            ViewBag.List = list;
            ViewBag.ChannelSordList = new SubjectController().GetChannelSordList(2);
            return View(parm);
        }

        /// <summary>
        /// 推广申请--展示页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddApply(string subjectNo, string flag)
        {
            //flag: AddApply添加申请推广，CheckApply网推检查确认推广信息，AddSeo 添加SEO优化
            if (string.IsNullOrWhiteSpace(flag))
            {
                return Content("flag参数错误");
            }
            if (string.IsNullOrWhiteSpace(subjectNo))
            {
                return Content("subjectNo参数错误");
            }
            SWfsSubjectService service = new SWfsSubjectService();

            ViewBag.Flag = flag;
            flag = flag.ToLower();
            #region //添加推广信息
            if (flag.Equals("addapply"))
            {
                SubjectInfo mode = service.GetSubjectInfo(subjectNo);
                if (mode == null)
                {
                    return Content("活动不存在");
                }
                if (mode != null && mode.IsPreheat == 1)
                {
                    SubjectPreheatInfoM predheatM = service.GetSubjectPreheatInfo(subjectNo);
                    ViewBag.PreTime = predheatM != null ? predheatM.PreheatTime.ToString("yyyy-MM-dd HH:mm:ss") : "";
                }
                else
                {
                    ViewBag.PreTime = "";
                }
                ViewBag.SubjectNo = subjectNo;
                SWfsSubjectApplyPromotion tempmodel = new MarketOptionService().GetModelBySubjectNo(subjectNo);
                return View(tempmodel);
            }
            #endregion

            #region //网推检查确认推广信息
            if (flag.Equals("checkapply"))
            {
                SubjectInfo mode = service.GetSubjectApplyInfo(subjectNo);
                Dictionary<string, List<SWfsSubjectChannelSordRef>> dicSordRef = new SWfsSubjectService().GetSordBySubjectNoList(subjectNo.Split(',').ToArray());
                mode.ChannelSordList = dicSordRef.Keys.Contains(subjectNo) ? dicSordRef[subjectNo] : null;


                List<SWfsSubjectPromotionChannel> spList = new MarketOptionService().SelectPromotionChannelList();
                ViewBag.PromotionChannelList = spList;
                ViewBag.ChannelSordList = new SubjectController().GetChannelSordList(2);
                return View("CheckApply", mode);
            }
            #endregion

            #region //添加SEO优化
            if (flag.Equals("addseo"))
            {
                SubjectInfo mode = service.GetSubjectApplyInfo(subjectNo);
                return View("AddSeo", mode);
            }
            #endregion

            return Content("flag参数错误，正确值为addapply|checkapply|addseo之一");
        }

        /// <summary>
        /// 推广添加和修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddApply(SWfsSubjectApplyPromotion model)
        {
            if (Request.Form["flag"] == null)
            {
                return Json(new { rs = "error", msg = "flag参数错误" });
            }
            string flag = Request.Form["flag"].Trim();
            if (string.IsNullOrWhiteSpace(flag))
            {
                return Json(new { rs = "error", msg = "flag参数错误" });
            }
            if (string.IsNullOrWhiteSpace(model.SubjectNo))
            {
                return Json(new { rs = "error", msg = "subjectNo参数错误" });
            }
            MarketOptionService service = new MarketOptionService();
            SWfsSubjectApplyPromotion tempmodel = service.GetModelBySubjectNo(model.SubjectNo);
            flag = flag.ToLower();
            #region 添加活动推广申请
            if (flag.Equals("addapply"))
            {
                int rs = 0;
                model.ApplyUserId = PresentationHelper.GetPassport().UserName;
                if (tempmodel != null && tempmodel.APID > 0)//存在，则更新
                {
                    try
                    {
                        model.APID = tempmodel.APID;
                        model.PromotionConfirmTime = tempmodel.PromotionConfirmTime;
                        model.CreateDateTime = tempmodel.CreateDateTime;
                        model.ConfirmEditDateTime = tempmodel.ConfirmEditDateTime;
                        model.PromotionApplyTime = tempmodel.PromotionApplyTime;
                        service.UpdateApplyPromotion(model);
                        return Json(1);
                    }
                    catch (Exception e)
                    {
                        return Json(e.Message);
                    }
                }
                else //添加
                {
                    model.PromotionConfirmTime = new DateTime(1900, 1, 1);
                    model.CreateDateTime = DateTime.Now;
                    model.ConfirmEditDateTime = new DateTime(1900, 1, 1);
                    model.PromotionApplyTime = DateTime.Now;//推广申请时间目前默认是该条记录创建时间--后期也可以填写控制
                    try
                    {
                        rs = service.AddApply(model);
                        return Json(rs);
                    }
                    catch (Exception e)
                    {
                        return Json(e.Message);
                    }
                }
            }
            #endregion

            #region 确认活动推广信息
            if (flag.Equals("checkapply"))
            { 
                if (model.PromotionConfirmTime == new DateTime(0001, 1, 1) || model.PromotionConfirmTime == new DateTime(1900, 1, 1))
                {
                    return Json(new { rs = "error", msg = "推广确认时间填写有误" });
                }
                if (string.IsNullOrWhiteSpace(model.PromotionChannelNo))
                {
                    return Json(new { rs = "error", msg = "推广渠道填写有误" });
                }
                tempmodel.PromotionConfirmTime = model.PromotionConfirmTime;//推广时间
                tempmodel.PromotionChannelNo = Request.Form["PromotionChannelNo"];
                tempmodel.IsChecked = 1;
                tempmodel.ConfirmUserId = PresentationHelper.GetPassport().UserName;//确认推广的操作人
                tempmodel.ConfirmEditDateTime = DateTime.Now;//确认这个操作的执行时间
                try
                {
                    service.UpdateApplyPromotion(tempmodel);
                    return Json(new { rs = "ok", msg = "确认成功" });
                }
                catch (Exception e)
                {
                    return Json(new { rs = "error", msg = e.Message });
                }
            }
            #endregion

            #region 修改SEO信息
            if (flag.Equals("addseo"))
            {
                if (string.IsNullOrWhiteSpace(model.SeoTitle))
                {
                    return Json(new { rs = "error", msg = "活动标题不能为空" });
                }
                if (string.IsNullOrWhiteSpace(model.SeoKeyWords))
                {
                    return Json(new { rs = "error", msg = "活动关键词不能为空" });
                }
                if (string.IsNullOrWhiteSpace(model.SeoDescription))
                {
                    return Json(new { rs = "error", msg = "活动描述不能为空" });
                }

                tempmodel.SeoTitle = model.SeoTitle;
                tempmodel.SeoKeyWords = model.SeoKeyWords;
                tempmodel.SeoDescription = model.SeoDescription;

                try
                {
                    service.UpdateApplyPromotion(tempmodel);
                    return Json(new { rs = "ok", msg = "确认成功" });
                }
                catch (Exception e)
                {
                    return Json(new { rs = "error", msg = e.Message });
                }
            }
            #endregion

            return Json(new { rs = "error", msg = "flag参数错误" });
           
        }


        [HttpPost]
        public JsonResult AddPromotionChannel()
        {
            if (string.IsNullOrWhiteSpace(Request.Form["name"]))
            {
                return Json(new { rs = "error", msg = "参数错误" });
            }
            string name = Request.Form["name"].Trim();
            MarketOptionService service = new MarketOptionService();
            SWfsSubjectPromotionChannel model = service.SelectPromotionChannel(name);
            if (model == null)
            {
                model = new SWfsSubjectPromotionChannel
                {
                    ChannelName = name,
                    CreateDateTime = DateTime.Now,
                    CreateUserId = PresentationHelper.GetPassport().UserName
                };
                try
                {
                    int id= service.AddPromotionChannel(model);
                    return Json(new { rs = "ok",id=id,msg = "添加成功" });
                }
                catch (Exception)
                {
                    return Json(new { rs = "error", msg = "添加失败" });
                }
            }
            else
            {
                return Json(new { rs = "error", msg = "添加成功" });
            }
        }
    }
}
