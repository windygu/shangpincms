using Shangpin.Entity.Wfs;
using Shangpin.Framework.Common;
using Shangpin.Ocs.Entity.Extenstion.Login;
using Shangpin.Ocs.Entity.Extenstion.ShangPin;
using Shangpin.Ocs.Service;
using Shangpin.Ocs.Service.Common;
using Shangpin.Ocs.Service.Shangpin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml;

namespace Shangpin.Ocs.Web.Areas.Shangpin.Controllers
{
    public class NewChannelsController : Controller
    {
        //
        // GET: /Shangpin/NewChannels/
        private Dictionary<string, string> rsPic = new Dictionary<string, string>();
        public ActionResult Shoes()
        {
            SWfsOperationPictureService picService = new SWfsOperationPictureService();
            SWfsBrandWallService brandWallService = new SWfsBrandWallService();
            ViewBag.Banner = picService.GetIndexSwitchPictures("CHANNEL_NV003", "SwitchPicture");//头图Banner
            ViewBag.BottomBanner = picService.GetIndexOperationPictureBottom("CHANNEL_NV003", "bottomBanner");//得到全部底
            ViewBag.Banner70 = picService.GetIndexOperationPicture("CHANNEL_NV003", "BannerADHeight70");//横幅运营位
            ViewBag.RecommendBrand = brandWallService.GetHomePageRecommendBrand("CHANNEL_NV003", "");

            return View();
        }

        #region 鞋靴轮播图管理模块

        /// <summary>
        /// 轮播图列表管理
        /// </summary>
        /// <returns></returns>
        public ActionResult ShoesTopPiclist()
        {
            PaginationInfoModel pageinfo = new PaginationInfoModel();
            int gender = Rq.GetIntQueryString("gender");
            string keyWord = Rq.GetStringQueryString("keyword");
            string status = Rq.GetStringQueryString("status");
            string startTime = Rq.GetStringQueryString("StartTime");
            string endTime = Rq.GetStringQueryString("EndTime");
            int currentPage = Rq.GetIntQueryString("pageindex");
            int pictureIndex = Rq.GetIntQueryString("PagePosition");
            string startTimeValue = string.Empty, endTimeValue = string.Empty;
            string pageNo = Request.QueryString["PageNo"] == null ? "" : Request.QueryString["PageNo"];
            #region 条件筛选
            startTimeValue = string.IsNullOrEmpty(startTime) ? "" : startTime + " 00:00:00";
            endTimeValue = string.IsNullOrEmpty(endTime) ? "" : endTime + " 23:59:59";
            pageinfo.CurrentPage = currentPage;
            pageinfo.Condition += pictureIndex > 0 ? "PictureIndex=" + pictureIndex : "PictureIndex in(1,2,3,4,5,6)  ";

            pageinfo.Condition += (string.IsNullOrEmpty(keyWord) || keyWord == "名称") ? "" : " and (PictureName like '%" + keyWord + "%')";
            pageinfo.Condition += " and WebSiteNo='shangpin.com' and PageNo='" + pageNo + "' and PagePositionNo='SwitchPicture'";
            pageinfo.Condition += (string.IsNullOrEmpty(status)) ? "" : "and Status =" + status + " ";
            pageinfo.Condition += "and DataState=1";
            pageinfo.Condition += (string.IsNullOrEmpty(startTimeValue)) ? "" : "  and DateBegin >='" + startTimeValue + "'";
            pageinfo.Condition += (string.IsNullOrEmpty(endTimeValue)) ? "" : "  and DateBegin <='" + endTimeValue + "'";
            #endregion

            IList<SWfsOperationPicture> picManagerList = new SWfsOperationPictureService().GetSWfsPictureManagerListTopSwitchV(ref pageinfo);
            ViewBag.PictureManagerList = picManagerList;
            ViewBag.PaginationInfoSingel = pageinfo;
            ViewBag.KeyWord = keyWord;
            ViewBag.Status = status;
            ViewBag.StartTime = startTime;
            ViewBag.EndTime = endTime;
            ViewBag.PagePosition = pictureIndex.ToString();
            ViewBag.GenderValue = gender.ToString();
            ViewBag.PagePosition = pictureIndex.ToString();


            return View();
        }

        public ActionResult ShoesUpdateTopSwitch()
        {
            int pid = Rq.GetIntQueryString("pid");
            SWfsOperationPicture picmanagerSingle = new SWfsOperationPictureService().GetModel(pid.ToString());
            if (picmanagerSingle == null)
            {
                CommonHelp.Alert("请重新选择要修改的位置图信息", "/shangpin/NewChannels/Shoes.html");
            }
            ViewBag.PicManagerSingle = picmanagerSingle;
            return View();
        }

        [HttpPost]
        public ActionResult CreateTopSwitchNew()
        {
            SWfsOperationPicture newobj = new SWfsOperationPicture();
            string pictureName = Rq.GetStringForm("SubjectName");
            string Status = Rq.GetStringForm("Status");
            string PagePosition = Rq.GetStringForm("PagePosition");
            string linkAddress = Rq.GetStringForm("HotTwoUrl");
            string DateBegin = Rq.GetStringForm("DateBegin");
            string relationGender = Rq.GetStringForm("relationGender");
            string pictureManageEdit = Rq.GetStringForm("PictureManageEdit");
            string pictureManageId = Rq.GetStringForm("PictureManageId");
            string PictureFileNo = string.Empty;
            string pageNo = Request.QueryString["PageNo"] == null ? "" : Request.QueryString["PageNo"];
            #region 获取上传图片参数并且判断
            if (Request.Files["FlapHotTwo"] != null && Request.Files["FlapHotTwo"].ContentLength > 0)
            {
                string picSizeStr = "width:{0},Height:{1},Length:500";
                if (PagePosition == "4")//首页背景广告图片
                { picSizeStr = string.Format(picSizeStr, 0, 0); }
                else
                {//三个普通轮播图
                    picSizeStr = string.Format(picSizeStr, 770, 320);
                }
                PictureFileNo = SaveImg(Request.Files["FlapHotTwo"], picSizeStr);
                if (rsPic.Keys.Contains("error"))
                {
                    return Json(new { result = "-1", message = "图片不合法" + rsPic["error"] + "" }, "text/html");
                }
            }
            newobj = new SWfsOperationPictureService().GetModel(pictureManageId);
            if (newobj == null)
            { newobj = new SWfsOperationPicture(); newobj.DateCreate = DateTime.Now; }
            else
            {
                if (string.IsNullOrEmpty(PictureFileNo))
                {
                    PictureFileNo = newobj.PictureFileNo;
                }
            }
            newobj.PictureFileNo = PictureFileNo;
            newobj.PictureName = pictureName;
            if (string.IsNullOrEmpty(linkAddress))
            {
                linkAddress = "";
            }
            else
            {
                if (linkAddress.Length < 5 || linkAddress.Substring(0, 4) != "http")
                {
                    return Json(new { result = "-1", message = "输入链接不正确，请重新输入" }, "text/html");
                }

            }
            newobj.LinkAddress = linkAddress;
            newobj.WebSiteNo = "shangpin.com";
            newobj.PageNo = pageNo;
            newobj.PagePositionNo = "SwitchPicture";
            newobj.PagePositionName = "频道页轮播图";
            newobj.PictureIndex = Int16.Parse(PagePosition);
            newobj.Status = int.Parse(Status);
            newobj.SortId = 1;
            newobj.OperatorUserId = PresentationHelper.GetPassport().UserName;
            newobj.DateBegin = string.IsNullOrEmpty(DateBegin) ? DateTime.Now : DateTime.Parse(DateBegin);
            newobj.DateEnd = DateTime.MaxValue;
            newobj.DataState = 1;
            if (newobj.PictureManageId > 0)
            {
                new SWfsOperationPictureService().Update(newobj);
            }
            else
            {
                new SWfsOperationPictureService().Add(newobj);
            }

            return Json(new { result = "1", message = "保存成功" }, "text/html");
            #endregion
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

        #region 修改位置图状态 删除，开启，关闭 首页共用此方法（轮播图，大小运营位置图）
        /// <summary>
        /// 修改位置图状态 删除，开启，关闭 首页共用此方法（轮播图，大小运营位置图）
        /// </summary>
        /// <param name="subjectNo"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PictureManagerStatusModify(string SubjectNo, string status, string DelectStatus)
        {
            bool rs = false;
            string messageStr = string.Empty;
            SWfsOperationPictureService service = new SWfsOperationPictureService();
            SWfsOperationPicture model = service.GetModel(SubjectNo);

            #region 查询是否每一帧是否有一条数据
            short pictureIndex = model.PictureIndex;
            if (model.PagePositionNo == "SwitchPicture")
            {
                pictureIndex = 0;
            }
            var resultList = service.GetSWfsOperationPictureSwitchCondition(model.PageNo, model.PagePositionNo, pictureIndex);
            #endregion

            if (model != null)
            {
                if (DelectStatus != "0")
                {
                    int delcount = service.DelSwitchPicture(SubjectNo);
                    if (delcount > 0)
                    {
                        messageStr = "删除成功";
                        rs = true;
                    }
                }
                else
                {
                    model.Status = int.Parse(status);
                    rs = service.Update(model);
                    messageStr = "状态修改成功";
                }
                //}
            }
            try
            {
                if (rs)
                {
                    return Json(new { result = "1", message = messageStr });
                }
                else
                { return Json(new { result = "0", message = "操作失败！" }); }
            }
            catch (Exception ex)
            {
                return Json(new { result = "0", message = "操作失败！" });
            }
        }
        #endregion

        public ActionResult shoescreatetopswitch()
        {
            return View();
        }

        #endregion

        #region  品牌墙管理
        //查询列表
        public ActionResult ShoesBrandSelectList(string pagePositionNo = "", int isRecommendBrand = 0)
        {
            string pageNo = Request.QueryString["PageNo"] == null ? "" : Request.QueryString["PageNo"];
            if (pageNo == "")
            {
                pageNo = Rq.GetStringQueryString("PageNo");
            }
            pagePositionNo = Request.QueryString["PagePositionNo"] == null ? "" : Request.QueryString["PagePositionNo"];
            string startTime = Rq.GetStringQueryString("starttime");
            string endTime = Rq.GetStringQueryString("endtime");
            string brandNoAndBrandName = Rq.GetStringQueryString("brandNoAndBrandName");
            SWfsBrandWallService brandWallService = new SWfsBrandWallService();

            IEnumerable<SpHomeRecommendBrandExtends> list = brandWallService.GetBrandList(pageNo, pagePositionNo, isRecommendBrand, startTime, endTime, brandNoAndBrandName);
            list = list.OrderBy(n => n.SortId).ThenByDescending(p => p.CreateDate);//.OrderByDescending(p => p.CreateDate);
            //已添加品牌总数
            ViewBag.BrandCount = list == null ? 0 : list.Count();
            return View(list);
        }

        /// <summary>
        /// 添加品牌
        /// </summary>
        /// <returns></returns>
        public ActionResult AddShoesBrands()
        {
            return View();
        }

        /// <summary>
        /// 添加品牌
        /// </summary>
        /// <param name="PageNo"></param>
        /// <param name="brandNo"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddShoesBrands(string pageNo, string pagePositionName, string pagePositionNo)
        {
            SWfsBrandWallService service = new SWfsBrandWallService();

            string brandSelectedValue = Request.Params["brandSelected"] ?? "";
            //string VName = Request.Params["Vname"] == null ? "" : Request.Params["Vname"];
            brandSelectedValue = brandSelectedValue.TrimEnd(',');
            if (string.IsNullOrEmpty(brandSelectedValue))
            {
                return Json(new { status = 1, message = "请选择品牌" });
            }

            if (string.IsNullOrEmpty(pagePositionName) || pagePositionName.Length > 12)
            {
                return Json(new { status = 1, message = "名称不能为空或者不能超过12个字" });
            }
            var brandList = service.GetBrandList(pageNo, pagePositionNo, 0, "", "", "");

            if (brandList != null && brandList.Count() > 0)
            {
                if (brandList.Count() + brandSelectedValue.Split(',').Except(new string[] { "" }).Count() < 15)
                {
                    return Json(new { status = 1, message = "选择品牌数量必须大于等于15个" });
                }
            }
            else
            {
                if (brandSelectedValue.Split(',').Except(new string[] { "" }).Count() < 15)
                {
                    return Json(new { status = 1, message = "选择品牌数量必须大于等于15个" });
                }
            }


            var brands = brandSelectedValue.Split(',');

            foreach (var brandNo in brands)
            {
                SWfsSpHomeRecommendBrand recommentBrand = new SWfsSpHomeRecommendBrand()
                {
                    PagePositionNo = pagePositionNo,
                    IsRecommendBrand = 0,
                    CreateDate = DateTime.Now,
                    DataState = 1,
                    UpdateDate = DateTime.Now,
                    BrandNO = brandNo,
                    Status = 1,
                    OperateUserId = PresentationHelper.GetPassport().UserName,
                    PageNo = pageNo,
                    PictureFileNo = "",
                    UpdateOperateUserId = "",
                    WebSiteNo = "shangpin.com",
                    PagePositionName = pagePositionName
                };

                service.InsertSWfsSpChannelRecommendBrand(recommentBrand);
            }

            //更改positionName值
            if (pageNo + "" != "" && pagePositionNo + "" != "" && pagePositionName + "" != "")
            {
                service.UpdateRecommendBrandPositionName(pageNo, pagePositionNo, pagePositionName);
            }
            return Json(new { status = 2, message = "保存成功" });
        }

        //按主键删除首页推荐品牌
        public ActionResult DeleteHomeBrandByID(int id, string pageNo, int brandCount, string pagePositionNo)
        {

            SWfsBrandWallService service = new SWfsBrandWallService();
            brandCount = service.GetHomePageRecommendBrand(pageNo, pagePositionNo).Count();
            if (pagePositionNo != "InternationalRecommendBrand" && pagePositionNo != "PopularRecommendBrand")
            {
                if (brandCount <= 15)
                {
                    return Json(new { status = 0, message = "品牌最少为15个,删除无效" });
                }
                else
                {
                    SWfsOperationPictureService picSercive = new SWfsOperationPictureService();
                    picSercive.DeleteHomeBrand(id);
                }
            }
            else
            {
                if (brandCount <= 1)
                {
                    return Json(new { status = 0, message = "推荐品牌不能少于1个,删除无效" });
                }
                else
                {
                    SWfsOperationPictureService picSercive = new SWfsOperationPictureService();
                    picSercive.DeleteHomeBrand(id);
                }
            }
            return Json(new { status = 1, message = "删除成功" });
        }

        //批量修改排序值
        public JsonResult EditeListSortValue()
        {
            try
            {
                SWfsBrandWallService picSercive = new SWfsBrandWallService();
                if (string.IsNullOrEmpty(Request.Form["bList"]))
                {
                    return Json(new { status = 1, message = "品牌编号不存在" }, JsonRequestBehavior.AllowGet);
                }
                string[] bList = Request.Form["bList"].Split(',');
                string[] bIndex = Request.Form["bIndex"].Split(',');
                for (int i = 0; i < bList.Length; i++)
                {
                    picSercive.EditeBrandSortValue(int.Parse(bList[i]), int.Parse(bIndex[i]));
                }
                return Json(new { status = 0, message = "修改成功" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(new { status = 1, message = "修改失败" + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        //热门推荐品牌右侧广告管理列表
        public ActionResult AddShoesHomeBrands()
        {
            //SWfsBrandWallService service = new SWfsBrandWallService();
            //string pageNo = Rq.GetStringQueryString("PageNo");
            //string pagePositionNo = Rq.GetStringQueryString("PagePositionNo");
            //string pagePositionName = Rq.GetStringQueryString("PagePositionName");
            //var list = service.GetBrandList(pageNo, pagePositionNo, 1, "", "", "");
            //return View(list);
            return View();
        }

        [HttpPost]
        public string SaveShoesHomeBrand()
        {
            string PictureFileNo = "";
            string pagePositionNo = Rq.GetStringForm("pagePositionNo");
            string pagePositionName = Rq.GetStringForm("pagePositionName");
            string RecommendBrandID = Rq.GetStringForm("RedId");
            string brandNo = Rq.GetStringForm("brandNo");
            string StartTime = Rq.GetStringForm("txt_date");
            PictureFileNo = Rq.GetStringForm("PictureFileNo");
            string pageNo = Request.QueryString["PageNo"] == null ? "" : Request.QueryString["PageNo"];
            if (pageNo == "")
            {
                pageNo = Rq.GetStringQueryString("PageNo");
            }
            SWfsBrandWallService service = new SWfsBrandWallService();
            if (string.IsNullOrEmpty(StartTime))
            {
                return "开始时间不能为空！";
            }
            if (string.IsNullOrEmpty(pageNo))
            {
                return "页面编号错误！";
            }
            if (string.IsNullOrEmpty(brandNo))
            {
                return "品牌编号不能为空";
            }
            if (service.IsExistHomeBrand(pageNo, pagePositionNo, brandNo) > 1)
            {
                return "已存在该品牌";
            }
            if (service.IsExistHomeBrand(pageNo, pagePositionNo, brandNo) > 200)
            {
                return "不能添加超过200个品牌";
            }
            if (PictureFileNo == "")
            {
                if (Request.Files["FlapHotTwo"] != null && Request.Files["FlapHotTwo"].ContentLength > 0)
                {
                    string picSizeStr = "width:{0},Height:{1},Length:500";

                    picSizeStr = string.Format(picSizeStr, 128, 200);

                    PictureFileNo = SaveImg(Request.Files["FlapHotTwo"], picSizeStr);
                    if (rsPic.Keys.Contains("error"))
                    {
                        return "图片不合法";
                    }
                }
            }
            if (string.IsNullOrEmpty(PictureFileNo))
            {
                return "请上传图片";
            }
            var list = service.GetBrandList(pageNo, pagePositionNo, 1, "", "", brandNo);

            if (string.IsNullOrEmpty(PictureFileNo))
            {
                PictureFileNo = list.FirstOrDefault().PictureFileNo;
            }

            SWfsSpHomeRecommendBrand recommentBrand = new SWfsSpHomeRecommendBrand()
            {
                RecommendBrandID = RecommendBrandID == "" ? 0 : Convert.ToInt32(RecommendBrandID),
                PagePositionNo = pagePositionNo,
                IsRecommendBrand = 1,
                CreateDate = Convert.ToDateTime(StartTime),
                DataState = 1,
                UpdateDate = DateTime.Now,
                BrandNO = brandNo,
                Status = 1,
                OperateUserId = PresentationHelper.GetPassport().UserName,
                PageNo = pageNo,
                PictureFileNo = PictureFileNo,
                UpdateOperateUserId = "",
                WebSiteNo = "shangpin.com",
                PagePositionName = pagePositionName

            };

            var result = service.InsertSWfsSpChannelRecommendBrandChannel(recommentBrand);


            if (result > 0)
            {
                return "1";
            }
            else
            {
                return "0";
            }
        }
        #endregion

        #region  横幅广告管理列表
        /// <summary>
        /// 运营位置图 展现页面
        /// </summary>
        public ActionResult ShoesIndexBottonList()
        {
            PaginationInfoModel pageinfo = new PaginationInfoModel();
            //int gender = Rq.GetIntQueryString("gender");
            string keyWord = Rq.GetStringQueryString("keyword");
            string status = Rq.GetStringQueryString("status");
            string startTime = Rq.GetStringQueryString("StartTime");
            string endTime = Rq.GetStringQueryString("EndTime");
            int currentPage = Rq.GetIntQueryString("pageindex");
            int pictureIndex = Rq.GetIntQueryString("PictureIndex");
            string pagePositionNo = Rq.GetStringQueryString("PagePositionNo");
            ViewBag.PagePositionNo = pagePositionNo;
            string startTimeValue = string.Empty, endTimeValue = string.Empty;
            string pageNo = Request.QueryString["PageNo"] == null ? "" : Request.QueryString["PageNo"];
            if (pageNo == "")
            {
                pageNo = Rq.GetStringQueryString("PageNo");
            }
            #region 条件筛选
            startTimeValue = string.IsNullOrEmpty(startTime) ? "" : startTime + " 00:00:00";
            endTimeValue = string.IsNullOrEmpty(endTime) ? "" : endTime + " 23:59:59";
            pageinfo.CurrentPage = currentPage;
            pageinfo.Condition += "PageNo='" + pageNo + "' and PagePositionNo='" + pagePositionNo + "'";
            pageinfo.Condition += (string.IsNullOrEmpty(keyWord) || keyWord == "名称") ? "" : " and (PictureName like '%" + keyWord + "%')";
            pageinfo.Condition += (string.IsNullOrEmpty(status)) ? "" : "and Status =" + status + "";
            pageinfo.Condition += " and DataState=1";
            pageinfo.Condition += (string.IsNullOrEmpty(startTimeValue)) ? "" : "  and DateCreate >='" + startTimeValue + "'";
            pageinfo.Condition += (string.IsNullOrEmpty(endTimeValue)) ? "" : "  and DateCreate <='" + endTimeValue + "'";
            #endregion

            IList<SWfsOperationPicture> picManagerList = new SWfsOperationPictureService().GetSWfsPictureManagerListTopSwitchV(ref pageinfo);
            ViewBag.PictureManagerList = picManagerList;
            ViewBag.PaginationInfoSingel = pageinfo;
            ViewBag.KeyWord = keyWord;
            ViewBag.Status = status;
            ViewBag.StartTime = startTime;
            ViewBag.EndTime = endTime;
            //ViewBag.GenderValue = gender.ToString();
            ViewBag.PagePosition = pictureIndex.ToString();
            return View();
        }

        public ActionResult ShoesCreateBottonPagePosition()
        {
            ViewBag.positionNo = Rq.GetStringQueryString("PagePositionNo");
            ViewBag.positionName = Rq.GetStringQueryString("PagePositionName");
            return View();
        }

        public ActionResult ShoesUpdateBottonPagePosition()
        {
            ViewBag.positionNo = Rq.GetStringQueryString("PagePositionNo");
            ViewBag.positionName = Rq.GetStringQueryString("PagePositionName");
            //ViewBag.PictureIndex = PictureIndex;
            int pid = Rq.GetIntQueryString("pid");
            SWfsOperationPicture picmanagerSingle = new SWfsOperationPictureService().GetModel(pid.ToString());
            string pageNo = Request.QueryString["PageNo"] == null ? "" : Request.QueryString["PageNo"];
            if (pageNo == "")
            {
                pageNo = Rq.GetStringForm("PageNo");
            }
            if (picmanagerSingle == null)
            {
                CommonHelp.Alert("请重新选择要修改的位置图信息", "/shangpin/NewChannels/ShoesIndexBottonList.html?PagePositionNo=" + picmanagerSingle.PagePositionNo + "&PageNo=" + pageNo);
            }
            ViewBag.PicManagerSingle = picmanagerSingle;
            return View();
        }

        /// <summary>
        /// 添加运营位置图  
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateBottonPagePositionData()
        {
            SWfsOperationPicture newobj = new SWfsOperationPicture();
            string pictureName = Rq.GetStringForm("SubjectName");//图片标题
            string pictureIndex = Rq.GetStringForm("PictureIndex");//位置内索引 
            pictureIndex = string.IsNullOrEmpty(pictureIndex) ? "0" : pictureIndex;
            string linkAddress = Rq.GetStringForm("HotTwoUrl");//链接地址
            string positionName = Rq.GetStringForm("PagePositionName");//位置名称
            string pagePositionNo = Rq.GetStringForm("PagePositionNo");//页面内位置
            string DateBegin = Rq.GetStringForm("DateBegin");
            string pictureManageId = Rq.GetStringForm("PictureManageId");
            string PictureFileNo = string.Empty;
            string pageNo = Request.QueryString["PageNo"] == null ? "" : Request.QueryString["PageNo"];
            if (pageNo == "")
            {
                pageNo = Rq.GetStringForm("PageNo");
            }
            if (string.IsNullOrEmpty(pictureIndex) && pagePositionNo == "bottomBanner")
            {
                return Json(new { result = "-1", message = "请选择图片位置填写信息，请重新选择位置图片" }, "text/html");
            }
            #region 获取上传图片参数并且判断
            if (Request.Files["FlapHotTwo"] != null && Request.Files["FlapHotTwo"].ContentLength > 0)
            {
                string picSizeStr = "width:{0},Height:{1},Length:200";
                if (pagePositionNo.IndexOf("bottomBanner") > -1)
                {
                    picSizeStr = string.Format(picSizeStr, 315, 150);
                }
                else
                {
                    picSizeStr = string.Format(picSizeStr, 770, 70);
                }
                PictureFileNo = SaveImg(Request.Files["FlapHotTwo"], picSizeStr);
                if (rsPic.Keys.Contains("error"))
                {
                    return Json(new { result = "-1", message = "图片不合法" + rsPic["error"] + "" }, "text/html");
                }
            }
            newobj = new SWfsOperationPictureService().GetModel(pictureManageId);
            if (newobj == null)
            {
                newobj = new SWfsOperationPicture();
                newobj.DateCreate = DateTime.Now;
            }
            else
            {
                if (string.IsNullOrEmpty(PictureFileNo))
                {
                    PictureFileNo = newobj.PictureFileNo;
                }
            }
            newobj.PictureFileNo = PictureFileNo;
            newobj.PictureName = pictureName;
            if (string.IsNullOrEmpty(linkAddress))
            {
                linkAddress = "";
            }
            else
            {
                if (linkAddress.Length < 5 || linkAddress.Substring(0, 4) != "http")
                {
                    return Json(new { result = "-1", message = "输入链接不正确，请重新输入" }, "text/html");
                }

            }

            newobj.LinkAddress = linkAddress;
            newobj.Status = 1;
            newobj.DataState = 1;
            newobj.SortId = 1;
            newobj.OperatorUserId = PresentationHelper.GetPassport().UserName;
            newobj.WebSiteNo = "shangpin.com";
            newobj.PageNo = pageNo;
            newobj.PagePositionNo = pagePositionNo;
            newobj.PagePositionName = positionName;
            newobj.PictureIndex = Int16.Parse(pictureIndex);
            newobj.DateBegin = string.IsNullOrEmpty(DateBegin) ? DateTime.Now : Convert.ToDateTime(DateBegin);
            newobj.DateEnd = DateTime.MaxValue;
            if (newobj.PictureManageId > 0)
            {
                new SWfsOperationPictureService().Update(newobj);
            }
            else
            {
                new SWfsOperationPictureService().Add(newobj);
            }

            return Json(new { result = "1", message = "保存成功" }, "text/html");
            #endregion
        }

        /// <summary>
        /// 获取横幅广告列表
        /// </summary>
        /// <param name="PageNo"></param>
        /// <returns></returns>
        public string GetStatusbanner(string PageNo)
        {
            string result = "";
            NewsChannelsSservice NewServiceNotice = new NewsChannelsSservice();
            result = NewServiceNotice.GetStatusBananaPicture(PageNo);
            return result;
        }

        /// <summary>
        /// 关闭横幅广告列表
        /// </summary>
        /// <param name="PageNo"></param>
        /// <param name="Status"></param>
        /// <returns></returns>
        public string ChannelStatusbanner(string PageNo, string Status)
        {
            string result = "";
            NewsChannelsSservice NewServiceNotice = new NewsChannelsSservice();
            Status = Status == "0" ? "1" : "0";
            result = NewServiceNotice.ChannelBananaPicture(PageNo, Status);
            return result;
        }
        #endregion

        #region 楼层管理 -hbq
        NewsChannelsSservice _newsChannelsService = new NewsChannelsSservice();
        SWfsIndexModuleService sWfsIndexModuleService = new SWfsIndexModuleService();
        SWfsIndexModuleLinkService sWfsIndexModuleLinkService = new SWfsIndexModuleLinkService();
        #region 楼层
        public ActionResult AlterFloorName(int ModuleId, string ModuleName)
        {
            try
            {
                SWfsIndexModule module = sWfsIndexModuleService.GetSWfsIndexModuleById(ModuleId);
                module.ModuleName = ModuleName;
                sWfsIndexModuleService.InsertOrUpdateSWfsIndexModule(module);
                return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #region  频道状态切换
        public JsonResult ChangeChannelStatus(string PageNo, string value)
        {
            try
            {
                if (!string.IsNullOrEmpty(PageNo))
                    new SWfsProductCommentService().UpdateGlobalConfig(PageNo, value);
            }
            catch (Exception)
            {
                return Json(new { status = -1 }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetChannelStatus(string PageNo)
        {
            try
            {
                SWfsGlobalConfig config = new SWfsGlobalConfig();
                if (!string.IsNullOrEmpty(PageNo))
                    config = new SWfsProductCommentService().GetGlobalConfigByFNo(PageNo) ?? new SWfsGlobalConfig { ConfigtName = "", ConfigValue = "" };
                return Json(new { status = 1, data = new { name = config.ConfigtName, value = config.ConfigValue } }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = -1 }, JsonRequestBehavior.AllowGet);
            }

        }
        #endregion
        #region 楼层链接管理
        /// <summary>
        /// 楼层链接管理
        /// </summary>
        /// <returns></returns>
        public ActionResult FloorLinkSet(string moduleId, string moduleName)
        {
            int id = 0;
            List<SWfsIndexModuleLink> links = new List<SWfsIndexModuleLink>();
            if (int.TryParse(moduleId, out id) && id > 0)
            {
                links = sWfsIndexModuleLinkService.GetSWfsIndexModuleLinkByModuleId(id).Where(a => a.LinkType == 1).FilterList() ?? new List<SWfsIndexModuleLink>();
            }
            ViewData["links"] = links;
            ViewBag.moduleId = moduleId;
            ViewBag.moduleName = moduleName;
            return View();
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult FloorLinkSet(FormCollection F)
        {
            try
            {
                int moduleId = 0;
                List<SWfsIndexModuleLink> links = new List<SWfsIndexModuleLink>();
                if (!string.IsNullOrEmpty(F["moduleId"]) && int.TryParse(F["moduleId"], out moduleId) && moduleId > 0)
                {
                    for (int i = 1; i < 2; i++)
                    {
                        #region 对象赋值
                        SWfsIndexModuleLink link = new SWfsIndexModuleLink();
                        link.LinkId = Convert.ToInt32(F["linkid" + i]);
                        if (link.LinkId > 0)
                        {
                            link = sWfsIndexModuleLinkService.GetSwfsIndexModuleLinkByLinkId(link.LinkId);
                        }
                        link.ModuleId = moduleId;
                        link.LinkName = F["word" + i];
                        link.LinkHref = F["link" + i];
                        link.CategoryNo = string.Empty;
                        link.LinkType = (short)1;
                        link.SortId = i - 1;
                        if (link.LinkId == 0)
                        {
                            link.DateCreate = DateTime.Now;
                            link.UpdateDate = _newsChannelsService.MinTime;
                            link.OperateUserId = PresentationHelper.GetPassport().UserName;
                        }
                        else
                        {
                            link.UpdateDate = DateTime.Now;
                        }
                        link.UpdateDate = Convert.ToDateTime("1991/02/10 01:01:00");
                        link.UpdateOperateUserId = link.LinkId > 0 ? PresentationHelper.GetPassport().UserName : string.Empty;
                        link.Stutas = 1;
                        link.DataState = 1;
                        #endregion
                        links.Add(link);
                    }
                    sWfsIndexModuleLinkService.InsertOrUpdateFloorSWfsIndexModuleLink(links);
                    return Json(new { status = 1, message = "操作成功!" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { status = 1, message = "数据格式不正确!" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(new { status = 0, message = "操作失败!" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #region 楼层广告图管理
        #region 分类楼层广告

        /// <summary>
        /// 分类楼层
        /// </summary>
        /// <returns></returns>
        public ActionResult ClassifiedFloorADManager(string PageNo = null, int pageIndex = 1, int pageSize = 20)
        {
            PageNo = string.IsNullOrEmpty(PageNo) ? NewsChannelsSservice.WomenShoes : PageNo;
            ViewBag.PageNo = PageNo;
            SWfsIndexModule Module = _newsChannelsService.GetChanelFloors(PageNo).First(a => a.ADShowType == 5);
            ViewBag.moduleName = Module.ModuleName;
            ViewBag.moduleId = Module.ModuleId;
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("PictureName", string.IsNullOrEmpty(Request.QueryString["PictureName"]) ? "" : Request.QueryString["PictureName"]);
            dic.Add("DateBegin", string.IsNullOrEmpty(Request.QueryString["DateBegin"]) ? "" : Request.QueryString["DateBegin"]);
            dic.Add("DateEnd", string.IsNullOrEmpty(Request.QueryString["DateEnd"]) ? "" : Request.QueryString["DateEnd"]);
            dic.Add("PictureIndex", string.IsNullOrEmpty(Request.QueryString["PictureIndex"]) ? -1 : Convert.ToInt32(Request.QueryString["PictureIndex"]));
            dic.Add("PagePositionNo", Module.ModuleId.ToString());
            dic.Add("object", new
            {
                PagePositionNo = Module.ModuleId.ToString(),
                PictureName = dic["PictureName"] + "",
                DateBegin = dic["DateBegin"] + "",
                DateEnd = dic["DateEnd"] + "",
                pageIndex = pageIndex,
                pageSize = pageSize,
                PictureIndex = dic["PictureIndex"],
                WebSiteNo = Module.WebSiteNo,
                PageNo = Module.PageNo
            });
            int count = 0;
            ViewBag.list = _newsChannelsService.GetSWfsindexModuleRefPic(dic, pageIndex, pageSize, out count);
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;
            ViewBag.TotalCount = count;
            ViewBag.picSizeAndTip = _newsChannelsService.picSizeAndTip;
            return View();
        }
        /// <summary>
        /// 分类图编辑
        /// </summary>
        /// <param name="picId"></param>
        /// <param name="moduleId"></param>
        /// <param name="pictureIndex"></param>
        /// <returns></returns>
        public ActionResult EditClassifiedFloorAD(int picId = 0, int moduleId = 0, int pictureIndex = 0)
        {

            ViewBag.dic = _newsChannelsService.GetPicSizeAndTip();
            SWfsOperationPicture pic = new SWfsOperationPicture();
            SWfsIndexModule Module = sWfsIndexModuleService.GetSWfsIndexModuleById(moduleId);
            pic.PictureIndex = Convert.ToInt16(pictureIndex);
            ViewBag.sizeTip = _newsChannelsService.picSizeAndTip.Keys.Contains("pic" + (pictureIndex + 1) + "Tip") ? _newsChannelsService.picSizeAndTip["pic" + (pictureIndex + 1) + "Tip"] : "";
            ViewBag.moduleId = moduleId;
            ViewBag.moduleName = Module.ModuleName;
            ViewBag.ADShowType = Module.ADShowType;
            ViewBag.PicType = _newsChannelsService.picSizeAndTip.Keys.Contains("pic" + (pictureIndex + 1) + "Name") ? _newsChannelsService.picSizeAndTip["pic" + (pictureIndex + 1) + "Name"] : "";
            if (picId > 0)
                pic = sWfsIndexModuleService.GetSWfsOperationPictureByPictureManageId(picId);
            pic.PictureIndex = (short)pictureIndex;//重新定位 
            return View(pic);
        }
        /// <summary>
        /// 保存分类图
        /// </summary>
        /// <param name="F"></param>
        /// <param name="PageNo"></param>
        /// <returns></returns>
        [HttpPost, ValidateInput(false)]
        public string SaveClassifiedFloorAD(FormCollection F, string PageNo = null)
        {
            try
            {
                PageNo = string.IsNullOrEmpty(PageNo) ? "" : PageNo;
                SWfsOperationPicture picTopModel = _newsChannelsService.GetEmptySWfsOperationPictureModel();
                Dictionary<string, string> dic = new Dictionary<string, string>();
                string picSizeStr = _newsChannelsService.picSizeAndTip.ContainsKey("pic" + (Convert.ToInt32(F["PictureIndex"]) + 1) + "Check") ? _newsChannelsService.picSizeAndTip["pic" + (Convert.ToInt32(F["PictureIndex"]) + 1) + "Check"] : "";
                int picId = Convert.ToInt32(F["PictureManageId"]);
                short newIndex = Convert.ToInt16(F["pictureIndex"]);
                string picOldFileId = "";
                if (picId > 0)
                {
                    picTopModel = sWfsIndexModuleService.GetSWfsOperationPictureByPictureManageId(picId);
                    picOldFileId = picTopModel.PictureFileNo;
                    Boolean flag = false;
                    switch (picTopModel.PictureIndex)
                    {
                        case 0: { if (newIndex == 0 || newIndex == 4) flag = true; }; break;
                        case 1: { if (newIndex == 1 || newIndex == 3) flag = true; }; break;
                        case 2: { if (newIndex == 2) flag = true; }; break;
                        case 3: { if (newIndex == 1 || newIndex == 3) flag = true; }; break;
                        case 4: { if (newIndex == 0 || newIndex == 4) flag = true; }; break;
                    }
                    if (!flag && !(Request.Files["picFile"] != null && Request.Files["picFile"].ContentLength > 0))//如果类型切换的尺寸不兼容
                    {
                        return "{\"status\" : -1, \"message\" : \"切换至此位置,图片尺寸发生变化,请重新上传图片\"}";
                    }
                    SWfsIndexModule module = sWfsIndexModuleService.GetSWfsIndexModuleById(Convert.ToInt32(picTopModel.PagePositionNo));
                    List<SWfsOperationPicture> pics = _newsChannelsService.GetChannelSWfsOperationPicture(module.ModuleId, module.ADShowType, 500);
                    if (picTopModel.DateBegin < DateTime.Now && (pics.Count(a => a.PictureIndex == picTopModel.PictureIndex) <= 1 && newIndex != picTopModel.PictureIndex))
                    {
                        return "{\"status\" : -1, \"message\" : \"修改无效,切换后会导致之前图片位置没有可用图片\"}";
                    }
                }
                picTopModel.PictureIndex = newIndex;
                picTopModel.PictureName = F["picTitleTop"];
                picTopModel.WebSiteNo = NewsChannelsSservice.Website;
                picTopModel.PageNo = PageNo;
                picTopModel.PagePositionNo = F["moduleId"];
                picTopModel.LinkAddress = F["LinkAddress"];
                picTopModel.PagePositionName = F["moduleName"];
                picTopModel.DateBegin = Convert.ToDateTime(F["startTimeTop"]);
                picTopModel.PageNo = F["PageNo"];
                //时间判定查重
                if (picId == 0 && sWfsIndexModuleService.CheckModulePictureSameTime(Convert.ToInt32(picTopModel.PagePositionNo), picTopModel.PictureIndex, picTopModel.DateBegin) > 0)
                {
                    return "{\"status\" : -1, \"message\" : \"此图片位置已存在相同开始时间图片\"}";
                }
                if (Request.Files["picFile"] != null && Request.Files["picFile"].ContentLength > 0)
                {
                    dic = sWfsIndexModuleService.SaveImg(Request.Files["picFile"], picSizeStr);
                    if (dic.Keys.Contains("error"))
                    {
                        return "{\"status\" : 0, \"message\" : \"图片不合法" + dic["error"] + "\"}";
                    }
                    else if (dic.Keys.Contains("success")) { picTopModel.PictureFileNo = dic["success"]; }
                    if (!string.IsNullOrEmpty(picOldFileId))//删除旧图片
                    {
                        sWfsIndexModuleService.DeleteImg(picOldFileId);
                    }
                }
                sWfsIndexModuleService.InsertOrUpdateSWfsOperationPicture(picTopModel);
                return "{\"status\" : 1, \"message\" : \"保存成功\"}";
            }
            catch (Exception)
            {
                return "{\"status\" : -1, \"message\" : \"操作失败请刷新页面重试\"}";
            }
        }
        #endregion
        /// <summary>
        /// 楼层广告图管理列表
        /// </summary>
        /// <returns></returns>
        [ValidateInput(false)]
        public ActionResult FloorADPicManager(string PageNo = null, int moduleId = 0, int pageIndex = 1, int pageSize = 20)
        {
            PageNo = string.IsNullOrEmpty(PageNo) ? NewsChannelsSservice.WomenShoes : PageNo;
            ViewBag.PageNo = PageNo;
            SWfsIndexModule Module = _newsChannelsService.GetChanelFloors(PageNo).First(a => a.ModuleId == moduleId);
            ViewBag.moduleName = Module.ModuleName;
            ViewBag.moduleId = moduleId;
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("PictureName", string.IsNullOrEmpty(Request.QueryString["PictureName"]) ? "" : Request.QueryString["PictureName"]);
            dic.Add("DateBegin", string.IsNullOrEmpty(Request.QueryString["DateBegin"]) ? "" : Request.QueryString["DateBegin"]);
            dic.Add("DateEnd", string.IsNullOrEmpty(Request.QueryString["DateEnd"]) ? "" : Request.QueryString["DateEnd"]);
            dic.Add("PictureIndex", string.IsNullOrEmpty(Request.QueryString["PictureIndex"]) ? -1 : Convert.ToInt32(Request.QueryString["PictureIndex"]));
            dic["PagePositionNo"] = moduleId.ToString();
            dic["object"] = new
            {
                PagePositionNo = Module.ModuleId.ToString(),
                PictureName = dic["PictureName"] + "",
                DateBegin = dic["DateBegin"] + "",
                DateEnd = dic["DateEnd"] + "",
                pageIndex = pageIndex,
                pageSize = pageSize,
                PictureIndex = dic["PictureIndex"],
                WebSiteNo = Module.WebSiteNo,
                PageNo = Module.PageNo
            };
            int count = 0;
            ViewBag.list = _newsChannelsService.GetSWfsindexModuleRefPic(dic, pageIndex, pageSize, out count);
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;
            ViewBag.TotalCount = count;
            ViewBag.picSizeAndTip = _newsChannelsService.picSizeAndTip;

            return View();
        }
        /// <summary>
        /// 编辑楼层广告图
        /// </summary>
        /// <param name="linkId"></param>
        /// <returns></returns>
        public ActionResult EditFloorAD(int picId, int moduleId, int pictureIndex = 0)
        {
            SWfsOperationPicture pic = new SWfsOperationPicture();
            SWfsIndexModule Module = sWfsIndexModuleService.GetSWfsIndexModuleById(moduleId);
            pic.PictureIndex = Convert.ToInt16(pictureIndex);
            ViewBag.sizeTip = _newsChannelsService.picSizeAndTip["picSingleTip"];
            ViewBag.moduleId = moduleId;
            ViewBag.moduleName = Module.ModuleName;
            ViewBag.ADShowType = Module.ADShowType;
            ViewBag.PicType = _newsChannelsService.picSizeAndTip["picSingleName"];
            if (picId > 0)
                pic = sWfsIndexModuleService.GetSWfsOperationPictureByPictureManageId(picId);
            pic.PictureIndex = (short)pictureIndex;//重新定位 
            return View(pic);
        }
        [HttpPost]
        public string EditFloorAD(FormCollection F)
        {
            try
            {
                SWfsOperationPicture picTopModel = _newsChannelsService.GetEmptySWfsOperationPictureModel();
                Dictionary<string, string> dic = new Dictionary<string, string>();
                string picSizeStr = _newsChannelsService.picSizeAndTip["picSingleCheck"];
                int picId = Convert.ToInt32(F["PictureManageId"]);
                string picOldFileId = "";
                if (picId > 0)
                {
                    picTopModel = sWfsIndexModuleService.GetSWfsOperationPictureByPictureManageId(picId);
                    picOldFileId = picTopModel.PictureFileNo;
                }
                picTopModel.PictureIndex = Convert.ToInt16(F["pictureIndex"]);
                picTopModel.PictureName = F["picTitleTop"];
                picTopModel.WebSiteNo = NewsChannelsSservice.Website;
                picTopModel.PagePositionNo = F["moduleId"];
                picTopModel.LinkAddress = F["LinkAddress"];
                picTopModel.PagePositionName = F["moduleName"];
                picTopModel.DateBegin = Convert.ToDateTime(F["startTimeTop"]);
                picTopModel.PageNo = F["PageNo"];
                //时间判定查重
                if (picId == 0 && sWfsIndexModuleService.CheckModulePictureSameTime(Convert.ToInt32(picTopModel.PagePositionNo), picTopModel.PictureIndex, picTopModel.DateBegin) > 0)
                {
                    return "{\"status\" : -1, \"message\" : \"此楼层同广告位已存在相同开始时间图片\"}";
                }
                if (Request.Files["picFile"] != null && Request.Files["picFile"].ContentLength > 0)
                {
                    dic = sWfsIndexModuleService.SaveImg(Request.Files["picFile"], picSizeStr);
                    if (dic.Keys.Contains("error"))
                    {
                        return "{\"status\" : 0, \"message\" : \"图片不合法" + dic["error"] + "\"}";
                    }
                    else if (dic.Keys.Contains("success")) { picTopModel.PictureFileNo = dic["success"]; }
                    if (!string.IsNullOrEmpty(picOldFileId))//删除旧图片
                    {
                        sWfsIndexModuleService.DeleteImg(picOldFileId);
                    }
                }
                sWfsIndexModuleService.InsertOrUpdateSWfsOperationPicture(picTopModel);
                return "{\"status\" : 1, \"message\" : \"保存成功\"}";
            }
            catch (Exception)
            {
                return "{\"status\" : -1, \"message\" : \"操作失败请刷新页面重试\"}";
            }
        }
        /// <summary>
        /// 删除楼层广告图 通过广告图ID
        /// </summary>
        /// <param name="picId"></param>
        /// <returns></returns>
        public ActionResult DeleteFloorPic(string picId)
        {
            try
            {
                int id = 0;
                if (int.TryParse(picId, out id) && id > 0)
                {
                    SWfsOperationPicture pic = sWfsIndexModuleService.GetSWfsOperationPictureByPictureManageId(id);
                    SWfsIndexModule module = sWfsIndexModuleService.GetSWfsIndexModuleById(Convert.ToInt32(pic.PagePositionNo));
                    List<SWfsOperationPicture> pics = _newsChannelsService.GetChannelSWfsOperationPicture(module.ModuleId, module.ADShowType, 100);
                    if (pic.DateBegin > DateTime.Now || pics.Count(a => a.PictureManageId == pic.PictureManageId) == 0) { }
                    else
                    {
                        if (pics.Count(a => a.PictureIndex == pic.PictureIndex) <= 1)
                        {
                            return Json(new { status = -1, message = "删除无效,当前楼层此位置,至少保留一张有效图片" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    sWfsIndexModuleService.DeleteSWfsOperationPictureByPicId(id);
                }
                else
                    return Json(new { status = -1 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = -1 }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region 楼层排序
        /// <summary>
        /// 楼层排序
        /// </summary>
        /// <returns></returns>
        public ActionResult FloorSort(string type = "")
        {
            type = String.IsNullOrEmpty(type) ? NewsChannelsSservice.WomenShoes : type;
            ViewData["floors"] = _newsChannelsService.GetChanelFloors(type).Where(a => a.ADShowType == 0 || a.ADShowType == 1).ToList();
            return View();
        }
        /// <summary>
        /// 批量保存楼层排序
        /// </summary>
        /// <param name="F"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveFloorSort(FormCollection F)
        {
            List<SWfsIndexModule> modules = new List<SWfsIndexModule>();
            foreach (string key in F.AllKeys)
            {
                if (key.EndsWith("floor"))
                {
                    string moduleid = F[key].Split('_')[0];
                    string sort = F[key].Split('_')[1];
                    string Stutas = F[key].Split('_')[2];
                    modules.Add(new SWfsIndexModule
                    {
                        Sort = Convert.ToInt32(sort),
                        ModuleId = Convert.ToInt32(moduleid),
                        Stutas = Convert.ToInt16(Stutas)
                    });
                }
            }
            if (_newsChannelsService.UpdateSWfsIndexModuleSort(modules) > 0)
            {

                return Json(new { status = 1 });
            }
            return Json(new { status = 0 });
        }

        #endregion
        #endregion

        #region 最新上架
        #region 上新商品管理
        /// <summary>
        /// 获取商品列表信息
        /// </summary>
        /// <param name="titleName">标题</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页显示数量</param>
        /// <returns></returns> 
        public ActionResult ShoesNewProductList(string titleName = "", string startTime = "", string endTime = "", string TagFloor = "", string moduleId = "", string PageNo = "CHANNEL_NV003", int pageIndex = 1, int pageSize = 20)
        {
            NewsChannelsSservice service = new NewsChannelsSservice();
            int total = 0;
            string endTimeTemp = "";
            if (!string.IsNullOrEmpty(endTime))
            {
                endTimeTemp = DateTime.Parse(endTime).AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss");
            }
            //频道编号
            string pageNo = string.IsNullOrEmpty(Request.QueryString["PageNo"]) ? "CHANNEL_NV003" : Request.QueryString["PageNo"];
            //楼层id
            string pagePositionNo = string.IsNullOrEmpty(Request.QueryString["moduleId"]) ? "" : Request.QueryString["moduleId"];
            if (string.IsNullOrEmpty(Request.QueryString["TagFloor"]))//表示不为楼层点击
            {
                pagePositionNo = "CHANNEL_NA003";
            }
            Dictionary<string, List<ProductInfoShoes>> dicList = service.GetNewAlterPictureList(titleName.Trim(), pageNo, pagePositionNo, startTime, endTimeTemp, pageIndex, pageSize, out total);
            if (dicList != null && dicList.Count() > 0)
            {
                ViewBag.List = dicList;
            }
            else
            {
                ViewBag.List = new Dictionary<string, List<ProductInfoShoes>>();
            }
            ViewBag.TitleName = titleName ?? "";
            ViewBag.StartTime = startTime ?? "";
            ViewBag.EndTime = endTime ?? "";
            ViewBag.TotalCount = total;
            ViewBag.PageIndex = pageIndex;
            ViewBag.PageSize = pageSize;
            return View();
        }

        /// <summary>
        /// 删除上新信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult NewArrivalDelete(string id, string TagFloor, string ModuleId, string PageNo)
        {
            NewsChannelsSservice service = new NewsChannelsSservice();
            try
            {
                service.Del(id);//删除上新
                service.DelNewProductsByNewArrivalId(id);//删除上新下的商品
                Response.Write("<script>alert('删除成功')</script>");
                return Redirect("/shangpin/newchannels/ShoesNewProductList?TagFloor=" + TagFloor + "&ModuleId=" + ModuleId + "&PageNo=" + PageNo);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }
        #region 添加上新
        /// <summary>
        /// 上新商品添加
        /// </summary>
        /// <returns></returns>
        public ActionResult AddShoesNew()
        {
            string titlename = "";
            string xingqi = "";
            string detailtime = "";

            string path = AppDomain.CurrentDomain.BaseDirectory;
            path = string.Format("{0}ConfigFileCollection/App/ShoesNewProductDate.xml", CommonHelper.GetParentPath(path, 2));
            //以下是循环读取xml文件中节点的值  
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);
            XmlNodeList NodeList = xmlDoc.SelectNodes("/shoesnewproductdate/date"); //xml节点的路径  
            //循环遍历节点  
            for (int i = 0; i < NodeList.Count; i++)
            {
                // titlename = NodeList[i].ChildNodes[0].InnerText;    //获取第一个Student节点的StuName  
                xingqi = NodeList[i].ChildNodes[1].InnerText;     //获取第一个Student节点的StuSex  
                detailtime = NodeList[i].ChildNodes[2].InnerText;     //获取第一个Student节点的StuAge  
                //循环读取xml节点信息  
            }
            if (!string.IsNullOrEmpty(Request.QueryString["title"]))
            {
                titlename = Request.QueryString["title"];
            }
            if (!string.IsNullOrEmpty(Request.QueryString["stime"]))
            {
                detailtime = Request.QueryString["stime"];
            }
            string tagFloor = Request.QueryString["TagFloor"] ?? "";//标识是否为楼层
            //楼层编号
            string pagePositionNo = Request.QueryString["ModuleId"].ToString() ?? "CHANNEL_NA003";
            //楼层名称
            string pagePositionName = !string.IsNullOrEmpty(tagFloor) ? (Request.QueryString["ModuleName"].ToString() ?? "") : titlename;
            //频道编号
            string pageNo = Request.QueryString["PageNo"] ?? "";
            ViewBag.NewTitle = titlename;
            ViewBag.CheckedAll = xingqi;
            ViewBag.DetailTime = detailtime;
            ViewBag.ModuleId = pagePositionNo;
            ViewBag.ModuleName = pagePositionName;
            ViewBag.PageNo = pageNo;
            ViewBag.TagFloor = tagFloor;
            return View();
        }

        /// <summary>
        /// ajax调用的添加上新方法
        /// </summary>
        /// <returns></returns>
        public ActionResult AddShoesNewManage()
        {
            //上新标题
            string title = Request.Form["Title"].ToString();
            //上新的商品
            string goods = Request.Form["NewProducts"].ToString();
            //上新的日期
            string date = Request.Form["CreateTime"].ToString();
            int sort = 0;//排序值
            string tagFloor = Request.Form["TagFloor"] ?? "";//标识是否为楼层
            //楼层编号
            string pagePositionNo = string.IsNullOrEmpty(Request.Form["ModuleId"].ToString()) ? "CHANNEL_NA003" : (Request.Form["ModuleId"].ToString() ?? "");
            //楼层名称
            string pagePositionName = !string.IsNullOrEmpty(tagFloor) ? (Request.Form["ModuleName"].ToString() ?? "") : title;
            //频道编号
            string pageNo = Request.QueryString["PageNo"] ?? "CHANNEL_NV003";

            NewsChannelsSservice service = new NewsChannelsSservice();
            sort = service.GetNewArrivalMaxSort(pageNo, pagePositionNo) + 1;
            //上新实体
            SWfsIndexNewArrival sWfsIndexNewArrival = new SWfsIndexNewArrival();
            sWfsIndexNewArrival.NewArrivalTitle = title;
            sWfsIndexNewArrival.StartDate = DateTime.Parse(date);
            sWfsIndexNewArrival.CreateDate = DateTime.Now;
            sWfsIndexNewArrival.OperateUserId = PresentationHelper.GetPassport().UserName;//获取当前用户
            sWfsIndexNewArrival.Status = 1;
            sWfsIndexNewArrival.DataState = 1;
            sWfsIndexNewArrival.SortValue = sort;
            sWfsIndexNewArrival.WebSiteNo = "shangpin.com";
            sWfsIndexNewArrival.PagePositionNo = pagePositionNo;
            sWfsIndexNewArrival.PagePositionName = pagePositionName;
            sWfsIndexNewArrival.PageNo = pageNo;//女鞋：CHANNEL_NV003

            //查询是否有重复添加相同的时间
            int Start = service.SelectNewArrivalDate(sWfsIndexNewArrival.StartDate, 0, pageNo, pagePositionNo);
            if (Start > 0 && string.IsNullOrEmpty(tagFloor))//只有上新才有此限制，楼层没有此限制
            {
                return Content("-1");
            }
            else
            {
                //执行添加上新返回主键编号以便于添加该上新下边的商品
                int Pkid = service.InsertNewArrivalInfo(sWfsIndexNewArrival);
                if (Pkid != 0)
                {
                    try
                    {
                        //向该上新下添加新的商品
                        service.AddNewProduct(Pkid.ToString(), goods, PresentationHelper.GetPassport().UserName, "0");
                        return Content("添加成功");
                    }
                    catch (Exception ex)
                    {
                        return Content(ex.Message);
                    }
                }
                return Content("添加成功");
            }
        }
        #endregion
        #region 编辑上新
        public ActionResult EditShoesNew()
        {
            //上新编号
            int pkid = int.Parse(Request.QueryString["id"].ToString());
            NewsChannelsSservice service = new NewsChannelsSservice();
            //获取上新信息根据编号
            SWfsIndexNewArrival model = service.SelectNewArrivalByNid(pkid);
            var img = service.GetShoesNewProudecList(pkid.ToString());

            string titlename = "";
            string xingqi = "";
            string detailtime = "";

            string path = AppDomain.CurrentDomain.BaseDirectory;
            path = string.Format("{0}ConfigFileCollection/App/ShoesNewProductDate.xml", CommonHelper.GetParentPath(path, 2));
            //以下是循环读取xml文件中节点的值  
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);
            XmlNodeList NodeList = xmlDoc.SelectNodes("/shoesnewproductdate/date"); //xml节点的路径  
            //循环遍历节点  
            for (int i = 0; i < NodeList.Count; i++)
            {
                // titlename = NodeList[i].ChildNodes[0].InnerText;    //获取第一个Student节点的StuName  
                xingqi = NodeList[i].ChildNodes[1].InnerText;     //获取第一个Student节点的StuSex  
                detailtime = NodeList[i].ChildNodes[2].InnerText;     //获取第一个Student节点的StuAge  
                //循环读取xml节点信息  
            }

            ViewBag.CommingTitle = titlename;
            ViewBag.CheckedAll = xingqi;
            ViewBag.DetailTime = detailtime;

            return View(model);
        }
        /// <summary>
        /// ajax调用编辑上新方法
        /// </summary>
        /// <returns></returns>
        public ActionResult EditShoesNewById()
        {
            //上新标题
            string title = Request.Form["Title"].ToString();
            //上新的编号
            string id = Request.Form["NewArrivalId"].ToString();
            //上新的日期
            string date = Request.Form["CreateTime"].ToString();
            //楼层
            string tagfloor = Request.Form["TagFloor"].ToString() ?? "";
            string pageno = Request.Form["PageNo"].ToString();
            string pagePositionNo = Request.Form["ModuleId"];
            NewsChannelsSservice service = new NewsChannelsSservice();
            if (string.IsNullOrEmpty(tagfloor) && pagePositionNo == "CHANNEL_NA003")
            {
                //查询是否有重复添加相同的时间
                int Start = service.SelectNewArrivalDate(DateTime.Parse(date), int.Parse(id), pageno, pagePositionNo);
                if (Start > 0 && string.IsNullOrEmpty(tagfloor))//只有上新才有此限制，楼层没有此限制
                {
                    return Content("-1");
                }
            }

            //上新实体
            SWfsIndexNewArrival sWfsIndexNewArrival = service.SelectNewArrivalByNid(int.Parse(id));
            sWfsIndexNewArrival.NewArrivalTitle = title;
            sWfsIndexNewArrival.StartDate = DateTime.Parse(date);
            sWfsIndexNewArrival.UpdateOperateUserId = PresentationHelper.GetPassport().UserName;//获取当前用户
            sWfsIndexNewArrival.UpdateDate = DateTime.Now;

            try
            {
                service.UpdateNewArrivalInfo(sWfsIndexNewArrival);
                return Content("修改成功");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }
        #endregion
        #endregion
        #region 上新商品下的商品管理列表
        public ActionResult NewProductManage()
        {
            NewsChannelsSservice service = new NewsChannelsSservice();
            string id = Request.QueryString["id"];
            List<ProductInfoShoes> list = service.GetShoesNewProudecList(id);
            foreach (var item in list)
            {
                ProductInventoryShoesNew inventory = service.GetInventoryByProductNo(item.ProductNo);
                item.Quantity = inventory.SumQuantity;
                item.LockQuantity = inventory.SumLockQuantity;
            }
            return View(list);
        }


        #region 某一上新商品下添加商品操作
        public ActionResult AddShoesNewProductList(string NewArrivalId, int pageIndex = 1, int pageSize = 20)
        {
            NewsChannelsSservice service = new NewsChannelsSservice();
            int total = 0;
            ViewBag.TagFloor = Request.QueryString["TagFloor"] ?? "";
            ViewBag.ModuleId = Request.QueryString["ModuleId"] ?? "";
            ViewBag.ModuleName = Request.QueryString["ModuleName"] ?? "";
            ViewBag.PageNo = Request.QueryString["PageNo"] ?? "";
            if (Request.QueryString["keyWord"] != null)
            {
                ViewBag.keyWord = Request.QueryString["keyWord"];
            }
            if (Request.QueryString["CategoryNo"] != null)
            {
                ViewBag.category = Request.QueryString["CategoryNo"];
            }
            if (Request.QueryString["Gender"] != null)
            {
                ViewBag.Gender = Request.QueryString["Gender"];
            }
            if (Request.QueryString["brandNO"] != null)
            {
                ViewBag.BrandNO = Request.QueryString["brandNO"];
            }
            if (Request.QueryString["starttime"] != null && Request.QueryString["starttime"] != "")
            {
                ViewBag.StartDateShelf = Request.QueryString["starttime"];
            }
            if (Request.QueryString["endtime"] != null && Request.QueryString["endtime"] != "")
            {
                ViewBag.EndDateShelf = Request.QueryString["endtime"];
            }
            List<ProductInfoShoes> list = null;
            if (string.IsNullOrEmpty(ViewBag.keyWord) && string.IsNullOrEmpty(ViewBag.category) && string.IsNullOrEmpty(ViewBag.Gender) && string.IsNullOrEmpty(ViewBag.BrandNO)
                && string.IsNullOrEmpty(ViewBag.StartDateShelf) && string.IsNullOrEmpty(ViewBag.EndDateShelf))
            {
                list = new List<ProductInfoShoes>();
            }
            else
            {
                list = service.GetSWfsProductList(ViewBag.Gender, ViewBag.BrandNO, ViewBag.category, ViewBag.keyWord, ViewBag.StartDateShelf, ViewBag.EndDateShelf, pageIndex, pageSize, out total);
                if (list != null && list.Count() > 0)
                {
                    foreach (var item in list)
                    {
                        //查询该商品下的sku信息
                        List<ProductInfoShoes> skulist = service.GetSkuPriceListByProductNo(item.ProductNo);
                        foreach (var sku in skulist)
                        {
                            item.IsShelf = sku.IsShelf;
                            item.DateShelf = sku.DateShelf;
                        }
                    }
                    if (!string.IsNullOrEmpty(ViewBag.StartDateShelf))
                    {
                        DateTime starttime = Convert.ToDateTime(ViewBag.StartDateShelf);
                        list = list.Where(a => a.DateShelf >= starttime).ToList();
                    }
                    if (!string.IsNullOrEmpty(ViewBag.EndDateShelf))
                    {
                        DateTime endtime = Convert.ToDateTime(ViewBag.EndDateShelf);
                        list = list.Where(a => a.DateShelf <= endtime).ToList();
                    }
                }
            }
            //查询当前频道下已经有的上新商品的总数
            List<ProductInfoShoes> newProList = service.GetShoesNewProudecList(NewArrivalId);
            if (newProList == null)
            {
                newProList = new List<ProductInfoShoes>();
            }
            ViewBag.NrePcount = newProList.Count;
            ViewBag.page = pageIndex;
            ViewBag.pageSize = pageSize;
            ViewBag.totalCount = total;
            return View(list);
        }
        /// <summary>
        /// ajax调用的添加上新商品的方法
        /// </summary>
        /// <param name="newArrivalId"></param>
        /// <param name="ProductNoStr"></param>
        /// <returns></returns>
        public ActionResult AddNewProduct(string newArrivalId, string ProductNoStr, string TagFloor, string IsBatch)
        {
            NewsChannelsSservice service = new NewsChannelsSservice();
            ProductNoStr = ProductNoStr.Trim().TrimEnd(',');
            Passport passport = PresentationHelper.GetPassport();
            if (IsBatch == "1")//表示輸入編號批量添加
            {
                string[] pnolist = ProductNoStr.Split(',');
                foreach (string pno in pnolist)
                {
                    Product proInfo = service.GetProductByProductNo(pno);
                    if (null == proInfo)   //判断输入的商品编号是否存在
                    {
                        return Json(new { result = "error", message = "对不起，不存在产品编号:" + pno + ",请重新输入。" });
                    }
                }
            }
            try
            {
                if (!string.IsNullOrEmpty(newArrivalId) && newArrivalId != "0")
                {
                    service.AddNewProduct(newArrivalId, ProductNoStr, passport.UserName, "1");
                    return Json(new { result = "success", message = "添加成功" });
                }
                else
                {
                    ProductNoStr = ProductNoStr + ",";
                    return Json(new { result = "backadd", message = ProductNoStr, floor = TagFloor });
                }
            }
            catch (Exception ex)
            {
                return Json(new { result = "error", message = "添加失败" });
            }
        }
        #endregion
        #region 修改商品排序值
        public JsonResult UpdateSortValue()
        {
            try
            {
                NewsChannelsSservice sercive = new NewsChannelsSservice();
                if (string.IsNullOrEmpty(Request.Form["idList"]))
                {
                    return Json(new { status = 1, message = "商品编号不存在" }, JsonRequestBehavior.AllowGet);
                }
                string[] idList = Request.Form["idList"].TrimEnd(',').Split(',');
                string[] sortList = Request.Form["sortList"].TrimEnd(',').Split(',');
                for (int i = 0; i < idList.Length; i++)
                {
                    sercive.EditeBrandSortValue(Convert.ToInt32(idList[i].ToString()), idList.Length - i);
                }
                return Json(new { status = 0, message = "修改成功" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(new { status = 1, message = "修改失败" + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #region 删除商品操作
        public ActionResult DelShoesNewProductList(string idStr)
        {
            idStr = idStr.Trim().TrimEnd(',');
            NewsChannelsSservice service = new NewsChannelsSservice();
            try
            {
                service.DelShoesNewProduct(idStr);
                return Json(new { result = "success", message = "删除成功。" });
            }
            catch (Exception ex)
            {
                return Json(new { result = "error", message = ex.Message });
            }
        }
        #endregion

        #endregion
        #region 设置上新时间
        /// <summary>
        /// 设置上新时间
        /// </summary>
        /// <returns></returns>
        public ActionResult AddShoesNewDate()
        {
            string titlename = "";
            string xingqi = "";
            string detailtime = "";

            string path = AppDomain.CurrentDomain.BaseDirectory;
            path = string.Format("{0}ConfigFileCollection/App/ShoesNewProductDate.xml", CommonHelper.GetParentPath(path, 2));
            //以下是循环读取xml文件中节点的值  
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);
            XmlNodeList NodeList = xmlDoc.SelectNodes("/shoesnewproductdate/date"); //xml节点的路径  
            //循环遍历节点  
            for (int i = 0; i < NodeList.Count; i++)
            {
                titlename = NodeList[i].ChildNodes[0].InnerText;    //获取第一个Student节点的StuName  
                xingqi = NodeList[i].ChildNodes[1].InnerText;     //获取第一个Student节点的StuSex  
                detailtime = NodeList[i].ChildNodes[2].InnerText;     //获取第一个Student节点的StuAge  
                //循环读取xml节点信息  
            }

            ViewBag.CommingTitle = titlename;
            ViewBag.CheckedAll = xingqi;
            ViewBag.DetailTime = detailtime;
            return View();
        }

        [HttpPost]
        /// <summary>
        /// 保存上新时间
        /// </summary>
        /// <returns></returns>
        public ActionResult SetShoesNewDate()
        {
            NewsChannelsSservice service = new NewsChannelsSservice();
            //上新标题
            string title = "";//根据选择的时间自动获取
            //上新的商品
            string xinqi = Request.Form["WeekDay"].ToString();
            //上新的日期
            string detailtime = Request.Form["CreateTime"].ToString();
            //根据时间自动获取标题
            string[] weeks = Request.Form["CheckedXq"].TrimEnd(',').Split(',');
            if (weeks.Length > 6)
            {
                title = "每天" + detailtime.Split(':')[0] + "点上新";
            }
            else
            {
                title = "每周";
                foreach (string xq in weeks)
                {
                    switch (xq)
                    {
                        case "1":
                            title += "一、";
                            break;
                        case "2":
                            title += "二、";
                            break;
                        case "3":
                            title += "三、";
                            break;
                        case "4":
                            title += "四、";
                            break;
                        case "5":
                            title += "五、";
                            break;
                        case "6":
                            title += "六、";
                            break;
                        case "0":
                            title += "日、";
                            break;
                        default:
                            break;
                    }
                }
                title = title.TrimEnd('、') + detailtime.Split(':')[0] + "点上新";
            }

            string path = AppDomain.CurrentDomain.BaseDirectory;
            path = string.Format("{0}ConfigFileCollection/App/ShoesNewProductDate.xml", CommonHelper.GetParentPath(path, 2));

            XmlDocument xmlDocx = new XmlDocument();
            //xmlDocx.Load(Server.MapPath("~/ConfigFileCollection/App/ShoesNewProductDate.xml").Replace("Shangpin.Ocs.Web", ""));//加载xml文件，文件
            xmlDocx.Load(path);
            XmlNode xns = xmlDocx.SelectSingleNode("shoesnewproductdate");//查找要修改的节点
            XmlNodeList xnl = xns.ChildNodes;//取出所有的子节点
            foreach (XmlNode xn in xnl)
            {
                XmlElement xe = (XmlElement)xn;//将节点转换一下类型
                if (xe.GetAttribute("genre") == "ShoesNewDate")//判断该子节点是否是要查找的节点
                {
                    XmlNodeList xnl2 = xe.ChildNodes;//取出该子节点下面的所有元素
                    foreach (XmlNode xn2 in xnl2)
                    {
                        XmlElement xe2 = (XmlElement)xn2;//转换类型
                        if (xe2.Name == "title")//判断是否是要查找的元素
                        {
                            xe2.InnerText = title;
                        }
                        if (xe2.Name == "xingqi")//判断是否是要查找的元素
                        {
                            xe2.InnerText = xinqi;
                        }
                        if (xe2.Name == "time")//判断是否是要查找的元素
                        {
                            xe2.InnerText = detailtime;
                        }
                    }
                }
            }
            xmlDocx.Save(path);
            SWfsGlobalConfig sgc = service.GetSWfsGlobalConfigByFunctionNo("ShoesNewArrival");
            if (null == sgc)//若为空则添加
            {
                sgc = new SWfsGlobalConfig();
                sgc.FunctionNo = "ShoesNewArrival";
                sgc.ConfigIntro = "女鞋频道最新上架标题";
                sgc.ConfigtName = "ShoesNewArrivalDateConfig";
                sgc.ConfigValue = title;
                sgc.CreateDate = DateTime.Now;
                try
                {
                    service.Insert(sgc);
                    return Content("保存成功！");
                }
                catch (Exception ex)
                {
                    return Content(ex.Message);
                }
            }
            else//否则修改
            {
                int num = service.UpdateShoesNewDateByFunctionNo(title);
                if (num > 0)
                {
                    return Content("保存成功！");
                }
                else
                {
                    return Content("保存失败！");
                }
            }
        }
        #endregion
        #region 修改上新名称
        public ActionResult ChangeNewArrivalName(string PageNo, string PagePositionNo, string ArrivalTitle)
        {
            try
            {
                NewsChannelsSservice service = new NewsChannelsSservice();
                SWfsIndexNewArrival model = service.SelectNewArrivalInfo(PageNo, PagePositionNo);
                SWfsIndexNewArrival newmodel = new SWfsIndexNewArrival();
                if (model != null)
                {
                    model.NewArrivalTitle = ArrivalTitle;
                    model.UpdateDate = DateTime.Now;
                    model.UpdateOperateUserId = PresentationHelper.GetPassport().UserName;
                    service.UpdateNewArrivalInfo(model);
                    return Json(new { status = 1, message = "修改成功。" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { status = 0, message = "修改失败。" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = 0, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #endregion

        #region 最新上架预告
        public ActionResult ShoesNewArrivalNoticeList(string PageNo, string KeyWord, string BeginTime, string EndTime)
        {
            NewsChannelsSservice NewServiceNotice = new NewsChannelsSservice();
            BeginTime = "1900-01-01 00:00:00";
            EndTime = "2900-01-01 00:00:00";
            KeyWord = KeyWord == null ? "" : KeyWord;
            List<SWfsNewArrivalNotice> NNoList = NewServiceNotice.GetNewArrivalNoticeList(PageNo, KeyWord, BeginTime, EndTime);
            return View(NNoList);
        }

        public JsonResult AjaxShoesNewArrivalNoticeList(string PageNo, string KeyWord, string BeginTime, string EndTime)
        {
            NewsChannelsSservice NewServiceNotice = new NewsChannelsSservice();

            BeginTime = BeginTime == "" ? "1900-01-01 00:00:00" : BeginTime.Replace("/", "-") + " 00:00:00";
            EndTime = EndTime == "" ? "2900-01-01 00:00:00" : EndTime.Replace("/", "-") + " 23:59:59";

            KeyWord = KeyWord == null ? "" : KeyWord;
            List<SWfsNewArrivalNotice> NNoList = NewServiceNotice.GetNewArrivalNoticeList(PageNo, KeyWord, BeginTime, EndTime);
            JsonResult res = new JsonResult();

            var NewNoticeList =
               NNoList.Select
               (
               p => new { p.NewArrivalNoticName, NewArrivalNoticDate = p.NewArrivalNoticDate.ToString("yyyy-MM-dd"), NewArrivalNoticeId = p.NewArrivalNoticeId, CreateDate = p.CreateDate.ToString("yyyy-MM-dd") }
               ).ToList();

            res.Data = NewNoticeList;
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        /// <summary>
        /// 添加最新上架预告
        /// </summary>
        /// <param name="name"></param>
        /// <param name="NDate"></param>
        /// <returns></returns>
        public string AddShoesNewArrivalNotice(string name, string NDate, string PageNo)
        {
            string result = "";
            SWfsNewArrivalNotice NDTO = new SWfsNewArrivalNotice
            {
                CreateDate = DateTime.Now,
                DataState = 1,
                NewArrivalNoticDate = Convert.ToDateTime(NDate),
                NewArrivalNoticName = name,
                PageNo = PageNo,
                PagePositionNo = "",
                Status = 1,
                UpdateDate = DateTime.Now,
                WebSiteNo = "shangpin.com",
                OperateUserId = PresentationHelper.GetPassport().UserName,
                UpdateOperateUserId = PresentationHelper.GetPassport().UserName
            };
            NewsChannelsSservice NewServiceNotice = new NewsChannelsSservice();
            result = NewServiceNotice.AddNewArrivalNotice(NDTO) + "";
            return result;
        }

        /// <summary>
        ///  修改最新上架预告
        /// </summary>
        /// <param name="name"></param>
        /// <param name="NDate"></param>
        /// <param name="nid"></param>
        /// <returns></returns>
        public string EidtShoesNewArrivalNotice(string name, string NDate, string nid)
        {
            string result = "";
            NewsChannelsSservice NewServiceNotice = new NewsChannelsSservice();
            result = NewServiceNotice.ModifyNewArrivalNoticeById(nid, name, NDate, PresentationHelper.GetPassport().UserName) + "";
            return result;
        }

        /// <summary>
        /// 删除最新上架预告
        /// </summary>
        /// <param name="nid"></param>
        /// <returns></returns>
        public string delShoesNewArrivalNotice(string nid)
        {
            string result = "";
            NewsChannelsSservice NewServiceNotice = new NewsChannelsSservice();
            result = NewServiceNotice.DeLNewArrivalNoticeById(nid, PresentationHelper.GetPassport().UserName) + "";
            return result;
        }

        /// <summary>
        ///检查日期是否重复
        /// </summary>
        /// <param name="PageNo"></param>
        /// <param name="DateValue"></param>
        /// <returns></returns>
        public string IsExitNewArrivalNotice(string id, string PageNo, string DateValue)
        {
            string result = "";
            NewsChannelsSservice NewServiceNotice = new NewsChannelsSservice();
            result = NewServiceNotice.IsExitNewArrivalNotice(id, PageNo, DateValue);
            return result;
        }

        /// <summary>
        /// 获取上新预告是否关闭
        /// </summary>
        /// <param name="pageNo"></param>
        /// <returns></returns>
        public string GetStatusNewsNotice(string pageNo)
        {
            string result = "";
            NewsChannelsSservice NewServiceNotice = new NewsChannelsSservice();
            result = NewServiceNotice.GetStatusNewsNotice(pageNo);
            return result;
        }
        /// <summary>
        /// 关闭或者开启上新预告
        /// </summary>
        /// <param name="pageNo"></param>
        /// <param name="DateSate"></param>
        /// <returns></returns>
        public string ChannelGetStatusNewsNotice(string pageNo, string DateSate)
        {
            string result = "";
            NewsChannelsSservice NewServiceNotice = new NewsChannelsSservice();
            DateSate = DateSate == "0" ? "1" : "0";
            result = NewServiceNotice.ChannelNewsNotice(pageNo, DateSate);
            return result;
        }
        #endregion

    }
}
