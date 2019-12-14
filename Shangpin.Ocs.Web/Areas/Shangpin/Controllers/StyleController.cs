using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Shangpin.Ocs.Service;
using Shangpin.Ocs.Service.Common;
using Shangpin.Ocs.Service.Shangpin;
using Shangpin.Entity.ComBeziPicLab;
using Shangpin.Entity.Wfs;
using Shangpin.Entity.Common;
using Shangpin.Ocs.Entity.Extenstion.ShangPin;
namespace Shangpin.Ocs.Web.Areas.Shangpin.Controllers
{
    [OCSAuthorization]
    public class StyleController : Controller
    {

        private readonly StyleService styleService;

        public StyleController()
        {
            styleService = new StyleService();
        }

        #region 品牌搭配 By lijia 20140909

        /// <summary>
        /// 搭配专题列表
        /// </summary>
        /// <returns></returns>
        public ActionResult MatchSpecialList(string name = "", string position = "0", string startTime = "", string endTime = "", int pageIndex = 1)
        {
            int pageSize = Convert.ToInt32(AppSettingManager.AppSettings["ComonListPageNum"]);
            int count = 0;
            IList<SWfsStyleMatchSpecial> list = styleService.GetList(name, position, startTime, endTime, pageIndex, pageSize, 1, out count);
            ViewBag.List = list;
            ViewBag.Count = count;
            ViewBag.CurPage = pageIndex;
            ViewBag.PageSize = pageSize;
            ViewBag.Name = name;
            ViewBag.BTime = startTime;
            ViewBag.ETime = endTime;
            ViewBag.Position = position;
            return View();
        }

        public ActionResult Manage(string act, int id = 0)
        {
            SWfsStyleMatchSpecial model = new SWfsStyleMatchSpecial();
            ViewBag.Act = act;
            switch (act)
            {
                case "add"://新建
                    model.StartTime = Convert.ToDateTime("1900-1-1");
                    model.EndTime = Convert.ToDateTime("1900-1-1");
                    return View("/Areas/Shangpin/Views/Style/ManageMatchSpecial.cshtml", model);
                case "edit"://修改
                    model = styleService.GetModel(id);
                    return View("/Areas/Shangpin/Views/Style/ManageMatchSpecial.cshtml", model);
            }
            return View();
        }


        [HttpPost]
        public JsonResult ManageMatchSpecial()
        {
            string imgSize = AppSettingManager.AppSettings["MatchSpecialPic"].ToString();
            string imgTyle = AppSettingManager.AppSettings["MatchSpecialPicType"].ToString();
            CommonService commonService = new CommonService();
            Dictionary<string, string> rsPic = new Dictionary<string, string>();
            int id = Convert.ToInt32(Request["ID"]);
            string position = Request["Position"];
            string picUrl = Request["PicUrl"];
            SWfsStyleMatchSpecial model = new SWfsStyleMatchSpecial();
            if (id != 0)
            {
                model = styleService.GetModel(id);
                if (null == model)
                {
                    return Json(new { reslut = -1, msg = "记录不存在" });
                }
            }
            model.SpecialName = Request["SpecialName"];

            if (!string.IsNullOrEmpty(picUrl) && !picUrl.StartsWith("http://"))
            {
                return Json(new { reslut = -1, msg = "图片链接地址格式不正确" });
            }
            else
            {
                model.PicUrl = picUrl;
            }
            model.StartTime = Convert.ToDateTime(Request["StartTime"]);
            model.EndTime = Convert.ToDateTime(Request["EndTime"]);
            model.UpdateDate = DateTime.Now;
            model.UpdateUserId = PresentationHelper.GetPassport().UserName;
            model.Type = 1;
            if (id == 0) //创建
            {
                model.Position = Convert.ToInt16(position);
                if (null != Request.Files["PicFile"] && Request.Files["PicFile"].ContentLength > 0)
                {
                    rsPic = commonService.PostImg(Request.Files["PicFile"], imgSize, imgTyle);
                    if (rsPic.Keys.Contains("error"))
                    {
                        return Json(new { reslut = -1, msg = rsPic.Values.FirstOrDefault() }, "text/plain", Encoding.UTF8);
                    }
                    if (rsPic.Keys.Contains("success"))
                    {
                        model.PicNo = rsPic["success"];
                    }
                }
                else
                {
                    return Json(new { reslut = -1, msg = "请选择图片" });
                }
                try
                {
                    model.CreateUserId = PresentationHelper.GetPassport().UserName;
                    model.CreateDate = DateTime.Now;
                    styleService.Add(model);
                    return Json(new { reslut = 1, msg = "添加成功" }, "text/plain", Encoding.UTF8);
                }
                catch (Exception e)
                {
                    return Json(new { reslut = 1, msg = e.Message });
                }
            }
            else //修改
            {
                if (model.Position.ToString() != position) //说明修改了广告位置
                {
                    if (null == Request.Files["PicFile"] || Request.Files["PicFile"].ContentLength <= 0)
                    {
                        return Json(new { reslut = -1, msg = "修改广告位置后请重新上传广告图" });
                    }
                }
                model.Position = Convert.ToInt16(position);
                if (null != Request.Files["PicFile"] && Request.Files["PicFile"].ContentLength > 0)
                {
                    rsPic = commonService.PostImg(Request.Files["PicFile"], imgSize, imgTyle);
                    if (rsPic.Keys.Contains("error"))
                    {
                        return Json(new { reslut = -1, msg = rsPic.Values.FirstOrDefault() }, "text/plain", Encoding.UTF8);
                    }
                    if (rsPic.Keys.Contains("success"))
                    {
                        model.PicNo = rsPic["success"];
                    }
                }
                model.ID = id;
                bool rs = styleService.Update(model);
                return Json(new { reslut = rs ? 1 : -1, msg = rs ? "修改成功" : "修改失败" }, "text/plain", Encoding.UTF8);
            }
        }

        public JsonResult MatchSpecialDelete(string id)
        {
            try
            {
                styleService.Delete(id);
                return Json(new { result = 1, msg = "删除成功" });
            }
            catch (Exception ex)
            {
                return Json(new { result = ex, msg = "删除失败" });

            }
        }
        #endregion

        #region 标签展示操作
        /// <summary>
        /// 标签展示列表页
        /// </summary>
        /// <returns></returns>
        public ActionResult TagRelList()
        {
            ViewBag.ListOne = null;
            ViewBag.ListTwo = null;
            ViewBag.ListThree = null;
            ViewBag.ListFour = null;
            ViewBag.ListOneCount = null;
            ViewBag.ListTwoCount = null;
            ViewBag.ListThreeCount = null;
            ViewBag.ListFourCount = null;
            IList<SWfsTagRel> list = styleService.GetTagsList();
            IList<SWfsTagRel> _ListOne = new List<SWfsTagRel>();
            IList<SWfsTagRel> _ListTwo = new List<SWfsTagRel>();
            IList<SWfsTagRel> _ListThree = new List<SWfsTagRel>();
            IList<SWfsTagRel> _ListFour = new List<SWfsTagRel>();
            if (list != null && list.Count() > 0)
            {
                _ListOne = list.Where(c => c.Location == 1).ToList();
                ViewBag.ListOne = _ListOne;
                ViewBag.ListOneCount = _ListOne.Count();

                _ListTwo = list.Where(c => c.Location == 2).ToList();
                ViewBag.ListTwo = _ListTwo;
                ViewBag.ListTwoCount = _ListTwo.Count();


                _ListThree = list.Where(c => c.Location == 3).ToList();
                ViewBag.ListThree = _ListThree;
                ViewBag.ListThreeCount = _ListThree.Count();

                _ListFour = list.Where(c => c.Location == 4).ToList();
                ViewBag.ListFour = _ListFour;
                ViewBag.ListFourCount = _ListFour.Count();
            }
            return View();
        }

        /// <summary>
        /// 标签列表添加页
        /// </summary>
        /// <returns></returns>
        public ActionResult TagRelCreateList(string tagid, string tagName, string StartTime, string EndTime, int pageIndex = 1, int pageSize = 18)
        {
            //已经展示的标签
            IList<SWfsTagRel> existlist = styleService.GetTagsList(short.Parse(tagid));
            int count = 0;
            ViewBag.existlist = null;
            ViewBag.createlist = null;
            if (existlist != null && existlist.Count() > 0)
            {
                ViewBag.existlist = existlist;
            }
            //标签原数据表
            if (tagName == "请输入标签名称")
            { tagName = ""; }
            IList<T_tag_baseExtenstion> createList = styleService.GetTagsCreatList(tagName, StartTime, EndTime, pageIndex, pageSize, out count);
            if (createList != null && createList.Count() > 0)
            {
                ViewBag.createlist = createList;
            }
            ViewBag.PageIndex = pageIndex;
            ViewBag.totalcount = count;

            return View();
        }

        /// <summary>
        /// 根据编号删除标签展示
        /// </summary>
        /// <returns></returns>
        public ActionResult ajaxTagDel()
        {
            string tagsId = Request["tagsStr"];
            string _location = Request["location"];
            if (!string.IsNullOrEmpty(tagsId) && !string.IsNullOrEmpty(_location))
            {
                styleService.DeleteTagsById(tagsId, short.Parse(_location));
                return Json(new { result = 1, message = "删除成功！" });
            }
            else
            {
                return Json(new { result = 0, message = "删除失败！" });
            }
        }

        /// <summary>
        /// 查询标签展示是否达到了最大限制数（18）
        /// </summary>
        /// <returns></returns>
        public ActionResult ajaxTagShowCount()
        {
            string tagsId = Request["tagid"];
            if (!string.IsNullOrEmpty(tagsId))
            {
                int _count = styleService.GetDataCount(short.Parse(tagsId));
                return Json(new { result = 1, message = _count });
            }
            else
            {
                return Json(new { result = 0, message = 18 });
            }
        }

        /// <summary>
        /// 标签展示排序
        /// </summary>
        /// <returns></returns>
        public ActionResult ajaxTagSave()
        {
            string tagsId = Request["tagsStr"];
            string _location = Request["location"];
            string _des = Request["des"];
            if (_des == "sort")
            {
                if (!string.IsNullOrEmpty(tagsId) && !string.IsNullOrEmpty(_location))
                {
                    IList<SWfsTagRel> TagList = new List<SWfsTagRel>();
                    string[] tagsArray = tagsId.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    //styleService.DeleteTagsByLocation();
                    TagList = styleService.GetTagsList(short.Parse(_location));
                    int j = 1;
                    for (int i = 0; i < tagsArray.Length; i++)
                    {
                        SWfsTagRel tagEntity = TagList.Where(c => c.TagNo == tagsArray[i]).FirstOrDefault();
                        tagEntity.Sort = j;
                        tagEntity.CreateUserId = PresentationHelper.GetPassport().UserName;
                        if (styleService.UpdateTags(tagEntity))
                        {
                            j++;
                        }
                        else
                        {
                            return Json(new { result = 0, message = "排序失败！" });
                        }
                    }
                    return Json(new { result = 1, message = "排序成功！" });
                }
                else
                {
                    return Json(new { result = 0, message = "排序失败,没有获取到排序的对象！" });
                }
            }
            else if (_des == "add")
            {
                IList<t_tag_base> tagBaseList = new List<t_tag_base>();
                if (!string.IsNullOrEmpty(tagsId) && !string.IsNullOrEmpty(_location))
                {
                    tagBaseList = styleService.GetTagsCreatListByTagIds(tagsId);
                    if (tagBaseList != null && tagBaseList.Count() > 0)
                    {
                        foreach (t_tag_base TagbaseEntity in tagBaseList)
                        {
                            SWfsTagRel tagRelEntity = new SWfsTagRel();
                            tagRelEntity.TagNo = TagbaseEntity.tag_id;
                            tagRelEntity.TagName = TagbaseEntity.tag_name;
                            tagRelEntity.Status = 1;
                            tagRelEntity.Location = short.Parse(_location);
                            tagRelEntity.Sort = 18;
                            tagRelEntity.Tag_code = TagbaseEntity.tag_code;
                            tagRelEntity.CreateDate = DateTime.Now;
                            tagRelEntity.CreateUserId = PresentationHelper.GetPassport().UserName;
                            styleService.AddTags(tagRelEntity);

                        }
                    }
                    return Json(new { result = 1, message = "添加成功！" });
                }
                else
                { return Json(new { result = 0, message = "排序失败,没有获取到排序的对象！" }); }
            }
            else
            { return Json(new { result = 0, message = "排序失败,没有获取到排序的对象！" }); }

        }
        #endregion

        #region 活动图管理 by zhangwei 20140909

        /// <summary>
        /// 添加活动图
        /// </summary>
        /// <returns></returns>
        public ActionResult ActivityPicCreate()
        {
            return View();
        }

        /// <summary>
        /// 保存添加活动图
        /// </summary>
        /// <returns></returns>
        public ActionResult ActivityPicCreateSave()
        {
            SWfsStyleActivityPic model = new SWfsStyleActivityPic();

            string ActivityName = Request.Params["ActivityName"] ?? "";
            string PicUrl = Request.Params["PicUrl"] ?? "";
            string StartTime = Request.Params["StartTime"] ?? "";

            #region 上传图片
            string imgerror = string.Empty;
            string congigPic = "width:180,Height:350,Length:200";
            string picNo = ImgNameGet("PicNo", congigPic, true, out imgerror);
            if (!string.IsNullOrEmpty(imgerror))
            {
                return Json(new { result = "-1", message = imgerror }, "text/plain", Encoding.UTF8);
            }
            #endregion

            model.ActivityName = ActivityName;
            model.PicNo = picNo;
            model.PicUrl = PicUrl;
            model.StartTime = DateTime.Parse(StartTime);
            model.CreateDate = DateTime.Now;
            model.CreateUserId = PresentationHelper.GetPassport().UserName;
            try
            {
                #region 插入活动图
                styleService.InsertStyleActivityPic(model);
                return Json(new { result = "1", message = "添加成功！" }, "text/plain", Encoding.UTF8);
                #endregion

            }
            catch (Exception ex)
            {
                return Json(new { result = "0", message = ex.Message });
            }
        }

        /// <summary>
        /// 编辑活动图
        /// </summary>
        /// <returns></returns>
        public ActionResult ActivityPicEdit()
        {
            SWfsStyleActivityPic model = new SWfsStyleActivityPic();
            string id = Request.Params["id"] ?? "";
            if (!string.IsNullOrEmpty(id))
            {
                model = styleService.GetStyleActivityPicModel(id);
            }
            return View(model);
        }

        /// <summary>
        /// 保存编辑活动图
        /// </summary>
        /// <returns></returns>
        public ActionResult ActivityPicEditSave()
        {
            SWfsStyleActivityPic model = new SWfsStyleActivityPic();
            string id = Request.Form["hidSAID"] ?? "";
            if (!string.IsNullOrEmpty(id))
            {
                model = styleService.GetStyleActivityPicModel(id);
                if (model == null || model.SAID == 0)
                {
                    return Json(new { result = "-1", message = "无此活动图" }, "text/plain", Encoding.UTF8);
                }
            }
            string ActivityName = Request.Params["ActivityName"] ?? "";
            string PicUrl = Request.Params["PicUrl"] ?? "";
            string StartTime = Request.Params["StartTime"] ?? "";

            #region 上传图片
            string imgerror = string.Empty;
            string ImgName = string.Empty;
            ImgName = "PicNo";
            string congigPic = "width:180,Height:350,Length:200";
            string picNo = string.Empty;
            if (Request.Files[ImgName] != null && Request.Files[ImgName].ContentLength > 0)
            {
                picNo = ImgNameGet(ImgName, congigPic, true, out imgerror);
                if (!string.IsNullOrEmpty(imgerror))
                {
                    return Json(new { result = "-1", message = imgerror }, "text/plain", Encoding.UTF8);
                }
            }
            #endregion

            model.ActivityName = ActivityName;
            model.PicNo = !string.IsNullOrWhiteSpace(picNo) ? picNo : model.PicNo;
            model.PicUrl = PicUrl;
            model.StartTime = DateTime.Parse(StartTime);
            model.CreateUserId = model.CreateUserId;
            try
            {
                #region 修改活动图
                styleService.UpdateStyleActivityPic(model);
                return Json(new { result = "1", message = "修改成功！" }, "text/plain", Encoding.UTF8);
                #endregion

            }
            catch (Exception ex)
            {
                return Json(new { result = "0", message = ex.Message });
            }
        }

        /// <summary>
        /// 活动图管理
        /// </summary>
        /// <param name="activityName">活动图名称</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="pageindex">当前页</param>
        /// <param name="pageSize">页码</param>
        /// <returns></returns>
        public ActionResult ActivityPicManage(string activityName = "", string startTime = "", string endTime = "", int pageindex = 1, int pageSize = 20)
        {
            ViewBag.ActivityName = !string.IsNullOrEmpty(activityName) ? activityName.Trim() : "";
            ViewBag.StartTime = !string.IsNullOrEmpty(startTime) ? startTime.Trim() : "";
            ViewBag.EndTime = !string.IsNullOrEmpty(endTime) ? endTime.Trim() : "";
            RecordPage<SWfsStyleActivityPicM> list = styleService.SelectActivityPicList(activityName, startTime, endTime, pageindex, pageSize);
            return View(list);
        }

        /// <summary>
        ///  物理删除活动图
        /// </summary>
        /// <param name="id">活动图编号</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateActivityPicToDel(string id)
        {
            string dicMsg = string.Empty;
            int flag = 0;
            if (!string.IsNullOrEmpty(id))
            {
                flag = styleService.DelStyleActivityPic(id);
                if (flag == 1)
                {
                    dicMsg = "删除成功！";
                }
                else
                {
                    dicMsg = "删除失败！";
                }
            }
            return Json(new { result = flag, errorMsg = dicMsg });
        }
        #endregion

        #region 上传图片
        /// <summary>
        /// 上传图片显示图片名称
        /// </summary>
        /// <param name="ImgName"></param>
        /// <param name="CongifName"></param>
        /// <param name="flag">true图片是固定大小 flase图片大小在区间之内</param>
        /// <param name="error"></param>
        /// <returns></returns>
        private string ImgNameGet(string ImgName, string CongifName, bool flag, out string error)
        {
            error = string.Empty;
            if (Request.Files[ImgName] == null || Request.Files[ImgName].ContentLength == 0)
            {
                error = "未添加图片";
                return string.Empty;
            }
            Dictionary<string, string> belongsSubjectPic = new CommonService().PostImg(Request.Files[ImgName], CongifName, flag);
            string belongsSubjectPicNo = belongsSubjectPic.Values.FirstOrDefault();
            string belongsSubjectPicNokey = belongsSubjectPic.Keys.FirstOrDefault();
            if (!string.IsNullOrEmpty(belongsSubjectPicNo))
            {
                if (belongsSubjectPicNokey != "error")
                {
                    return belongsSubjectPicNo;
                }
                else
                {
                    error = belongsSubjectPicNo;
                    return string.Empty;
                }
            }
            else
            {
                error = "图片上传失败";
                return string.Empty;
            }
        }
        #endregion

        #region 轮播图管理
        public ActionResult StyleCarouselList(string name = "", string position = "0", string startTime = "", string endTime = "", int pageIndex = 1)
        {
            int pageSize = Convert.ToInt32(AppSettingManager.AppSettings["ComonListPageNum"]);
            int count = 0;
            IList<SWfsStyleMatchSpecial> list = styleService.GetList(name, position, startTime, endTime, pageIndex, pageSize, 2, out count);
            ViewBag.List = list;
            ViewBag.Count = count;
            ViewBag.CurPage = pageIndex;
            ViewBag.PageSize = pageSize;
            ViewBag.Name = name;
            ViewBag.BTime = startTime;
            ViewBag.ETime = endTime;
            ViewBag.Status = position;
            list=styleService.GetList("", "1", DateTime.Now.ToString(), endTime, pageIndex, 0, 2, out count);
            ViewBag.ShowCount = count;
            return View();
        }

        public ActionResult CarouselManage(string act, int id = 0)
        {
            SWfsStyleMatchSpecial model = new SWfsStyleMatchSpecial();
            ViewBag.Act = act;
            switch (act)
            {
                case "add"://新建
                    model.StartTime = Convert.ToDateTime("1900-1-1");
                    model.EndTime = Convert.ToDateTime("1900-1-1");
                    return View("/Areas/Shangpin/Views/Style/StyleCarouselManage.cshtml", model);
                case "edit"://修改
                    model = styleService.GetModel(id);
                    return View("/Areas/Shangpin/Views/Style/StyleCarouselManage.cshtml", model);
            }
            return View();
        }

        public ActionResult StyleCarouselManage()
        {
            string imgSize = AppSettingManager.AppSettings["StylCarouselPic"].ToString();
            string imgTyle = AppSettingManager.AppSettings["StylCarouselPicType"].ToString();
            CommonService commonService = new CommonService();
            Dictionary<string, string> rsPic = new Dictionary<string, string>();
            int id = Convert.ToInt32(Request["ID"]);
            string picUrl = Request["PicUrl"];
            SWfsStyleMatchSpecial model = new SWfsStyleMatchSpecial();
            if (id != 0)
            {
                model = styleService.GetModel(id);
                if (null == model)
                {
                    return Json(new { reslut = -1, msg = "记录不存在" });
                }
            }
            model.SpecialName = Request["SpecialName"];

            if (!string.IsNullOrEmpty(picUrl) && !picUrl.StartsWith("http://"))
            {
                return Json(new { reslut = -1, msg = "链接地址格式不正确" });
            }
            else
            {
                model.PicUrl = picUrl;
            }
            model.StartTime = Convert.ToDateTime(Request["StartTime"]);
            model.EndTime = Convert.ToDateTime(Request["EndTime"]);
            model.UpdateDate = DateTime.Now;
            model.UpdateUserId = PresentationHelper.GetPassport().UserName;
            model.Position = 1;
            model.Type = 2;
            if (id == 0) //创建
            {
                if (null != Request.Files["PicFile"] && Request.Files["PicFile"].ContentLength > 0)
                {
                    rsPic = commonService.PostImg(Request.Files["PicFile"], imgSize, imgTyle);
                    if (rsPic.Keys.Contains("error"))
                    {
                        return Json(new { reslut = -1, msg = rsPic.Values.FirstOrDefault() }, "text/plain", Encoding.UTF8);
                    }
                    if (rsPic.Keys.Contains("success"))
                    {
                        model.PicNo = rsPic["success"];
                    }
                }
                else
                {
                    return Json(new { reslut = -1, msg = "请选择图片" });
                }
                try
                {
                    model.CreateUserId = PresentationHelper.GetPassport().UserName;
                    model.CreateDate = DateTime.Now;
                    styleService.Add(model);
                    return Json(new { reslut = 1, msg = "添加成功" }, "text/plain", Encoding.UTF8);
                }
                catch (Exception e)
                {
                    return Json(new { reslut = 1, msg = e.Message });
                }
            }
            else //修改
            {
                if (null != Request.Files["PicFile"] && Request.Files["PicFile"].ContentLength > 0)
                {
                    rsPic = commonService.PostImg(Request.Files["PicFile"], imgSize, imgTyle);
                    if (rsPic.Keys.Contains("error"))
                    {
                        return Json(new { reslut = -1, msg = rsPic.Values.FirstOrDefault() }, "text/plain", Encoding.UTF8);
                    }
                    if (rsPic.Keys.Contains("success"))
                    {
                        model.PicNo = rsPic["success"];
                    }
                }
                model.ID = id;
                bool rs = styleService.Update(model);
                return Json(new { reslut = rs ? 1 : -1, msg = rs ? "修改成功" : "修改失败" }, "text/plain", Encoding.UTF8);
            }
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult StyleCarouselDelete(string id)
        {
            try
            {
                styleService.Delete(id);
                return Json(new { result = 1, msg = "删除成功" });
            }
            catch (Exception ex)
            {
                return Json(new { result = ex, msg = "删除失败" });

            }
        }

        /// <summary>
        /// 修改状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult UpdateStatus(int id)
        {
            SWfsStyleMatchSpecial model = new SWfsStyleMatchSpecial();
            model = styleService.GetModel(id);
            if (model.Position == 1)
            {
                model.Position = 2;
            }
            else
            {
                model.Position = 1;
            }
            model.ID = id;
            bool rs = styleService.Update(model);
            return Json(new { result = rs ? 1 : -1, msg = rs ? "修改成功" : "修改失败" }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}