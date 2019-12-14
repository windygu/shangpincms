using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Shangpin.Entity.Wfs;
using System.Collections;
using Shangpin.Framework.Common;
using Shangpin.Ocs.Service.Shangpin;
using Shangpin.Ocs.Service;
using Shangpin.Ocs.Entity.Extenstion.ShangPin;
using Shangpin.Entity.Common;
using System.Text;
using Shangpin.Framework.Common.XmlSync;
using Shangpin.Ocs.Service.Common;
using System.Data;
using System.IO;

namespace Shangpin.Ocs.Web.Areas.Shangpin.Controllers
{
    public class VCodeController : Controller
    {
        //
        // GET: /Shangpin/VCode/
        VCodeService Vcode = new VCodeService();
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 列表页
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult VCodeList(int pageIndex = 1, int pageSize = 10)
        {
            int count = 0;
            ViewBag.list = VCodeLists(pageIndex, pageSize, out count);
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;
            ViewBag.TotalCount = count;
            return View();
        }
        public IEnumerable<SWfsVActivity> VCodeLists(int pageIndex, int pageSize, out int count)
        {
            SWfsVActivity obj = new SWfsVActivity();
            var dic = new Dictionary<string, object>();
            string code = Request.QueryString["ActivityCode"];
            string name = Request.QueryString["ActivityName"];
            string datetem = Request.QueryString["tb_Query_Time"];
            ViewBag.ActivityCode = code;
            ViewBag.ActivityName = name;
            ViewBag.ListingOutletFlag = Request.QueryString["ListingOutletFlag"];
            dic.Add("ActivityCode", code ?? "");
            dic.Add("ActivityName", name ?? "");
            if (Request.QueryString["ListingOutletFlag"] != null && Request.QueryString["ListingOutletFlag"] != "-1")
            {
                dic.Add("ListingOutletFlag", Request.QueryString["ListingOutletFlag"]);
            }
            else
            {
                dic.Add("ListingOutletFlag", "");
            }
            if (Request.QueryString["Query_Time"] != null && Request.QueryString["Query_Time"] != "-1" && Request.QueryString["tb_Query_Time"] != null && Request.QueryString["tb_Query_Time"] != "")
            {
                if (Request.QueryString["Query_Time"] == "1")
                {
                    dic.Add("ActivityDateStart", datetem);
                }
                else
                {
                    dic.Add("ActivityDateStart", "");
                }
                if (Request.QueryString["Query_Time"] == "2")
                {
                    dic.Add("ActivityDateEnd", datetem);
                }
                else
                {
                    dic.Add("ActivityDateEnd", "");
                }
                if (Request.QueryString["Query_Time"] == "3")
                {
                    dic.Add("DateCreate", datetem);
                }
                else
                {
                    dic.Add("DateCreate", "");
                }
                //DateTime time = DateTime.Parse(Request.QueryString["tb_Query_Time"]);
                //string timeType = string.Empty;
                //switch (Request.QueryString["Query_Time"])
                //{
                //    case "1": timeType = "ActivityDateStart"; break;
                //    case "2": timeType = "ActivityDateEnd"; break;
                //    case "3": timeType = "DateCreate"; break;
                //}
                //dic.Add(timeType, time);
                //dic.Add("ActivityDateStart", timeType);
                //dic.Add("ActivityDateEnd", timeType);
                //dic.Add("DateCreate", time);
                ViewBag.Query_Time = Request.QueryString["Query_Time"];
                ViewBag.tb_Query_Time = datetem;
            }
            else
            {
                if (Request.QueryString["Query_Time"] == "1")
                {
                    dic.Add("ActivityDateStart", "");
                }
                else
                {
                    dic.Add("ActivityDateStart", "");
                }
                if (Request.QueryString["Query_Time"] == "2")
                {
                    dic.Add("ActivityDateEnd", "");
                }
                else
                {
                    dic.Add("ActivityDateEnd", "");
                }
                if (Request.QueryString["Query_Time"] == "3")
                {
                    dic.Add("DateCreate", "");
                }
                else
                {
                    dic.Add("DateCreate", "");
                }
            }
            IEnumerable<SWfsVActivity> list = DapperUtil.Query<SWfsVActivity>("ComBeziWfs_WfsCmsContent_SWfsVActivity_List", dic, new { ActivityCode = dic["ActivityCode"], ActivityName = dic["ActivityName"], ActivityDateStart = Request.QueryString["tb_Query_Time"], ActivityDateEnd = Request.QueryString["tb_Query_Time"], DateCreate = Request.QueryString["tb_Query_Time"], ListingOutletFlag = Request.QueryString["ListingOutletFlag"], pageIndex = pageIndex, pageSize = pageSize }).OrderByDescending(c => c.DateCreate).ToList();
            count = DapperUtil.Query<int>("ComBeziWfs_WfsCmsContent_SWfsVActivity_List_Count", dic, new { ActivityCode = dic["ActivityCode"], ActivityName = dic["ActivityName"], ActivityDateStart = dic["ActivityDateStart"], ActivityDateEnd = Request.QueryString["tb_Query_Time"], DateCreate = Request.QueryString["tb_Query_Time"], ListingOutletFlag = Request.QueryString["ListingOutletFlag"], pageIndex = pageIndex, pageSize = pageSize }).First<int>();
            return list;
        }


        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public ActionResult VCodeCreate(string activityId)
        {
            if (!string.IsNullOrEmpty(activityId))
            {
                VCodeService vcodeid = new VCodeService();
                var lis = vcodeid.VCodeId(activityId);

                ViewBag.Vcode = lis;
            }
            return View();
        }
        [HttpPost]
        public string VCodeCreate()
        {
            SWfsVActivity obj = null;
            bool isupdate = false;//用isupdat来区分添加和修改操作
            string activityId = Request.Form["ActivityId"];
            if (!string.IsNullOrEmpty(activityId))
            {
                isupdate = true;
                obj = DapperUtil.QueryByIdentityWithNoLock<SWfsVActivity>(activityId);
            }
            else
            {
                obj = new SWfsVActivity();
                obj.DateCreate = DateTime.Now;
            }

            //添加操作需要赋值的部分
            if (isupdate == false)
            {
                string activityNo = DateTime.Now.ToString("yyyyMMdd") + DateTime.Now.Millisecond.ToString();
                obj.ActivityId = activityNo;
            }




            //修改添加都需要赋值的【公共部分】
            obj.ActivityTypeId = 7;
            obj.ActivityName = Request.Form["ActivityName"];
            //如保存过v码则不允许编辑对于disabled数据后台获取不到 需判断值 
            if (!string.IsNullOrEmpty(Request.Form["ActivityCode"]))
            {
                obj.ActivityCode = Request.Form["ActivityCode"];
            }
            obj.ActivityStatus = Request.Form["ActivityStatus"] != null ? Convert.ToInt16(Request.Form["ActivityStatus"]) : (short)1;
            obj.ActivityDateStart = DateTime.Parse(Request.Form["ActivityDateStart"]);
            obj.ActivityDateEnd = DateTime.Parse(Request.Form["ActivityDateEnd"]);
            obj.OperatorId = PresentationHelper.GetPassport().UserName;
            obj.ListingOutletFlag = short.Parse(Request.Form["siteNo"]);

            //根据isupdat谁的操作去做谁的事
            if (isupdate == false)
            {
                if (Vcode.VCodeCreate(obj) >= 0)
                {
                    return "{\"result\":\"success\"}";
                }
                else
                {
                    return "{\"result\":\"false\"}";
                }
            }
            else
            {
                if (Vcode.VCodeUpdate(obj))
                {
                    return "{\"result\":\"success\"}";
                }
                else
                {
                    return "{\"result\":\"false\"}";
                }
            }
        }

        /// <summary>
        /// 修改V码状态
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public ActionResult VCodeStatusUpdate(string activityId, int status)
        {
            Vcode.UpdateVCodeStatus(activityId, status);
            return Redirect("VCodeList.html");
        }

        /// <summary>
        /// 删除V码
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public ActionResult DeleVCode(string activityId)
        {
            Vcode.DeleteVCode(activityId);
            return Redirect("VCodeList.html");
        }

        /// <summary>
        /// V码管理
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult VCodeManagement(string activityId, string vcode, int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.VCode = Request.QueryString["VCode"];
            ViewBag.ActivityIds = Request.QueryString["ActivityIds"];
            if (string.IsNullOrEmpty(ViewBag.ActivityIds))
            {
                ViewBag.ActivityIds = activityId;
            }
            //微码类型
            ViewBag.VCodeType = Request.QueryString["codeType"];
            int count = 0;
            ViewBag.list = VCodeManagementList(activityId, pageIndex, pageSize, out count);
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;
            ViewBag.TotalCount = count;
            return View();
        }
        public IEnumerable<VCodeInfo> VCodeManagementList(string activityId, int pageIndex, int pageSize, out int count)
        {
            if (activityId == null)
            {
                activityId = Request.QueryString["ActivityIds"];
            }
            var dic = new Dictionary<string, object>();
            int counts = Convert.ToInt32(Request.QueryString["CreateCount"]);
            //activityId = Request.Form["ActivityIds"];
            #region 搜索条件
            if (Request.QueryString["VCode"] != "" && Request.QueryString["VCode"] != null)
            {
                dic.Add("VCode", Request.QueryString["VCode"]);
                ViewBag.KeyWord = Request.QueryString["VCode"];
            }
            else
            {
                dic.Add("VCode", "");
                ViewBag.KeyWord = Request.QueryString["VCode"];
            }
            if (Request.QueryString["UserName"] != "" && Request.QueryString["UserName"] != null)
            {
                dic.Add("OperatorId", Request.QueryString["UserName"]);
                ViewBag.Prompt = Request.QueryString["UserName"];
            }
            else
            {
                dic.Add("OperatorId", "");
                ViewBag.Prompt = Request.QueryString["UserName"];
            }
            if (Request.QueryString["UseCodeStatus"] != "-1" && Request.QueryString["UseCodeStatus"] != "" && Request.QueryString["UseCodeStatus"] != null)
            {
                dic.Add("UseCodeStatus", Request.QueryString["UseCodeStatus"]);
            }
            else
            {
                dic.Add("UseCodeStatus", "");
            }

            if (Request.QueryString["CodeType"] != "0" && Request.QueryString["CodeType"] != "" && Request.QueryString["CodeType"] != null)
            {
                dic.Add("VCodeType", Request.QueryString["CodeType"]);
            }
            else
            {
                dic.Add("VCodeType", "");
            }

            if (Request.QueryString["DateCreate"] != "" && Request.QueryString["DateCreate"] != null)
            {
                dic.Add("DateCreate", Request.QueryString["DateCreate"]);
                ViewBag.CreateDate = Request.QueryString["DateCreate"];
            }
            else
            {
                dic.Add("DateCreate", "");
                ViewBag.CreateDate = Request.QueryString["DateCreate"];
            }
            if (Request.QueryString["EndDate"] != "" && Request.QueryString["EndDate"] != null)
            {
                dic.Add("EndDate", Request.QueryString["EndDate"]);
                ViewBag.EndCreateDate = Request.QueryString["EndDate"];
            }
            else
            {
                dic.Add("EndDate", "");
                ViewBag.EndCreateDate = Request.QueryString["EndDate"];
            }
            #endregion
            dic.Add("ActivityId", activityId);
            List<VCodeInfo> list = DapperUtil.Query<VCodeInfo>("ComBeziWfs_WfsCmsContent_SWfsVActivityCodesRef_List", dic,
                new
                {
                    VCode = Request.QueryString["VCode"],
                    OperatorId = Request.QueryString["UserName"],
                    UseCodeStatus = Request.QueryString["UseCodeStatus"],
                    VCodeType = Request.QueryString["CodeType"],
                    DateCreate = Request.QueryString["DateCreate"],
                    EndDate = Request.QueryString["EndDate"],
                    ActivityId = activityId,
                    pageIndex = pageIndex,
                    pageSize = pageSize
                }).ToList();
            count = DapperUtil.Query<int>("ComBeziWfs_WfsCmsContent_SWfsVActivityCodesRef_List_Count", dic,
                new
                {
                    VCode = Request.QueryString["VCode"],
                    OperatorId = Request.QueryString["UserName"],
                    UseCodeStatus = Request.QueryString["UseCodeStatus"],
                    VCodeType = Request.QueryString["CodeType"],
                    DateCreate = Request.QueryString["DateCreate"],
                    EndDate = Request.QueryString["EndDate"],
                    ActivityId = activityId,
                    pageIndex = pageIndex,
                    pageSize = pageSize
                }).First<int>();
            return list;
        }

        /// <summary>
        /// 关联专题
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public ActionResult RelatedTopicsList(string activityId, int pageIndex = 1, int pageSize = 20)
        {
            int count = 0;
            ViewBag.list = RelatedTopicsLists(activityId, pageIndex, pageSize, out count);
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;
            ViewBag.TotalCount = count;
            return View();
        }

        public IEnumerable<SWfsTopics> RelatedTopicsLists(string activityId, int pageIndex, int pageSize, out int count)
        {
            if (activityId == null)
            {
                activityId = Request.QueryString["activityId"];
            }
            var dic = new Dictionary<string, object>();
            string TopicName = Request.QueryString["TopicName"];
            string Status = Request.QueryString["Status"];
            string RelatedStatus = Request.QueryString["RelatedStatus"];
            if (!string.IsNullOrEmpty(TopicName))
            {
                dic.Add("TopicName", TopicName);
            }
            else
            {
                dic.Add("TopicName", "");
            }
            if (Status != null && Status != "" && Status != "-1")
            {
                dic.Add("Status", Status);
            }
            else
            {
                dic.Add("Status", "");
            }
            if (RelatedStatus != null && RelatedStatus != "" && RelatedStatus != "-1")
            {
                dic.Add("RelatedStatus", RelatedStatus);
            }
            else
            {
                dic.Add("RelatedStatus", "");
            }
            IEnumerable<SWfsTopics> list = DapperUtil.Query<SWfsTopics>("ComBeziWfs_WfsCmsContent_SWfsTopics_List", dic, new { TopicName = TopicName, Status = Status, RelatedStatus = RelatedStatus, ActivityId = activityId, pageIndex = pageIndex, pageSize = pageSize });
            count = DapperUtil.Query<int>("ComBeziWfs_WfsCmsContent_SWfsTopics_List_Count", dic, new { TopicName = TopicName, Status = Status, RelatedStatus = RelatedStatus, ActivityId = activityId, pageIndex = pageIndex, pageSize = pageSize }).First<int>();
            return list;
        }

        /// <summary>
        /// 关联最新活动信息
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public ActionResult RelatedTopicsListV(string activityId, int pageIndex = 1, int pageSize = 20)
        {
            string TopicName = Request.QueryString["TopicName"];
            string Status = Request.QueryString["Status"];
            string RelatedStatus = Request.QueryString["RelatedStatus"];
            string acUrl = AppSettingManager.AppSettings["ActiveHrefUrlShow"];
            int count = 0;
            ViewBag.SWfsNewSubjectVCodeRefList = RelatedTopicsListsV(activityId, pageIndex, pageSize, out count);
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;
            ViewBag.TotalCount = count;
            ViewBag.ActivityIdShow = activityId;
            ViewBag.TopicName = TopicName;
            ViewBag.RelatedStatus = RelatedStatus;
            ViewBag.Status = Status;
            ViewBag.ActiveHrefUrlShow = acUrl;
            return View();
        }
        /// <summary>
        /// 关联最新活动信息 以上信息 
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public IEnumerable<SWfsNewSubjectVCodeRef> RelatedTopicsListsV(string activityId, int pageIndex, int pageSize, out int count)
        {
            if (activityId == null)
            {
                activityId = Request.QueryString["activityId"];
            }
            var dic = new Dictionary<string, object>();
            string TopicName = Request.QueryString["TopicName"];
            string Status = Request.QueryString["Status"];
            string RelatedStatus = Request.QueryString["RelatedStatus"];
            if (!string.IsNullOrEmpty(TopicName))
            {
                dic.Add("TopicName", TopicName);
            }
            else
            {
                dic.Add("TopicName", "");
            }
            if (Status != null && Status != "" && Status != "-1")
            {
                dic.Add("Status", Status);
            }
            else
            {
                dic.Add("Status", "");
            }
            if (RelatedStatus != null && RelatedStatus != "" && RelatedStatus != "-1")
            {
                dic.Add("RelatedStatus", RelatedStatus);
            }
            else
            {
                dic.Add("RelatedStatus", "");
            }
            IEnumerable<SWfsNewSubjectVCodeRef> list = DapperUtil.Query<SWfsNewSubjectVCodeRef>("ComBeziWfs_WfsCmsContent_SWfsNewSubject_List", dic, new { SubjectName = TopicName, Status = Status, RelatedStatus = RelatedStatus, ActivityId = activityId, pageIndex = pageIndex, pageSize = pageSize });
            count = DapperUtil.Query<int>("ComBeziWfs_WfsCmsContent_SWfsNewSubject_List_Count", dic, new { SubjectName = TopicName, Status = Status, RelatedStatus = RelatedStatus, ActivityId = activityId, pageIndex = pageIndex, pageSize = pageSize }).First<int>();
            return list;
        }

        /// <summary>
        /// 生成微码
        /// </summary>
        /// <param name="passport"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public ActionResult CreateVCode(SWfsVActivityCodesRef obj)
        {
            //ServiceResult result = new ServiceResult();
            string activityId = Request.Form["ActivityId"];
            int count = int.Parse(Request.Form["CreateCount"]);
            //V码类型，1：独享 2：共享
            short vcodeType = short.Parse(Request.Form["VCodeType"]);

            var dapp = DapperUtil.Query<SWfsVActivity>("ComBeziWfs_SWfsVActivity_ID", new { ActivityId = activityId });

            string code = string.Empty;
            for (int i = 0; i < count; i++)
            {
                code = dapp.FirstOrDefault().ActivityCode + GetRandomCodeII(6);
                //obj.ActivityCodesId = int.Parse(Request.QueryString["ActivityCodesId"]);
                obj.ActivityId = activityId;
                obj.VCode = code;
                obj.VCodeType = vcodeType;
                obj.Source = "系统生成";
                obj.DateCreate = DateTime.Now;
                obj.OperatorId = PresentationHelper.GetPassport().UserName;
                Vcode.CreateCrode(obj);
            }
            return Redirect("VCodeManagement.html?activityId=" + activityId);
        }
        /// <summary>
        /// 随机数
        /// </summary>
        /// <param name="N"></param>
        /// <returns></returns>
        public static string GetRandomCodeII(int N)
        {
            char[] arrChar = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
            StringBuilder num = new StringBuilder();
            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            for (int i = 0; i < N; i++)
            {
                num.Append(arrChar[rnd.Next(0, arrChar.Length)].ToString());
            }
            return num.ToString();
        }
        /// <summary>
        /// 导出V码
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ExportVCodeList(string activityId, string vCode, string userName, string useCodeStatus, string vCodeType, string dateCreate, string endDate)
        {
            var dic = new Dictionary<string, object>();
            #region 搜索条件

            dic.Add("VCode", vCode);
            ViewBag.KeyWord = vCode;

            dic.Add("OperatorId", userName);
            ViewBag.Prompt = userName;

            dic.Add("UseCodeStatus", useCodeStatus != "-1" ? useCodeStatus : "");

            dic.Add("VCodeType", vCodeType.Equals("0") ? "" : vCodeType);

            dic.Add("DateCreate", dateCreate);
            ViewBag.CreateDate = dateCreate;

            dic.Add("EndDate", endDate);
            ViewBag.EndCreateDate = endDate;
            #endregion
            //var CodeReflist = DapperUtil.Query<SWfsVActivityCodesRef>("ComBeziWfs_SWfsVActivityCodesRef_List",dic, new { ActivityId = activityId });
            var ByVCodeList = DapperUtil.Query<VCodeInfo>("ComBeziWfs_SwfsVActivityCodesRef_VCodeList", dic,
               new
               {
                   VCode = dic["VCode"].ToString(),
                   OperatorId = dic["OperatorId"].ToString(),
                   UseCodeStatus = dic["UseCodeStatus"].ToString(),
                   VCodeType = dic["VCodeType"].ToString(),
                   DateCreate = dic["DateCreate"].ToString(),
                   EndDate = dic["EndDate"].ToString(),
                   ActivityId = activityId
               });
            DataTable dt = new DataTable();
            string[] titles = "微码,用户名,邮箱,绑定状态,类型,生成时间,绑定时间".Split(',');
            string[] titleCode = "VCode,UserName,MailAddress,UseCodeStatus,VCodeType,DateCreate,BindTime".Split(',');            
            dt.Columns.Clear();
            foreach (string titleName in titles)
            {
                dt.Columns.Add(titleName, typeof(string));
            }
            foreach (VCodeInfo datarow in ByVCodeList)
            {
                dt.Rows.Add(datarow.VCode, datarow.OperatorId, datarow.MailAddress, datarow.UseCodeStatus == 0 ? "未绑定" : datarow.UseCodeStatus == 1 ? "已绑定" : "作废",
                    datarow.VCodeType == 1 ? "独享" : "共享", datarow.DateCreate, datarow.BindTime.ToString() == "0001/1/1 0:00:00" ? "" : datarow.BindTime.ToString());           
            }

            string ExcleSavePath = AppSettingManager.AppSettings["ExcleSavePath"].ToString();
            string ExcelFileWeb = AppSettingManager.AppSettings["ExcelFileWeb"].ToString();
            string name = "V码列表" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xls";
            string path = ExcleSavePath + "/" + name;
            CreateExecl(dt, path);

            return Json(new { result = "1", Url = ExcelFileWeb + "/" + name }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 生成Excel
        /// </summary>
        /// <param name="dtable">数据集</param>
        /// <param name="name">保存路径+文件名称</param>
        private void CreateExecl(DataTable dt, string name)
        {
            if (string.IsNullOrEmpty(name) || name.IndexOf('\\') < 0)
            {
                throw new Exception("文件路径错误！");
            }
            if (!Directory.Exists(name.Substring(0, name.LastIndexOf('\\'))))
            {
                Directory.CreateDirectory(name.Substring(0, name.LastIndexOf('\\')));
            }

            FileInfo FInfo = new FileInfo(name);
            try
            {
                StringBuilder str = new StringBuilder();
                StreamWriter sw = new StreamWriter(name, false, System.Text.Encoding.GetEncoding("gb2312"));


                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    str.Append(dt.Columns[i].ColumnName.ToString());
                    if (i < (dt.Columns.Count - 1))
                        str.Append("\t");
                }
                sw.WriteLine(str.ToString());
                int n = 0;

                StringBuilder txt = new StringBuilder();
                foreach (DataRow dr in dt.Rows)
                {
                    n++;
                    for (int j = 0; j < dr.Table.Columns.Count; j++)
                    {
                        txt.Append(dr[j] + "\t");
                    }
                    sw.WriteLine(txt.ToString());

                    txt.Remove(0, txt.Length);
                }
                sw.Flush();
                sw.Close();
            }
            catch (Exception e)
            {
                throw e;
            }
        }//CreateExecl
        /// <summary>
        /// 修改微码状态
        /// </summary>
        /// <param name="vcode"></param>
        /// <param name="status"></param>
        /// <param name="activityId"></param>
        /// <param name="activityid"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ActionResult UserCodeStatus(string vcode, int status, string activityid, string userId)
        {
            //string check = Request.QueryString["checks"];
            //string[] arry = check.Split(',');


            Vcode.UserCodeStatus(vcode, status, userId);
            return Redirect("VCodeManagement.html?activityId=" + activityid);
        }

        /// <summary>
        /// 保存活动(使用新的活动lxy)
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public ActionResult TopicRefCreate(SWfsVActivityTopicsRef obj, string activityId)
        {
            if (Request.Form["cb_KeyID"] != null)
            {
                obj.ActivityId = Request.Form["activityIds"];
                obj.OperatorId = PresentationHelper.GetPassport().UserName;
                obj.DateCreate = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd").Replace("/", ""));
                string[] arry = Request.Form["cb_KeyID"].Split(',');
                for (int i = 0; i < arry.Length; i++)
                {
                    obj.TopicNo = arry[i];
                    Vcode.TopicsRefCreate(obj);
                }
            }
            return Content("<script>alert('关联成功'); window.location.href='/Shangpin/VCode/RelatedTopicsListV.html?activityId=" + obj.ActivityId + "'</script>");
        }

        /// <summary>
        /// 取消关联(新版活动关联lxy)
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public ActionResult TopicRefDele(string topicno)
        {
            string activityId = Request.Form["activityIds"];
            topicno = Request.Form["cb_KeyID"];
            if (topicno != null)
            {
                string[] arry = topicno.Split(',');
                for (int i = 0; i < arry.Length; i++)
                {
                    string topicnoV = arry[i];
                    Vcode.TopicRefDelete(topicnoV, activityId);
                }
            }
            return Content("<script>alert('取消关联成功'); window.location.href='/Shangpin/VCode/RelatedTopicsListV.html?activityId=" + activityId + "'</script>");
        }

        /// <summary>
        /// 取消关联(新版活动关联lxy)
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult TopicRefDeleById(string SubjectNo, string ActivityId, int Status)
        {
            int count = 0;
            string mess = string.Empty;
            if (Status > 0)
            {

                //是否已经关联，如果关联不执行添加方法，直接count=1成功
                SWfsVActivityTopicsRef obj = Vcode.SelectSingleBySubjectNoRef(SubjectNo, ActivityId);
                if (obj == null)
                {
                    obj = new SWfsVActivityTopicsRef();

                    obj.ActivityId = ActivityId;
                    obj.OperatorId = PresentationHelper.GetPassport().UserName;
                    obj.DateCreate = DateTime.Now;
                    obj.TopicNo = SubjectNo;
                    count = Vcode.TopicsRefCreate(obj);
                }
                else { count = 1; }
                mess = "关联成功";
            }
            else
            { count = Vcode.TopicRefDelete(SubjectNo, ActivityId); mess = "取消关联"; }

            if (count > 0)
            {
                return Json(new { result = "1", message = mess });
            }
            else
            {
                return Json(new { result = "0", message = "修改失败！" });
            }
        }

    }
}
