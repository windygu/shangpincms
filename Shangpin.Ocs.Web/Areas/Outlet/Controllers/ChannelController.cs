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
using System.IO;
using Shangpin.Entity.Common;
using Shangpin.Framework.WebMvc;
using System.Text;

namespace Shangpin.Ocs.Web.Areas.Outlet.Controllers
{
    [OCSAuthorization]
    public class ChannelController : BaseController
    {

        #region 频道列表首页
        public ActionResult Index(int pageIndex = 1)
        {
            int pageSize = Convert.ToInt32(AppSettingManager.AppSettings["ComonListPageNum"]);
            SWfsChannelService service = new SWfsChannelService();
            int count = 0;
            IList<SWfsChannel> list = service.GetList(pageIndex, pageSize, out count);
            ViewBag.List = list;
            ViewBag.Count = count;
            ViewBag.CurrPageIndex = pageIndex;
            ViewBag.pageSize = pageSize;
            return View();
        }
        #endregion

        #region Ajax操作
        public ActionResult AjaxChannelSort(string chennelNo, string sortNo)
        {
            SWfsChannelService service = new SWfsChannelService();
            bool flag = service.UpdateSort(chennelNo, Convert.ToInt32(sortNo));
            if (flag)
            {
                return Json(new { result = 1, message = "保存成功！" });
            }
            else
            {
                return Json(new { result = 0, message = "保存失败！" });
            }
        }
        public ActionResult AjaxSaveHoliDay(string channelNO, string holidayMode)
        {
            SWfsChannelService service = new SWfsChannelService();
            SWfsChannel channel = service.GetChannelByChannelNo(channelNO);
            string isholidayMode = holidayMode == "checked" ? "1" : "0";
            channel.HolidayMode = Convert.ToInt16(isholidayMode);
            try
            {
                service.UpdateChannelHoliDay(channel);
            }
            catch (Exception ex)
            {
                return Json(new { result = "0", message = "更改假日模式失败！" });
            }
            return Json(new { result = "1", message = "更改假日模式成功！" });
        }
        public ActionResult AjaxDelete(string channelNo)
        {
            SWfsChannelService service = new SWfsChannelService();
            int i = service.DeleteChannel(channelNo);
            if (i >= 0)
            {
                #region 日志信息
                SWfsSubjectService channelService = new SWfsSubjectService();
                SWfsSubjectOrChannelLog log = new SWfsSubjectOrChannelLog();
                log.SubjectOrChannelNo = channelNo;
                log.TypeValue = 1; //1频道
                log.DateOperator = DateTime.Now;
                log.OperatorContent = "删除频道";
                log.OperatorUserId = PresentationHelper.GetPassport().UserName;
                log.OperatorActionType = LogActionType.Delete.GetHashCode();
                channelService.InsertSubjectOrChannelLog(log);
                #endregion

                return Json(new { result = "1", message = "删除成功！" });
            }
            else
            {
                return Json(new { result = "0", message = "删除失败！" });
            }
        }

        public ActionResult AjaxUpdateStatus(string channelNo, string status)
        {
            SWfsChannelService service = new SWfsChannelService();
            SWfsChannel channel = service.GetChannelInfo(channelNo);
            channel.Status = Convert.ToInt16(status == "1" ? "0" : "1");
            try
            {
                service.UpdateStatus(channel);

                #region 日志信息
                SWfsSubjectService channelService = new SWfsSubjectService();
                SWfsSubjectOrChannelLog log = new SWfsSubjectOrChannelLog();
                log.SubjectOrChannelNo = channelNo;
                log.TypeValue = 1; //1频道
                log.DateOperator = DateTime.Now;
                log.OperatorContent = (status.Equals("1") ? "关闭频道" : "开启频道");
                log.OperatorUserId = PresentationHelper.GetPassport().UserName;
                log.OperatorActionType = LogActionType.Edit.GetHashCode();
                channelService.InsertSubjectOrChannelLog(log);
                #endregion

                string str = status == "0" ? "开启成功！" : "关闭成功！";
                return Json(new { result = "1", message = str });
            }
            catch (Exception ex)
            {
                return Json(new { result = "0", message = ex.Message });
            }
        }

        #endregion

        #region 创建频道
        public ActionResult Create()
        {
            SWfsSubjectService service = new SWfsSubjectService();
            IList<SWfsChannelSord> channelSordList = service.GetChannelSordList(2);
            ViewBag.ChannelSordList = channelSordList;
            return View();
        }

        public ActionResult CreateChannel()
        {
            SWfsChannelService service = new SWfsChannelService();
            string channelName = Request.Params["ChannelName"];
            SWfsChannel channel = service.GetChannelByName(channelName);
            if (channel != null)
            {
                return Json(new { result = "0", message = "频道名称已存在！" });
            }
            CommonService cs = new CommonService();
            string channelNo = DateTime.Now.ToString("yyyyMMdd");
            string channelId = cs.GetNextCounterId("ChannelNo").ToString("000");
            channelNo += channelId.Substring(channelId.Length - 3, 3);
            string sordNos = Request.Params["SordNo"];
            short status = Convert.ToInt16(Request.Params["Status"]);
            if (!string.IsNullOrEmpty(sordNos))
            {
                SWfsChannelSordRef sordRef = new SWfsChannelSordRef();
                string[] sordNoList = sordNos.Split(',');
                foreach (string sordNo in sordNoList)
                {
                    sordRef.SordNo = sordNo;
                    sordRef.ChannelNo = channelNo;
                    try
                    {
                        service.InsertChannelSordRef(sordRef);
                    }
                    catch (Exception ex)
                    {
                        return Json(new { result = "0", message = ex.Message });
                    }
                }
            }
            channel = new SWfsChannel();
            channel.ChannelNo = channelNo;
            channel.ChannelName = channelName;
            channel.Status = status;
            channel.SiteNo = 2;
            channel.CreateUserId = PresentationHelper.GetPassport().UserName;
            channel.DateCreate = DateTime.Now;
            channel.SortNo = 0;
            channel.DeleteFlag = 0;
            channel.Description = "";
            channel.HtmlContent = "";
            channel.BackgroundPic = "";
            channel.HolidayMode = 0;//是否开启假日模式（0：未开启，1：开启）
            try
            {
                service.InsertChannel(channel);
                return Json(new { result = "1", message = "添加成功！" }, "text/plain", Encoding.UTF8);
            }
            catch (Exception ex)
            {
                return Json(new { result = "0", message = ex.Message }, "text/plain", Encoding.UTF8);
            }
        }

        #endregion

        #region 编辑频道
        [HttpGet]
        public ActionResult Edit(string channelNo)
        {
            SWfsChannelService channel = new SWfsChannelService();
            SWfsSubjectService service = new SWfsSubjectService();
            SWfsChannel model = channel.GetChannelInfo(channelNo);
            IList<SWfsChannelSordRef> channelSordRefList = channel.GetSordByChannelNo(channelNo);
            ViewBag.ChannelSordRefList = channelSordRefList;
            IList<SWfsChannelSord> channelSordList = service.GetChannelSordList(2);
            ViewBag.ChannelSordList = channelSordList;
            return View(model);
        }

        public ActionResult EditChannel()
        {
            SWfsChannelService service = new SWfsChannelService();
            CommonService cs = new CommonService();
            SWfsChannel channel = new SWfsChannel();
            string channelName = Request.Params["ChannelName"];
            string channelNo = Request.Params["ChannelNo"];
            channel = service.GetChannelInfo(channelNo);
            if (service.GetChannelByName(channelName) != null && channelName != channel.ChannelName)
            {
                return Json(new { result = "0", message = "频道名称已存在！" });
            }
            string sordNos = Request.Params["SordNo"];
            short status = Convert.ToInt16(Request.Params["Status"]);
            if (!string.IsNullOrEmpty(sordNos))
            {
                int isDelete = service.DeleteChannelSordRef(channelNo);
                if (isDelete >= 0)
                {
                    SWfsChannelSordRef sordRef = new SWfsChannelSordRef();
                    string[] sordNoList = sordNos.Split(',');
                    foreach (string sordNo in sordNoList)
                    {
                        sordRef.SordNo = sordNo;
                        sordRef.ChannelNo = channelNo;
                        try
                        {
                            service.InsertChannelSordRef(sordRef);
                        }
                        catch (Exception ex)
                        {
                            return Json(new { result = "0", message = ex.Message });
                        }
                    }
                }
            }
            channel.ChannelNo = channelNo;
            channel.ChannelName = channelName;
            channel.Status = status;
            try
            {
                bool flag = service.UpdateChannel(channel);
                if (flag)
                {
                    return Json(new { result = "1", message = "修改成功！" }, "text/plain", Encoding.UTF8);
                }
                else
                {
                    return Json(new { result = "0", message = "修改失败！" }, "text/plain", Encoding.UTF8);
                }
            }
            catch (Exception ex)
            {
                return Json(new { result = "0", message = ex.Message }, "text/plain", Encoding.UTF8);
            }
        }

        #endregion

        #region 修改活动类型
        public ActionResult UpdateChannelSubjectType()
        {
            SWfsChannelService service = new SWfsChannelService();
            int currType = Convert.ToInt32(Request.Params["CurrentType"]);
            string subjectNoFalse = Request.Params["subjectNoFalse"];
            string subjectNoTrue = Request.Params["subjectNoTrue"];
            string channelNo = Request.Params["currChannel"];
            string[] subjectNoTrues = subjectNoTrue.TrimEnd(',').Split(',');//勾选的
            foreach (string item in subjectNoTrues)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    IList<SWfsSubjectLableType> list = service.GetSubjectLableTypeList(currType, channelNo, item);
                    if (list == null || list.Count == 0)  //执行写入操作
                    {
                        SWfsSubjectLableType labletype = new SWfsSubjectLableType();
                        labletype.ChannelNo = channelNo;
                        labletype.SubjectNo = item;
                        labletype.Type = currType;

                        try
                        {
                            service.InsertSubjectLabelType(labletype);
                        }
                        catch (Exception ex)
                        {
                            return Json(new { result = "0", message = ex.Message });
                        }
                    }
                }
            }
            string[] subjectNoFalses = subjectNoFalse.TrimEnd(',').Split(',');//没勾选的
            foreach (string item in subjectNoFalses)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    IList<SWfsSubjectLableType> list = service.GetSubjectLableTypeList(currType, channelNo, item);
                    if (list != null && list.Count > 0)  //执行删除操作
                    {
                        try
                        {
                            service.DeleteSubjectLableType(currType, channelNo, item);
                            //return Json(new { result = "1", message = "保存成功" });
                        }
                        catch (Exception ex)
                        {
                            return Json(new { result = "0", message = ex.Message });
                        }
                    }
                }
            }
            return Json(new { result = "1", message = "保存成功" });
        }

        #endregion

        #region 上传推广活动图
        /// <summary>
        /// 上传活动推广图
        /// </summary>
        /// <param name="subjectNo"></param>
        /// <returns></returns>
        public ActionResult ajaxSaveSpreadPicture(string subjectNo)
        {
            SWfsSubjectService service = new SWfsSubjectService();
            SubjectInfo subject = service.GetSubjectInfo(subjectNo);
            Dictionary<string, string> spreadPic = new CommonService().PostImg(Request.Files["SpreadPicture"], "width:460,Height:460,Length:150");
            string spreadPicNo = spreadPic.Values.FirstOrDefault();
            string spreadPicNoKey = spreadPic.Keys.FirstOrDefault();
            if (!string.IsNullOrEmpty(spreadPicNo))
            {
                if (spreadPicNoKey != "error")
                {
                    subject.SpreadPicture = spreadPicNo;
                    bool flag = service.UpdateSpreadPicture(subject);
                    if (flag)
                    {
                        return Json(new { result = "1", message = "上传成功！" });
                    }
                    else
                    {
                        return Json(new { result = "0", message = "上传失败！" });
                    }
                }
                else
                {
                    return Json(new { result = "SpreadPictureUpLoad", message = spreadPicNo });
                }
            }
            else
            {
                return Json(new { result = "0", message = "请选择图片后上传！" });
            }
        }

        #endregion

        #region 活动列表
        public ActionResult SubjectList(string type, string channel, string spreadStatus = "-1", string promotionType = "0", string status = "-1", string belongsSubjectType = "0", string key = "", string startTime = "", string endTime = "", int pageIndex = 1)
        {
            SWfsChannelService service = new SWfsChannelService();
            int pageSize = int.Parse(AppSettingManager.AppSettings["ComonListPageNum"].ToString());
            int count = 0;
            IList<SubjectInfo> list = service.GetSubjectList(type, channel, spreadStatus, promotionType, status, belongsSubjectType, key, startTime, endTime, pageIndex, pageSize, out count);
            ViewBag.SubjectList = list;
            ViewBag.TotalCount = count;
            ViewBag.CurPage = pageIndex;
            ViewBag.PageSize = pageSize;
            ViewBag.Type = type;
            ViewBag.ChannelNo = channel;
            ViewBag.Channel = service.GetChannelByChannelNo(channel);

            #region 增加显示推荐品牌信息--byliulei2014-1-10
            List<SubjectBrand> brandList = null;
            List<string> subjectNoList = null;
            if (list != null && list.Count > 0)
            {
                subjectNoList = list.Select(r => r.SubjectNo).ToList();
                brandList = service.GetSubjectBrandList(subjectNoList);
            }
            ViewBag.SubjectBrand = brandList;
            #endregion
            //增加显示关联专题预热信息
            Dictionary<string, string> dicSPage = new Dictionary<string, string>();
            if (type.Equals("7"))
            {
                dicSPage = service.GetSubjectDicSPage(subjectNoList);
            }
            ViewBag.SubjectSPage = dicSPage;
            return View();
        }
        #endregion

        #region 专题关联活动预热页面
        [HttpPost]
        public JsonResult AddYRPage()
        {
            string subjectNo = Request.Form["subjectNo"];
            string pageId = Request.Form["pageId"];
            string flag = Request.Form["flag"]; //1关联，0取消
            if (string.IsNullOrEmpty(subjectNo))
            {
                return Json(new { rs = "0", msg = "活动编号不能为空" });
            }
            if (flag.Equals("1") && string.IsNullOrEmpty(pageId))
            {
                return Json(new { rs = "0", msg = "请选择关联的预热页面" });
            }
            SWfsChannelService service = new SWfsChannelService();
            SWfsSubjectTopExpand model = service.SelectSubjectTopBySubjectNo(subjectNo);
            if (flag.Equals("0")) //取消
            {
                if (model == null)
                {
                    return Json(new { rs = "1", msg = "取消成功" });
                }
                else
                {
                    model.StExpand = "0";
                    service.EditSubjectTop(model);
                    return Json(new { rs = "1", msg = "取消成功" });
                }
            }
            else //关联
            {
                if (null != model) //更新
                {
                    try
                    {
                        model.StExpand = pageId;
                        service.EditSubjectTop(model);
                        return Json(new { rs = "1", msg = "关联成功" });
                    }
                    catch (Exception e)
                    {
                        return Json(new { rs = "0", msg = e.Message });
                    }
                }
                else //写入
                {
                    try
                    {
                        model = new SWfsSubjectTopExpand();
                        model.TopCreateTime = new DateTime(1900, 1, 1);//关联静态页，默认时间最小,置顶后按置顶时间先后显示
                        model.SubjectNo = subjectNo;
                        model.StExpand = pageId;
                        service.InsertSubjectTop(model);
                        return Json(new { rs = "1", msg = "关联成功" });
                    }
                    catch (Exception e)
                    {
                        return Json(new { rs = "0", msg = e.Message });
                    }
                }
            }


        }
        #endregion

        #region 置顶
        [HttpPost]
        public ActionResult AjaxStickTime()
        {
            string subjectNo = Request.Params["subjectNo"];
            if (string.IsNullOrEmpty(subjectNo))
            {
                return Json(new { result = "-1", message = "信息获取错误" });
            }
            SWfsChannelService service = new SWfsChannelService();
            SWfsSubjectTopExpand top = service.SelectSubjectTopBySubjectNo(subjectNo);
            try
            {
                if (top == null)
                {
                    top = new SWfsSubjectTopExpand();
                    top.SubjectNo = subjectNo;
                    top.TopCreateTime = DateTime.Now;
                    top.StExpand = string.Empty;
                    service.InsertSubjectTop(top);
                }
                else
                {
                    top.TopCreateTime = DateTime.Now;
                    service.EditSubjectTop(top);
                }

            }
            catch
            {
                return Json(new { result = "-1", message = "置顶错误" });
            }
            return Json(new { result = "1", message = "该活动已经置顶" });
        }
        #endregion

        #region 修改活动状态
        /// <summary>
        /// 修改活动状态
        /// </summary>
        /// <param name="subjectNo"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SubjectStatusModify(string subjectNo)
        {
            SWfsSubjectService service = new SWfsSubjectService();
            SubjectInfo subject = service.GetSubjectInfo(subjectNo);
            //没有获取到活动信息
            if (subject == null)
            {
                return Json(new { result = "0", message = "信息获取错误" });
            }
            string tempStatue = subject.Status == 0 ? "1" : "0";
            //如果状态是关闭活动 点击后开启
            if (subject.Status == 0)
            {
                IList<string> list = service.GetProductListBySubjectNo(subjectNo, "0");
                //存在商品
                if (list != null && list.Count > 0)
                {
                    if (string.IsNullOrEmpty(subject.BelongsSubjectPic) || string.IsNullOrEmpty(subject.TitlePic2) || string.IsNullOrEmpty(subject.IPhonePic))
                    {
                        return Json(new { result = "0", message = "由于此活动的图片上传不完全，所以不能开启该活动！" });
                    }

                    if (subject.SubjectPreStartTemplateType != 1 && string.IsNullOrEmpty(subject.BackgroundPic))
                    {
                        return Json(new { result = "0", message = "由于此活动的图片上传不完全，所以不能开启该活动！" });
                    }
                }
                else
                {
                    return Json(new { result = "0", message = "此活动中没有符合前台网站售卖条件的商品，不能开启！" });
                }

            }
            try
            {
                service.SubjectStatusModify(subjectNo, tempStatue);
                #region 修改活动状态日志
                SWfsSubjectOrChannelLog log = new SWfsSubjectOrChannelLog();
                log.SubjectOrChannelNo = subjectNo;
                log.TypeValue = 0;
                log.DateOperator = DateTime.Now;
                log.OperatorContent = "修改活动状态";
                log.OperatorUserId = PresentationHelper.GetPassport().UserName;
                service.InsertSubjectOrChannelLog(log);
                #endregion
                return Json(new { result = "1", message = "活动状态修改成功！" });
            }
            catch (Exception ex)
            {
                return Json(new { result = "0", message = "活动状态修改失败！" });
            }
        }

        #endregion

        #region 促销秒杀商品展示
        public ActionResult Spike(string productNoName = "", string subjectNo = "", string date = "", int pageIndex = 1, int pageSize = 5)
        {
            if (productNoName == "商品名称/商品编号")
            {
                productNoName = "";
            }
            if (subjectNo == "活动编号")
            {
                subjectNo = "";
            }
            DateTime now;
            try
            {
                now = Convert.ToDateTime(date);
            }
            catch (Exception)
            {
                now = DateTime.Now;
            }
            int totalCount = 0;
            SWfsChannelService service = new SWfsChannelService();
            //Dictionary<DateTime, List<SpikeProductManage>> dic = service.GetSpikeProductList(productNoName, subjectNo, now, pageIndex, pageSize, out totalCount);
            //EP项目用
            Dictionary<DateTime, List<SpikeProductManage>> dic = service.SelectSpikeProductListByTimeAndName(productNoName, subjectNo, now, pageIndex, pageSize, out totalCount);

            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;
            ViewBag.totalCount = totalCount;

            ViewBag.productNoName = productNoName;
            ViewBag.date = now;
            ViewBag.SubjectNo = subjectNo;
            return View(dic);
        }

        #endregion

        #region 促销秒杀商品添加和修改
        public ActionResult SpikeProduct()
        {   //act=edit&Id=1
            string Id = Request.QueryString["Id"];
            string act = Request.QueryString["act"];
            SWfsSubjectSpikeProductManage model = null;
            if (!string.IsNullOrEmpty(Id) && !string.IsNullOrEmpty(act) && act.Equals("edit"))
            {
                SWfsChannelService channelService = new SWfsChannelService();
                model = channelService.GetSWfsSubjectSpikeProductManage(Id);
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult AddSpikeProduct(SWfsSubjectSpikeProductManage model)
        {
            //if (string.IsNullOrWhiteSpace(model.ProductNo))
            //{
            //    return Json(new { reslut = false, msg = "请选择秒杀商品" });
            //}
            //if (model.ShowTime == null)
            //{
            //    return Json(new { reslut = false, msg = "请选择展示日期" });
            //}
            //增加逻辑判断，如果存在同一时段的秒杀商品，则不能添加成功
            SWfsChannelService service = new SWfsChannelService();
            string tmpMsg = string.Empty;
            if (service.IsValidSpikeProduct(model.ShowTime, model.ID, out tmpMsg))
            {
                return Json(new { reslut = false, msg = tmpMsg });
            }
            if (null != Request.Files["ShowProductPicFileNo"] && Request.Files["ShowProductPicFileNo"].ContentLength > 0)
            {
                CommonService commonService = new CommonService();
                Dictionary<string, string> rsPic = commonService.PostImg(Request.Files["ShowProductPicFileNo"], "width:300,Height:620,Length:150");
                if (rsPic.Keys.Contains("error"))
                {
                    return Json(new { reslut = false, msg = rsPic["error"] });
                }
                if (rsPic.Keys.Contains("success"))
                {
                    model.ShowProductPicFileNo = rsPic["success"];
                }
            }
            else
            {
                if (!model.ID.Equals(0)) //修改没有上传图片 则
                {
                    model.ShowProductPicFileNo = Request.Form["ESPPicFileNo"];
                }
                else
                {
                    return Json(new { reslut = false, msg = "请上传展示图片" });
                }
            }
            model.Status = (short)1;//暂时没用上，备用始终为1
            if (model.ID.Equals(0))//新增
            {
                model.DateCreate = DateTime.Now;
                model.CreateUserId = PresentationHelper.GetPassport().UserName;
                try
                {
                    service.AddSWfsSubjectSpikeProduct(model);
                    return Json(new { reslut = true, msg = "添加成功" });
                }
                catch (Exception)
                {
                    return Json(new { reslut = false, msg = "添加失败" });
                }
            }
            else //修改
            {
                if (string.IsNullOrWhiteSpace(model.ProductNo))
                {
                    model.Type = 1;
                }
                else { model.Type = 0; }
                try
                {
                    service.UpdateSWfsSubjectSpikeProduct(model);
                    return Json(new { reslut = true, msg = "修改成功" });
                }
                catch (Exception)
                {
                    return Json(new { reslut = false, msg = "修改失败" });
                }
            }
        }


        /// <summary>
        /// 查询秒杀商品
        /// </summary>
        /// <param name="subjectNo"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult SearchSubjectProduct(string subjectNo, int pageIndex, int pageSize)
        {
            if (string.IsNullOrWhiteSpace(subjectNo))
            {
                return Json(new { rs = false, msg = "活动编号错误" }, JsonRequestBehavior.AllowGet);
            }
            SWfsSubjectService subjectService = new SWfsSubjectService();
            SubjectInfo model = subjectService.GetSubjectInfo(subjectNo);
            if (model == null)
            {
                return Json(new { rs = false, msg = "活动不存在" }, JsonRequestBehavior.AllowGet);
            }
            if (!model.SubjectTemplate.Equals(5))
            {
                return Json(new { rs = false, msg = "活动不是促销秒杀类型" }, JsonRequestBehavior.AllowGet);
            }
            if (!model.Status.Equals(1))
            {
                return Json(new { rs = false, msg = "活动尚未开启" }, JsonRequestBehavior.AllowGet);
            }
            if (model.DateEnd > DateTime.Now)
            {
                RecordPage<SubjectProductRef> productList = subjectService.GetSubjectSpikeProductList(subjectNo, pageIndex, pageSize);
                if (productList == null || productList.Items.Count() <= 0)
                {
                    return Json(new { rs = false, msg = "活动尚未添加有效商品" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { rs = true, content = RenderPartialViewToString("/Areas/Outlet/Views/Channel/SearchSubjectProduct.cshtml", productList) }, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json(new { rs = false, msg = "活动已过期" }, JsonRequestBehavior.AllowGet);
            }
        }


        #endregion

        /// <summary>
        /// 查询有效活动
        /// </summary>
        /// <param name="subjectNo"></param>
        /// <returns></returns>
        public ActionResult SearchSubject(string subjectNo)
        {
            SWfsSubjectService subjectService = new SWfsSubjectService();
            SubjectInfo model = subjectService.GetSubjectInfo(subjectNo);
            if (model != null)
            {
                if (model.DateEnd > DateTime.Now && model.Status == 1 && model.IsAudited == 1 && model.ChannelNo.Contains("zsqd001"))
                {
                    return Json(new { result = "1" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { result = "0" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { result = "0" }, JsonRequestBehavior.AllowGet);
            }
        }

        #region 频道主推活动

        #region 频道主推活动列表
        public ActionResult FeaturedEventsList(string type = "4", string channel = "0", string channelName = "", string key = "", string startTime = "", string endTime = "", int pageIndex = 1)
        {
            SWfsChannelService service = new SWfsChannelService();
            int pageSize = int.Parse(AppSettingManager.AppSettings["ComonListPageNum"].ToString());
            int count = 0;
            IList<ChannelFeaturedEventsInfo> list = service.GetChannelFeaturedEventsList(channel, key.Trim(), startTime.Trim(), endTime.Trim(), pageIndex, pageSize, out count);
            if (list != null && list.Count() > 0)
            {
                int id = list[0].ID;
                ViewBag.FeaturedList = list;
                ViewBag.TotalCount = count;
                ViewBag.CurPage = pageIndex;
                ViewBag.PageSize = pageSize;
            }
            ViewBag.Type = type;
            ViewBag.ChannelNo = channel;
            ViewBag.ChannelName = channelName;
            ViewBag.Channel = service.GetChannelByChannelNo(channel);

            return View();
        }
        #endregion
        #region 创建或编辑频道主推活动
        [HttpPost]
        public ActionResult FeaturedEventAdd()
        {
            CommonService commonService = new CommonService();
            SWfsChannelService service = new SWfsChannelService();
            Dictionary<string, string> rsPic = new Dictionary<string, string>();
            DateTime showDate = Convert.ToDateTime(Request.Params["showDate"]);
            int location = Convert.ToInt32(Request.Params["loc"]);
            string subjectNo = Request.Params["subjectNo"];
            string channelNo = Request.Form["ChannelNo"];
            if (subjectNo.IndexOf(',') > 0)
            {
                subjectNo = subjectNo.Substring(0, subjectNo.IndexOf(','));
            }
            string spreadPicture = "";
            //判断是修改还是写入 
            int ID = 0;
            if (Request.Form["ID"] != null)
            {
                ID = int.Parse(Request.Form["ID"]);
            }
            #region 写入前数据验证
            //根据活动位置和时间获取频道主推活动信息列表
            List<SWfsSubjectFeaturedEvents> sfeList = service.GetFeatureEventsByDateShow(channelNo, showDate, location);

            //获取一条活动信息
            SubjectInfo subjectInfo = service.GetSubjectInfoBySubjectNo(subjectNo);

            if (subjectInfo != null)
            {
                if (subjectInfo.Status == 0)
                {
                    return Json(new { rs = "error", message = "该活动已关闭。" });
                }
                if (subjectInfo.Status == 2)
                {
                    return Json(new { rs = "error", message = "该活动未开启。" });
                }
                if (subjectInfo.IsAudited != 1)
                {
                    return Json(new { rs = "error", message = "该活动未审核完成。" });
                }
                if (subjectInfo.DateEnd < DateTime.Now)
                {
                    return Json(new { rs = "error", message = "该活动已经结束。" });
                }
            }
            else
            {
                return Json(new { rs = "error", message = "该活动不存在。" });
            }
            #endregion


            //修改
            if (ID > 0)
            {
                SWfsSubjectFeaturedEvents model = service.GetFeatureEventInfo(ID);
                if (model.Location != location || model.DateShow != showDate)
                {
                    if (sfeList != null && sfeList.Count > 0)
                    {
                        return Json(new { rs = "error", message = "同位置同时间不可创建大于两场含两场的主推。" });
                    }
                }
                model.DateShow = showDate;
                model.Location = location;
                model.SubjectNo = subjectNo;
                #region 上传图片
                if (Request.Files["AdPicFile"] == null)
                {
                    if (!string.IsNullOrEmpty(model.SpreadPicture))
                        spreadPicture = Request.Params["hidAdPicUp"].ToString();//获取隐藏域中的值
                    else
                        return Json(new { rs = "error", message = "请上传图片！" });
                }
                else
                {
                    string picSize = AppSettingManager.AppSettings["ChannelFeAdPic"].ToString();
                    string picType = AppSettingManager.AppSettings["ChannelFeAdPicType"].ToString();
                    rsPic = commonService.PostImg(Request.Files["AdPicFile"], picSize, picType);
                    if (rsPic.Keys.Contains("error"))
                    {
                        return Json(new { rs = "error", message = rsPic["error"] });
                    }
                    if (rsPic.Keys.Contains("success"))
                    {
                        spreadPicture = rsPic["success"];
                    }
                }
                #endregion
                model.SpreadPicture = spreadPicture;
                try
                {
                    service.Update(model);
                    return Json(new { rs = "ok", message = "修改成功。" }, "text/plain", Encoding.UTF8);
                }
                catch (Exception ex)
                {
                    return Json(new { rs = "error", message = ex.Message }, "text/plain", Encoding.UTF8);
                }
            }
            else//添加
            {
                if (sfeList != null && sfeList.Count > 0)
                {
                    return Json(new { rs = "error", message = "同位置同时间不可创建大于两场含两场的主推。" });
                }
                #region 上传图片
                if (null != Request.Files["AdPicFile"] && Request.Files["AdPicFile"].ContentLength > 0)
                {
                    string picSize = AppSettingManager.AppSettings["ChannelFeAdPic"].ToString();
                    string picType = AppSettingManager.AppSettings["ChannelFeAdPicType"].ToString();
                    rsPic = new CommonService().PostImg(Request.Files["AdPicFile"], picSize, picType);
                    if (rsPic.Keys.Contains("error"))
                    {
                        return Json(new { rs = "error", message = rsPic["error"] });
                    }
                    if (rsPic.Keys.Contains("success"))
                    {
                        spreadPicture = rsPic["success"];
                    }
                }
                else
                {
                    return Json(new { rs = "error", message = "请选择图片后上传！" });
                }
                #endregion
                SWfsSubjectFeaturedEvents model = new SWfsSubjectFeaturedEvents()
                {
                    ChannelNo = string.IsNullOrEmpty(channelNo) ? "" : channelNo,
                    SubjectNo = subjectNo,
                    DateShow = showDate,
                    Type = 1,
                    Location = location,
                    SpreadPicture = spreadPicture,
                    DateCreate = DateTime.Now,
                    CreateUserId = PresentationHelper.GetPassport().UserName
                };
                try
                {
                    service.Insert(model);
                    return Json(new { rs = "ok", message = "添加成功。" }, "text/plain", Encoding.UTF8);
                }
                catch (Exception ex)
                {
                    return Json(new { rs = "error", message = ex.Message }, "text/plain", Encoding.UTF8);
                }
            }
        }
        #endregion
        #region 创建或修改频道主推活动的呈现
        public ActionResult FeaturedEventAdd(string act, string fId = "", string channelNo = "", string channelName = "")
        {
            SWfsSubjectFeaturedEvents model = new SWfsSubjectFeaturedEvents();
            ViewBag.Act = act;
            SWfsChannelService service = new SWfsChannelService();
            //修改
            if (!string.IsNullOrEmpty(act) && !string.IsNullOrEmpty(fId.ToString()) && act == "edit")
            {
                model = service.GetFeatureEventInfo(Convert.ToInt32(fId));
            }
            //删除
            if (!string.IsNullOrEmpty(act) && !string.IsNullOrEmpty(fId.ToString()) && act.Equals("del"))
            {
                service.DeleteFeatureEventById(Convert.ToInt32(fId));
                return Redirect("/outlet/channel/FeaturedEventsList?channel=" + channelNo + "&channelName=" + channelName);
            }
            return View(model);
        }
        #endregion

        #endregion

        #region 根据活动编号查询秒杀商品_EP by zhangman 20141002
        /// <summary>
        /// 根据活动编号查询秒杀商品
        /// </summary>
        /// <param name="subjectNo"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult SelectSubjectProductList(string subjectNo, int pageIndex, int pageSize)
        {
            if (string.IsNullOrWhiteSpace(subjectNo))
            {
                return Json(new { rs = false, msg = "活动编号错误" }, JsonRequestBehavior.AllowGet);
            }
            SWfsSubjectService subjectService = new SWfsSubjectService();
            SubjectInfo model = subjectService.GetSubjectInfo(subjectNo);
            if (model == null)
            {
                return Json(new { rs = false, msg = "活动不存在" }, JsonRequestBehavior.AllowGet);
            }
            if (!model.SubjectTemplate.Equals(5))
            {
                return Json(new { rs = false, msg = "活动不是促销秒杀类型" }, JsonRequestBehavior.AllowGet);
            }
            if (!model.Status.Equals(1))
            {
                return Json(new { rs = false, msg = "活动尚未开启" }, JsonRequestBehavior.AllowGet);
            }
            if (model.DateEnd > DateTime.Now)
            {
                RecordPage<SubjectProductRef> productList = subjectService.OutletGetSubjectSpikeProductList(subjectNo, pageIndex, pageSize);

                //修改状态数据  by lijibo 20141003
                productList.Items = subjectService.TransformationEntityListRef(productList.Items.ToList());

                if (productList == null || productList.Items.Count() <= 0)
                {
                    return Json(new { rs = false, msg = "活动尚未添加有效商品" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { rs = true, content = RenderPartialViewToString("/Areas/Outlet/Views/Channel/SelectSubjectProductList.cshtml", productList) }, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json(new { rs = false, msg = "活动已过期" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

    }
}