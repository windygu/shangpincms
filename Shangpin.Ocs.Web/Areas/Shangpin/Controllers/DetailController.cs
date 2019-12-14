using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Shangpin.Ocs.Service.Shangpin;
using Shangpin.Ocs.Service;
using Shangpin.Entity.Wfs;
using Shangpin.Framework.WebMvc;
using Shangpin.Framework.Common.Cache;
using Shangpin.Ocs.Entity.Extenstion.ShangPin;
using Shangpin.Ocs.Service.Common;
using System.IO;
using System.Web.Script.Serialization;
using Shangpin.Framework.Configuration;
using Shangpin.Entity.ComBeziPic;
using Shangpin.Framework.Common;
using Shangpin.Ocs.Service.Outlet;
using System.Transactions;
using System.Web.UI.WebControls;
using System.Drawing;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using System.Text;
using System.Threading;

namespace Shangpin.Ocs.Web.Areas.Shangpin.Controllers
{
    [OCSAuthorization]
    public class DetailController : BaseController
    {
        #region 构造器
        private readonly SWfsProductCommentService productCommentService;
        public DetailController()
        {
            productCommentService = new SWfsProductCommentService();
        }
        #endregion

        #region 评论
        #region 加载评论管理列表
        public ActionResult CommentManager(int pageIndex = 1, int pageSize = 20)
        {
            int total = 0;
            //商品编号，货号，商品名称
            if (Request.QueryString["keyWord"] != null)
            {
                ViewBag.keyWord = Request.QueryString["keyWord"];
            }
            //分类
            if (Request.QueryString["CategoryNo"] != null)
            {
                ViewBag.category = Request.QueryString["CategoryNo"];
            }
            //是否显示：1=显示，0=不显示
            if (Request.QueryString["Status"] != null)
            {
                ViewBag.Status = Request.QueryString["Status"];
            }
            //品牌
            if (Request.QueryString["brandNO"] != null)
            {
                ViewBag.BrandNO = Request.QueryString["brandNO"];
            }
            //星级
            if (Request.QueryString["StarLevel"] != null)
            {
                ViewBag.StarLevel = Request.QueryString["StarLevel"];
            }
            //评论内容关键词
            if (Request.QueryString["commentKeyWord"] != null)
            {
                ViewBag.CommentKeyWord = Request.QueryString["commentKeyWord"];
            }
            //用户名
            if (Request.QueryString["userName"] != null)
            {
                ViewBag.userName = Request.QueryString["userName"];
            }
            //时间范围--起始时间值
            if (Request.QueryString["StartTime"] != null)
            {
                ViewBag.StartTime = Request.QueryString["StartTime"];
            }
            //时间范围--结束时间值
            if (Request.QueryString["EndTime"] != null)
            {
                ViewBag.EndTime = Request.QueryString["EndTime"];
            }
            if (Request.QueryString["orderNo"] != null)
            {
                ViewBag.OrderNo = Request.QueryString["orderNo"];
            }
            if (!string.IsNullOrEmpty(Request["IsResult"]))
            {
                ViewBag.IsResult = Request["IsResult"];
            }
            IEnumerable<SWfsProductCommentExtends> list = productCommentService.GetSWfsProductCommentList(
                ViewBag.Status, ViewBag.BrandNO, ViewBag.category,
                ViewBag.keyWord, ViewBag.StarLevel, ViewBag.CommentKeyWord,
                ViewBag.userName, ViewBag.StartTime, ViewBag.EndTime,
                ViewBag.OrderNo, ViewBag.IsResult, pageIndex, pageSize, out total);
            ViewBag.totalCount = total;
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;
            //ViewBag.CommentPic=productCommentService.GetCommentPicByCommentId()
            //当着评论审核配置状
            var GlobalConfigStatus = productCommentService.GetGlobalConfigByFNo("FC01");
            switch (GlobalConfigStatus.ConfigValue)
            {
                case "1":
                    ViewBag.GlobalConfigStatus = "只审核带图片";
                    break;
                case "2":
                    ViewBag.GlobalConfigStatus = "全部需审核";
                    break;
                case "3":
                    ViewBag.GlobalConfigStatus = "全部不需审核";
                    break;
            }
            return View(list.OrderByDescending(n => n.CommentId));
        }
        #endregion

        public ActionResult CommentVerify(int cCommentId, int cStatus)
        {
            //ViewBag.verify = verify;
            ViewBag.CommentId = cCommentId;
            ViewBag.Status = cStatus;
            return View();
            //return View(new { CommentId = cCommentId, Status = cStatus });
        }

        #region 修改评论显示状态
        public ActionResult UpdateASWfsProductCommentStatus(int verify, int cCommentId, int cStatus)
        {
            try
            {
                var result = productCommentService.UpdateASWfsProductCommentStatus(verify, cCommentId, cStatus);
                return Json(new { Successed = result, Message = "ok" });
            }
            catch (Exception ex)
            {
                return Json(new { Successed = false, Message = ex.Message });
            }
            //return Redirect("CommentManager.html?CategoryNo=" + Request.QueryString["CategoryNo"] 
            //    + "&BrandName=" + Request.QueryString["BrandName"] 
            //    + "&BrandNo=" + Request.QueryString["BrandNo"] 
            //    + "&Status=" + Request.QueryString["Status"] 
            //    + "&StarLevel=" + Request.QueryString["StarLevel"] 
            //    + "&keyWord=" + Request.QueryString["keyWord"] 
            //    + "&userName=" + Request.QueryString["userName"] 
            //    + "&commentKeyWord=" + Request.QueryString["commentKeyWord"] 
            //    + "&StartTime=" + Request.QueryString["StartTime"] 
            //    + "&EndTime=" + Request.QueryString["EndTime"] 
            //    + CommonService.GetTimeStamp("&"));
        }
        #endregion

        #region 根据评论Id获取评论
        public string GetCommentById(string commentId)
        {
            JavaScriptSerializer json = new JavaScriptSerializer();
            return json.Serialize(productCommentService.GetCommentById(Convert.ToInt32(commentId)));
        }
        #endregion

        #region 更新回复内容
        public bool UpdateCommentRContent(string commentId, string commentRContent, string reType)
        {
            return productCommentService.UpdateCommentRContentById(commentId, commentRContent.Replace(',', '，').Replace(':', '：').Trim(), reType);
            //return false;
        }
        #endregion

        #region 评论批量导入、模板下载
        #region 下载模板
        public void DownLoadExcelTemplate()
        {
            string filename = "test.xlsx";
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", filename));

            XSSFWorkbook workbook = new XSSFWorkbook();
            ISheet sheet1 = workbook.CreateSheet("Sheet1");
            //冻结第一行
            sheet1.CreateFreezePane(0, 1, 0, 1);
            //创建表头
            //sheet1.CreateRow(0).CreateCell(0).SetCellValue("This is a Sample");
            int x = 1;
            for (int i = 0; i <= 15; i++)
            {
                IRow row = sheet1.CreateRow(i);
                for (int j = 0; j < 15; j++)
                {
                    row.CreateCell(j).SetCellValue(x++);
                }
            }
            using (var f = System.IO.File.Create(@"d:\test.xlsx"))
            {
                workbook.Write(f);
            }
            Response.WriteFile(@"d:\test.xlsx");
            Response.Flush();
            Response.End();
        }
        #endregion

        #region 导入Excel模板
        public ActionResult ImportExcelTemplate()
        {
            HttpFileCollection hfc = System.Web.HttpContext.Current.Request.Files;
            if (hfc.Count > 0)
            {
                if (hfc[0].ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" && hfc[0].ContentType != "application/vnd.ms-excel" && hfc[0].ContentType != "application/octet-stream")
                {
                    return Json(new { Successed = false, Message = "上传文件格式不正确。" }, "text/html", JsonRequestBehavior.AllowGet);
                }
                //for (int i = 0; i < hfc.Count; i++) { 
                return Json(new { Successed = true, Message = RenderToDb(hfc[0].InputStream) }, "text/html", JsonRequestBehavior.AllowGet);
                //}
            }
            return Json(new { Successed = false, Message = "请先选择文件再上传。" }, "text/html", JsonRequestBehavior.AllowGet);
        }
        private string RenderToDb(Stream excelFileStream)
        {
            try
            {
                IWorkbook workbook = new XSSFWorkbook(excelFileStream);

                if (workbook.NumberOfSheets > 0)
                {
                    int sheetNum = workbook.NumberOfSheets;
                    StringBuilder workbookSB = new StringBuilder();
                    for (int s = 0; s < sheetNum - 1; s++)
                    {
                        ISheet sheet = workbook.GetSheetAt(s);//取第s个工作表
                        StringBuilder builder = new StringBuilder();
                        IRow headerRow = sheet.GetRow(0);//第一行为标题行
                        int cellCount = headerRow.LastCellNum;//LastCellNum = PhysicalNumberOfCells
                        int rowCount = sheet.LastRowNum;//LastRowNum = PhysicalNumberOfRows - 1

                        if (rowCount < 1) return "从第" + s + "个\"" + workbook.GetSheetAt(s).SheetName + "\"Sheet开始,都没有添加成功，请检查！";
                        for (int i = (sheet.FirstRowNum + 1); i <= rowCount; i++)
                        {
                            string insertCommentSql = @"DECLARE @ID" + s + i + @" INT 
                                                                    INSERT INTO SWfsProductComment(ProductNo,SkuNo,NickName,StarLevel,CommentContent,Status,CommentSource,CommentType,UserLevel,UserId,OrderNo,DateCreate)";
                            StringBuilder sb = new StringBuilder();
                            string[] random = { "0001", "0002", "0003", "0004" };
                            Random r = new Random();
                            string UserLevel = random[r.Next(4)];

                            IRow row = sheet.GetRow(i);
                            if (row != null)
                            {
                                builder.Append(insertCommentSql);
                                builder.Append(" values (");
                                for (int j = 0; j < cellCount; j++)
                                {
                                    string insertCommentItemSql = @"INSERT INTO SWfsProductCommentItem(CommentId,ObjectName,ObjectValue,CreateDate,ItemType)";
                                    //有空字段时，全部返回
                                    if ((headerRow.GetCell(j).ToString() != "肤质" && headerRow.GetCell(j).ToString() != "身高(cm)" && headerRow.GetCell(j).ToString() != "体重(kg)") && string.IsNullOrEmpty(row.GetCell(j).ToString())) return "从第" + s + "个\"" + workbook.GetSheetAt(s).SheetName + "\"Sheet,第" + i + "行，第" + j + "列开始,都没有添加成功，请检查！";
                                    switch (headerRow.GetCell(j).ToString())
                                    {
                                        case "舒适度评分":
                                            sb.Append(insertCommentItemSql);
                                            sb.Append(" values (");
                                            sb.AppendFormat("{0},'{1}','{2}','{3}','{4}'", "@ID" + s + i, headerRow.GetCell(j).ToString().Substring(0, 3), row.GetCell(j).ToString(), DateTime.Now, 5);
                                            sb.Append(");");
                                            break;
                                        case "综合评分":
                                            builder.AppendFormat("'{0}',", row.GetCell(j).ToString());
                                            sb.Append(insertCommentItemSql);
                                            sb.Append(" values (");
                                            sb.AppendFormat("{0},'{1}','{2}','{3}','{4}'", "@ID" + s + i, headerRow.GetCell(j).ToString(), row.GetCell(j).ToString(), DateTime.Now, 5);
                                            sb.Append(");");
                                            break;
                                        case "款式评分":
                                        case "外观评分":
                                        case "肤质":
                                            sb.Append(insertCommentItemSql);
                                            sb.Append(" values (");
                                            sb.AppendFormat("{0},'{1}','{2}','{3}','{4}'", "@ID" + s + i, headerRow.GetCell(j).ToString().Substring(0, 2), row.GetCell(j).ToString(), DateTime.Now, 5);
                                            sb.Append(");");
                                            break;
                                        case "尺码大小":
                                            sb.Append(insertCommentItemSql);
                                            sb.Append(" values (");
                                            sb.AppendFormat("{0},'{1}','{2}','{3}','{4}'", "@ID" + s + i, headerRow.GetCell(j).ToString().Substring(0, 2), row.GetCell(j).ToString(), DateTime.Now, 2);
                                            sb.Append(");");
                                            break;
                                        case "身高(cm)":
                                        case "体重(kg)":
                                            if (row.GetCell(j).ToString() == "0") break;
                                            sb.Append(insertCommentItemSql);
                                            sb.Append(" values (");
                                            sb.AppendFormat("{0},'{1}','{2}','{3}','{4}'", "@ID" + s + i, headerRow.GetCell(j).ToString().Substring(0, 2), row.GetCell(j).ToString(), DateTime.Now, 1);
                                            sb.Append(");");
                                            break;
                                        default:
                                            builder.AppendFormat("'{0}',", row.GetCell(j).ToString());
                                            break;
                                    }
                                }
                                //builder.Length = builder.Length - 1;
                                Thread.Sleep(10);
                                builder.Append("1,1,2,'" + UserLevel + "','','" + DateTime.Now.Ticks.ToString() + "','" + DateTime.Now.AddHours(-(r.Next(12))).AddMinutes(-(r.Next(60))) + "');SELECT @ID" + s + i + " = @@IDENTITY;");
                                builder.Append(sb.ToString());
                            }
                        }
                        //string rowAffected = builder.ToString();
                        workbookSB.Append(builder.ToString());
                        //productCommentService.BetchInsertComment(builder.ToString());
                        builder.Length = 0;
                    }
                    productCommentService.BetchInsertComment(workbookSB.ToString());
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
            return "全部上传成功！";
        }
        #endregion
        #endregion

        #region 评论审核配置
        public ActionResult CommentVerifyConfig()
        {
            return View();
        }

        #region 后台添加评论
        public ActionResult AddComment(string skuNo)
        {
            ViewBag.UserLevel = productCommentService.GetUserLevel();
            skuNo = String.IsNullOrEmpty(skuNo) ? "" : skuNo.Trim();
            List<ProductCommentEntity> entities = productCommentService.GetProductDetailOrCommentDetailBySku(skuNo);
            ViewBag.skuNo = skuNo ?? "";
            return View(entities);
        }
        /// <summary>
        /// 检测图片是否通过,或者保存图片
        /// </summary>
        /// <param name="isSave"></param>
        /// <returns></returns>
        [NonAction]
        private Dictionary<string, string> CheckOrSaveExperienceReportImg(Boolean isSave = false, Boolean notcheck = false)
        {
            #region 图片上传与检测
            Dictionary<string, string> result = new Dictionary<string, string>();
            result["success"] = "";
            result["error"] = "";
            HttpFileCollection Files = System.Web.HttpContext.Current.Request.Files;
            int fileCount = 0;
            if (Files != null && Files.Count > 0)// && Files[0].ContentLength > 0)
            {
                List<string> indexes = new List<string>();
                string types = null;// AppSettingManager.AppSettings["InputUserCommentImgType"];
                string[] allowTypes = string.IsNullOrEmpty(types) ? (new string[] { ".jpg", ".gif", ".png", ".jpeg" }) : (types.Split('/'));

                //允许上传的大小 , 以字节为单位 , 稍后读取配置文件
                int allowSize = 4 * 1024 * 1024;
                for (int i = 0; i < Files.Count; i++)
                {
                    if (indexes.Contains(i.ToString()) || Files[i].ContentLength == 0) continue;
                    string fileFullName = Files[i].FileName;
                    string fileType = System.IO.Path.GetExtension(fileFullName).ToLower();
                    string fileName = Path.GetFileName(fileFullName);
                    ImageTypeEnum[] allowType = new ImageTypeEnum[] { ImageTypeEnum.JPG, ImageTypeEnum.PNG, ImageTypeEnum.GIF };
                    if (notcheck || allowTypes.Contains(fileType) && (allowType.Contains(ImageTypeCheck.CheckImageType(Files[i].InputStream, false))))
                    {
                        int conLen = Files[i].ContentLength;
                        if (conLen <= allowSize && conLen > 0)//需要保存时
                        {
                            if (fileCount == 3)
                            {
                                result["error"] = "上传图片数量最多为3张";
                                return result;
                            }
                            fileCount++;
                            if (!isSave) continue;
                            UserPictureFile model = new UserPictureFile();
                            //读取文件为 二进制流 , 保存到  图片表 , 返回 图片编号 
                            model = productCommentService.SavePostedFile(Files[i]);
                            result["success"] = result["success"] + "|" + model.PictureFileNo;
                        }
                        else if (conLen > allowSize)
                        {
                            result["error"] = "上传文件长度超过4MB";
                            return result;
                        }
                    }
                    else
                    {
                        result["error"] = "上传文件类型错误";
                        return result;
                    }
                }
            }
            else
            {
                result["success"] = "";
            }
            if (fileCount == 0) { result["success"] = ""; result.Add("noimg", "noimg"); }
            else { if (result["success"].Length == 0) result["success"] = "1"; }
            return result;
            #endregion
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult AddReviewBack(FormCollection F)
        {
            if (F.AllKeys.Count() == 0) return View();
            Dictionary<string, string> dic = new Dictionary<string, string>();

            try
            {
                #region 表单检测
                string orderNo = Guid.NewGuid().ToString().Substring(0, 30);
                string productNo = F["productNo"];
                string areaValue = F["commentarea"];
                string optionstr = F["optionitems"];//  A∈b∈c∨1∈2∈3  值∨键
                string username = F["username"];
                string skuNo = F["skuNo"].Trim();
                dic["error"] = "";
                dic["success"] = "";
                if (string.IsNullOrEmpty(username))
                {
                    dic["error"] += ",用户名不能为空";
                }
                areaValue = string.IsNullOrEmpty(areaValue) ? "" : areaValue.Replace("\r\n", "");
                if (areaValue.Length == 0 || areaValue.Length > 200)
                {
                    dic["error"] += ",评论内容不能为空";
                }
                string[] arrResult = null;
                string[] commvalues = null;
                string[] commids = null;
                bool falg = false;
                string optionError = "";
                if (optionstr.Length > 0)
                {
                    arrResult = optionstr.Split('∨');
                    commvalues = arrResult[0].Split('∈');
                    commids = arrResult[1].Split('∈');
                    falg = productCommentService.CheckInputValueType(commids, commvalues, ref optionError);
                }
                if (!falg)
                {
                    Response.Write("<script>if(top.saveCommentCallBack)top.saveCommentCallBack('" + F["clearform"] + "','0','" + optionError + "');</script>");
                    return View();
                }
                #endregion
                if (dic["error"].Length == 0 && optionstr.Length > 0)
                {

                    dic = CheckOrSaveExperienceReportImg();
                    DateTime now = DateTime.Now;
                    if (dic["error"].Length == 0)
                    {
                        int startValue = 5;
                        if (!int.TryParse(commvalues[0], out startValue))
                            startValue = 5;
                        //using (var scope = new TransactionScope())
                        //{
                        #region 初始化数据
                        SWfsProductComment productComment = new SWfsProductComment();
                        productComment.DateCreate = now;
                        productComment.OrderNo = orderNo;
                        productComment.ProductNo = productNo;
                        productComment.StarLevel = startValue;
                        if (F["UserLevel"] == "0")//如果是0为随机否则 为赋值
                            productComment.UserLevel = "000" + new Random().Next(1, 5).ToString();//随机1-4 
                        else
                            productComment.UserLevel = F["UserLevel"];
                        productComment.ResultDate = Convert.ToDateTime("1900-1-1");
                        productComment.CommentRDate = Convert.ToDateTime("1900-1-1");
                        productComment.CommentType = dic.Keys.Contains("noimg") ? (short)2 : (short)1;//无图 评论类型1,有图 2,无图
                        productComment.CommentSource = 1;//管理员评论
                        productComment.OperateUserId = PresentationHelper.GetPassport().UserName;
                        productComment.NickName = username;
                        productComment.CommentContent = areaValue;
                        productComment.SkuNo = skuNo;
                        productComment.Status = 1;
                        var result = productCommentService.AddReview(productComment);
                        #endregion
                        SWfsProductComment comment = productCommentService.GetCommentByProductNoAndOrderNoAndDate(productNo, orderNo, now);
                        if (optionstr.Length > 0 && comment != null)//添加评论项
                        {
                            List<SWfsProductCommentItem> items = new List<SWfsProductCommentItem>();

                            productCommentService.AddCommentItem(commids, commvalues, comment.CommentId);
                        }
                        // scope.Complete();
                        //  }
                        if (dic["success"].Length > 0 && !dic.ContainsKey("noimg"))//添加评论图片
                        {
                            dic = CheckOrSaveExperienceReportImg(true, true);//向数据库保存图片 
                            string[] picIds = dic["success"].Split('|').Except(new string[] { "" }).ToArray();
                            List<SWfsProductCommentPicRef> pics = new List<SWfsProductCommentPicRef>();
                            for (int i = 0; i < picIds.Length; i++)
                            {
                                pics.Add(new SWfsProductCommentPicRef { CommentId = comment.CommentId, CreateDate = now, SortId = i, PictureFileNo = picIds[i] });
                            }
                            productCommentService.AddSWfsProductCommentPicRef(pics);
                        }
                        dic["success"] = "1";
                        Response.Write("<script>if(top.saveCommentCallBack)top.saveCommentCallBack('" + F["clearform"] + "','" + dic["success"] + "','" + dic["error"] + "');</script>");
                        return View();
                    }
                    dic["success"] = "0";
                    dic["error"] = dic["error"];

                }
            }
            catch (Exception)
            {
                dic["success"] = "0";
                dic["error"] = "保存失败,请稍后再试.";
            }
            Response.Write("<script>if(top.saveCommentCallBack)top.saveCommentCallBack('" + F["clearform"] + "','" + dic["success"] + "','" + dic["error"] + "');</script>");
            return View();
        }

        #endregion

        public ActionResult SaveCommentConfig(string functionNo, string configV)
        {
            try
            {
                return Json(new { Successed = productCommentService.UpdateGlobalConfig(functionNo, configV), Message = "修改成功！" });
            }
            catch (Exception ex)
            {
                return Json(new { Successed = false, Message = ex.Message });
            }
        }
        /// <summary>
        /// 得到配置值根据功能编号FC01 GetGlobalConfig
        /// </summary>
        /// <param name="functionNo"></param>
        /// <returns></returns>
        public ActionResult GetCommentConfig(string functionNo)
        {
            return Json(new { Successed = true, Message = productCommentService.GetGlobalConfig(functionNo).ConfigValue });
        }
        #endregion
        #endregion

        #region 咨询

        #region 咨询主表

        #region 加载咨询管理列表
        public ActionResult QuestAnswerManager(string productKeywords, string questKeywords, string answerKeywords, int pageIndex = 1, int pageSize = 20)
        {
            int total = 0;
            IEnumerable<SWfsQuestAnswer> list = productCommentService.GetSWfsSWfsQuestAnswerList(productKeywords, questKeywords, answerKeywords, pageIndex, pageSize, out total);
            ViewBag.totalCount = total;
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;
            IEnumerable<SWfsQuestAnswerExtends> list1 = productCommentService.GetSWfsSWfsQuestAnswerRefList();
            ViewBag.QuestAnswerRef = list1;
            return View(list.OrderByDescending(n => n.QuestAnswerId));
        }
        #endregion

        #region 更新前台显示状态
        public ActionResult UpdateSWfsQuestAnswerIsShow(int questAnswerId, int isShow)
        {
            productCommentService.UpdateSWfsQuestAnswerIsShow(questAnswerId, isShow);
            return Redirect("questanswermanager.html?pageindex=" + Request.QueryString["pageindex"] + "&productKeywords=" + Request.QueryString["productKeywords"] + "&questKeywords=" + Request.QueryString["questKeywords"] + "&answerKeywords=" + Request.QueryString["answerKeywords"] + CommonService.GetTimeStamp("&"));
        }
        #endregion

        #region 删除一条咨询
        public ActionResult DeleteQuestAnswer(string questAnswerId)
        {
            var result = productCommentService.DeleteSWfsQuestAnswer(questAnswerId);
            return Redirect("questanswermanager.html?pageindex=" + Request.QueryString["pageindex"] + "&productKeywords=" + Request.QueryString["productKeywords"] + "&questKeywords=" + Request.QueryString["questKeywords"] + "&answerKeywords=" + Request.QueryString["answerKeywords"] + CommonService.GetTimeStamp("&"));
        }
        #endregion

        #region 咨询弹窗控制器
        public ActionResult CreateNewQA(string questAnswerId, string isEdit)
        {
            var model = productCommentService.GetSWfsSWfsQuestAnswerById(questAnswerId);
            if (model != null && isEdit == "1")
            {
                ViewBag.QuestAnswerId = model.QuestAnswerId;
                ViewBag.Quest = model.Quest;
                ViewBag.Sort = model.Sort;
                ViewBag.Answer = model.Answer;
                ViewBag.DateAnswer = model.DateAnswer;
                ViewBag.DateQuest = model.DateQuest;
                ViewBag.IsShow = model.IsShow;
                ViewBag.IsEdit = isEdit;
            }
            return View();
        }
        #endregion

        #region 创建新的咨询
        public ActionResult CreateQANew()
        {
            try
            {
                string QuestionParams = null; string msg = null;
                if (!string.IsNullOrEmpty(Request.Params["myQuestion"]))
                {
                    QuestionParams = Request.Params["myQuestion"];
                }
                string AnwserParams = null;
                if (!string.IsNullOrEmpty(Request.Params["yourAnwser"]))
                {
                    AnwserParams = Request.Params["yourAnwser"];
                }
                string SortParams = null;
                if (!string.IsNullOrEmpty(Request.Params["sort"]))
                {
                    SortParams = Request.Params["sort"];
                }
                string StartTimeParams = null;
                if (!string.IsNullOrEmpty(Request.Params["StartTime"]))
                {
                    StartTimeParams = Request.Params["StartTime"];
                }

                string EndTimeParams = null;
                if (!string.IsNullOrEmpty(Request.Params["EndTime"]))
                {
                    EndTimeParams = Request.Params["EndTime"];
                }

                string IsShowParams = null;
                if (!string.IsNullOrEmpty(Request.Params["IsShow"]))
                {
                    IsShowParams = Request.Params["IsShow"];
                }
                SWfsQuestAnswer QuestAnswer = new SWfsQuestAnswer();

                QuestAnswer.Quest = QuestionParams;
                QuestAnswer.Answer = AnwserParams;
                QuestAnswer.Sort = Convert.ToInt32(SortParams);
                QuestAnswer.DateQuest = string.IsNullOrEmpty(StartTimeParams) ? DateTime.Now : DateTime.Parse(StartTimeParams);
                QuestAnswer.DateAnswer = string.IsNullOrEmpty(StartTimeParams) ? DateTime.Now : DateTime.Parse(EndTimeParams);
                QuestAnswer.IsShow = Convert.ToInt16(IsShowParams);

                if ((!string.IsNullOrEmpty(Request["isEdit"])) && Request["isEdit"] == "1")
                {
                    QuestAnswer.QuestAnswerId = Convert.ToInt32(Request.Params["questAnswerId"]);
                    if (productCommentService.UpdateSWfsQuestAnswer(QuestAnswer))
                    {
                        msg = "修改成功";
                    }
                    else
                        msg = "修改失败";
                }
                else
                {
                    if (productCommentService.InsertSWfsQuestAnswer(QuestAnswer) == 0)
                    {
                        msg = "添加成功";
                    }
                    else
                        msg = "添加失败";
                }

                return Json(new { result = "1", message = msg });
            }
            catch (Exception ex)
            {
                return Json(new { result = "0", message = ex.Message });
            }
        }
        #endregion
        #endregion

        #region 关联表
        #region 编辑咨询
        public ActionResult QuestAnswerRefEdit(string questAnswerId)
        {
            IEnumerable<SWfsQuestAnswerExtends> list = productCommentService.GetSWfsSWfsQuestAnswerRefListByQuestAnswerId(questAnswerId);
            ViewBag.questAnswerId = questAnswerId;
            return View(list);
        }
        #endregion

        #region 删除咨询
        public ActionResult DeleteSWfsQuestAnswerRefById(string refId, string questAnswerId)
        {
            productCommentService.DeleteSWfsQuestAnswerRefById(refId, questAnswerId);
            return Redirect("questanswermanager.html?pageindex=" + Request.QueryString["pageindex"]
                + "&productKeywords=" + Request.QueryString["productKeywords"]
                + "&questKeywords=" + Request.QueryString["questKeywords"]
                + "&answerKeywords=" + Request.QueryString["answerKeywords"]
                + CommonService.GetTimeStamp("&"));
        }
        #endregion

        #region 关联商品或品类或品牌
        public ActionResult CreateQuestanswerRef(string questAnswerId, string productNos, string BtnSaveCB, string BtnSaveProduct)
        {
            string brandSelectedValue = Request.Params["brandSelectedValue"];
            string categorySelectedValue = Request.Params["categorySelectedValue"];
            SWfsQuestAnswerRef qar = new SWfsQuestAnswerRef();
            qar.QuestAnswerId = Convert.ToInt32(questAnswerId);
            //关联商品
            if (!string.IsNullOrEmpty(BtnSaveProduct))
            {
                foreach (string item in productNos.Split(','))
                {
                    qar.ProductNo = item;
                    qar.RefType = 1;
                    if (CheckQuestanswerRef(item, "", "", questAnswerId))
                    { continue; }
                    productCommentService.InsertSWfsQuestAnswerRef(qar);
                }
            }
            //关联分类和品牌
            if (!string.IsNullOrEmpty(BtnSaveCB))
            {
                if (!string.IsNullOrEmpty(categorySelectedValue) && !string.IsNullOrEmpty(brandSelectedValue))
                {
                    qar.RefType = 4;
                    foreach (string itemc in categorySelectedValue.Split(','))
                    {
                        qar.CategoryNo = itemc;

                        foreach (string itemb in brandSelectedValue.Split(','))
                        {
                            qar.BrandNo = itemb;
                            if (CheckQuestanswerRef("", itemc, itemb, questAnswerId))
                            { continue; }
                            productCommentService.InsertSWfsQuestAnswerRef(qar);
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(categorySelectedValue))
                {
                    qar.RefType = 2;
                    foreach (string itemc in categorySelectedValue.Split(','))
                    {
                        qar.CategoryNo = itemc;
                        if (CheckQuestanswerRef("", itemc, "", questAnswerId))
                        { continue; }
                        productCommentService.InsertSWfsQuestAnswerRef(qar);
                    }
                }
                else if (!string.IsNullOrEmpty(brandSelectedValue))
                {
                    qar.RefType = 3;
                    foreach (string itemb in brandSelectedValue.Split(','))
                    {
                        qar.BrandNo = itemb;
                        //qar.RefType = 3;
                        if (CheckQuestanswerRef("", "", itemb, questAnswerId))
                        { continue; }
                        productCommentService.InsertSWfsQuestAnswerRef(qar);
                    }
                }
            }
            return Redirect("questanswermanager.html?pageindex=" + Request.QueryString["pageindex"]
                + "&productKeywords=" + Request.QueryString["productKeywords"]
                + "&questKeywords=" + Request.QueryString["questKeywords"]
                + "&answerKeywords=" + Request.QueryString["answerKeywords"]
                + CommonService.GetTimeStamp("&"));
        }
        #endregion

        #region 检查关联
        public bool CheckQuestanswerRef(string productNo, string categoryNo, string brandNo, string questAnswerId)
        {
            if (productCommentService.CheckQuestAnswerRefByNos(productNo, categoryNo, brandNo, questAnswerId) != null)
            {
                return true;
            }
            return false;
        }
        #endregion
        #endregion

        #endregion
    }
}
