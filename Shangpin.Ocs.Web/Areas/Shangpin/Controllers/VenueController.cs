using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Shangpin.Entity.Wfs;
using Shangpin.Ocs.Service;
using Shangpin.Ocs.Service.Shangpin;
using Shangpin.Ocs.Service.Common;
using Shangpin.Ocs.Entity.Extenstion.ShangPin;
using System.Web.Script.Serialization;
using Shangpin.Entity.Common;
using Shangpin.Ocs.Entity.Extenstion.Outlet;
using Shangpin.Ocs.Service.Outlet;
using System.Collections;
using Shangpin.Framework.WebMvc;
using Shangpin.Framework.Common.Cache;
using Shangpin.Ocs.Entity.Extenstion.Login;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Text;


namespace Shangpin.Ocs.Web.Areas.Shangpin.Controllers
{
    [OCSAuthorization]
    public class VenueController : BaseController
    {
        //
        // GET: /Shangpin/Meeting/
        private static string loginCookieName = AppSettingManager.AppSettings["LoginCookieName"].ToString();
        private readonly SwfsVenueService venueService;
        private Dictionary<string, string> rsPic = new Dictionary<string, string>();
        public VenueController()
        {
            venueService = new SwfsVenueService();
            ReadConfig();

        }
        string key = string.Empty;
        string Shangpinkey = string.Empty;
        /// <summary>
        /// 读取配置文件
        /// </summary>
        void ReadConfig()
        {

            string configAolaiKey = AppSettingManager.AppSettings["AolaiDomain"];
            if (configAolaiKey != "")
                key = configAolaiKey;
            else
                return;
            string configShangpinKey = AppSettingManager.AppSettings["ShangpinDomain"];
            if (configShangpinKey != "")
                Shangpinkey = configShangpinKey;
            else
                return;
        }

        #region 模板管理
        //模板列表
        public ActionResult TemplateVersionList()
        {
            int totalCount = 0;
            IEnumerable<SWfsMeetingTemplateInfo> list = venueService.GetTemplateList
                (Request.QueryString["pageIndex"], 20, Request.QueryString["tempNO"], Request.QueryString["tempName"],
                Request.QueryString["isPreView"], Request.QueryString["isMobile"], out totalCount);
            ViewBag.totalCount = totalCount;
            return View(list);
        }
        //模板添加/修改
        public ActionResult TemplateEdite()
        {
            SWfsMeetingTemplateInfo obj = venueService.GetTemplateObjByID(Request.QueryString["tempID"]);
            if (obj == null)
            {
                return View(new SWfsMeetingTemplateInfo() { MettingNo = DateTime.Now.ToString("yyyyMMddHHmmss") });
            }
            return View(obj);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult TemplateEdite(SWfsMeetingTemplateInfo obj)
        {
            #region 读取隐藏域逻辑处理
            string tempImgDetail = Request.Form["ImgAllDetail"];  //隐藏值
            //当前方法的全局图片集合
            IList<SWfsMeetingTemplatePicInfo> _singleEntityList = new List<SWfsMeetingTemplatePicInfo>();
            if (!string.IsNullOrEmpty(tempImgDetail))
            {
                string[] tempAllDataDetail = tempImgDetail.Split(new char[] { '$' });
                if (tempAllDataDetail.Length > 1)       // 说明当前上传了多组图片信息
                {
                    for (int i = 0; i < tempAllDataDetail.Length; i++)
                    {
                        //分组图片信息组合值
                        string[] tempAllDetail = tempAllDataDetail[i].Split(new char[] { '#' });
                        if (tempAllDetail.Length == 3)   //判断当前组合值，如果分割是三个的话，说明正常上传
                        {
                            SWfsMeetingTemplatePicInfo picEntity = new SWfsMeetingTemplatePicInfo();
                            picEntity.PicNo = string.IsNullOrEmpty(tempAllDetail[0]) ? "" : tempAllDetail[0];
                            picEntity.FileName = string.IsNullOrEmpty(tempAllDetail[1]) ? "" : tempAllDetail[1];

                            //判断当前最后一个值不是数字的话，说明有误
                            short tempType = 0;
                            if (short.TryParse(tempAllDetail[2], out tempType))
                            {
                                picEntity.Type = tempType;
                                picEntity.TemplateID = obj.MettingNo;
                                IList<SWfsMeetingTemplatePicInfo> isExistData = _singleEntityList.Where(c => c.FileName == picEntity.FileName && c.Type == picEntity.Type).ToList();
                                if (isExistData == null || isExistData.Count() == 0)
                                {
                                    _singleEntityList.Add(picEntity);
                                }
                            }
                        }
                    }
                }
                else      // 说明只有一组图片信息
                {
                    //分组图片信息组合值
                    string[] tempAllDetail = tempImgDetail.Split(new char[] { '#' });
                    if (tempAllDetail.Length == 3)   //判断当前组合值，如果分割是三个的话，说明正常上传
                    {
                        SWfsMeetingTemplatePicInfo picEntity = new SWfsMeetingTemplatePicInfo();
                        picEntity.PicNo = string.IsNullOrEmpty(tempAllDetail[0]) ? "" : tempAllDetail[0];
                        picEntity.FileName = string.IsNullOrEmpty(tempAllDetail[1]) ? "" : tempAllDetail[1];

                        //判断当前最后一个值不是数字的话，说明有误
                        short tempType = 0;
                        if (short.TryParse(tempAllDetail[2], out tempType))
                        {
                            picEntity.Type = tempType;
                            picEntity.TemplateID = obj.MettingNo;
                            IList<SWfsMeetingTemplatePicInfo> isExistData = _singleEntityList.Where(c => c.FileName == picEntity.FileName && c.Type == picEntity.Type).ToList();
                            if (isExistData == null || isExistData.Count() == 0)
                            {
                                _singleEntityList.Add(picEntity);
                            }
                        }
                    }
                }
            }
            #endregion

            #region 添加或修改模版图片关系
            IList<SWfsMeetingTemplatePicInfo> UpdateEntity = new List<SWfsMeetingTemplatePicInfo>();

            //查出当前模版所有图片信息
            IList<SWfsMeetingTemplatePicInfo> ALLEntityList = venueService.GetTemplateImgEntity(obj.MettingNo, -2);

            //需要更新的实体集合
            UpdateEntity = (from s in _singleEntityList
                            join a in ALLEntityList on s.Type equals a.Type
                            where s.FileName == a.FileName
                            select a).ToList();

            //循环更新
            foreach (SWfsMeetingTemplatePicInfo updatemodel in UpdateEntity)
            {
                SWfsMeetingTemplatePicInfo Opentity = _singleEntityList.FirstOrDefault(c => c.FileName == updatemodel.FileName);
                updatemodel.PicNo = Opentity.PicNo;
                if (!venueService.UpdateTemplateImg(updatemodel))
                {
                    ViewData["tip"] = new HtmlString("<script>alert('图片信息更新有误！');</script>");
                    return View(obj);
                }
                _singleEntityList.Remove(Opentity);  // 当前上传图片中已经更新的重复的实体将它移除
            }
            //循环添加
            foreach (SWfsMeetingTemplatePicInfo addmodel in _singleEntityList)
            {
                if (!venueService.AddTemplateImg(addmodel))
                {
                    ViewData["tip"] = new HtmlString("<script>alert('图片信息添加有误！');</script>");
                    return View(obj);
                }
            }
            #endregion

            #region 模版图片信息操作（性能问题，暂时舍弃）
            /*
            string tempImgDetail = Request.Form["ImgAllDetail"];  //隐藏值

            //当前方法的全局集合
            IList<SWfsMeetingTemplatePicInfo> singleEntityList = new List<SWfsMeetingTemplatePicInfo>();

            string[] tempAllDataDetail = tempImgDetail.Split(new char[] { '$' });
            if (tempAllDataDetail.Length > 1)       // 说明当前上传了多组图片信息
            {
                for (int i = 0; i < tempAllDataDetail.Length; i++)
                {
                    //分组图片信息组合值
                    string[] tempAllDetail = tempAllDataDetail[i].Split(new char[] { '#' });
                    if (tempAllDetail.Length == 3)   //判断当前组合值，如果分割是三个的话，说明正常上传
                    {
                        SWfsMeetingTemplatePicInfo picEntity = new SWfsMeetingTemplatePicInfo();
                        picEntity.PicNo = string.IsNullOrEmpty(tempAllDetail[0]) ? "" : tempAllDetail[0];
                        picEntity.FileName = string.IsNullOrEmpty(tempAllDetail[1]) ? "" : tempAllDetail[1];

                        //判断当前最后一个值不是数字的话，说明有误
                        short tempType = 0;
                        if (short.TryParse(tempAllDetail[2], out tempType))
                        {
                            picEntity.Type = tempType;
                            picEntity.TemplateID = obj.MettingNo;

                            //判断当前集合中是否已经存在值，担心多组图片信息
                            if (singleEntityList != null && singleEntityList.Count() > 0)
                            {
                                IList<SWfsMeetingTemplatePicInfo> tempSingleEntityList = new List<SWfsMeetingTemplatePicInfo>();
                                tempSingleEntityList = singleEntityList;   //加入到新的集合中

                                // 判断当前图片过度库中有无此模版图片信息，如果有更新图片信息
                                singleEntityList = venueService.GetTemplateImgEntity(picEntity.TemplateID, picEntity.Type, picEntity.FileName);
                                if (singleEntityList != null && singleEntityList.Count() > 0)
                                {
                                    singleEntityList[0].PicNo = picEntity.PicNo;
                                    if (!venueService.UpdateTemplateImg(singleEntityList[0]))
                                    {
                                        ViewData["tip"] = new HtmlString("<script>alert('图片信息修改失败！');</script>");
                                        return View(obj);
                                    }
                                    foreach (SWfsMeetingTemplatePicInfo model in singleEntityList)
                                    {
                                        tempSingleEntityList.Add(model);   // 向新的集合再次添加值
                                    }

                                }
                                else   //如果没有则添加
                                {
                                    if (!venueService.AddTemplateImg(picEntity))
                                    {
                                        ViewData["tip"] = new HtmlString("<script>alert('图片信息保存失败！');</script>");
                                        return View(obj);
                                    }
                                    tempSingleEntityList.Add(picEntity);
                                }
                                singleEntityList = tempSingleEntityList;   // 从新的集合赋值到全局集合中
                            }
                            else
                            {
                                // 判断当前图片过度库中有无此模版图片信息，如果有更新图片信息
                                singleEntityList = venueService.GetTemplateImgEntity(picEntity.TemplateID, picEntity.Type, picEntity.FileName);
                                if (singleEntityList != null && singleEntityList.Count() > 0)
                                {
                                    singleEntityList[0].PicNo = picEntity.PicNo;
                                    if (!venueService.UpdateTemplateImg(singleEntityList[0]))
                                    {
                                        ViewData["tip"] = new HtmlString("<script>alert('图片信息修改失败！');</script>");
                                        return View(obj);
                                    }
                                }
                                else   //如果没有则添加
                                {
                                    if (!venueService.AddTemplateImg(picEntity))
                                    {
                                        ViewData["tip"] = new HtmlString("<script>alert('图片信息保存失败！');</script>");
                                        return View(obj);
                                    }
                                    singleEntityList.Add(picEntity);
                                }
                            }
                        }
                    }
                }
            }
            else      // 说明只有一组图片信息
            {
                //分组图片信息组合值
                string[] tempAllDetail = tempImgDetail.Split(new char[] { '#' });
                if (tempAllDetail.Length == 3)   //判断当前组合值，如果分割是三个的话，说明正常上传
                {
                    SWfsMeetingTemplatePicInfo picEntity = new SWfsMeetingTemplatePicInfo();
                    picEntity.PicNo = string.IsNullOrEmpty(tempAllDetail[0]) ? "" : tempAllDetail[0];
                    picEntity.FileName = string.IsNullOrEmpty(tempAllDetail[1]) ? "" : tempAllDetail[1];

                    //判断当前最后一个值不是数字的话，说明有误
                    short tempType = 0;
                    if (short.TryParse(tempAllDetail[2], out tempType))
                    {
                        picEntity.Type = tempType;
                        picEntity.TemplateID = obj.MettingNo;

                        // 判断当前图片过度库中有误此模版图片信息，如果有更新图片信息
                        singleEntityList = venueService.GetTemplateImgEntity(picEntity.TemplateID, picEntity.Type, picEntity.FileName);
                        if (singleEntityList != null && singleEntityList.Count() > 0)
                        {
                            singleEntityList[0].PicNo = picEntity.PicNo;
                            if (!venueService.UpdateTemplateImg(singleEntityList[0]))
                            {
                                ViewData["tip"] = new HtmlString("<script>alert('图片信息修改失败！');</script>");
                                return View(obj);
                            }
                        }
                        else   //如果没有则添加
                        {
                            if (!venueService.AddTemplateImg(picEntity))
                            {
                                ViewData["tip"] = new HtmlString("<script>alert('图片信息保存失败！');</script>");
                                return View(obj);
                            }
                            singleEntityList.Add(picEntity);
                        }
                    }
                    else
                    {
                        ViewData["tip"] = new HtmlString("<script>alert('js或者css类型获取失败！');</script>");
                        return View(obj);
                    }
                }
                else     //页面约定，当前只有一个类型值，查询
                {
                    short tempType = 0;
                    if (short.TryParse(tempImgDetail, out tempType))
                    {
                        singleEntityList = venueService.GetTemplateImgEntity(obj.MettingNo, tempType);
                    }
                    else
                    {
                        ViewData["tip"] = new HtmlString("<script>alert('js或者css类型获取失败！');</script>");
                        return View(obj);
                    }
                }
            }

            **/
            #endregion

            #region 暂时舍弃

            #region css替换操作
            /*
            string tempCssFileName = obj.CssFileName;
            if (!string.IsNullOrEmpty(tempCssFileName))
            {
                if (singleEntityList != null && singleEntityList.Count() > 0)
                {
                    // 以 “background:”分割对象，下面做字符串查找，截取
                    string[] _tempCssFileName = tempCssFileName.Split(new string[] { "background:" }, StringSplitOptions.None);

                    for (int i = 0; i < _tempCssFileName.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(_tempCssFileName[i]))
                        {
                            string singleData = _tempCssFileName[i];
                            if (singleData.IndexOf("url(") > -1)
                            {
                                string tempUrl = singleData.Substring((singleData.IndexOf("url(") + 4));

                                if (tempUrl.IndexOf(")") > -1)
                                {
                                    //当前全路径
                                    tempUrl = tempUrl.Substring(0, (tempUrl.IndexOf(")")));
                                    if (tempUrl.LastIndexOf("/") > -1)
                                    {
                                        //得到css中的图片名称，判断
                                        string tempCodeImg = tempUrl.Substring((tempUrl.LastIndexOf("/") + 1));
                                        List<SWfsMeetingTemplatePicInfo> singleEntity = singleEntityList.Where(c => c.FileName == tempCodeImg && c.Type == 0).ToList();
                                        if (singleEntity != null && singleEntity.Count() > 0)
                                        {
                                            //替换
                                            tempCssFileName = tempCssFileName.Replace(tempUrl, singleEntity[0].PicNo);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                obj.CssFileName = tempCssFileName;
            }
             * */
            #endregion

            #region 模版替换操作
            /*
            string tempTemplateCode = obj.TemplateCode;
            if (!string.IsNullOrEmpty(tempTemplateCode))
            {
                if (singleEntityList != null && singleEntityList.Count() > 0)
                {
                    // 以 “<img”分割对象，下面做字符串查找，截取
                    string[] _tempTemplateCode = tempTemplateCode.Split(new string[] { "<img" }, StringSplitOptions.None);

                    for (int i = 0; i < _tempTemplateCode.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(_tempTemplateCode[i]))
                        {
                            string singleData = _tempTemplateCode[i];
                            if (singleData.IndexOf("lazy=\"") > -1 && singleData.IndexOf("src=\"") > -1)
                            {
                                string tempUrl = singleData.Substring((singleData.IndexOf("lazy=\"") + 6));
                                if (tempUrl.IndexOf("\"") > -1)
                                {
                                    //当前全路径
                                    tempUrl = tempUrl.Substring(0, (tempUrl.IndexOf("\"")));
                                    if (tempUrl.LastIndexOf("/") > -1)
                                    {
                                        //得到模版中的图片名称，判断
                                        string tempCodeImg = tempUrl.Substring((tempUrl.LastIndexOf("/") + 1));
                                        List<SWfsMeetingTemplatePicInfo> singleEntity = singleEntityList.Where(c => c.FileName == tempCodeImg && c.Type == 2).ToList();
                                        if (singleEntity != null && singleEntity.Count() > 0)
                                        {
                                            //替换
                                            tempTemplateCode = tempTemplateCode.Replace(tempUrl, singleEntity[0].PicNo);
                                        }
                                    }
                                }
                             }
                            else if (singleData.IndexOf("lazy=\"") == -1 && singleData.IndexOf("src=\"") > -1)
                            {
                                string tempUrl = singleData.Substring((singleData.IndexOf("src=\"") + 5));
                                if (tempUrl.IndexOf("\"") > -1)
                                {
                                    //当前全路径
                                    tempUrl = tempUrl.Substring(0, (tempUrl.IndexOf("\"")));
                                    if (tempUrl.LastIndexOf("/") > -1)
                                    {
                                        //得到模版中的图片名称，判断
                                        string tempCodeImg = tempUrl.Substring((tempUrl.LastIndexOf("/") + 1));
                                        List<SWfsMeetingTemplatePicInfo> singleEntity = tempPicList.Where(c => c.FileName == tempCodeImg && c.Type == 2).ToList();
                                        if (singleEntity != null && singleEntity.Count() > 0)
                                        {
                                            //替换
                                            singleData = singleData.Replace(tempUrl, Shangpin.Framework.WebMvc.ServicePic.ResolveUGCImage("2", singleEntity[0].PicNo, 0, 0));
                                        }
                                    }
                                }
             
                                singleData = Regex.Replace(singleData, @"(src=['|\""]([^\""]+)['|\""])", "src=\"http://pic12.shangpin.com/images/e.gif\"  lazy=\"$2\"");
                                tempTemplateCode = tempTemplateCode.Replace(_tempTemplateCode[i], singleData);
                            }
                        }
                    }
                     tempTemplateCode = Regex.Replace(tempTemplateCode, @"(src=['|\""]([^\""]+)['|\""])", "src=\"http://pic12.shangpin.com/images/e.gif\"");
                }
                obj.TemplateCode = tempTemplateCode;
            }
             * * */
            #endregion


            #endregion

            if (obj.TemplateID == 0)//添加
            {
                if (venueService.IsExistTemplateObjByNO(obj.MettingNo) > 0)
                {
                    ViewData["tip"] = new HtmlString("<script>alert('编号已存在');</script>");
                    return View(obj);
                }

                venueService.AddTemplate(obj);
                ViewData["tip"] = new HtmlString("<script>alert('添加成功');location.href='/Shangpin/Venue/TemplateVersionList';</script>");
            }
            else//修改
            {
                venueService.EditeTemplate(obj);
                ViewData["tip"] = new HtmlString("<script>alert('修改成功');location.href='/Shangpin/Venue/TemplateVersionList';</script>");
            }
            return View(obj);
        }
        //按ID删除模板
        public ActionResult DeleteTemplateByID(int tempID)
        {
            venueService.DeleteTemplateByID(tempID);
            return Redirect("/Shangpin/Venue/TemplateVersionList");
        }
        //批量删除模板
        [HttpPost]
        public ActionResult DeleteByIDList(string objID)
        {
            if (!string.IsNullOrEmpty(objID))
            {
                venueService.DeleteByIdList(objID);
            }
            return Redirect("/Shangpin/Venue/TemplateVersionList");
        }
        //模板代码编辑
        [HttpPost]
        public string EditeTemplateContent(string tempID)
        {
            SWfsMeetingTemplateInfo obj = venueService.GetTemplateObjByID(tempID);
            if (obj == null)
            {
                return null;
            }
            return venueService.GetTemplateContent(obj.TemplateCode);
        }
        //保存模板代码
        [HttpPost, ValidateInput(false)]
        public int SaveTempContent(string tempID, string tempContent)
        {
            SWfsMeetingTemplateInfo obj = venueService.GetTemplateObjByID(tempID);
            if (obj == null)
            {
                return 0;
            }
            return venueService.SaveTemplateContent(obj.TemplateCode, tempContent);
        }

        //模板文件创建和编辑管理
        public ActionResult TemplateFileEdite()
        {
            return View();
        }
        public string GetTemplateOrCssContent(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return null;
            }
            return venueService.GetTemplateOrCssContent(filePath);
        }
        [HttpPost, ValidateInput(false)]
        public int SaveTemplateOrCssContent(string filePath, string fileText)
        {
            return venueService.SaveTemplateOrCssContent(filePath, fileText);
        }
        #endregion

        #region 会场区块管理
        /// <summary>
        /// 加载会场模板
        /// </summary>
        /// <param name="venueID">会场主键ID</param>
        /// <returns></returns>
        public ActionResult VenueTemplateEditeByID(string venueID = "0")
        {
            SWfsMeetingInfo obj = venueService.GetVenueByID(venueID);
            if (obj == null)
            {
                return View(new SWfsMeetingInfo());
            }
            return View(obj);
        }
        public ActionResult VenueTemplateEdite(string venueID = "0")
        {
            SWfsMeetingInfo obj = venueService.GetVenueByID(venueID);
            if (obj == null)
            {
                return View(new SWfsMeetingInfo());
            }
            return View(obj);
        }
        public ActionResult VenueTemplateiframe(string venueID = "0")
        {
            SWfsMeetingInfo obj = venueService.GetVenueByID(venueID);
            if (obj == null)
            {
                return View(new SWfsMeetingInfo());
            }
            return View(obj);
        }

        /// <summary>
        /// 保存区块内容
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost, ValidateInput(false)]
        public string SaveRegionRelationContent(SWfsMeetingRelationRegion obj, string OldRelationContent)
        {
            //return Json(new { status = 1, message = obj });
            JavaScriptSerializer json = new JavaScriptSerializer();
            SWfsMeetingRelationRegionContent newobj = null;
            #region 数据验证
            if (string.IsNullOrEmpty(obj.MettingNO) || string.IsNullOrEmpty(obj.MainMeetingNO) || obj.RegionID <= 0 || obj.RelationType <= 0 || obj.RelationType > 4 || string.IsNullOrEmpty(obj.TemplateNO))
            {
                return "{\"status\" : 0, \"message\" : \"数据不合法\"}";
            }

            if (Request.Files["imgfile"] != null && Request.Files["imgfile"].ContentLength > 0)
            {
                obj.ImgNO = SaveImg(Request.Files["imgfile"], "width:0,Height:0,Length:1000");
                if (rsPic.Keys.Contains("error"))
                {
                    return "{\"status\" : 0, \"message\" : \"图片不合法\"}";
                }
            }
            #endregion

            #region 拼接xml
            if (obj.RelationType == 4)
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append("<conditions>");

                if (Request.Form["timeby"] == "0")
                {
                    sb.Append("<timeby startorend=\"" + Request.Form["startorend"] + "\" startorendregion=\"" + Request.Form["startorendregion"] + "\">");
                    switch (Request.Form["startorendregionday"])
                    {
                        case "0":
                            sb.Append(DateTime.Now.ToShortDateString());
                            break;
                        case "1":
                            sb.Append(DateTime.Now.AddDays(-1).ToShortDateString());
                            break;
                        case "2":
                            sb.Append(DateTime.Now.AddDays(1).ToShortDateString());
                            break;
                        case "3":
                            sb.Append(Request.Form["startorendregiondate"]);
                            break;
                        default:
                            break;
                    }
                    sb.Append("<timeby>");
                }
                if (Request.Form["activeorspecialby"] == "0")
                {
                    sb.Append("<activeorspecialby>" + Request.Form["activeorspecial"] + "</activeorspecialby>");
                }
                if (Request.Form["closeoropentby"] == "0")
                {
                    sb.Append("<closeoropentby>" + Request.Form["closeoropent"] + "</closeoropentby>");
                }
                if (Request.Form["brandby"] == "0")
                {
                    sb.Append("<brandby brandregion=\"" + Request.Form["brandregion"] + "\" >" + Request.Form["brandNo"] + "</brandby>");
                }
                if (Request.Form["erpcategoryby"] == "0")
                {
                    sb.Append("<erpcategoryby erpcategoryregion=\""
                        + Request.Form["erpcategoryregion"] + "\" >" + Request.Form["erpcategoryNO"] + "</erpcategoryby>");
                }
                if (Request.Form["genderby"] == "0")
                {
                    sb.Append("<genderby>" + Request.Form["gender"] + "</genderby>");
                }
                sb.Append("<columcount>" + Request.Form["columcount"] + "</columcount>");
                sb.Append("<showcount>" + Request.Form["showcount"] + "</showcount>");
                if (Request.Form["isselect"] == "0")
                {
                    sb.Append("<isselect>0</isselect>");
                }
                sb.Append("</conditions>");
                obj.RelationContent = sb.ToString();
            }
            #endregion

            if (!venueService.SaveRegionRelationInfo(obj, Request.Form["oldRelationType"], Request.Form["dataobj"], OldRelationContent)) //保存块编辑的数据
            {
                return "{\"status\" : 0, \"message\" : \"数据保存异常\"}";
            }
            //obj.ImgNO=ServicePic.ResolveUGCImage("2", obj.ImgNO, 0, 0);
            if (obj.RelationType == 2)//如果是活动 查询主副标题
            {
                newobj = venueService.GetRegionRelationInfoByCondition("", obj.MettingNO, obj.RegionID + "", obj.RelationType + "", obj.TemplateNO).FirstOrDefault();
                if (newobj == null)
                {
                    venueService.DeleteRelationRegionByCondition(obj.MettingNO, obj.TemplateNO, obj.RegionID);
                    return "{\"status\" : 0, \"message\" : \"未找到关联活动的主副标题\"}";
                }
                newobj.ImgNOJPG = ServicePic.ResolveUGCImage("2", newobj.ImgNO, 0, 0);
                return "{\"status\" : 1, \"message\" : " + json.Serialize(newobj) + "}";
            }
            else
            {
                if (newobj == null)
                {
                    newobj = new SWfsMeetingRelationRegionContent();
                    newobj.MeetingRelationID = obj.MeetingRelationID;
                    newobj.MainMeetingNO = obj.MainMeetingNO;
                    if (obj.RelationType == 1)
                    {
                        newobj.MeetingDomain = Request.Form["partMeetingDomain"].ToString();
                    }
                    newobj.ImgNO = obj.ImgNO;
                    newobj.ImgNOJPG = ServicePic.ResolveUGCImage("2", obj.ImgNO, 0, 0);
                    newobj.MettingNO = obj.MettingNO;
                    newobj.RegionID = obj.RegionID;
                    newobj.RelationContent = obj.RelationContent;
                    newobj.RelationType = obj.RelationType;
                    newobj.TemplateNO = obj.TemplateNO;
                    newobj.AboutID = obj.AboutID;
                }
                return "{\"status\":1, \"message\":" + json.Serialize(newobj) + "}";
            }
        }
        /// <summary>
        /// 根据模板编号加载--模板
        /// </summary>
        /// <param name="venueNO">会场编号</param>
        /// <param name="templateNO">模板编号</param>
        /// <returns></returns>
        public ActionResult CreateHTMLByTemplate(string venueNO, string templateNO)
        {
            if (string.IsNullOrEmpty(templateNO) || string.IsNullOrEmpty(venueNO))
            {
                return Content("0");
            }
            SWfsMeetingTemplateInfo obj = venueService.GetTemplateInfoByNO(templateNO);//获取模板
            if (obj == null)
            {
                return Content("0");
            }
            if (string.IsNullOrEmpty(obj.TemplateCode))
            {
                return Content("0");
            }
            if (!System.IO.File.Exists(Server.MapPath(obj.TemplateCode))) //验证模板是否存在
            {
                return Content("0");
            }
            //查询填充模板的数据列表
            IEnumerable<SWfsMeetingRelationRegionContent> Opratorlist = venueService.GetRegionRelationInfoByCondition("", venueNO, "", "", templateNO);
            return View(obj.TemplateCode, Opratorlist);
        }
        /// <summary>
        /// 异步获取会场分页列表数据返回json
        /// </summary>
        /// <returns></returns>
        public string GetVenueListJsonByPage()
        {
            JavaScriptSerializer json = new JavaScriptSerializer();
            int totalCount = 0;
            SWfsMeetingRelationRegionContent newobj = new SWfsMeetingRelationRegionContent();
            if (!string.IsNullOrEmpty(Request.Form["MeetingRelationID"]) && Request.Form["MeetingRelationID"] + "" != "0")
            {
                SWfsMeetingRelationRegion obj = venueService.GetMeetingRelationByID(Request.Form["MeetingRelationID"]);
                newobj.MeetingRelationID = obj.MeetingRelationID;
                newobj.MainMeetingNO = obj.MainMeetingNO;
                newobj.ImgNO = obj.ImgNO;
                newobj.ImgNOJPG = ServicePic.ResolveUGCImage("2", obj.ImgNO, 0, 0);
                newobj.MettingNO = obj.MettingNO;
                newobj.RegionID = obj.RegionID;
                newobj.RelationContent = obj.RelationContent;
                newobj.RelationType = obj.RelationType;
                newobj.TemplateNO = obj.TemplateNO;
                newobj.AboutID = obj.AboutID;
                newobj.Direction = obj.Direction;
            }
            else
            {
                //查询会场区块信息的关联内容--根据（会场编号--区域块ID--关联模块RelationType--模板编号)
                //newobj = venueService.GetRegionRelationInfoByCondition("", Request.Form["venueNO"], Request.Form["regionID"], "1", Request.Form["TemplateNO"]).FirstOrDefault();
                SWfsMeetingRelationRegion obj = venueService.GetRelationobjByContent(Request.Form["venueNO"], int.Parse(Request.Form["regionID"]), Request.Form["TemplateNO"]);
                if (obj != null)
                {
                    newobj.MeetingRelationID = obj.MeetingRelationID;
                    newobj.MainMeetingNO = obj.MainMeetingNO;
                    newobj.ImgNO = obj.ImgNO;
                    newobj.ImgNOJPG = ServicePic.ResolveUGCImage("2", obj.ImgNO, 0, 0);
                    newobj.MettingNO = obj.MettingNO;
                    newobj.RegionID = obj.RegionID;
                    newobj.RelationContent = obj.RelationContent;
                    newobj.RelationType = obj.RelationType;
                    newobj.TemplateNO = obj.TemplateNO;
                    newobj.AboutID = obj.AboutID;
                    newobj.Direction = obj.Direction;
                }
            }
            //获取会场列表
            IList<SWfsMeetingInfo> list = venueService.GetVenueListByPage(Request.Form["pageIndex"], 10,
                Request.Form["nameAndDomain"], "", Request.Form["status"], "", "",
                Request.Form["activeNO"], Request.Form["startDate"], Request.Form["endDate"], Request.Form["venueID"], out totalCount);
            if (newobj != null)
            {
                if (!string.IsNullOrEmpty(newobj.RelationContent))
                {
                    if (list.Count(p => p.MeetingID + "" == newobj.RelationContent) == 1)
                    {
                        list.Single(p => p.MeetingID + "" == newobj.RelationContent).MeetingID = -1;
                    }
                }
            }
            else
            {
                newobj = new SWfsMeetingRelationRegionContent();
            }
            for (int i = 0; i < list.Count; i++)
            {
                list[i].WebPreViewCode = list[i].StartTime.ToString("yyyy-MM-dd HH:mm:ss");
                list[i].WebStartCode = list[i].EndTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
            return "{\"relationobj\":" + json.Serialize(newobj) + ",\"count\":" + Math.Ceiling(totalCount / 10.0) + ",\"list\":" + json.Serialize(list) + "}";
        }
        //加载活动分页列表
        public string GetActiveListJsonByPage()
        {
            JavaScriptSerializer json = new JavaScriptSerializer();
            int totalCount = 0;
            SWfsMeetingRelationRegionContent newobj = new SWfsMeetingRelationRegionContent();
            if (!string.IsNullOrEmpty(Request.Form["MeetingRelationID"]) && Request.Form["MeetingRelationID"] + "" != "0")
            {
                SWfsMeetingRelationRegion obj = venueService.GetMeetingRelationByID(Request.Form["MeetingRelationID"]);
                newobj.MeetingRelationID = obj.MeetingRelationID;
                newobj.MainMeetingNO = obj.MainMeetingNO;
                newobj.ImgNO = obj.ImgNO;
                newobj.ImgNOJPG = ServicePic.ResolveUGCImage("2", obj.ImgNO, 0, 0);
                newobj.MettingNO = obj.MettingNO;
                newobj.RegionID = obj.RegionID;
                newobj.RelationContent = obj.RelationContent;
                newobj.RelationType = obj.RelationType;
                newobj.TemplateNO = obj.TemplateNO;
                newobj.AboutID = obj.AboutID;
                newobj.Direction = obj.Direction;
            }
            else
            {
                //查询会场区块信息的关联内容--根据（会场编号--区域块ID--关联模块RelationType)
                //newobj = venueService.GetRegionRelationInfoByCondition("", Request.Form["venueNO"], Request.Form["regionID"], "2", Request.Form["TemplateNO"]).FirstOrDefault();
                SWfsMeetingRelationRegion obj = venueService.GetRelationobjByContent(Request.Form["venueNO"], int.Parse(Request.Form["regionID"]), Request.Form["TemplateNO"]);
                if (obj != null)
                {
                    newobj.MeetingRelationID = obj.MeetingRelationID;
                    newobj.MainMeetingNO = obj.MainMeetingNO;
                    newobj.ImgNO = obj.ImgNO;
                    newobj.ImgNOJPG = ServicePic.ResolveUGCImage("2", obj.ImgNO, 0, 0);
                    newobj.MettingNO = obj.MettingNO;
                    newobj.RegionID = obj.RegionID;
                    newobj.RelationContent = obj.RelationContent;
                    newobj.RelationType = obj.RelationType;
                    newobj.TemplateNO = obj.TemplateNO;
                    newobj.AboutID = obj.AboutID;
                    newobj.Direction = obj.Direction;
                }
            }

            //获取活动列表
            IList<ActiveAndSpecial> list = venueService.GetActiveAndSpecialByPage(Request.Form["mainVenueNO"], Request.Form["pageIndex"], 10,
                Request.Form["activeNameAndNO"], Request.Form["webSource"], Request.Form["activeType"], Request.Form["activeStatus"],
                Request.Form["activeStartDate"], Request.Form["activeEndDate"], out totalCount);
            if (newobj != null)
            {
                if (!string.IsNullOrEmpty(newobj.RelationContent))
                {
                    if (list.Count(p => p.ActiveID + "" == newobj.RelationContent) == 1)
                    {
                        list.Single(p => p.ActiveID + "" == newobj.RelationContent).ActiveID = -1;
                    }
                }
            }
            else
            {
                newobj = new SWfsMeetingRelationRegionContent();
            }
            return "{\"relationobj\":" + json.Serialize(newobj) + ",\"count\":" + Math.Ceiling(totalCount / 10.0) + ",\"list\":" + json.Serialize(list) + "}";
        }
        //加载自定义链接
        public string GetRelationLinkJson()
        {
            JavaScriptSerializer json = new JavaScriptSerializer();
            SWfsMeetingRelationRegionContent newobj = new SWfsMeetingRelationRegionContent();
            if (!string.IsNullOrEmpty(Request.Form["MeetingRelationID"]) && Request.Form["MeetingRelationID"] + "" != "0")
            {
                SWfsMeetingRelationRegion obj = venueService.GetMeetingRelationByID(Request.Form["MeetingRelationID"]);
                newobj.MeetingRelationID = obj.MeetingRelationID;
                newobj.MainMeetingNO = obj.MainMeetingNO;
                newobj.ImgNO = obj.ImgNO;
                newobj.ImgNOJPG = ServicePic.ResolveUGCImage("2", obj.ImgNO, 0, 0);
                newobj.MettingNO = obj.MettingNO;
                newobj.RegionID = obj.RegionID;
                newobj.RelationContent = obj.RelationContent;
                newobj.RelationType = obj.RelationType;
                newobj.TemplateNO = obj.TemplateNO;
                newobj.AboutID = obj.AboutID;
                newobj.Direction = obj.Direction;
            }
            else
            {
                //查询会场区块信息的关联内容--根据（会场编号--区域块ID--关联模块RelationType)
                //newobj = venueService.GetRegionRelationInfoByCondition("", Request.Form["venueNO"], Request.Form["regionID"], "3", Request.Form["TemplateNO"]).FirstOrDefault();
                SWfsMeetingRelationRegion obj = venueService.GetRelationobjByContent(Request.Form["venueNO"], int.Parse(Request.Form["regionID"]), Request.Form["TemplateNO"]);
                if (obj != null)
                {
                    newobj.MeetingRelationID = obj.MeetingRelationID;
                    newobj.MainMeetingNO = obj.MainMeetingNO;
                    newobj.ImgNO = obj.ImgNO;
                    newobj.ImgNOJPG = ServicePic.ResolveUGCImage("2", obj.ImgNO, 0, 0);
                    newobj.MettingNO = obj.MettingNO;
                    newobj.RegionID = obj.RegionID;
                    newobj.RelationContent = obj.RelationContent;
                    newobj.RelationType = obj.RelationType;
                    newobj.TemplateNO = obj.TemplateNO;
                    newobj.AboutID = obj.AboutID;
                    newobj.Direction = obj.Direction;
                }
            }
            return json.Serialize(newobj);
        }
        //加载模块数据
        public string GetModuleDate()
        {
            JavaScriptSerializer json = new JavaScriptSerializer();
            SWfsMeetingRelationRegionContent newobj = new SWfsMeetingRelationRegionContent();
            if (!string.IsNullOrEmpty(Request.Form["MeetingRelationID"]) && Request.Form["MeetingRelationID"] + "" != "0")
            {
                SWfsMeetingRelationRegion obj = venueService.GetMeetingRelationByID(Request.Form["MeetingRelationID"]);
                newobj.MeetingRelationID = obj.MeetingRelationID;
                newobj.MainMeetingNO = obj.MainMeetingNO;
                newobj.ImgNO = obj.ImgNO;
                newobj.ImgNOJPG = ServicePic.ResolveUGCImage("2", obj.ImgNO, 0, 0);
                newobj.MettingNO = obj.MettingNO;
                newobj.RegionID = obj.RegionID;
                newobj.RelationContent = obj.RelationContent;
                newobj.RelationType = obj.RelationType;
                newobj.TemplateNO = obj.TemplateNO;
                newobj.AboutID = obj.AboutID;
                newobj.Direction = obj.Direction;
            }
            else
            {
                //查询会场区块信息的关联内容--根据（会场编号--区域块ID--关联模块RelationType)
                //SWfsMeetingRelationRegionContent oldobj = venueService.GetRegionRelationInfoByCondition("", Request.Form["venueNO"], Request.Form["regionID"], "4", Request.Form["TemplateNO"]).FirstOrDefault();
                SWfsMeetingRelationRegion obj = venueService.GetRelationobjByContent(Request.Form["venueNO"], int.Parse(Request.Form["regionID"]), Request.Form["TemplateNO"]);
                if (obj != null)
                {
                    newobj.MeetingRelationID = obj.MeetingRelationID;
                    newobj.MainMeetingNO = obj.MainMeetingNO;
                    newobj.ImgNO = obj.ImgNO;
                    newobj.ImgNOJPG = ServicePic.ResolveUGCImage("2", obj.ImgNO, 0, 0);
                    newobj.MettingNO = obj.MettingNO;
                    newobj.RegionID = obj.RegionID;
                    newobj.RelationContent = obj.RelationContent;
                    newobj.RelationType = obj.RelationType;
                    newobj.TemplateNO = obj.TemplateNO;
                    newobj.AboutID = obj.AboutID;
                    newobj.Direction = obj.Direction;
                }
            }
            return json.Serialize(newobj);
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
        //发布会场
        [HttpPost, ValidateInput(false)]
        public string PublishVenue(string venuehtml)
        {
            if (venuehtml.Contains("请选择模板"))
            {
                return ",请选择模板！";
            }
            else
            {
                try
                {
                    VenueService service = new VenueService();
                    int isPreView = int.Parse(Request.Form["isPreView"]);
                    int isMobile = int.Parse(Request.Form["isMobile"]);
                    int meetingId = int.Parse(Request.Form["MeetingID"]);
                    if (venueService.SaveVenueHtml(new SWfsMeetingInfo() { MeetingID = meetingId, MobilePreViewCode = venuehtml }, isPreView, isMobile))
                    {
                        SWfsMeetingInfoHtml htmlModel = service.GetHtmlByMeetingId(meetingId);
                        string type = string.Empty;
                        if (htmlModel == null)
                        {
                            type = "0000";
                            htmlModel = new SWfsMeetingInfoHtml();
                        }
                        else
                        {
                            type = htmlModel.TemplateType;
                        }
                        IList<char> list = type.ToList();
                        if (isPreView == 0 && isMobile == 0)//Mobile预热
                        {
                            htmlModel.MobilePreViewCode = venuehtml;
                            list[0] = '1';
                        }
                        else if (isPreView == 1 && isMobile == 0)//Mobile进行
                        {
                            htmlModel.MobileStartCode = venuehtml;
                            list[1] = '1';
                        }
                        else if (isPreView == 0 && isMobile == 1)//web预热
                        {
                            htmlModel.WebPreViewCode = venuehtml;
                            list[2] = '1';
                        }
                        else if (isPreView == 1 && isMobile == 1)//web进行
                        {
                            htmlModel.WebStartCode = venuehtml;
                            list[3] = '1';
                        }
                        htmlModel.TemplateType = string.Join("", list);
                        htmlModel.IsPublish = 1;
                        htmlModel.UpdateDate = DateTime.Now;
                        htmlModel.UpdateUserId = PresentationHelper.GetPassport().UserName;
                        if (htmlModel != null)
                        {
                            htmlModel.ID = htmlModel.ID;
                            htmlModel.MobilePreViewCode = venuehtml;
                            service.SaveVenueHtml(htmlModel, isPreView, isMobile);
                        }
                        else
                        {
                            htmlModel.CreateDate = DateTime.Now;
                            htmlModel.CreateUserId = PresentationHelper.GetPassport().UserName;
                            service.AddMeetingHtml(htmlModel);
                        }
                        venueService.AddOperationLog(Request.Form["venueNO"], 0, 1, LogActionType.Edit, "发布会场成功");
                        return "1";
                    }
                    return "0";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }
        /// <summary>
        /// 判断是否含有重复值 
        /// </summary>
        /// <param name="venuehtml"></param>
        /// <returns></returns>
        public string ISduplicateValues(string venuehtml)
        {

            try
            {
                string _venuehtml = venuehtml.Replace("\r", "").Replace("\n", "").Replace("\t", "");
                List<string> list = new List<string>();
                string pattern = @"<img.*?(opratorregion=['|\""](\d*)['|\""])(.*?)>";
                Regex regObj = new Regex(pattern);
                //string ResultStr = "";
                MatchCollection results = regObj.Matches(_venuehtml);
                if (results.Count > 0)
                {
                    foreach (Match item in results)
                    {
                        Match Result = regObj.Match(item.Value);
                        if (Result.Success)
                        {
                            if (list.Count > 0)
                            {
                                foreach (var childItem in list)
                                {
                                    if (childItem.Equals(Result.Groups[2].ToString()))
                                    {

                                        return "图片位置opratorregion=" + Result.Groups[2].ToString() + "处重复!";

                                    }

                                }
                            }
                            list.Add(Result.Groups[2].ToString());

                        }

                    }

                }
                return "1";

            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }
        //新增保存html块儿
        [HttpPost, ValidateInput(false)]
        public string SaveVenue(string venuehtml)
        {
            VenueService service = new VenueService();
            try
            {
                int isPreView = int.Parse(Request.Form["isPreView"]);
                int isMobile = int.Parse(Request.Form["isMobile"]);
                SWfsMeetingInfoHtml meetHtml = new SWfsMeetingInfoHtml();
                int meetingId = int.Parse(Request.Form["MeetingID"]);
                if (venuehtml.Contains("请选择模板"))
                {
                    return ",请选择模板!";
                }
                else
                {
                    if (meetingId > 0)
                    {
                        meetHtml.MettingId = meetingId;
                    }
                    string str = ISduplicateValues(venuehtml);
                    if (str != "1")
                    {
                        return str;
                        //return "0";
                    }
                    else
                    {
                        SWfsMeetingInfoHtml model = service.GetHtmlByMeetingId(meetingId);
                        string TemplateType = null;
                        Regex r = new Regex("0");
                        if (model != null)
                        {
                            TemplateType = model.TemplateType;
                            r = new Regex("1");
                        }
                        else
                        {
                            TemplateType = "0000";
                        }
                        if (isPreView == 0 && isMobile == 0)//Mobile预热ll
                        {
                            TemplateType = r.Replace(TemplateType, "0", 1, 0);
                            meetHtml.MobilePreViewCode = venuehtml;
                            meetHtml.TemplateType = TemplateType;
                            meetHtml.MobileStartCode = model == null ? "" : model.MobileStartCode;
                            meetHtml.WebStartCode = model == null ? "" : model.WebStartCode;
                            meetHtml.WebPreViewCode = model == null ? "" : model.WebPreViewCode;
                        }
                        else if (isPreView == 1 && isMobile == 0)//Mobile进行
                        {
                            TemplateType = r.Replace(TemplateType, "0", 1, 1);
                            meetHtml.MobileStartCode = venuehtml;
                            meetHtml.TemplateType = TemplateType;
                            meetHtml.MobilePreViewCode = model == null ? "" : model.MobilePreViewCode;
                            meetHtml.WebStartCode = model == null ? "" : model.WebStartCode;
                            meetHtml.WebPreViewCode = model == null ? "" : model.WebPreViewCode;
                        }
                        else if (isPreView == 0 && isMobile == 1)//web预热
                        {
                            TemplateType = r.Replace(TemplateType, "0", 1, 2);
                            meetHtml.WebPreViewCode = venuehtml;
                            meetHtml.TemplateType = TemplateType;
                            meetHtml.MobilePreViewCode = model == null ? "" : model.MobilePreViewCode;
                            meetHtml.WebStartCode = model == null ? "" : model.WebStartCode;
                            meetHtml.MobileStartCode = model == null ? "" : model.MobileStartCode;
                        }
                        else if (isPreView == 1 && isMobile == 1)//web进行
                        {
                            TemplateType = r.Replace(TemplateType, "0", 1, 3);
                            meetHtml.WebStartCode = venuehtml;
                            meetHtml.TemplateType = TemplateType;
                            meetHtml.MobilePreViewCode = model == null ? "" : model.MobilePreViewCode;
                            meetHtml.WebPreViewCode = model == null ? "" : model.WebPreViewCode;
                            meetHtml.MobileStartCode = model == null ? "" : model.MobileStartCode;
                        }
                        meetHtml.IsPublish = 0;
                        meetHtml.UpdateDate = DateTime.Now;
                        meetHtml.UpdateUserId = PresentationHelper.GetPassport().UserName;
                        bool flag = false;
                        if (model == null)
                        {
                            meetHtml.CreateDate = DateTime.Now;
                            meetHtml.CreateUserId = PresentationHelper.GetPassport().UserName;
                            int meet = venueService.AddMeetingHtml(meetHtml);
                            flag = meet == 0 ? true : false;
                        }
                        else
                        {
                            meetHtml.ID = model.ID;
                            flag = venueService.AddNenueHtml(meetHtml, isPreView, isMobile);
                        }
                        if (flag)
                        {
                            venueService.AddOperationLog(Request.Form["MeetingID"], 0, 1, LogActionType.Edit, "编辑会场HTML");
                            return "1";
                        }
                        return "0";

                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        //查询会场区块列表
        public ActionResult GetRelationRegionList()
        {
            return View(venueService.GetRelationRegionList(Request.QueryString["MeetingNO"], Request.QueryString["TemplateNO"]));
        }
        //按ID删除会场区块列表
        public ActionResult DeleteRelationRegionByID(int MeetingRelationID, string MeetingNO, string TemplateNO)
        {
            venueService.DeleteRelationRegionByID(MeetingRelationID);
            return Redirect("GetRelationRegionList?MeetingNO=" + MeetingNO + "&TemplateNO=" + TemplateNO);
        }
        #endregion

        #region 会场数据统计
        public ActionResult VenueVisitList()
        {
            IEnumerable<SwfsVenueVisitInfoExtends> list = null;
            if (!string.IsNullOrEmpty(Request.QueryString["MeetingNO"]))
            {
                list = venueService.GetVenueVisitList(Request.QueryString["MeetingNO"], Request.QueryString["timeregion"],
                string.IsNullOrEmpty(Request.QueryString["StartDate"]) ? DateTime.Now : DateTime.Parse(Request.QueryString["StartDate"]),
                string.IsNullOrEmpty(Request.QueryString["EndDate"]) ? DateTime.Now : DateTime.Parse(Request.QueryString["EndDate"]), string.IsNullOrEmpty(Request.QueryString["ClickRegionID"]) ? "0" : Request.QueryString["ClickRegionID"]);
            }
            return View(list);
        }
        #endregion

        #region 会场管理 by zhangwei 20130910
        /// <summary>
        /// 添加和修改页面
        /// </summary>
        /// <param name="meetId">会场ID</param>
        /// <param name="parentId">主会场ID</param>
        /// <param name="type">0主会场，1分会场</param>
        /// <returns></returns>
        public ActionResult MeetingIndex(string DomainName, int meetId = 0, int parentId = 0, int type = 0, string ParentMeetingName = "")
        {

            SWfsMeetingInfo model = new SWfsMeetingInfo();
            ViewBag.MeetingID = meetId;
            ViewBag.ParentID = parentId;
            ViewBag.MeetingNO = string.Empty;
            ViewBag.MainDomain = string.Empty;
            ViewBag.SpDisabled = string.Empty;
            ViewBag.AlDisabled = string.Empty;

            if (meetId == 0 && parentId > 0 && type == 1)
            {
                ViewBag.CurrentTitle = "添加分会场";
                SWfsMeetingInfo m = venueService.GetVenueByID(parentId.ToString());
                model.SourceFrom = m.SourceFrom;
                ViewBag.CurrentParentMeeting = m.MeetingName;
                ViewBag.Show = "-";
                RadioSelect(model.SourceFrom);
            }
            else if (meetId == 0 && parentId == 0 && type == 0)
            {
                ViewBag.CurrentTitle = "添加会场";
                ViewBag.WebUrl = key;
                ViewBag.SWebUrl = Shangpinkey;
                ViewBag.MainDomain = DomainName;

            }

            ViewBag.Type = type; //0主会场，1分会场
            ViewBag.FormAction = "/shangpin/venue/AddMeeting?type=" + type;
            if (meetId > 0) //修改
            {
                model = venueService.GetVenueByID(meetId.ToString());
                ViewBag.MeetingNO = model.MeetingNO;
                SWfsMeetingMetaInfo metaModel = new SWfsMeetingMetaInfo();
                metaModel = venueService.GetInfoByMeetingID(meetId.ToString());
                if (metaModel != null)
                {
                    ViewBag.Tilte = metaModel.Title;
                    ViewBag.Keyworks = metaModel.KeyWords;
                    ViewBag.Description = metaModel.Description;
                }
                if (parentId > 0 && type == 1)
                {
                    RadioSelect(model.SourceFrom);
                    if (model.WebOrMobile > 0)
                    {
                        //模板名称回填
                        if (model.WebOrMobile == 1)
                        {
                            if (model.WebStartNO != null)
                            {
                                ViewBag.WebStartNO = venueService.GetTemplateName(model.WebStartNO);
                            }
                            if (model.WebPreViewNO != null)
                            {
                                ViewBag.WebPreViewNO = venueService.GetTemplateName(model.WebPreViewNO);
                            }
                            ViewBag.MobileChk = "none";
                        }

                        if (model.WebOrMobile == 2)
                        {
                            if (model.MobilePreViewNO != null)
                            {
                                ViewBag.MobilePreViewNO = venueService.GetTemplateName(model.MobilePreViewNO);

                            }
                            if (model.MobileStartNO != null)
                            {
                                ViewBag.MobileStartNO = venueService.GetTemplateName(model.MobileStartNO);
                            }
                            ViewBag.WebChk = "none";
                        }
                        if (model.WebOrMobile == 3)
                        {
                            if (model.WebStartNO != null)
                            {
                                ViewBag.WebStartNO = venueService.GetTemplateName(model.WebStartNO);
                            }
                            if (model.WebPreViewNO != null)
                            {
                                ViewBag.WebPreViewNO = venueService.GetTemplateName(model.WebPreViewNO);
                            }
                            if (model.MobilePreViewNO != null)
                            {
                                ViewBag.MobilePreViewNO = venueService.GetTemplateName(model.MobilePreViewNO);
                            }
                            if (model.MobileStartNO != null)
                            {
                                ViewBag.MobileStartNO = venueService.GetTemplateName(model.MobileStartNO);

                            }
                            ViewBag.WebChk = "block";
                            ViewBag.MobileChk = "block";
                        }

                    }

                    if (model.SourceFrom == 2)
                    {

                        ViewBag.WebUrl = model.MeetingDomain;
                        ViewBag.SWebUrl = Shangpinkey;
                        ViewBag.CurrentParentMeeting = model.MeetingName;
                        ViewBag.ParentMeetingDomin = model.MeetingDomain.Substring(model.MeetingDomain.LastIndexOf("-") + 1, model.MeetingDomain.Length - (model.MeetingDomain.LastIndexOf("-") + 1)).Replace(Shangpinkey, "").Replace("//", "/");
                        ViewBag.TitleShow = "奥莱";
                        model.MeetingDomain = key + model.MeetingDomain.Substring(0, model.MeetingDomain.IndexOf("-") + 1);//+ (model.MeetingDomain.Replace("//", "/"));

                    }
                    else if (model.SourceFrom == 1)
                    {

                        ViewBag.WebUrl = key;
                        ViewBag.SWebUrl = model.MeetingDomain;
                        ViewBag.CurrentParentMeeting = model.MeetingName;
                        //处理原始会场域名类型数据
                        if (model.MeetingDomain.Replace(Shangpinkey, "").LastIndexOf("/") > -1)
                        {
                            ViewBag.ParentMeetingDomin =
                                (model.MeetingDomain.Replace(Shangpinkey, "").Substring(model.MeetingDomain.Replace(Shangpinkey, "").LastIndexOf("/") + 1, model.MeetingDomain.Replace(Shangpinkey, "").Length - (model.MeetingDomain.Replace(Shangpinkey, "").LastIndexOf("/") + 1))).Replace("//", "/");
                        }
                        else
                        {
                            ViewBag.ParentMeetingDomin = model.MeetingDomain.Substring(model.MeetingDomain.LastIndexOf("-") + 1, model.MeetingDomain.Length - (model.MeetingDomain.LastIndexOf("-") + 1)).Replace(Shangpinkey, "").Replace("//", "/");
                        }
                        ViewBag.TitleShow = "尚品";
                        if (!model.MeetingDomain.Contains(Shangpinkey) && model.MeetingDomain.IndexOf("-") > -1)
                        {
                            model.MeetingDomain = Shangpinkey + model.MeetingDomain.Substring(0, model.MeetingDomain.IndexOf("-") + 1); // + (model.MeetingDomain.Replace(Shangpinkey, "")).Replace("//", "/");
                        }
                        else if (model.MeetingDomain.Contains(Shangpinkey) && model.MeetingDomain.LastIndexOf("/") > -1)
                        {
                            model.MeetingDomain = model.MeetingDomain.Substring(0, model.MeetingDomain.LastIndexOf("/")) + "-"; // + (model.MeetingDomain.Replace(Shangpinkey, "")).Replace("//", "/");
                        }

                    }

                    ViewBag.CurrentTitle = "编辑分会场";
                }
                else if (parentId == 0 && type == 0)
                {
                    ViewBag.CurrentTitle = "编辑会场";
                    ViewBag.CurrentParentMeeting = model.MeetingName;
                    RadioSelect(model.SourceFrom);
                    if (model.WebOrMobile > 0)
                    {
                        //模板名称回填
                        if (model.WebOrMobile == 1)
                        {
                            if (model.WebStartNO != null)
                            {
                                ViewBag.WebStartNO = venueService.GetTemplateName(model.WebStartNO);
                            }
                            if (model.WebPreViewNO != null)
                            {
                                ViewBag.WebPreViewNO = venueService.GetTemplateName(model.WebPreViewNO);
                            }
                            ViewBag.MobileChk = "none";
                            //model.WebStartNO = "";
                            //model.mo
                        }

                        if (model.WebOrMobile == 2)
                        {
                            if (model.MobilePreViewNO != null)
                            {
                                ViewBag.MobilePreViewNO = venueService.GetTemplateName(model.MobilePreViewNO);

                            }
                            if (model.MobileStartNO != null)
                            {
                                ViewBag.MobileStartNO = venueService.GetTemplateName(model.MobileStartNO);
                            }
                            ViewBag.WebChk = "none";
                        }
                        if (model.WebOrMobile == 3)
                        {
                            if (model.WebStartNO != null)
                            {
                                ViewBag.WebStartNO = venueService.GetTemplateName(model.WebStartNO);

                            }
                            if (model.WebPreViewNO != null)
                            {
                                ViewBag.WebPreViewNO = venueService.GetTemplateName(model.WebPreViewNO);
                            }
                            if (model.MobilePreViewNO != null)
                            {
                                ViewBag.MobilePreViewNO = venueService.GetTemplateName(model.MobilePreViewNO);
                            }
                            if (model.MobileStartNO != null)
                            {
                                ViewBag.MobileStartNO = venueService.GetTemplateName(model.MobileStartNO);

                            }
                            ViewBag.WebChk = "block";
                            ViewBag.MobileChk = "block";
                        }

                    }

                    if (model.SourceFrom == 2)
                    {

                        ViewBag.WebUrl = model.MeetingDomain;
                        ViewBag.SWebUrl = Shangpinkey;
                        ViewBag.ParentMeetingDomin = model.MeetingDomain.Replace(key, "").Replace("/", "");
                        ViewBag.TitleShow = "奥莱";
                        model.MeetingDomain = key; //+ (model.MeetingDomain.Replace(key, "")).Replace("//", "/");
                    }
                    else if (model.SourceFrom == 1)
                    {

                        ViewBag.WebUrl = key;
                        ViewBag.SWebUrl = model.MeetingDomain;
                        if (model.MeetingDomain.Contains(Shangpinkey + "/"))
                        {
                            ViewBag.ParentMeetingDomin = (model.MeetingDomain.Replace(Shangpinkey, "")).Replace("/", "");
                        }
                        else
                        {
                            ViewBag.ParentMeetingDomin = model.MeetingDomain.Replace(Shangpinkey, "").Replace("//", "/");
                        }

                        ViewBag.TitleShow = "尚品";
                        model.MeetingDomain = Shangpinkey; //+ (model.MeetingDomain.Replace(Shangpinkey, "")).Replace("//", "/");
                    }
                }
                ViewBag.FormAction = "/shangpin/venue/EditMeeting?type=" + type;
            }
            else
            {
                if (meetId == 0 && parentId > 0 && type == 1)
                {
                    SWfsMeetingInfo m = venueService.GetVenueByID(parentId.ToString());

                    if (m != null && m.MeetingID > 0)
                    {
                        if (m.SourceFrom == 2)
                        {
                            ViewBag.MainDomain = key + (m.MeetingDomain.Trim().Replace(key, "")).Replace("//", "/");
                        }
                        else if (m.SourceFrom == 1)
                        {
                            ViewBag.MainDomain = Shangpinkey + (m.MeetingDomain.Trim().Replace(Shangpinkey, "")).Replace("//", "/");
                        }


                    }
                }


            }

            return View(model);
        }//MeetingIndex
        void RadioSelect(int Soruceform)
        {
            if (Soruceform == 1)
            {
                ViewBag.SpDisabled = string.Empty;
                ViewBag.AlDisabled = "disabled";
            }
            else if (Soruceform == 2)
            {
                ViewBag.AlDisabled = string.Empty;
                ViewBag.SpDisabled = "disabled";
            }
            else
            {
                ViewBag.AlDisabled = string.Empty;
                ViewBag.SpDisabled = string.Empty;
            }
        }


        /// <summary>
        /// 添加会场
        /// </summary>
        /// <param name="model">会场信息</param>
        /// <param name="type">0添加修改主会场，1添加修改分会场</param>
        /// <param name="webOrMobileType">展示渠道 1网站,2移动端,3全部</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddMeeting(SWfsMeetingInfo model, SWfsMeetingMetaInfo metaModel, int type, int webOrMobileType = 0, int parentId = 0)
        {
            int result = 0;
            string msg = string.Empty;
            string domain = Request.Form["hidMainDomainName"] != null ? Request.Form["hidMainDomainName"] : string.Empty;

            #region 生成会场编号
            string meetingNo = CreateMeetNo();
            #endregion
            ViewBag.MainDomain = string.Empty;
            model.MeetingNO = meetingNo;
            model.WebOrMobile = (short)webOrMobileType;
            if (model.MeetingDomain != null)
            {
                Regex reg = new Regex(@"^[0-9a-zA-Z]{1,15}$");
                if (reg.IsMatch(model.MeetingDomain))
                {
                    model.MeetingDomain = model.MeetingDomain;
                }
                else
                {
                    return Json(new { result = "-1", message = "会场域名不能为空且域名只能为数字或字母!" }, "text/plain", Encoding.UTF8);
                }
            }
            if (type == 1 && !string.IsNullOrEmpty(domain))
            {
                model.MeetingDomain = (domain.Substring(domain.LastIndexOf("/") + 1, domain.Length - (domain.LastIndexOf("/") + 1)) + "-" + model.MeetingDomain.Trim()).ToLower().Replace("//", "/");
            }

            if (type == 0 && !string.IsNullOrEmpty(domain))
            {
                if (model.SourceFrom == 1)
                {
                    model.MeetingDomain = model.MeetingDomain.Trim().ToLower().Replace("//", "/");
                }
                else if (model.SourceFrom == 2)
                {
                    model.MeetingDomain = model.MeetingDomain.Trim().ToLower().Replace("//", "/");
                }
            }
            if (!string.IsNullOrEmpty(model.MeetingDomain.Trim()))
            {
                bool isExist = venueService.CheckMeetingDomainIsExist(model.MeetingID, model.MeetingDomain);
                if (isExist)
                {
                    return Json(new { result = "-1", message = "会场域名已经存在！" }, "text/plain", Encoding.UTF8);
                }
            }
            if (new DateTime(model.PreViewTime.Year, model.PreViewTime.Month, model.PreViewTime.Day, model.PreViewTime.Hour, model.PreViewTime.Minute, 0) >= new DateTime(model.StartTime.Year, model.StartTime.Month, model.StartTime.Day, model.StartTime.Hour, model.StartTime.Minute, 0))
            {
                return Json(new { result = "-1", message = "预热日期不能大于或等于会场开始日期！" }, "text/plain", Encoding.UTF8);
            }

            if (model.WebOrMobile == 1 && string.IsNullOrEmpty(model.WebStartNO) && string.IsNullOrEmpty(model.WebPreViewNO))
            {
                return Json(new { result = "-1", message = "请选择会场网页模板" }, "text/plain", Encoding.UTF8);
            }


            if (model.WebOrMobile == 2 && string.IsNullOrEmpty(model.MobileStartNO) && string.IsNullOrEmpty(model.MobilePreViewNO))
            {
                return Json(new { result = "-1", message = "请选择会场移动端模板" }, "text/plain", Encoding.UTF8);
            }
            if (model.WebOrMobile == 2 || model.WebOrMobile == 3)
            {
                model.MobileShowImg = "";
                //if (null != Request.Files["MobileShowImg"] && !string.IsNullOrEmpty(Request.Files["MobileShowImg"].FileName))
                //{
                //    string key = "MeetingMobilePic";
                //    string PicSize = AppSettingManager.AppSettings[key].ToString();
                //    Dictionary<string, string> flapPic = new CommonService().PostImg(Request.Files["MobileShowImg"], PicSize);
                //    string flapPicNo = flapPic.Values.FirstOrDefault();
                //    string flapPicNoKey = flapPic.Keys.FirstOrDefault();
                //    if (!String.IsNullOrEmpty(flapPicNo))
                //    {
                //        if (flapPicNoKey != "error")
                //        {
                //            model.MobileShowImg = flapPicNo;
                //        }
                //        else
                //        {
                //            return Json(new { result = "-1", message = flapPicNo }, "text/plain", Encoding.UTF8);
                //        }
                //    }
                //    else { model.MobileShowImg = ""; }
                //}
                //else
                //{
                //    return Json(new { result = "-1", message = "请选择移动端图片" }, "text/plain", Encoding.UTF8);
                //}
            }
            if (null != Request.Form["Keywords"])
            {

                metaModel.KeyWords = Convert.ToString(Request.Form["Keywords"]).Trim();

            }
            if (null != Request.Form["Description"])
            {
                metaModel.Description = Convert.ToString(Request.Form["Description"]).Trim();
            }

            model.IsSelect = false;
            model.CreateTime = DateTime.Now;

            if (venueService.AddMeeting(model))
            {

                metaModel.MeetingID = model.MeetingID;
                metaModel.Title = model.MeetingName;
                if (venueService.AddMateMeeting(metaModel))
                {
                    result = 1;
                    if (type == 0)
                    {
                        msg = "主会场添加成功！";
                        venueService.AddOperationLog(meetingNo, 1, 1, LogActionType.Add, "添加主会场");
                    }
                    else if (type == 1)
                    {
                        msg = "分会场添加成功！";
                        venueService.AddOperationLog(meetingNo, 2, 1, LogActionType.Add, "添加分会场");
                    }

                }


            }
            return Json(new { result = result, message = msg }, "text/plain", Encoding.UTF8);
        }//AddProduct

        /// <summary>
        /// 生成会场编号
        /// </summary>
        /// <returns></returns>
        private string CreateMeetNo()
        {
            string meetingNo = (DateTime.Now.Year.ToString().Substring(3, 1) + "-" + DateTime.Now.ToString("MM-dd")).Replace("-", "");
            CommonService cs = new CommonService();
            string code = cs.GetNextCounterId("MeetingNO").ToString("00000");
            meetingNo += code.Substring(code.Length - 5, 5);

            #region 判断会场编号是否存在
            if (venueService.CheckMeetingNoIsExist(meetingNo))
            {
                //如果存在进行递归生成判断
                CreateMeetNo();
            }
            #endregion

            return meetingNo;
        }//CreateMeetNo

        /// <summary>
        /// 修改会场
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult EditMeeting(SWfsMeetingInfo model, SWfsMeetingMetaInfo metaModel, int type, int webOrMobileType = 0, string hidMobileShowImg = "")
        {

            int result = 0;
            string msg = string.Empty;
            string domain = Request.Form["hidMainDomainName"] != null ? Request.Form["hidMainDomainName"] : string.Empty;
            if (model.MeetingDomain != null)
            {

                Regex reg = new Regex(@"^[0-9a-zA-Z]{1,15}$");
                if (reg.IsMatch(model.MeetingDomain))
                {
                    model.MeetingDomain = model.MeetingDomain;
                }
                else
                {
                    return Json(new { result = "-1", message = "会场域名不能为空且域名只能为数字或字母!" }, "text/plain", Encoding.UTF8);
                }
            }

            if (type == 1 && !string.IsNullOrEmpty(domain))
            {
                if (model.SourceFrom == 2)
                {
                    if (domain.Length - (domain.LastIndexOf("/") + 1) > 0)
                    {
                        model.MeetingDomain = (domain.Substring(domain.LastIndexOf("/") + 1, domain.Length - (domain.LastIndexOf("/") + 1)) + "-" + model.MeetingDomain.Trim()).ToLower().Replace("//", "/");
                    }
                    else
                    {
                        model.MeetingDomain = model.MeetingDomain.Trim().ToLower().Replace("//", "/");
                    }
                }
                else if (model.SourceFrom == 1)
                {
                    model.MeetingDomain = (domain + "-" + model.MeetingDomain.Trim()).ToLower().Replace("//", "/");
                }
            }
            if (type == 0 && model.SourceFrom > 0)
            {
                if (model.SourceFrom == 1)
                {
                    //model.MeetingDomain = Shangpinkey + model.MeetingDomain.Trim().ToLower().Replace("//", "/");
                    model.MeetingDomain = model.MeetingDomain.Trim().ToLower().Replace("//", "/");
                }
                else if (model.SourceFrom == 2)
                {
                    model.MeetingDomain = model.MeetingDomain.Trim().ToLower().Replace("//", "/");
                }
            }
            if (type == 1 && model.SourceFrom > 0)
            {
                if (model.MeetingID > 0)
                {
                    SWfsMeetingInfo modle = venueService.GetVenueByID(model.MeetingID.ToString());
                    if (model.SourceFrom == 1)
                    {
                        if (modle.MeetingDomain.LastIndexOf("/") > 0)
                        {
                            model.MeetingDomain = (modle.MeetingDomain.Substring(0,
                               modle.MeetingDomain.LastIndexOf("/")) + "-" + model.MeetingDomain).Replace(Shangpinkey, "");
                        }
                        else if (modle.MeetingDomain.LastIndexOf("-") > 0)
                        {
                            model.MeetingDomain = (modle.MeetingDomain.Substring(0,
                               modle.MeetingDomain.LastIndexOf("-")) + "-" + model.MeetingDomain).Replace("//", "/");
                        }
                    }
                    else if (model.SourceFrom == 2)
                    {
                        model.MeetingDomain = (modle.MeetingDomain.Substring(0,
                           modle.MeetingDomain.LastIndexOf("-")) + "-" + model.MeetingDomain).Replace("//", "/");

                    }
                }

            }
            if (!string.IsNullOrEmpty(model.MeetingDomain.Trim()))
            {
                bool isExist = venueService.CheckMeetingDomainIsExist(model.MeetingID, model.MeetingDomain);
                if (isExist)
                {
                    return Json(new { result = "-1", message = "会场域名已经存在！" }, "text/plain", Encoding.UTF8);
                }
            }
            model.WebOrMobile = (short)webOrMobileType;
            if (new DateTime(model.PreViewTime.Year, model.PreViewTime.Month, model.PreViewTime.Day, model.PreViewTime.Hour, model.PreViewTime.Minute, 0) >= new DateTime(model.StartTime.Year, model.StartTime.Month, model.StartTime.Day, model.StartTime.Hour, model.StartTime.Minute, 0))
            {
                return Json(new { result = "-1", message = "预热日期不能大于或等于会场开始日期！" }, "text/plain", Encoding.UTF8);
            }
            if (model.WebOrMobile == 1 && string.IsNullOrEmpty(model.WebStartNO) && string.IsNullOrEmpty(model.WebPreViewNO))
            {
                return Json(new { result = "-1", message = "请选择会场网页模板" }, "text/plain", Encoding.UTF8);
            }

            if (model.WebOrMobile == 2 && string.IsNullOrEmpty(model.MobileStartNO) && string.IsNullOrEmpty(model.MobilePreViewNO))
            {
                return Json(new { result = "-1", message = "请选择会场移动端模板" }, "text/plain", Encoding.UTF8);
            }
            if (model.WebOrMobile == 2 || model.WebOrMobile == 3)
            {
                if (null != Request.Files["MobileShowImg"] && !string.IsNullOrEmpty(Request.Files["MobileShowImg"].FileName))
                {
                    string key = "MeetingMobilePic";
                    string PicSize = AppSettingManager.AppSettings[key].ToString();
                    Dictionary<string, string> flapPic = new CommonService().PostImg(Request.Files["MobileShowImg"], PicSize);
                    string flapPicNo = flapPic.Values.FirstOrDefault();
                    string flapPicNoKey = flapPic.Keys.FirstOrDefault();
                    if (!String.IsNullOrEmpty(flapPicNo))
                    {
                        if (flapPicNoKey != "error")
                        {
                            model.MobileShowImg = flapPicNo;
                        }
                        else
                        {
                            return Json(new { result = "-1", message = flapPicNo }, "text/plain", Encoding.UTF8);
                        }
                    }
                    else { model.MobileShowImg = ""; }

                }
                else if (hidMobileShowImg == "")
                {
                    return Json(new { result = "-1", message = "请选择移动端图片!" }, "text/plain", Encoding.UTF8);
                }
                else
                {
                    model.MobileShowImg = hidMobileShowImg;
                }
            }
            if (null != Request.Form["Keywords"])
            {
                metaModel.KeyWords = Convert.ToString(Request.Form["Keywords"]).Trim();
            }
            if (null != Request.Form["Description"])
            {
                metaModel.Description = Convert.ToString(Request.Form["Description"]).Trim();
            }
            SWfsMeetingInfo meeting = venueService.GetVenueByID(model.MeetingID.ToString());
            model.WebPreViewCode = meeting.WebPreViewCode;
            model.WebStartCode = meeting.WebStartCode;
            model.MobilePreViewCode = meeting.MobilePreViewCode;
            model.MobileStartCode = meeting.MobileStartCode;
            string oldWebStartNO = Request.Form["oldWebStartNO"];
            string oldWebPreNO = Request.Form["oldWebPreViewNO"];
            string oldMobileStartNO = Request.Form["oldMobileStartNO"];
            string oldMobilePreNO = Request.Form["oldMobilePreViewNO"];
            if (oldWebStartNO != model.WebStartNO)
            {
                model.WebStartCode = "";
            }
            if (oldWebPreNO != model.WebPreViewNO)
            {
                model.WebPreViewCode = "";
            }
            if (oldMobileStartNO != model.MobileStartNO)
            {
                model.MobileStartCode = "";
            }
            if (oldMobilePreNO != model.MobilePreViewNO)
            {
                model.MobilePreViewCode = "";
            }
            try
            {
                if (venueService.EditMeeting(model))
                {
                    if (model.MeetingID == metaModel.MeetingID)
                    {
                        SWfsMeetingMetaInfo NewmetaModel = new SWfsMeetingMetaInfo();
                        NewmetaModel = venueService.GetInfoByMeetingID(metaModel.MeetingID.ToString());
                        if (NewmetaModel != null)
                        {
                            metaModel.ID = NewmetaModel.ID;
                            metaModel.Title = model.MeetingName;
                            if (venueService.EditMateMeeting(metaModel))
                            {
                                result = 1;
                                if (type == 0)
                                {
                                    msg = "主会场编辑成功！";
                                    ClearVenueCach(model.MeetingID);
                                    venueService.AddOperationLog(model.MeetingNO, 1, 1, LogActionType.Edit, "修改主会场");
                                }
                                else if (type == 1)
                                {
                                    msg = "分会场编辑成功！";
                                    ClearVenueCach(model.MeetingID);
                                    venueService.AddOperationLog(model.MeetingNO, 2, 1, LogActionType.Edit, "修改分会场");
                                }
                            }
                        }
                        else
                        {
                            metaModel.ID = model.MeetingID;
                            metaModel.Title = model.MeetingName;
                            if (venueService.AddMateMeeting(metaModel))
                            {
                                result = 1;
                                if (type == 0)
                                {
                                    msg = "主会场编辑成功！";
                                    ClearVenueCach(model.MeetingID);
                                    venueService.AddOperationLog(model.MeetingNO, 1, 1, LogActionType.Edit, "修改主会场");
                                }
                                else if (type == 1)
                                {
                                    msg = "分会场编辑成功！";
                                    ClearVenueCach(model.MeetingID);
                                    venueService.AddOperationLog(model.MeetingNO, 2, 1, LogActionType.Edit, "修改分会场");
                                }
                            }
                        }



                    }

                }
            }
            catch (Exception ex)
            {
                return Json(new { result = result, message = ex.Message }, "text/plain", Encoding.UTF8);
            }

            return Json(new { result = result, message = msg }, "text/plain", Encoding.UTF8);
        }//EditMeeting

        /// <summary>
        /// 主会场列表
        /// </summary>
        /// <returns></returns>
        public ActionResult MeetListManage(string meetingId = "", string meetingNameOrDomain = "", string topicSubjectType = "", string activityNo = "", int pageIndex = 1, string status = "", string webOrMobile = "", string startTime = "", string endTime = "", string SourceFrom = "")
        {
            int pageSize = 20;
            int recountCount = 0;
            bool isSelect = false; //false主会场，true分会场
            meetingNameOrDomain = string.IsNullOrEmpty(meetingNameOrDomain) || meetingNameOrDomain.Trim() == "会场名称/域名" ? "" : meetingNameOrDomain.Trim();
            activityNo = string.IsNullOrEmpty(activityNo) || activityNo.Trim() == "专题活动编号" ? "" : activityNo.Trim();
            ViewBag.MeetingNameOrDomain = meetingNameOrDomain;
            ViewBag.TopicSubjectType = topicSubjectType;
            ViewBag.ActiveNO = activityNo;
            ViewBag.Status = status;
            ViewBag.WebOrMobile = webOrMobile;
            ViewBag.StartTime = startTime;
            ViewBag.EndTime = endTime;
            ViewBag.SourceFrom = SourceFrom;

            IList<SWfsMeetingHtmlInfoList> list = venueService.GetMeetingDataList(meetingId, isSelect, meetingNameOrDomain, topicSubjectType, activityNo, status, webOrMobile, SourceFrom, startTime, endTime, pageIndex, pageSize, out recountCount);
            //IList<SWfsMeetingInfoHtml> ListHtml = venueService.GetMeetingInfoHtmlList();
            foreach (var item in list)
            {

                if (item.SourceFrom == 2)
                {
                    if (item.MeetingDomain.Contains(key + "/"))
                    {
                        item.MeetingDomain = key + (item.MeetingDomain.Replace(key, "")).Replace("/", ""); ;
                    }
                    else
                    {
                        item.MeetingDomain = key + (item.MeetingDomain.Replace(key, "")).Replace("//", "/");
                    }
                }
                else if (item.SourceFrom == 1)
                {

                    if (item.MeetingDomain.Contains(Shangpinkey + "/"))
                    {
                        item.MeetingDomain = item.MeetingDomain.Replace("//", "/");
                    }
                    else
                    {
                        item.MeetingDomain = Shangpinkey + (item.MeetingDomain.Replace(Shangpinkey, "").Replace("//", "/"));
                    }

                }

            }
            ViewBag.pageIndex = pageIndex;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = recountCount;
            return View(list);
        }//MeetListManage

        /// <summary>
        /// 分会场列表
        /// </summary>
        /// <returns></returns>
        public ActionResult ChildMeetListManage(int meetingId = 0, string meetingNameOrDomain = "", string topicSubjectType = "", string activityNo = "", int pageIndex = 1, string status = "", string webOrMobile = "", string SourceFrom = "", string startTime = "", string endTime = "", string MeetingName = "", int parentId = 0)
        {
            int pageSize = 20;
            int recountCount = 0;
            bool isSelect = true; //false主会场，true分会场
            meetingNameOrDomain = string.IsNullOrEmpty(meetingNameOrDomain) || meetingNameOrDomain.Trim().Equals("会场名称/域名") ? "" : meetingNameOrDomain.Trim();
            activityNo = string.IsNullOrEmpty(activityNo) || activityNo.Trim().Equals("专题活动编号") ? "" : activityNo.Trim();
            if (parentId > 0)
            {
                ViewBag.MID = parentId;
            }
            else if (meetingId > 0)
            {
                ViewBag.MID = meetingId;
            }
            ViewBag.MeetingNameOrDomain = meetingNameOrDomain;
            ViewBag.TopicSubjectType = topicSubjectType;
            ViewBag.ActiveNO = activityNo;
            ViewBag.Status = status;
            ViewBag.WebOrMobile = webOrMobile;
            ViewBag.StartTime = startTime;
            ViewBag.EndTime = endTime;
            ViewBag.MeetingName = MeetingName;
            ViewBag.SourceFrom = SourceFrom;
            IList<SWfsMeetingHtmlInfoList> list = venueService.GetMeetingDataList((meetingId > 0 ? meetingId.ToString() : ""), isSelect, meetingNameOrDomain, topicSubjectType, activityNo, status, webOrMobile, SourceFrom, startTime, endTime, pageIndex, pageSize, out recountCount);
            foreach (var item in list)
            {
                if (item.ParentID > 0)
                {
                    if (item.SourceFrom == 2)
                    {
                        item.MeetingDomain = key + (item.MeetingDomain.Replace(key, "")).Replace("//", "/");
                    }
                    else if (item.SourceFrom == 1)
                    {
                        item.MeetingDomain = Shangpinkey + (item.MeetingDomain.Replace(Shangpinkey, "").Replace("//", "/"));
                    }

                }
            }
            ViewBag.pageIndex = pageIndex;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = recountCount;
            return View(list);
        }//ChildMeetListManage

        /// <summary>
        /// 更新会场状态
        /// </summary>
        /// <param name="status">0关闭，1开启</param>
        /// <param name="mId">会场ID</param>
        /// <param name="mNo">会场编号</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UpdateStatus(int status, int mId, string mNo)
        {
            bool flag = venueService.UpdateMeetingStatus(mId.ToString(), status);
            if (flag)
            {
                string message = status == 1 ? "开启成功" : "关闭成功";
                venueService.AddOperationLog(mNo, 1, 0, LogActionType.Add, "会场编号（" + mNo + "）" + message);
                return Json(new { result = flag, message = message });
            }
            venueService.AddOperationLog(mNo, 1, 0, LogActionType.Add, "会场编号（" + mNo + "）更新失败");
            return Json(new { result = false, message = "更新失败！" });
        }//UpdateStatus

        /// <summary>
        /// 选择会场模板
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="flag">1网页，2移动端</param>
        /// <param name="status">0进行中，1预热中</param>
        /// <returns></returns>
        public ActionResult SelectMeetingTemplate(string pageIndex = "1", string flag = "", string status = "")
        {
            int totalCount = 0;
            IEnumerable<SWfsMeetingTemplateInfo> list = venueService.GetTemplateList(pageIndex, 5, "", "", flag, status, out totalCount);
            ViewBag.totalCount = totalCount;
            return View(list);
        }//SelectMeetingTemplate

        /// <summary>
        /// 添加会场专题活动
        /// </summary>
        /// <param name="meetNo">主会场编号</param>
        /// <param name="childMeetId">分会场编号</param>
        /// <param name="keyword">关键词，专题活动名称或编号</param>
        /// <param name="status">状态 0关闭，1开启</param>
        /// <param name="channelNo">渠道</param>
        /// <param name="topicSubjectType">类型 0专题，1活动</param>
        /// <param name="webSiteType">所属网站 0尚品，1奥莱</param>
        /// <param name="startTime">专题活动开始时间</param>
        /// <param name="endTime">专题活动结束时间</param>
        /// <param name="pageindex">当前页</param>
        /// <returns></returns>
        public ActionResult AddSubjectTopic(string meetNo, string childMeetId = "0", string keyword = "", string status = "", string channelNo = "", int topicSubjectType = 1, string webSiteType = "0", string startTime = "", string endTime = "", int pageindex = 1)
        {
            int pageSize = 20;
            int readCount = 0;
            keyword = keyword.Trim().Equals("专题活动名称/编号") ? "" : keyword.Trim();
            ViewBag.MainMeetNo = meetNo;
            ViewBag.ChildMeetNo = childMeetId;
            ViewBag.TopicSubjectType = topicSubjectType;
            ViewBag.KeyWord = keyword;
            ViewBag.Status = status;
            ViewBag.WebSiteType = webSiteType;
            ViewBag.StartTime = startTime;
            ViewBag.EndTime = endTime;
            ViewBag.BindList = venueService.SelectBrandList(); //获取品牌
            ViewBag.TypeName = topicSubjectType == 1 ? "活动" : "专题";
            ViewBag.ListName = string.Empty;
            RecordPage<SubjectInfo> subjectlist = null; //奥莱活动列表
            IList<SWfsNewSubject> newsubjectlist = null; //尚品活动列表
            IList<SWfsTopics> topiclist = null;
            IList<SWfsMeetingActiveSpecial> masList = null;
            if (!string.IsNullOrEmpty(meetNo) && !string.IsNullOrEmpty(childMeetId))
            {
                masList = venueService.GetMeetingActiveSpecialList(meetNo, Convert.ToInt32(childMeetId), topicSubjectType.ToString());
            }
            if (topicSubjectType == 1) //活动
            {
                if (!string.IsNullOrEmpty(webSiteType) && webSiteType == "1") //奥莱
                {
                    ViewBag.ListName = "奥莱";
                    SWfsSubjectService subject = new SWfsSubjectService();
                    subjectlist = subject.SelectSubjectList(keyword, "", status, "", channelNo, startTime, endTime, pageindex, pageSize);
                    ViewBag.pageIndex = subjectlist.CurrentPage;
                    ViewBag.TotalCount = subjectlist.TotalItems;
                }
                else if (!string.IsNullOrEmpty(webSiteType) && webSiteType == "0") //尚品
                {
                    ViewBag.ListName = "尚品";
                    newsubjectlist = venueService.GetSelectNewSubjectList(keyword, "", status, "", channelNo, startTime, endTime, pageindex, pageSize, out readCount);
                    ViewBag.pageIndex = pageindex;
                    ViewBag.TotalCount = readCount;
                }
            }
            else if (topicSubjectType == 0) //专题
            {
                SWfsTopicService service = new SWfsTopicService();
                topiclist = service.GetTopicList(keyword.Trim(), "", status, "", pageindex, pageSize, out readCount);
                ViewBag.pageIndex = pageindex;
                ViewBag.TotalCount = readCount;
            }
            ViewBag.PageSize = pageSize;
            ViewBag.SubjectList = subjectlist; //奥莱活动列表
            ViewBag.NewSubjectList = newsubjectlist; //尚品活动列表
            ViewBag.TopicList = topiclist; //专题列表
            ViewBag.MeetingSubjectTopicList = masList; //当前会场已有专题活动数据列表
            return View();
        }//AddSubjectTopic

        /// <summary>
        /// 根据会场编号获取专题活动列表
        /// </summary>
        /// <param name="mainMeetNo">主会场编号</param>
        /// <param name="meetNo">分会场编号</param>
        /// <param name="subjectTopicName">专题活动名称</param>
        /// <param name="activityTopicNo">专题活动编号</param>
        /// <param name="status">专题活动状态</param>
        /// <param name="webSiteType">所属网站，默认为奥莱1</param>
        /// <param name="channelNo">展示渠道</param>
        /// <param name="topicSubjectType">类型 0专题 1活动，默认为活动</param>
        /// <param name="startTime">专题活动开始日期</param>
        /// <param name="endTime">专题活动结束日期</param>
        /// <param name="pageIndex">当前页</param>
        /// <returns></returns>
        public ActionResult SubjectTopicList(string mainMeetNo = "", string meetNo = "0", string subjectTopicName = "", string activityTopicNo = "", string status = "", string webSiteType = "1", string channelNo = "", string topicSubjectType = "1", string startTime = "", string endTime = "", int pageIndex = 1)
        {
            int pageSize = 20;
            int recountCount = 0;
            ViewBag.MainMeetNo = mainMeetNo;
            ViewBag.ChildMeettNo = meetNo;
            int meetingType = (!string.IsNullOrEmpty(mainMeetNo) && !string.IsNullOrEmpty(meetNo) && long.Parse(meetNo) > 0) ? 1 : 0; //0主会场，1分会场
            subjectTopicName = string.IsNullOrEmpty(subjectTopicName) || subjectTopicName.Trim().Equals("专题活动名称") ? "" : subjectTopicName.Trim();
            activityTopicNo = string.IsNullOrEmpty(activityTopicNo) || activityTopicNo.Trim().Equals("专题活动编号") ? "" : activityTopicNo.Trim();
            ViewBag.SubjectTopicName = subjectTopicName;
            ViewBag.ActivityTopicNo = activityTopicNo;
            ViewBag.Status = status;
            ViewBag.WebSiteType = webSiteType;
            if (webSiteType == "0" && !string.IsNullOrEmpty(channelNo))
            {
                channelNo = (channelNo == "zsqd001" ? "1" : "2");
            }
            ViewBag.ChannelNo = channelNo;
            ViewBag.TopicSubjectType = topicSubjectType;
            ViewBag.StartTime = startTime;
            ViewBag.EndTime = endTime;
            ViewBag.TypeName = topicSubjectType.Equals("1") ? "活动" : "专题";
            ViewBag.ListName = webSiteType == "1" ? "奥莱" : "尚品";
            IList<SWfsMeetingSubjectTopicM> list = venueService.GetMeetingSubjectTopicDataList(mainMeetNo, meetNo, meetingType, subjectTopicName, activityTopicNo, status, webSiteType, channelNo, topicSubjectType, startTime, endTime, pageIndex, pageSize, out recountCount);
            ViewBag.pageIndex = pageIndex;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = recountCount;
            ViewBag.BindList = venueService.SelectBrandList(); //获取品牌
            return View(list);
        }//SubjectTopicList

        /// <summary>
        /// 删除会场专题活动
        /// </summary>
        /// <param name="type">0单个删除，1批量删除</param>
        /// <param name="aId">专题活动关联ID</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DelSubjectTopicRelation(int type, string aId)
        {
            bool flag = true;
            string[] arrayId = aId.Split(',');
            if (arrayId.Length > 0)
            {
                for (int i = 0; i < arrayId.Length; i++)
                {
                    if (!string.IsNullOrEmpty(arrayId[i].Trim()))
                    {
                        if (venueService.DelMeetSubjectTopicRelation(arrayId[i]) <= 0)
                        {
                            flag = false;
                        }
                    }
                }
            }
            return Json(new { result = flag, message = flag ? "移除成功！" : "称除失败！" });
        }//DelSubjectTopicRelation

        /// <summary>
        /// 会场关联专题活动
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RelationSubjectTopic()
        {
            IList<SWfsMeetingActiveSpecial> list = new List<SWfsMeetingActiveSpecial>();
            string MainMeetingNO = Request.Form["MainMeetingNO"]; //主会场编号
            string MeetingNO = Request.Form["MeetingNO"]; //分主场编号，默认为0
            int IsActive = Convert.ToInt32(Request.Form["IsActive"]); //1活动，0专题
            int webSiteType = Convert.ToInt32(Request.Form["webSiteType"]); //0尚品，1奥莱
            string name = (IsActive == 1 ? "活动" : "专题");

            string[] subjectNo = ConvertParams(Request.Form["hidCheckId"]); //活动专题编号
            string[] subjectTopicName = ConvertParams(Request.Form["SubjectTopicName"]); //活动专题名称
            string[] activityUrl = ConvertParams(Request.Form["activityUrl"]); //活动地址
            string[] brandUrl = ConvertParams(Request.Form["brandUrl"]); //品牌地址
            string[] brandStatus = ConvertParams(Request.Form["brandStatus"]);  //品牌编号
            DateTime dt = DateTime.Now;
            bool result = true;
            if (subjectNo != null && subjectNo.Length > 0)
            {
                for (int i = 0; i < subjectNo.Length; i++)
                {
                    if (!string.IsNullOrEmpty(subjectNo[i]) && long.Parse(subjectNo[i]) > 0)
                    {
                        SWfsMeetingActiveSpecial model = new SWfsMeetingActiveSpecial();
                        model.ActiveNO = subjectNo[i];
                        model.MainMeetingNO = MainMeetingNO;
                        model.MeetingNO = MeetingNO;
                        model.IsActive = (short)IsActive;
                        if (subjectTopicName != null)
                        {
                            if (subjectTopicName.Length >= i - 1)
                            {
                                model.ActiveName = subjectTopicName[i];
                            }
                        }

                        if (string.IsNullOrEmpty(model.ActiveName))
                        {
                            model.ActiveName = "";
                        }

                        if (activityUrl != null)
                        {
                            if (activityUrl.Length >= i - 1)
                            {
                                model.ActiveLink = activityUrl[i];
                            }
                        }

                        if (string.IsNullOrEmpty(model.ActiveLink))
                        {
                            model.ActiveLink = "";
                        }

                        if (brandUrl != null)
                        {
                            if (brandUrl.Length >= -1)
                            {
                                model.BrandLink = brandUrl[i];
                            }
                        }

                        if (string.IsNullOrEmpty(model.BrandLink))
                        {
                            model.BrandLink = "";
                        }
                        if (brandStatus != null)
                        {
                            if (brandStatus.Length >= i - 1)
                            {
                                model.BrandNO = brandStatus[i];
                            }
                        }

                        if (string.IsNullOrEmpty(model.BrandNO))
                        {
                            model.BrandNO = "";
                        }
                        model.CreateTime = dt;
                        model.ImgNo = "";
                        model.IsSelect = false;
                        if (IsActive == 1)
                        {
                            model.WebSite = (short)(webSiteType == 0 ? 1 : 2);  //1尚品 2奥莱
                        }
                        else
                        {
                            model.WebSite = 0;
                        }

                        int flag = venueService.AddMeetSubjectTopicRelation(model);
                        if (flag <= 0)
                        {
                            result = false;
                            venueService.AddOperationLog(MainMeetingNO, 1, (IsActive == 1 ? 1 : 2), LogActionType.Add, name + "编号：" + subjectNo[i] + "修改失败");
                        }
                        else
                        {
                            venueService.AddOperationLog(MainMeetingNO, 1, (IsActive == 1 ? 1 : 2), LogActionType.Add, name + "编号：" + subjectNo[i]);
                        }
                    }
                }
                return Json(new { result = result, message = result ? "添加" + name + "成功" : "添加" + name + "失败！" });
            }
            else
            {
                return Json(new { result = false, message = "请选择需要添加的" + name + "！" });
            }
        }//RelationSubjectTopic

        /// <summary>
        /// 修改会场专题活动关联信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateRelationSubjectTopic()
        {
            IList<SWfsMeetingActiveSpecial> list = new List<SWfsMeetingActiveSpecial>();
            int IsActive = Convert.ToInt32(Request.Form["IsActive"]); //1活动，0专题
            string name = (IsActive == 1 ? "活动" : "专题");
            string MainMeetingNO = Request.Form["MainMeetingNO"]; //主会场编号
            string[] subjectNo = ConvertParams(Request.Form["hidCheckId"]); //活动专题编号
            string[] activityUrl = ConvertParams(Request.Form["activityUrl"]); //活动地址
            string[] brandUrl = ConvertParams(Request.Form["brandUrl"]); //品牌地址
            string[] brandStatus = ConvertParams(Request.Form["brandStatus"]);  //品牌编号
            string[] stNo = ConvertParams(Request.Form["hidSTNo"]); //专题活动编号
            DateTime dt = DateTime.Now;
            bool result = true;
            if (subjectNo != null && subjectNo.Length > 0)
            {
                for (int i = 0; i < subjectNo.Length; i++)
                {
                    if (!string.IsNullOrEmpty(subjectNo[i]) && long.Parse(subjectNo[i]) > 0)
                    {
                        SWfsMeetingActiveSpecial model = new SWfsMeetingActiveSpecial();
                        model.ActiveID = int.Parse(subjectNo[i]);
                        model.ActiveLink = activityUrl[i];
                        model.BrandLink = brandUrl[i];
                        model.BrandNO = brandStatus[i];
                        bool flag = venueService.EditMeetingSubjectTopicRelation(model);
                        if (!flag)
                        {
                            result = false;
                            venueService.AddOperationLog(MainMeetingNO, 1, (IsActive == 1 ? 1 : 2), LogActionType.Edit, name + "编号：" + stNo[i] + "修改失败");
                        }
                        else
                        {
                            venueService.AddOperationLog(MainMeetingNO, 1, (IsActive == 1 ? 1 : 2), LogActionType.Edit, name + "编号：" + stNo[i]);
                        }
                    }
                }
                return Json(new { result = result, message = result ? "修改" + name + "成功！" : "修改" + name + "失败！" });
            }
            else
            {
                return Json(new { result = false, message = "请选择需要修改的" + name + "！" });
            }
        }//UpdateRelationSubjectTopic

        /// <summary>
        /// 转换为数组
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        private string[] ConvertParams(string val)
        {
            string[] arraylist = null;
            if (!string.IsNullOrEmpty(val))
            {
                arraylist = val.Split(',').ToArray();

            }
            return arraylist;
        }//ConvertParams

        /// <summary>
        /// 切换会场展示类型
        /// </summary>
        /// <param name="venueID">会场ID</param>
        /// <param name="isPreview">0为预热会场1</param>
        /// <returns></returns>
        public ActionResult ChangeVenueType(string venueID, int isPreview)
        {
            venueService.UpdateVenueIsSelectByIDList(venueID, isPreview == 1);
            return Redirect("MeetListManage");
        }

        public string ClearVenueCach(int id)
        {
            try
            {
                EnyimMemcachedClient.Instance.Remove("VenueAll_SWfsMeetingInfo_" + id);
                MemcachedProvider.Instance.Delete("VenueAll_SWfsMeetingInfo_" + id);
                RedisCacheProvider.Instance.Remove("ShangPin_MeetingInfo_" + id);
                RedisCacheProvider.Instance.Remove("AoLai_MeetingInfo_" + id);

                return "清除成功";
            }
            catch (Exception ex)
            {

                return "清除异常" + ex.Message;
            }
        }
        #endregion

        #region 操作日志管理 by zhangwei 20130922
        /// <summary>
        /// 根据会场编号获取会场日志
        /// </summary>
        /// <returns></returns>
        public ActionResult LogManageList(string meetNo, string mId = "", int meetType = 1, int pageIndex = 1)
        {
            int pageSize = 20;
            int recountCount = 0;
            string valStr = string.IsNullOrEmpty(Request["keyword"]) || Request["keyword"].Trim().Equals("请输入关键词") ? "" : Request["keyword"].Trim();
            string startTime = Request["startTime"] != null && !string.IsNullOrEmpty(Request["startTime"]) ? Request["startTime"] : "";
            string endTime = Request["endTime"] != null && !string.IsNullOrEmpty(Request["endTime"]) ? Request["endTime"] : "";
            int actionType = GetLogActionTypeKey(valStr);
            ViewBag.MeetType = meetType;
            ViewBag.MID = mId;
            ViewBag.MainMeetNo = meetNo;
            ViewBag.KeyWord = valStr;
            ViewBag.StartTime = startTime;
            ViewBag.EndTime = endTime;
            IList<SWfsMeetingOperationDiary> list = venueService.GetMeetingOperationLogDataList(meetNo, valStr, actionType, startTime, endTime, pageIndex, pageSize, out recountCount);
            ViewBag.pageIndex = pageIndex;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = recountCount;
            return View(list);
        }//LogManageList

        /// <summary>
        /// 查询日志动作类型
        /// </summary>
        /// <param name="keywork">关键词</param>
        /// <returns></returns>
        private int GetLogActionTypeKey(string keywork)
        {
            if (!string.IsNullOrEmpty(keywork))
            {
                if (LogActionType.Add.GetDescription().Contains(keywork))
                {
                    return LogActionType.Add.GetHashCode();
                }
                else if (LogActionType.Edit.GetDescription().Contains(keywork))
                {
                    return LogActionType.Edit.GetHashCode();
                }
                else if (LogActionType.Delete.GetDescription().Contains(keywork))
                {
                    return LogActionType.Delete.GetHashCode();
                }
            }
            return 0;
        }//GetLogActionTypeKey
        #endregion

        #region 秒杀
        public ActionResult SkillManager(int meetingId, int pageIndex = 1)
        {
            int pageSize = 15;
            int count = 0;
            var list = venueService.GetSwfsSkillGroupList(meetingId, pageIndex, pageSize, out count);
            ViewBag.MeetingId = meetingId;
            ViewBag.PageIndex = pageIndex;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = count;
            return View(list);
        }

        public ActionResult SkillGroupManager(int meetingId, int groupId)
        {
            if (groupId == 0)
            {
                ViewBag.ShowTitle = "添加促销商品组";
                return View(new SwfsSkillGroup() { MeetingID = meetingId });

            }
            else
            {
                ViewBag.ShowTitle = "编辑促销商品组";
                return View(venueService.GetSwfsSkillGroupById(groupId));
            }
        }
        [HttpPost]
        public ActionResult SkillGroupManager(SwfsSkillGroup model)
        {
            Passport passport = PresentationHelper.GetPassport();

            if (model.Id > 0)
            {
                var skillgroup = venueService.GetSwfsSkillGroupById(model.Id);
                model.CreateUser = skillgroup.CreateUser;
                model.CreateTime = skillgroup.CreateTime;
                model.ModifyTime = DateTime.Now;
                if (passport != null)
                    model.ModifyUser = passport.UserName;
            }
            else
            {
                if (passport != null)
                    model.CreateUser = model.ModifyUser = passport.UserName; ;
                model.CreateTime = model.ModifyTime = DateTime.Now;
            }


            return Json(new { result = venueService.SaveSkillGroup(model) });

        }

        [HttpPost]
        public ActionResult DeleteSkillGroup(int groupId)
        {
            return Json(new { result = venueService.DeleteSkillGroup(groupId) });
        }
        #endregion

        #region 秒杀产品
        //加载秒杀商品列表
        public ActionResult AboutSkillProductList(int skillGroupID, int pageIndex = 1, int pageSize = 10)
        {
            int totalCount = 0;
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
            IList<ProductInfo> list = venueService.GetProductList(ViewBag.Gender, ViewBag.category, ViewBag.keyWord, pageIndex, pageSize, out totalCount);
            //查询添加过的商品
            IEnumerable<SkillProductExtends> oldidlist = venueService.GetSwfsSkillProductList(skillGroupID);
            if (oldidlist != null)
            {
                if (oldidlist.Count() > 0)
                {
                    foreach (var item in oldidlist)
                    {
                        if (list.Count(p => p.ProductNo == item.ProductNo) > 0)
                        {
                            list.Where(p => p.ProductNo == item.ProductNo).First().Status = -2;
                        }
                    }
                }
            }
            ViewBag.totalCount = totalCount;
            return View(list);
        }
        //查询秒杀商品
        public ActionResult SkillProductList(int skillGroupID)
        {
            return View(venueService.GetSwfsSkillProductList(skillGroupID));
        }
        //上传图片
        [HttpPost]
        public string UpSkillProductImg()
        {
            string imgNO = null;
            if (string.IsNullOrEmpty(Request.Form["skillId"]))
            {
                return "{\"status\" : 0, \"message\" : \"数据不存在\"}";
            }
            if (Request.Files["imgfile"] != null && Request.Files["imgfile"].ContentLength > 0)
            {
                imgNO = SaveImg(Request.Files["imgfile"], "width:0,Height:0,Length:1000");
                if (rsPic.Keys.Contains("error"))
                {
                    return "{\"status\" : 0, \"message\" : \"图片不合法\"}";
                }
                if (venueService.AddProductImg(int.Parse(Request.Form["skillId"]), imgNO) > 0)
                {
                    return "{\"status\" : 1, \"message\" : \"" + imgNO + "\"}";
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(Request.Form["imgNO"]))
                {
                    return "{\"status\" : 1, \"message\" : \"" + Request.Form["imgNO"] + "\"}";
                }
            }
            return "{\"status\" : 0, \"message\" : \"图片不合法\"}";
        }
        //删除秒杀产品
        public int DeleteSkillProduct(int id)
        {
            return venueService.DeleteSkillProduct(id);
        }
        //批量添加秒杀商品
        [HttpPost]
        public int AddSkillProductNOList(int groupid, string productNO)
        {
            return venueService.AddSkillProduct(groupid, productNO);
        }
        [HttpPost]
        public ActionResult AddSkilProduct(int skillgroupid)
        {
            venueService.AddSkillProduct(skillgroupid, Request.Form["productNO"]);
            return Redirect("/shangpin/venue/AboutSkillProductList?skillgroupid=" + skillgroupid + "&pageindex=" + Request.QueryString["pageindex"] + "&meetingId=" + Request.QueryString["meetingId"]);
        }
        //保存排序
        public int SaveSkillProductSort(int id, int sortValue)
        {
            return venueService.SaveSortSkillProduct(id, sortValue);
        }
        //批量删除秒杀产品
        [HttpPost]
        public ActionResult DeleteSkillProductByIdList()
        {
            venueService.DeleteSkillProductByIdList(Request.Form["objID"]);
            return Redirect("/shangpin/venue/SkillProductList?skillgroupid=" + Request.QueryString["skillgroupid"] + "&meetingId=" + Request.QueryString["meetingId"]);
        }
        #endregion

        #region 会场导航管理
        //会场一级列表
        public ActionResult MeetingNavList(int meetingid)
        {
            int total = 0;
            IEnumerable<SWfsMeetingNavigation> list = venueService.GetMeetingNavList("", Request.QueryString["status"], "", -1, 0, meetingid, 1, 30, out total);
            ViewBag.totalCount = total;
            return View(list);
        }
        //会场二级列表
        public ActionResult NavContentList(int meetingid, int parentid)
        {
            int total = 0;
            IEnumerable<SWfsMeetingNavigation> list = venueService.GetMeetingNavList("", Request.QueryString["status"], "", -1, parentid, meetingid, 1, 30, out total);
            ViewBag.totalCount = total;
            return View(list);
        }
        //会场三级列表
        public ActionResult NavThirdList(int meetingid, int parentid)
        {
            int total = 0;
            IEnumerable<SWfsMeetingNavigation> list = venueService.GetMeetingNavList(Request.QueryString["navname"], "", Request.QueryString["navlink"], !string.IsNullOrEmpty(Request.QueryString["navtype"]) ? int.Parse(Request.QueryString["navtype"]) : -1, parentid, meetingid, 1, 30, out total);
            ViewBag.totalCount = total;
            return View(list);
        }
        //按ID查询会场导航
        public ActionResult AddMeetingNav(int id = 0)
        {
            SWfsMeetingNavigation obj = null;
            if (id == 0)
            {
                obj = new SWfsMeetingNavigation();
                return View(obj);
            }
            obj = venueService.GetNavObj(id);
            if (obj == null)
            {
                obj = new SWfsMeetingNavigation();
            }
            return View(obj);
        }
        //添加编辑导航
        [HttpPost]
        public ActionResult AddMeetingNav(SWfsMeetingNavigation obj)
        {
            if (obj.MeetingID == 0)
            {
                ViewData["tip"] = new HtmlString("<script>alert('不存在关联的会场');</script>");
                return View(obj);
            }
            if (string.IsNullOrEmpty(obj.NavName))
            {
                ViewData["tip"] = new HtmlString("<script>alert('导航名称不能为空');</script>");
                return View(obj);
            }
            else
            {
                if (obj.NavName.Length > 30)
                {
                    ViewData["tip"] = new HtmlString("<script>alert('不能超过30个字');</script>");
                    return View(obj);
                }
            }

            if (string.IsNullOrEmpty(obj.NavLink))
            {
                obj.NavLink = "";
            }


            if (Request.Form["NavStatus"] == "0")
            {
                obj.NavStatus = false;
            }
            else
            {
                obj.NavStatus = true;
            }
            obj.DateCreate = DateTime.Now;
            if (venueService.AddMeetingNav(obj) > 0)
            {
                ViewData["tip"] = new HtmlString("<script>alert('操作成功');</script>");
            }
            else
            {
                ViewData["tip"] = new HtmlString("<script>alert('操作失败');</script>");
            }
            return View(obj);
        }
        //开启关闭
        public int UpdateNavStatus(int id, int status)
        {
            return venueService.UpdateStatus(id, status);
        }
        //修改排序
        public int UpdateOrder(int id, int order)
        {
            return venueService.UpdateOrder(id, order);
        }
        //删除一级导航
        public ActionResult DeleteFirst(int id)
        {
            venueService.DeleteFirstNav(id);
            return Redirect("/shangpin/venue/MeetingNavList?meetingid=" + Request.QueryString["meetingid"]);
        }
        //删除二级导航
        public ActionResult DeleteSecond(int id)
        {
            venueService.DeleteSecondNav(id);
            return Redirect("/shangpin/venue/NavContentList?parentid=" + Request.QueryString["parentid"] + "&meetingid=" + Request.QueryString["meetingid"]);
        }
        //删除三级导航
        public ActionResult DeleteThird(int id)
        {
            venueService.DeleteThirdNav(id);
            return Redirect("/shangpin/venue/NavThirdList?parentid=" + Request.QueryString["parentid"] + "&meetingid=" + Request.QueryString["meetingid"] + "&pp=" + Request.QueryString["pp"]);
        }
        //添加三级导航
        public string AddThirdNav(SWfsMeetingNavigation obj)
        {
            if (obj.ParentNO == 0)
            {
                return "{\"status\":" + 0 + ",\"message\":\"" + "不存在关联的父类" + "\"}";
            }
            if (obj.MeetingID == 0)
            {
                return "{\"status\":" + 0 + ",\"message\":\"" + "不存在关联的会场" + "\"}";
            }
            if (string.IsNullOrEmpty(obj.NavName))
            {
                return "{\"status\":" + 0 + ",\"message\":\"" + "导航名称不能为空" + "\"}";
            }
            else
            {
                if (obj.NavName.Length > 30)
                {
                    return "{\"status\":" + 0 + ",\"message\":\"" + "不能超过30个字" + "\"}";
                }
            }

            if (string.IsNullOrEmpty(obj.NavLink))
            {
                //return "{\"status\":" + 0 + ",\"message\":\"" + "链接不能为空" + "\"}";
                obj.NavLink = "";
            }


            obj.DateCreate = DateTime.Now;
            if (venueService.AddMeetingNav(obj) > 0)
            {
                return "{\"status\":" + 1 + ",\"message\":\"" + "操作成功" + "\"}";

            }
            else
            {
                return "{\"status\":" + 0 + ",\"message\":\"" + "操作失败" + "\"}";
            }
        }
        //按id获取会场导航对象
        public string GetNavObjByID(int id)
        {
            SWfsMeetingNavigation obj = null;
            obj = venueService.GetNavObj(id);
            if (obj == null)
            {
                return null;
            }
            JavaScriptSerializer json = new JavaScriptSerializer();
            return json.Serialize(obj);
        }
        //发布会场导航
        [HttpPost, ValidateInput(false)]
        public int PubListMeetingNav(int meetingid, string htmlcode)
        {
            return venueService.PublistMeetingNav(meetingid, htmlcode);
        }
        #endregion

        #region 多图片上传

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

        #region 图片上传部分视图页面

        /// <summary>
        /// 图片上传部分视图页面
        /// </summary>
        /// <param name="id"></param>
        /// <param name="act"></param>
        /// <returns></returns>
        public ActionResult PartUpoladPicPage(string tempID, string act, int pageIndex = 1, int pageSize = 100)
        {
            //异步刷新所需参数
            string tempTemplateID = Request["tempID"];
            string tempTemplateType = Request["type"];
            string tempImgDetail = Request["tempImgAllDetail"];//隐藏值

            if (tempID == "0")
                act = "create";
            else
                act = "edit";
            if (string.IsNullOrEmpty(tempTemplateType))
            {
                tempTemplateType = "0";
            }
            SwfsVenueService service = new SwfsVenueService();
            SWfsMeetingTemplateInfo model;
            List<string> piclist = new List<string>();
            switch (act)
            {
                case "edit"://修改
                    model = service.GetTemplateObjByID(tempID);
                    if (model != null)
                    {
                        IList<SWfsMeetingTemplatePicInfo> singleEntityList = new List<SWfsMeetingTemplatePicInfo>();
                        if (string.IsNullOrEmpty(tempTemplateType))
                        {
                            singleEntityList = venueService.GetTemplateImgEntity(model.MettingNo, -2);
                        }
                        else
                        {
                            singleEntityList = venueService.GetTemplateImgEntity(model.MettingNo, float.Parse(tempTemplateType));

                        }
                        foreach (SWfsMeetingTemplatePicInfo item in singleEntityList)
                        {
                            piclist.Add(item.PicNo);//图片编号
                        }
                    }
                    break;
                default:
                    model = new SWfsMeetingTemplateInfo();
                    break;
            }

            #region 读取隐藏域
            //当前方法的全局集合
            IList<SWfsMeetingTemplatePicInfo> _singleEntityList = new List<SWfsMeetingTemplatePicInfo>();
            if (!string.IsNullOrEmpty(tempImgDetail))
            {
                string[] tempAllDataDetail = tempImgDetail.Split(new char[] { '$' });
                if (tempAllDataDetail.Length > 1)       // 说明当前上传了多组图片信息
                {
                    for (int i = 0; i < tempAllDataDetail.Length; i++)
                    {
                        //分组图片信息组合值
                        string[] tempAllDetail = tempAllDataDetail[i].Split(new char[] { '#' });
                        if (tempAllDetail.Length == 3)   //判断当前组合值，如果分割是三个的话，说明正常上传
                        {
                            SWfsMeetingTemplatePicInfo picEntity = new SWfsMeetingTemplatePicInfo();
                            picEntity.PicNo = string.IsNullOrEmpty(tempAllDetail[0]) ? "" : tempAllDetail[0];
                            picEntity.FileName = string.IsNullOrEmpty(tempAllDetail[1]) ? "" : tempAllDetail[1];

                            //判断当前最后一个值不是数字的话，说明有误
                            short tempType = 0;
                            if (short.TryParse(tempAllDetail[2], out tempType))
                            {
                                picEntity.Type = tempType;
                                picEntity.TemplateID = tempTemplateID;
                                IList<SWfsMeetingTemplatePicInfo> isExistData = _singleEntityList.Where(c => c.FileName == picEntity.FileName && c.Type == picEntity.Type).ToList();
                                if (isExistData == null || isExistData.Count() == 0)
                                {
                                    _singleEntityList.Add(picEntity);
                                }
                            }
                        }
                    }
                }
                else      // 说明只有一组图片信息
                {
                    //分组图片信息组合值
                    string[] tempAllDetail = tempImgDetail.Split(new char[] { '#' });
                    if (tempAllDetail.Length == 3)   //判断当前组合值，如果分割是三个的话，说明正常上传
                    {
                        SWfsMeetingTemplatePicInfo picEntity = new SWfsMeetingTemplatePicInfo();
                        picEntity.PicNo = string.IsNullOrEmpty(tempAllDetail[0]) ? "" : tempAllDetail[0];
                        picEntity.FileName = string.IsNullOrEmpty(tempAllDetail[1]) ? "" : tempAllDetail[1];

                        //判断当前最后一个值不是数字的话，说明有误
                        short tempType = 0;
                        if (short.TryParse(tempAllDetail[2], out tempType))
                        {
                            picEntity.Type = tempType;
                            picEntity.TemplateID = tempTemplateID;
                            IList<SWfsMeetingTemplatePicInfo> isExistData = _singleEntityList.Where(c => c.FileName == picEntity.FileName && c.Type == picEntity.Type).ToList();
                            if (isExistData == null || isExistData.Count() == 0)
                            {
                                _singleEntityList.Add(picEntity);
                            }
                        }
                    }
                }
            }
            #endregion

            if (piclist.Count() == 0)
            {
                foreach (SWfsMeetingTemplatePicInfo item in _singleEntityList)
                {
                    if (item.Type == short.Parse(tempTemplateType) && item.TemplateID == tempTemplateID)
                    {
                        piclist.Add(item.PicNo);//图片编号
                    }
                }
            }
            else
            {
                foreach (SWfsMeetingTemplatePicInfo item in _singleEntityList)
                {
                    if (item.Type == short.Parse(tempTemplateType) && item.TemplateID == tempTemplateID)
                    {
                        piclist.Add(item.PicNo);//图片编号
                    }
                }
                piclist = piclist.Distinct().ToList();
            }
            ViewBag.Current = pageIndex;
            ViewBag.PageSize = pageSize;
            ViewBag.Total = piclist.Count;

            piclist = piclist.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.PicList = piclist;
            if (!string.IsNullOrEmpty(Request.QueryString["flag"]) && Request.QueryString["flag"].Equals("js"))
            {
                return Json(new { rs = true, content = RenderPartialViewToString("/Areas/Shangpin/Views/Venue/PartUpoladPicPage.cshtml", model) }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return PartialView(model);
            }
        }
        #endregion

        #region 获取会场图片列表
        [HttpGet]
        public JsonResult GetRegionList(string meetingNo, string templateNo)
        {
            VenueService service = new VenueService();
            IList<MeetingRelationRegion> list = service.GetRelationRegionListNew(meetingNo, templateNo).ToList();
            string href = string.Empty;
            for (int i = 0; i < list.Count(); i++)
            {
                if (list[i].RelationType == 2)
                {
                    list[i].RelationContent = GetHref(list[i].AboutID);
                }
            }
            var result = (from r in list
                          select new MeetingRelationRegion()
                          {
                              MeetingRelationID = r.MeetingRelationID,
                              ImgNO = ServicePic.ResolveUGCImage("2", r.ImgNO, 0, 0),
                              RegionID = r.RegionID,
                              RelationType = r.RelationType,
                              RelationContent = r.RelationContent,
                              AboutID = r.AboutID
                          }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        /// <summary>
        /// 获得关联活动连接
        /// </summary>
        /// <param name="activeId"></param>
        /// <returns></returns>
        public string GetHref(int activeId)
        {
            VenueService service = new VenueService();
            SWfsMeetingActiveSpecial model = service.GetActiveSpecialInfoById(activeId);
            string href = string.Empty;
            if (model.WebSite == 1)
            {
                href = AppSettingManager.AppSettings["ShangpinDomain"] + "women/subject/" + model.ActiveNO;
            }
            else
            {
                href = AppSettingManager.AppSettings["AolaiDomain"] + "Subject/Index/" + model.ActiveNO;
            }
            return href;
        }

        /// <summary>
        /// 保存会场编辑的HTML by lijia
        /// </summary>
        /// <param name="venuehtml"></param>
        /// <returns></returns>
        [HttpPost, ValidateInput(false)]
        public string SaveVenueHtml(string venuehtml)
        {
            VenueService service = new VenueService();
            try
            {
                IList<string> regionIds = new List<string>();
                string regImg = @"(opratorregion=['|\""]([^\""]+)['|\""])";
                MatchCollection matches = Regex.Matches(venuehtml, regImg, RegexOptions.IgnoreCase);
                foreach (Match match in matches)
                {
                    if (!regionIds.Contains(match.Value))
                    {
                        regionIds.Add(match.Value);
                    }
                    else
                    {
                        return match.Value;
                    }
                }
                int isPreView = int.Parse(Request.Form["isPreView"]);
                int isMobile = int.Parse(Request.Form["isMobile"]);
                SWfsMeetingInfoHtml mHtml = new SWfsMeetingInfoHtml();
                int meetingId = int.Parse(Request.Form["MeetingID"]);
                mHtml.MettingId = meetingId;
                SWfsMeetingInfoHtml model = service.GetHtmlByMeetingId(meetingId);
                //TemplateType:表示有哪些模板已发布。
                //若为1111则表示四个会场都是已发布状态，不存在编辑了html而没有发布的情况
                //第一位表示是否Mobile(1000)，第二位表示Mobile是否预热(1100)，第三位表示是否Web,第四位表示是否web预热
                string type = model == null ? "0000" : model.TemplateType.ToString();
                IList<char> list = type.ToList();
                if (isPreView == 0 && isMobile == 0)//Mobile预热
                {
                    list[0] = '0';
                    mHtml.MobilePreViewCode = venuehtml;
                    mHtml.MobileStartCode = model == null ? "" : model.MobileStartCode;
                    mHtml.WebStartCode = model == null ? "" : model.WebStartCode;
                    mHtml.WebPreViewCode = model == null ? "" : model.WebPreViewCode;
                }
                else if (isPreView == 1 && isMobile == 0)//Mobile进行
                {
                    list[1] = '0';
                    mHtml.MobileStartCode = venuehtml;
                    mHtml.MobilePreViewCode = model == null ? "" : model.MobilePreViewCode;
                    mHtml.WebStartCode = model == null ? "" : model.WebStartCode;
                    mHtml.WebPreViewCode = model == null ? "" : model.WebPreViewCode;
                }
                else if (isPreView == 0 && isMobile == 1)//web预热
                {
                    type = type.Substring(2, 1).Replace("1", "0");
                    list[2] = '0';
                    mHtml.WebPreViewCode = venuehtml;
                    mHtml.MobilePreViewCode = model == null ? "" : model.MobilePreViewCode;
                    mHtml.WebStartCode = model == null ? "" : model.WebStartCode;
                    mHtml.MobileStartCode = model == null ? "" : model.MobileStartCode;
                }
                else if (isPreView == 1 && isMobile == 1)//web进行
                {
                    type = type.Substring(3, 1).Replace("1", "0");
                    list[3] = '0';
                    mHtml.WebStartCode = venuehtml;
                    mHtml.MobilePreViewCode = model == null ? "" : model.MobilePreViewCode;
                    mHtml.WebPreViewCode = model == null ? "" : model.WebPreViewCode;
                    mHtml.MobileStartCode = model == null ? "" : model.MobileStartCode;
                }
                mHtml.TemplateType = string.Join("", list);
                mHtml.IsPublish = 0;
                mHtml.UpdateDate = DateTime.Now;
                mHtml.UpdateUserId = PresentationHelper.GetPassport().UserName;
                bool flag = true;
                if (model == null)
                {
                    mHtml.CreateDate = DateTime.Now;
                    mHtml.CreateUserId = PresentationHelper.GetPassport().UserName;
                    int i = service.AddMeetingHtml(mHtml);
                    flag = i == 0 ? true : false;
                }
                else
                {
                    mHtml.ID = model.ID;
                    //mHtml.MobilePreViewCode = venuehtml;
                    flag = service.SaveVenueHtml(mHtml, isPreView, isMobile);
                }
                if (flag)
                {
                    venueService.AddOperationLog(Request.Form["MeetingID"], 0, 1, LogActionType.Edit, "编辑会场HTML");
                    return "1";
                }
                return "0";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
