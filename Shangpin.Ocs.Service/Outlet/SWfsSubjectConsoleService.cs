using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Entity.Wfs;
using Shangpin.Framework.Common;
using Shangpin.Entity.ComBeziPic;
using Shangpin.Ocs.Entity.Extenstion.Outlet;
using Shangpin.Entity.Common;
using Shangpin.Ocs.Service.Common;
using System.Text.RegularExpressions;
using System.Collections;
using Shangpin.Framework.Common.Cache;
using System.Threading.Tasks;

namespace Shangpin.Ocs.Service.Outlet
{
    public class SWfsSubjectConsoleService
    {
        #region 活动控制台列表

        #region 活动监控

        #region 站内预热
        /// <summary>
        /// 站外推广活动列表
        /// </summary>
        /// <param name="keyword">活动编号/名称</param>
        /// <param name="brandNo">品牌编号</param>
        /// <param name="channelSord">分类</param>
        /// <param name="bu">BU部门</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页码</param>
        /// <param name="startTime">查询开始时间</param>
        /// <param name="endTime">查询结束时间</param>
        /// <returns></returns>
        public RecordPage<SubjectInfo> SelectSubjectMonitorInPreheatList(string keyword, string brandNo, string channelSord, string bu = "", int pageIndex = 1, int pageSize = 10, string startTime = "", string endTime = "")
        {
            var dic = new Dictionary<string, object>();
            dic.Add("KeyWord", (keyword == null || keyword == "活动编号/名称") ? "" : keyword);
            dic.Add("BrandNo", (brandNo == null || brandNo == "品牌") ? "" : brandNo);
            dic.Add("ChannelSord", channelSord == null ? "" : channelSord);
            dic.Add("BU", bu);
            dic.Add("StartTime", startTime == null ? "" : startTime);
            dic.Add("EndTime", endTime == null ? "" : endTime);
            IEnumerable<SubjectInfo> query = DapperUtil.QueryPaging<SubjectInfo>("ComBeziWfs_SWfsSubject_SubjectMonitorInPreheatDataList", pageIndex, pageSize, "DateBegin desc", dic, new { KeyWord = keyword, ChannelSord = channelSord, BU = bu, BrandNo = brandNo, StartTime = startTime, EndTime = endTime });
            Dictionary<string, List<SWfsSubjectChannelSordRef>> dicSordRef = GetSordBySubjectNoList(query.Select(x => x.SubjectNo).ToArray());
            foreach (var subject in query)
            {
                subject.ChannelSordList = dicSordRef.Keys.Contains(subject.SubjectNo) ? dicSordRef[subject.SubjectNo] : null;
            }
            return PageConvertor.Convert(pageIndex, pageSize, query.ToList());
        }
        #endregion

        #region 站外推广
        /// <summary>
        /// 站外推广活动列表
        /// </summary>
        /// <param name="keyword">活动编号/名称</param>
        /// <param name="brandNo">品牌编号</param>
        /// <param name="channelSord">分类</param>
        /// <param name="bu">BU部门</param>
        /// <param name="ConfirmStartTime">推广确认开始时间</param>
        /// <param name="ConfirmEndTime">推广确认结束时间</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页码</param>
        /// <returns></returns>
        public RecordPage<SubjectInfo> SelectSubjectMonitorOutPromotionList(string keyword, string brandNo, string channelSord, string bu = "", string ConfirmStartTime = "", string ConfirmEndTime = "", int pageIndex = 1, int pageSize = 10)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("KeyWord", (keyword == null || keyword == "活动编号/名称") ? "" : keyword);
            dic.Add("BrandNo", (brandNo == null || brandNo == "品牌") ? "" : brandNo);
            dic.Add("ChannelSord", channelSord == null ? "" : channelSord);
            dic.Add("BU", bu);
            dic.Add("ConfirmStartTime", ConfirmStartTime);
            dic.Add("ConfirmEndTime", ConfirmEndTime);
            IEnumerable<SubjectInfo> query = DapperUtil.QueryPaging<SubjectInfo>("ComBeziWfs_SWfsSubject_SubjectMonitorOutPromotionDataList", pageIndex, pageSize, "PromotionConfirmTime desc", dic, new { KeyWord = keyword, ChannelSord = channelSord, BU = bu, BrandNo = brandNo, ConfirmStartTime = ConfirmStartTime, ConfirmEndTime = ConfirmEndTime });
            Dictionary<string, List<SWfsSubjectChannelSordRef>> dicSordRef = GetSordBySubjectNoList(query.Select(x => x.SubjectNo).ToArray());
            foreach (var subject in query)
            {
                subject.ChannelSordList = dicSordRef.Keys.Contains(subject.SubjectNo) ? dicSordRef[subject.SubjectNo] : null;
                subject.TotalHour = getTimeDiffer(subject.PromotionConfirmTime, DateTime.Now);
            }
            return PageConvertor.Convert(pageIndex, pageSize, query.ToList());
        }
        #endregion

        #region 已结束

        /// <summary>
        /// 已结束活动列表
        /// </summary>
        /// <param name="keyword">活动编号/名称</param>
        /// <param name="brandNo">品牌编号</param>
        /// <param name="channelSord">分类</param>
        /// <param name="bu">BU部门</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页码</param>
        /// <returns></returns>
        public RecordPage<SubjectInfo> SelectSubjectMonitorEndList(string keyword, string brandNo, string channelSord, string bu = "", int pageIndex = 1, int pageSize = 10)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("KeyWord", (keyword == null || keyword == "活动编号/名称") ? "" : keyword);
            dic.Add("BrandNo", (brandNo == null || brandNo == "品牌") ? "" : brandNo);
            dic.Add("ChannelSord", channelSord == null ? "" : channelSord);
            dic.Add("BU", bu);
            IEnumerable<SubjectInfo> query = DapperUtil.QueryPaging<SubjectInfo>("ComBeziWfs_SWfsSubject_SubjectMonitorEndDataList", pageIndex, pageSize, "DateEnd desc", dic, new { KeyWord = keyword, ChannelSord = channelSord, BU = bu, BrandNo = brandNo });
            Dictionary<string, List<SWfsSubjectChannelSordRef>> dicSordRef = GetSordBySubjectNoList(query.Select(x => x.SubjectNo).ToArray());
            foreach (var subject in query)
            {
                subject.ChannelSordList = dicSordRef.Keys.Contains(subject.SubjectNo) ? dicSordRef[subject.SubjectNo] : null;
            }
            return PageConvertor.Convert(pageIndex, pageSize, query.ToList());
        }

        #endregion

        #region 获取活动数量
        /// <summary>
        /// 获取活动控制台活动数量
        /// </summary>
        /// <param name="noType">排除统计 1今日新开，2进行中，3站内预热，4站外推广</param>
        /// <param name="todayNum">输出今日新开活动数量</param>
        /// <param name="runNum">输出进行中活动数量</param>
        /// <param name="inNum">输出站内预热活动数量</param>
        /// <param name="outNum">输出站外推广活动数量</param>
        public void GetSubjectConsoleCount(int noType, out int todayNum, out int runNum, out int inNum, out int outNum)
        {
            todayNum = 0;
            runNum = 0;
            inNum = 0;
            outNum = 0;
            if (noType != 1)
            {
                IEnumerable<string> listNum = DapperUtil.Query<string>("ComBeziWfs_SWfsSubject_SubjectMonitorTodayCount");
                if (listNum != null && listNum.Count() > 0)
                {
                    #region 排除重复今日新开广告活动

                    /*今日新开广告图*/
                    List<SWfsPictureManager> picADList = GetIndexADPicListALL(3, new int[] { 10 });
                    SWfsPictureManager todayAdModel = picADList.Where(r => r.Position.Equals(10)).OrderByDescending(r => r.DateCreate).FirstOrDefault();

                    //增加一个规则20140604如果今日新开的广告图关联的活动与今日新开中的活动重复，则排除掉今日新开中的活动
                    if (todayAdModel != null)
                    {
                        string adSubjectNo = GetSubjectNoByLink(todayAdModel.LinkAddress);
                        listNum = listNum.Where(r => r != adSubjectNo).ToList();
                    }

                    #endregion

                    #region 排除专题重复活动

                    List<SubjectTopicM> topPics = new List<SubjectTopicM>();
                    //修改逻辑如下20140515：
                    List<SubjectTopicM> tmptoplist = new List<SubjectTopicM>();
                    List<SubjectTopicM> toplist = GetFoucsAreaSubjectList();
                    if (toplist == null || toplist.Count() < 3)
                    {
                        List<string> subNos = new List<string>();
                        if (toplist != null) subNos = toplist.Select(t => t.SubjectNo).ToList();
                        int tmpNum = (toplist != null ? 3 - toplist.Count() : 3);
                        tmptoplist = GetSubjectTopicListNewNormal(tmpNum, subNos);//20140515修改为读取正常开启专题类型的活动
                    }
                    if (tmptoplist != null && toplist != null)
                    {
                        toplist.AddRange(tmptoplist);
                    }

                    topPics = (toplist == null ? tmptoplist : toplist); //前台判断链接

                    string[] subNosNew = null;
                    if (topPics != null)
                    {
                        subNosNew = topPics.Select(p => p.SubjectNo).ToArray();
                        listNum = (from today in listNum
                                   where !subNosNew.Contains(today)
                                   select today).ToList();
                    }
                    #endregion

                    todayNum = listNum != null && listNum.Count() > 0 ? listNum.Count() : 0;
                }
            }
            if (noType != 2)
            {
                runNum = DapperUtil.Query<int>("ComBeziWfs_SWfsSubject_SubjectMonitorRunCount").FirstOrDefault();
            }
            if (noType != 3)
            {
                inNum = DapperUtil.Query<int>("ComBeziWfs_SWfsSubject_SubjectMonitorInCount").FirstOrDefault();
            }
            if (noType != 4)
            {
                outNum = DapperUtil.Query<int>("ComBeziWfs_SWfsSubject_SubjectMonitorOutCount").FirstOrDefault();
            }
        }
        private string GetSubjectNoByLink(string linkUrl)
        {
            string subjectNo = string.Empty;
            if (string.IsNullOrEmpty(linkUrl))
            {
                return subjectNo;
            }
            if (linkUrl.ToLower().Contains("aolai.com/subject/index"))
            {
                string[] tempArry = linkUrl.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                Regex reg = new Regex(@"^\d{8}$", RegexOptions.IgnoreCase);
                foreach (var item in tempArry)
                {
                    if (reg.IsMatch(item))
                    {
                        subjectNo = item;
                        break;
                    }
                }
            }
            return subjectNo;
        }
        #endregion

        #endregion

        #region 活动管理
        /// <summary>
        /// 活动管理-全部活动
        /// </summary>
        /// <param name="keyword">活动编号/名称</param>
        /// <param name="productNo">商品编号</param>
        /// <param name="brandNo">品牌编号</param>
        /// <param name="status">活动状态</param>
        /// <param name="channelSord">分类</param>
        /// <param name="channelNo"渠道></param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="pageIndex"当前页></param>
        /// <param name="pageSize">页码</param>
        /// <param name="subjectTemplate">模板类型</param>
        /// <param name="isaudited">发布状态</param>
        /// <param name="bu">BU部门</param>
        /// <param name="runStatus">运行状态</param>
        /// <param name="subjectNos">活动编号</param>
        /// <returns></returns>
        public RecordPage<SubjectInfo> SelectSubjectList(string keyword, string productNo, string brandNo, string status, string channelSord, string channelNo, string startTime, string endTime, int pageIndex, int pageSize, string subjectTemplate = "", string isaudited = "", string bu = "", string runStatus = "", IList<string> subjectNos = null)
        {
            var dic = new Dictionary<string, object>();

            dic.Add("KeyWord", (keyword == null || keyword == "活动编号/名称") ? "" : keyword.Trim());
            dic.Add("ProductNo", (productNo == null || productNo == "商品编号") ? "" : productNo.Trim());
            dic.Add("BrandNo", (brandNo == null || brandNo == "品牌") ? "" : brandNo);
            dic.Add("Status", status == null ? "" : status);
            dic.Add("ChannelSord", channelSord == null ? "" : channelSord);
            dic.Add("ChannelNo", channelNo == null ? "" : channelNo);
            dic.Add("StartTime", startTime == null ? "" : startTime);
            dic.Add("EndTime", endTime == null ? "" : endTime);
            dic.Add("SubjectTemplate", subjectTemplate);
            dic.Add("BU", bu);
            dic.Add("IsAudited", isaudited);
            dic.Add("RunStatus", runStatus);
            object obj = null;
            if (subjectNos != null && subjectNos.Count() > 0)
            {
                dic.Add("SubjectNoArray", subjectNos);
                obj = new { KeyWord = keyword.Trim(), ProductNo = productNo.Trim(), Status = status, ChannelSord = channelSord, ChannelNo = channelNo, StartTime = startTime, EndTime = endTime, SubjectTemplate = subjectTemplate, BU = bu, IsAudited = isaudited, BrandNo = brandNo, SubjectNoArray = subjectNos };
            }
            else
            {
                dic.Add("SubjectNoArray", "");
                obj = new { KeyWord = keyword.Trim(), ProductNo = productNo.Trim(), Status = status, ChannelSord = channelSord, ChannelNo = channelNo, StartTime = startTime, EndTime = endTime, SubjectTemplate = subjectTemplate, BU = bu, IsAudited = isaudited, BrandNo = brandNo, SubjectNoArray = "" };
            }
            IEnumerable<SubjectInfo> query = DapperUtil.QueryPaging<SubjectInfo>("ComBeziWfs_SWfsSubject_SubjectConsoleList", pageIndex, pageSize, "DateCreate desc,DateBegin desc", dic, obj);
            Dictionary<string, List<SWfsSubjectChannelSordRef>> dicSordRef = GetSordBySubjectNoList(query.Select(x => x.SubjectNo).ToArray());

            foreach (var subject in query)
            {
                subject.ChannelNo = getChannel(subject.ChannelNo);
                subject.ChannelSordList = dicSordRef.Keys.Contains(subject.SubjectNo) ? dicSordRef[subject.SubjectNo] : null;
                subject.TotalHour = getTimeDiffer(subject.DateEnd, subject.DateBegin);
            }
            return PageConvertor.Convert(pageIndex, pageSize, query.ToList());
        }

        /// <summary>
        /// 活动管理-预热活动
        /// </summary>
        /// <param name="keyword">活动编号/名称</param>
        /// <param name="productNo">商品编号</param>
        /// <param name="brandNo">品牌编号</param>
        /// <param name="status">活动状态</param>
        /// <param name="channelSord">分类</param>
        /// <param name="channelNo"渠道></param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="pageIndex"当前页></param>
        /// <param name="pageSize">页码</param>
        /// <param name="subjectTemplate">模板类型</param>
        /// <param name="isaudited">发布状态</param>
        /// <param name="bu">BU部门</param>
        /// <returns></returns>
        public RecordPage<SubjectInfo> SelectSubjectPreheatList(string keyword, string productNo, string brandNo, string status, string channelSord, string channelNo, string startTime, string endTime, int pageIndex, int pageSize, string subjectTemplate = "", string isaudited = "", string bu = "", string runStatus = "")
        {
            var dic = new Dictionary<string, object>();
            dic.Add("KeyWord", (keyword == null || keyword == "活动编号/名称") ? "" : keyword);
            dic.Add("ProductNo", (productNo == null || productNo == "商品编号") ? "" : productNo.Trim());
            dic.Add("BrandNo", (brandNo == null || brandNo == "品牌") ? "" : brandNo);
            dic.Add("Status", status == null ? "" : status);
            dic.Add("ChannelSord", channelSord == null ? "" : channelSord);
            dic.Add("ChannelNo", channelNo == null ? "" : channelNo);
            dic.Add("StartTime", startTime == null ? "" : startTime);
            dic.Add("EndTime", endTime == null ? "" : endTime);
            dic.Add("SubjectTemplate", subjectTemplate);
            dic.Add("BU", bu);
            dic.Add("IsAudited", isaudited);
            dic.Add("RunStatus", runStatus);
            IEnumerable<SubjectInfo> query = DapperUtil.QueryPaging<SubjectInfo>("ComBeziWfs_SWfsSubject_SubjectConsolePreheatList", pageIndex, pageSize, "DateCreate desc,DateBegin desc", dic, new { KeyWord = keyword.Trim(), ProductNo = productNo.Trim(), Status = status, ChannelSord = channelSord, ChannelNo = channelNo, StartTime = startTime, EndTime = endTime, SubjectTemplate = subjectTemplate, BU = bu, IsAudited = isaudited, BrandNo = brandNo });
            Dictionary<string, List<SWfsSubjectChannelSordRef>> dicSordRef = GetSordBySubjectNoList(query.Select(x => x.SubjectNo).ToArray());

            foreach (var subject in query)
            {
                subject.ChannelNo = getChannel(subject.ChannelNo);
                subject.ChannelSordList = dicSordRef.Keys.Contains(subject.SubjectNo) ? dicSordRef[subject.SubjectNo] : null;
                subject.TotalHour = getTimeDiffer(subject.DateEnd, subject.DateBegin);
            }
            return PageConvertor.Convert(pageIndex, pageSize, query.ToList());
        }

        /// <summary>
        /// 计算时间间隔
        /// </summary>
        /// <param name="dt1"></param>
        /// <param name="dt2"></param>
        /// <returns></returns>
        private static double getTimeDiffer(DateTime dt1, DateTime dt2)
        {
            return dt2.Subtract(dt1).Duration().TotalHours;
        }
        /// <summary>
        /// 改变展示渠道序号为名字
        /// </summary>
        /// <param name="Msg"></param>
        /// <returns></returns>
        private static string getChannel(string Msg)
        {
            return Msg.Replace("zsqd001", "网站").Replace("zsqd002", "移动端").Replace("zsqd003", "其他渠道");
        }
        /// <summary>
        /// 根据活动编号获得所属分类
        /// </summary>
        /// <param name="subjectNo"></param>
        /// <returns></returns>
        public Dictionary<string, List<SWfsSubjectChannelSordRef>> GetSordBySubjectNoList(Array subjectNoLsit)
        {
            Dictionary<string, List<SWfsSubjectChannelSordRef>> dic = new Dictionary<string, List<SWfsSubjectChannelSordRef>>();
            List<SWfsSubjectChannelSordRef> lists = DapperUtil.Query<SWfsSubjectChannelSordRef>("ComBeziWfs_SWfsSubjectChannelSordRef_SelectBySubjectNoList", new { SubjectNoList = subjectNoLsit }).ToList();
            foreach (var item in lists)
            {
                if (dic.Keys.Contains(item.SubjectNo))
                {
                    dic[item.SubjectNo].Add(item);
                }
                else
                {
                    List<SWfsSubjectChannelSordRef> newlsit = new List<SWfsSubjectChannelSordRef>();
                    newlsit.Add(item);
                    dic.Add(item.SubjectNo, newlsit);
                }
            }
            return dic;

        }

        /// <summary>
        /// 根据活动编号获得所属商品品类
        /// </summary>
        /// <param name="subjectNo">活动编号</param>
        /// <returns></returns>
        public Dictionary<string, List<SWfsSubjectCategoryRef>> GetCategoryBySubjectNoList(Array subjectNoLsit)
        {
            Dictionary<string, List<SWfsSubjectCategoryRef>> dic = new Dictionary<string, List<SWfsSubjectCategoryRef>>();
            List<SWfsSubjectCategoryRef> lists = DapperUtil.Query<SWfsSubjectCategoryRef>("ComBeziWfs_SWfsSubjectCategoryRef_SelectCategoryBySubjectNoList", new { SubjectNoList = subjectNoLsit }).ToList();
            foreach (var item in lists)
            {
                if (dic.Keys.Contains(item.SubjectNo))
                {
                    dic[item.SubjectNo].Add(item);
                }
                else
                {
                    List<SWfsSubjectCategoryRef> newlsit = new List<SWfsSubjectCategoryRef>();
                    newlsit.Add(item);
                    dic.Add(item.SubjectNo, newlsit);
                }
            }
            return dic;
        }

        /// <summary>
        /// 根据活动编号查询所属频道
        /// </summary>
        /// <param name="subjectNoLsit"></param>
        /// <returns></returns>
        public Dictionary<string, List<SWfsChannel>> GetChannelBySubjectList(Array subjectNoLsit)
        {

            var dic = new Dictionary<string, List<SWfsChannel>>();
            List<SWfsChannel> data = DapperUtil.Query<SWfsChannel>("ComBeziWfs_SWfsChannel_ReadBySubjectNoList", new { SubjectNoList = subjectNoLsit }).ToList();
            foreach (SWfsChannel item in data)
            {
                string[] msg = item.ChannelNo.Split('|');
                string subjectNo = msg[0];
                item.ChannelNo = msg[1];
                if (dic.Keys.Contains(msg[0]))
                {
                    dic[subjectNo].Add(item);
                }
                else
                {
                    List<SWfsChannel> newitem = new List<SWfsChannel>();
                    newitem.Add(item);
                    dic.Add(subjectNo, newitem);
                }
            }
            return dic;
        }

        /// <summary>
        /// 根据活动编号获取所属商品数量
        /// </summary>
        /// <param name="subjectNo">活动编号，多个以逗号分隔</param>
        /// <returns></returns>
        public List<SubjectConsoleProductM> GetSubjectConsoleProductCount(string[] subjectNo)
        {
            List<SubjectConsoleProductM> list = DapperUtil.Query<SubjectConsoleProductM>("ComBeziWfs_SWfsSubjectProductRef_ConsoleSubjectProductNum", new { SubjectNo = subjectNo }).ToList();
            return list;
        }

        /// <summary>
        /// 修改活动信息
        /// </summary>
        /// <param name="type">1修改活动时间，2审核</param>
        /// <param name="subject"></param>
        /// <returns></returns>
        public bool UpdateSubjectItem(int type, SubjectInfo subject)
        {
            bool tf = false;
            if (type == 1)
            {
                tf = DapperUtil.UpdatePartialColumns<SWfsSubject>(new
                {
                    SubjectNo = subject.SubjectNo,
                    DateBegin = subject.DateBegin,
                    DateEnd = subject.DateEnd
                });
            }
            else if (type == 2)
            {
                tf = DapperUtil.UpdatePartialColumns<SWfsSubject>(new
                {
                    SubjectNo = subject.SubjectNo,
                    IsAudited = subject.IsAudited,
                    AuditedDateTime = subject.AuditedDateTime,
                    AuditedUserId = subject.AuditedUserId
                });
            }
            return tf;
        }

        /// <summary>
        /// 修改活动信息
        /// </summary>
        /// <param name="subject"></param>
        /// <returns></returns>
        public bool UpdateSubjectPreheatItem(SWfsSubjectTopExpand model)
        {
            return DapperUtil.UpdatePartialColumns<SWfsSubjectTopExpand>(new
            {
                SubjectNo = model.SubjectNo,
                PreheatTime = model.PreheatTime
            });
        }

        /// <summary>
        /// 获取活动预热提醒人数
        /// </summary>
        /// <param name="arrayNo">活动编号，多个以逗号隔开</param>
        /// <returns></returns>
        public List<SubjectConsoleProductM> GetSubjectSaleWarnCount(string[] arrayNo)
        {
            List<SubjectConsoleProductM> list = DapperUtil.Query<SubjectConsoleProductM>("ComBeziWfs_WfsSaleWarn_GetSaleWarnCount", new { SubjectNo = arrayNo }).ToList();
            return list;
        }

        /// <summary>
        /// 获取活动UV流量总数
        /// </summary>
        /// <param name="arrayNo">活动编号，多个以逗号隔开</param>
        /// <returns></returns>
        public List<SubjectUVCountM> GetSubjectUVResultCount(string[] arrayNo)
        {
            List<SubjectUVCountM> list = DapperUtil.Query<SubjectUVCountM>("ComBeziWfs_SWfsSubjectUVFluxForm_GetSubjectUVResultCount", new { SubjectNo = arrayNo }).ToList();
            return list;
        }
        /// <summary>
        /// 获取活动UV流量列表
        /// </summary>
        /// <param name="subjectNo">活动编号</param>
        /// <returns></returns>
        public List<SubjectUVFromM> GetSubjectUVFromList(string subjectNo, string startDateTime, string endDateTime)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("SubjectNo", subjectNo);
            dic.Add("StartDateTime", startDateTime);
            dic.Add("EndDateTime", endDateTime);
            List<SubjectUVFromM> list = DapperUtil.Query<SubjectUVFromM>("ComBeziWfs_SWfsSubjectUVFluxForm_GetSubjectUVFluxForm", dic, new { SubjectNo = subjectNo, StartDateTime = startDateTime, EndDateTime = endDateTime }).ToList();
            return list;
        }

        /// <summary>
        /// 删除活动
        /// </summary>
        /// <param name="subjectNo">活动编号</param>
        /// <returns></returns>
        public int UpdateSubjectStatusToDel(string subjectNo)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("SubjectNo", subjectNo);
            bool flag = DapperUtil.UpdatePartialColumns<SWfsSubject>(new
              {
                  SubjectNo = subjectNo,
                  Status = 3
              });
            return flag ? 1 : 0;
        }
        #endregion

        #region 活动排期
        /// <summary>
        /// 活动排期列表
        /// </summary>
        /// <param name="brandNo">品牌编号</param>
        /// <param name="channelSord">分类编号</param>
        /// <param name="startTime">查询开始时间</param>
        /// <param name="endTime">查询结束时间</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页码</param>
        /// <returns></returns>
        public RecordPage<SubjectInfo> GetSubjectWaitingListDataOld(string brandNo, string channelSord, string startTime, string endTime, int pageIndex, int pageSize)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("BrandNo", (brandNo == null || brandNo == "品牌") ? "" : brandNo);
            dic.Add("ChannelSord", channelSord == null ? "" : channelSord);
            dic.Add("StartTime", startTime == null ? "" : startTime);
            dic.Add("EndTime", endTime == null ? "" : endTime);
            //IEnumerable<SubjectInfo> query = DapperUtil.QueryPaging<SubjectInfo>("ComBeziWfs_SWfsSubject_SubjectWaitingList", pageIndex, pageSize, "DateCreate desc,DateBegin desc", dic, new { BrandNo = brandNo, ChannelSord = channelSord, StartTime = startTime, EndTime = endTime });
            IEnumerable<SubjectInfo> query = DapperUtil.Query<SubjectInfo>("ComBeziWfs_SWfsSubject_SubjectWaitingList", dic, new { BrandNo = brandNo, ChannelSord = channelSord, StartTime = startTime, EndTime = endTime });
            Dictionary<string, List<SWfsSubjectChannelSordRef>> dicSordRef = GetSordBySubjectNoList(query.Select(x => x.SubjectNo).ToArray());
            if (query != null && query.Count() > 0)
            {
                foreach (var subject in query)
                {
                    subject.ChannelSordList = dicSordRef.Keys.Contains(subject.SubjectNo) ? dicSordRef[subject.SubjectNo] : null;
                }
            }
            return PageConvertor.Convert(pageIndex, pageSize, query.ToList());
        }
        /// <summary>
        /// 活动排期列表
        /// </summary>
        /// <param name="brandNo">品牌编号</param>
        /// <param name="channelSord">分类编号</param>
        /// <param name="startTime">查询开始时间</param>
        /// <param name="endTime">查询结束时间</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页码</param>
        /// <returns></returns>
        public List<SubjectInfo> GetSubjectWaitingListData(string brandNo, string channelSord, string startTime, string endTime, int pageIndex, int pageSize)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("BrandNo", (brandNo == null || brandNo == "品牌") ? "" : brandNo);
            dic.Add("ChannelSord", channelSord == null ? "" : channelSord);
            dic.Add("StartTime", startTime == null ? "" : startTime);
            dic.Add("EndTime", endTime == null ? "" : endTime);
            List<SubjectInfo> resultList = new List<SubjectInfo>();
            List<SubjectInfo> tempList = new List<SubjectInfo>();
            //IEnumerable<SubjectInfo> query = DapperUtil.QueryPaging<SubjectInfo>("ComBeziWfs_SWfsSubject_SubjectWaitingList", pageIndex, pageSize, "DateCreate desc,DateBegin desc", dic, new { BrandNo = brandNo, ChannelSord = channelSord, StartTime = startTime, EndTime = endTime });
            IEnumerable<SubjectInfo> query = DapperUtil.Query<SubjectInfo>("ComBeziWfs_SWfsSubject_SubjectWaitingList", dic, new { BrandNo = brandNo, ChannelSord = channelSord, StartTime = startTime, EndTime = endTime });
            Dictionary<string, List<SWfsSubjectChannelSordRef>> dicSordRef = GetSordBySubjectNoList(query.Select(x => x.SubjectNo).ToArray());
            if (query != null && query.Count() > 0)
            {
                foreach (var subject in query)
                {
                    subject.ChannelSordList = dicSordRef.Keys.Contains(subject.SubjectNo) ? dicSordRef[subject.SubjectNo] : null;
                }
            }
            return query.ToList();
        }
        #endregion

        #region 检查是否有相同的品牌和品类
        /// <summary>
        /// 检查是否有相同的品牌和品类
        /// </summary>
        /// <param name="subjectNo">活动编号</param>
        /// <param name="brandNo">品牌编号</param>
        /// <param name="categoryList">当前操作活动品类</param>
        /// <param name="startTime">查询开始时间</param>
        /// <param name="endTime">查询结束时间</param>
        /// <returns></returns>
        public bool GetSubjectDiffBrandClassListData(string subjectNo, string brandNo, List<SWfsSubjectCategoryRef> childCategoryList, string startTime, string endTime)
        {
            bool flag = false;
            var dic = new Dictionary<string, object>();
            dic.Add("SubjectNo", string.IsNullOrEmpty(subjectNo) ? "" : subjectNo);
            dic.Add("BrandNo", string.IsNullOrEmpty(brandNo) ? "" : brandNo);
            dic.Add("StartTime", string.IsNullOrEmpty(startTime) ? "" : startTime);
            dic.Add("EndTime", string.IsNullOrEmpty(endTime) ? "" : endTime);
            IEnumerable<SubjectInfo> query = DapperUtil.Query<SubjectInfo>("ComBeziWfs_SWfsSubject_CheckSubjectDiffBrandClass", dic, new { SubjectNo = subjectNo, BrandNo = brandNo, StartTime = startTime, EndTime = endTime });
            if (query != null && query.Count() > 0)
            {
                Dictionary<string, List<SWfsSubjectCategoryRef>> dicSordRef = GetCategoryBySubjectNoList(query.Select(x => x.SubjectNo).ToArray());
                if (dicSordRef != null && dicSordRef.Count() > 0)
                {
                    foreach (var subject in query)
                    {
                        subject.ChannelCategoryList = dicSordRef.Keys.Contains(subject.SubjectNo) ? dicSordRef[subject.SubjectNo] : null;
                        if (childCategoryList != null && childCategoryList.Count() > 0)
                        {
                            IList<SWfsSubjectCategoryRef> parentCategoryList = subject.ChannelCategoryList;
                            int yesNum = 0;
                            int noNum = 0;
                            foreach (var item in childCategoryList)
                            {
                                int num = parentCategoryList.Where(a => a.Category.Equals(item.Category)).Count();
                                if (num > 0)
                                {
                                    yesNum++;
                                }
                                else
                                {
                                    noNum++;
                                }
                            }
                            if (yesNum > 0 && noNum == 0)
                            {
                                flag = true;  //冲突
                                return flag;
                            }
                        }
                    }
                }
            }
            return flag;
        }
        #endregion

        #endregion

        #region 获取频道下的活动
        /// <summary>
        /// 根据频道编号查询活动列表
        /// </summary>
        /// <param name="channelNo">频道编号</param>
        /// <param name="showType">展示渠道(zsqd001网站,zsqd002移动端)</param>
        /// <returns></returns>
        public IEnumerable<SubjectInfo> GetChannelSubjectList(string channelNo, string showType)
        {
            if (!string.IsNullOrEmpty(channelNo))
            {
                var dic = new Dictionary<string, object>();
                dic.Add("ShowType", !string.IsNullOrEmpty(showType) ? showType : "");
                IEnumerable<SubjectInfo> subjectlist = DapperUtil.Query<SubjectInfo>("ComBeziWfs_SWfsSubject_SelectByChannelNoNew", dic, new { ChannelNo = channelNo, ShowType = showType });
                return subjectlist;
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region  获取首页广告列表 by zhangwei 20140620
        /// <summary>
        /// 获取首页广告列表
        /// </summary>
        /// <param name="gender">频道 3首页</param>
        /// <param name="adPostion">位置编号</param>
        /// <returns></returns>
        public List<SWfsPictureManager> GetIndexADPicListALL(int gender, int[] adPostion)
        {
            return DapperUtil.Query<SWfsPictureManager>("ComBeziWfs_SWfsPictureManager_SelectPicListALL", new { Gender = gender, Position = adPostion }).ToList();
        }
        #endregion

        #region 获取首页专题活动 by zhangwei 20140620
        /// <summary>
        /// 获取首页专题活动
        /// </summary>
        /// <returns></returns>
        public List<SubjectTopicM> GetFoucsAreaSubjectListOld()
        {
            int count = 3;
            var dic = new Dictionary<string, object>();
            dic.Add("TopCount", count);
            List<SubjectTopicM> list = DapperUtil.Query<SubjectTopicM>("ComBeziWfs_SWfsSubjectFocusArea_GetFocusAreaInfo", dic).FilterList();
            return list;
        }

        /// <summary>
        /// 获取轮播图数据
        /// </summary>
        /// <returns></returns>
        public List<SubjectTopicM> GetFoucsAreaSubjectList()
        {
            int count = 3;
            List<SubjectTopicM> list = DapperUtil.Query<SubjectTopicM>("ComBeziWfs_SWfsSubjectFocusArea_GetFocusAreaInfoOutlet").FilterList();
            List<SubjectTopicM> _list = new List<SubjectTopicM>();
            if (list != null && list.Count() > 0)
            {
                foreach (SubjectTopicM TM in list)
                {
                    if (TM != null)
                    {
                        if (_list.Count() < count)
                        {
                            if (TM.Type == 1)
                            {
                                if (TM.SubjectTemplate == 4 && TM.IsAudited == 1 && TM.Status == 1 && TM.SubjectType == 0 && TM.IsRelated == 0 && TM.ChannelNo.StartsWith("zsqd001") && TM.DateEnd > DateTime.Now)
                                {
                                    _list.Add(TM);
                                }
                            }
                            else if (TM.Type == 2)
                            {
                                SubjectTopicM entity = new SubjectTopicM();
                                entity.BelongsSubjectPic = TM.WebPic;
                                entity.CustomUrl = TM.WebUrl;
                                entity.Type = TM.Type;
                                _list.Add(entity);
                            }
                        }
                    }
                }

                list = _list;
            }

            return list;
        }
        #endregion

        #region  应运营部门520活动要求更改为读取正常开启的专题活动
        /// <summary>
        /// 2014-5-15 应运营部门520活动要求更改为读取正常开启的专题活动
        /// </summary>
        /// <returns></returns>
        public List<SubjectTopicM> GetSubjectTopicListNewNormal(int num, List<string> subNos)
        { 
            //配置中读取要显示的专题数量
            int count = num;

            //先获取置顶的专题（根据DateTop desc置顶日期排序）
            var dic = new Dictionary<string, object>();
            dic.Add("TopCount", count);
            List<SubjectTopicM> list = DapperUtil.Query<SubjectTopicM>("ComBeziWfs_SWfsSubjectTopExpand_GetSubjectTopicListNormal", dic, new { ChannelNo = "zsqd001", SubjectNo = subNos.ToArray() }).FilterList();
            return list;
        }
        #endregion

        #region 奥莱频道排序
        /// <summary>
        /// 频道主推活动
        /// </summary>
        /// <param name="channelNo">频道编号</param>
        /// <returns></returns>
        public List<SubjectInfo> GetChannelMainSubjectList(string channelNo)
        {
            if (string.IsNullOrEmpty(channelNo))
            {
                return null;
            }

            SWfsChannel channel = DapperUtil.QueryByIdentity<SWfsChannel>(channelNo);
            if (channel == null)
            {
                return null;
            }

            List<SubjectInfo> subjectlist = DapperUtil.Query<SubjectInfo>("ComBeziWfs_SWfsSubject_FindChannelMainSubject", new { ChannelNo = channelNo }).ToList();

            return subjectlist;
        }
        #endregion

        #region 活动控制台_EP by zhangman 20141015

        #region 站内预热
        /// <summary>
        /// 站外推广活动列表
        /// </summary>
        /// <param name="keyword">活动编号/名称</param>
        /// <param name="brandNo">品牌编号</param>
        /// <param name="channelSord">分类</param>
        /// <param name="bu">BU部门</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页码</param>
        /// <param name="startTime">查询开始时间</param>
        /// <param name="endTime">查询结束时间</param>
        /// <returns></returns>
        public RecordPage<SubjectInfo> SelectSubjectMonitorInPreheatList_EP(string keyword, string brandNo, string channelSord, string bu = "", int pageIndex = 1, int pageSize = 10, string startTime = "", string endTime = "")
        {
            var dic = new Dictionary<string, object>();
            dic.Add("KeyWord", (keyword == null || keyword == "活动编号/名称") ? "" : keyword);
            dic.Add("BrandNo", (brandNo == null || brandNo == "品牌") ? "" : brandNo);
            dic.Add("ChannelSord", channelSord == null ? "" : channelSord);
            dic.Add("BU", bu);
            dic.Add("StartTime", startTime == null ? "" : startTime);
            dic.Add("EndTime", endTime == null ? "" : endTime);
            IEnumerable<SubjectInfo> query = DapperUtil.QueryPaging<SubjectInfo>("ComBeziWfs_SWfsSubject_SubjectMonitorInPreheatDataList_EP", pageIndex, pageSize, "DateBegin desc", dic, new { KeyWord = keyword, ChannelSord = channelSord, BU = bu, BrandNo = brandNo, StartTime = startTime, EndTime = endTime });
            Dictionary<string, List<SWfsSubjectChannelSordRef>> dicSordRef = GetSordBySubjectNoList(query.Select(x => x.SubjectNo).ToArray());
            foreach (var subject in query)
            {
                subject.ChannelSordList = dicSordRef.Keys.Contains(subject.SubjectNo) ? dicSordRef[subject.SubjectNo] : null;
            }
            return PageConvertor.Convert(pageIndex, pageSize, query.ToList());
        }
        #endregion

        #region 站外推广
        /// <summary>
        /// 站外推广活动列表
        /// </summary>
        /// <param name="keyword">活动编号/名称</param>
        /// <param name="brandNo">品牌编号</param>
        /// <param name="channelSord">分类</param>
        /// <param name="bu">BU部门</param>
        /// <param name="ConfirmStartTime">推广确认开始时间</param>
        /// <param name="ConfirmEndTime">推广确认结束时间</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页码</param>
        /// <returns></returns>
        public RecordPage<SubjectInfo> SelectSubjectMonitorOutPromotionList_EP(string keyword, string brandNo, string channelSord, string bu = "", string ConfirmStartTime = "", string ConfirmEndTime = "", int pageIndex = 1, int pageSize = 10)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("KeyWord", (keyword == null || keyword == "活动编号/名称") ? "" : keyword);
            dic.Add("BrandNo", (brandNo == null || brandNo == "品牌") ? "" : brandNo);
            dic.Add("ChannelSord", channelSord == null ? "" : channelSord);
            dic.Add("BU", bu);
            dic.Add("ConfirmStartTime", ConfirmStartTime);
            dic.Add("ConfirmEndTime", ConfirmEndTime);
            IEnumerable<SubjectInfo> query = DapperUtil.QueryPaging<SubjectInfo>("ComBeziWfs_SWfsSubject_SubjectMonitorOutPromotionDataList_EP", pageIndex, pageSize, "PromotionConfirmTime desc", dic, new { KeyWord = keyword, ChannelSord = channelSord, BU = bu, BrandNo = brandNo, ConfirmStartTime = ConfirmStartTime, ConfirmEndTime = ConfirmEndTime });
            Dictionary<string, List<SWfsSubjectChannelSordRef>> dicSordRef = GetSordBySubjectNoList(query.Select(x => x.SubjectNo).ToArray());
            foreach (var subject in query)
            {
                subject.ChannelSordList = dicSordRef.Keys.Contains(subject.SubjectNo) ? dicSordRef[subject.SubjectNo] : null;
                subject.TotalHour = getTimeDiffer(subject.PromotionConfirmTime, DateTime.Now);
            }
            return PageConvertor.Convert(pageIndex, pageSize, query.ToList());
        }
        #endregion

        #region 已结束

        /// <summary>
        /// 已结束活动列表
        /// </summary>
        /// <param name="keyword">活动编号/名称</param>
        /// <param name="brandNo">品牌编号</param>
        /// <param name="channelSord">分类</param>
        /// <param name="bu">BU部门</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页码</param>
        /// <returns></returns>
        public RecordPage<SubjectInfo> SelectSubjectMonitorEndList_EP(string keyword, string brandNo, string channelSord, string bu = "", int pageIndex = 1, int pageSize = 10)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("KeyWord", (keyword == null || keyword == "活动编号/名称") ? "" : keyword);
            dic.Add("BrandNo", (brandNo == null || brandNo == "品牌") ? "" : brandNo);
            dic.Add("ChannelSord", channelSord == null ? "" : channelSord);
            dic.Add("BU", bu);
            IEnumerable<SubjectInfo> query = DapperUtil.QueryPaging<SubjectInfo>("ComBeziWfs_SWfsSubject_SubjectMonitorEndDataList_EP", pageIndex, pageSize, "DateEnd desc", dic, new { KeyWord = keyword, ChannelSord = channelSord, BU = bu, BrandNo = brandNo });
            Dictionary<string, List<SWfsSubjectChannelSordRef>> dicSordRef = GetSordBySubjectNoList(query.Select(x => x.SubjectNo).ToArray());
            foreach (var subject in query)
            {
                subject.ChannelSordList = dicSordRef.Keys.Contains(subject.SubjectNo) ? dicSordRef[subject.SubjectNo] : null;
            }
            return PageConvertor.Convert(pageIndex, pageSize, query.ToList());
        }

        #endregion

        #region 获取活动数量
        /// <summary>
        /// 获取活动控制台活动数量
        /// </summary>
        /// <param name="noType">排除统计 1今日新开，2进行中，3站内预热，4站外推广</param>
        /// <param name="todayNum">输出今日新开活动数量</param>
        /// <param name="runNum">输出进行中活动数量</param>
        /// <param name="inNum">输出站内预热活动数量</param>
        /// <param name="outNum">输出站外推广活动数量</param>
        public void GetSubjectConsoleCount_EP(int noType, out int todayNum, out int runNum, out int inNum, out int outNum)
        {
            todayNum = 0;
            runNum = 0;
            inNum = 0;
            outNum = 0;
            if (noType != 1)
            {
                IEnumerable<string> listNum = DapperUtil.Query<string>("ComBeziWfs_SWfsSubject_SubjectMonitorTodayCount_EP");
                if (listNum != null && listNum.Count() > 0)
                {
                    #region 排除重复今日新开广告活动

                    /*今日新开广告图*/
                    List<SWfsPictureManager> picADList = GetIndexADPicListALL(3, new int[] { 10 });
                    SWfsPictureManager todayAdModel = picADList.Where(r => r.Position.Equals(10)).OrderByDescending(r => r.DateCreate).FirstOrDefault();

                    //增加一个规则20140604如果今日新开的广告图关联的活动与今日新开中的活动重复，则排除掉今日新开中的活动
                    if (todayAdModel != null)
                    {
                        string adSubjectNo = GetSubjectNoByLink(todayAdModel.LinkAddress);
                        listNum = listNum.Where(r => r != adSubjectNo).ToList();
                    }

                    #endregion

                    #region 排除专题重复活动

                    List<SubjectTopicM> topPics = new List<SubjectTopicM>();
                    //修改逻辑如下20140515：
                    List<SubjectTopicM> tmptoplist = new List<SubjectTopicM>();
                    List<SubjectTopicM> toplist = GetFoucsAreaSubjectList();
                    if (toplist == null || toplist.Count() < 3)
                    {
                        List<string> subNos = new List<string>();
                        if (toplist != null) subNos = toplist.Select(t => t.SubjectNo).ToList();
                        int tmpNum = (toplist != null ? 3 - toplist.Count() : 3);
                        tmptoplist = GetSubjectTopicListNewNormal(tmpNum, subNos);//20140515修改为读取正常开启专题类型的活动
                    }
                    if (tmptoplist != null && toplist != null)
                    {
                        toplist.AddRange(tmptoplist);
                    }

                    topPics = (toplist == null ? tmptoplist : toplist); //前台判断链接

                    string[] subNosNew = null;
                    if (topPics != null)
                    {
                        subNosNew = topPics.Select(p => p.SubjectNo).ToArray();
                        listNum = (from today in listNum
                                   where !subNosNew.Contains(today)
                                   select today).ToList();
                    }
                    #endregion

                    todayNum = listNum != null && listNum.Count() > 0 ? listNum.Count() : 0;
                }
            }
            if (noType != 2)
            {
                runNum = DapperUtil.Query<int>("ComBeziWfs_SWfsSubject_SubjectMonitorRunCount_EP").FirstOrDefault();
            }
            if (noType != 3)
            {
                inNum = DapperUtil.Query<int>("ComBeziWfs_SWfsSubject_SubjectMonitorInCount_EP").FirstOrDefault();
            }
            if (noType != 4)
            {
                outNum = DapperUtil.Query<int>("ComBeziWfs_SWfsSubject_SubjectMonitorOutCount_EP").FirstOrDefault();
            }
        }

        #endregion

        #region 活动管理
        /// <summary>
        /// 活动管理-全部活动
        /// </summary>
        /// <param name="keyword">活动编号/名称</param>
        /// <param name="productNo">商品编号</param>
        /// <param name="brandNo">品牌编号</param>
        /// <param name="status">活动状态</param>
        /// <param name="channelSord">分类</param>
        /// <param name="channelNo"渠道></param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="pageIndex"当前页></param>
        /// <param name="pageSize">页码</param>
        /// <param name="subjectTemplate">模板类型</param>
        /// <param name="isaudited">发布状态</param>
        /// <param name="bu">BU部门</param>
        /// <param name="runStatus">运行状态</param>
        /// <param name="subjectNos">活动编号</param>
        /// <returns></returns>
        public RecordPage<SubjectInfo> SelectSubjectList_EP(string keyword, string productNo, string brandNo, string status, string channelSord, string channelNo, string startTime, string endTime, int pageIndex, int pageSize, string subjectTemplate = "", string isaudited = "", string bu = "", string runStatus = "", IList<string> subjectNos = null)
        {
            var dic = new Dictionary<string, object>();

            dic.Add("KeyWord", (keyword == null || keyword == "活动编号/名称") ? "" : keyword.Trim());
            dic.Add("ProductNo", (productNo == null || productNo == "商品编号") ? "" : productNo.Trim());
            dic.Add("BrandNo", (brandNo == null || brandNo == "品牌") ? "" : brandNo);
            dic.Add("Status", status == null ? "" : status);
            dic.Add("ChannelSord", channelSord == null ? "" : channelSord);
            dic.Add("ChannelNo", channelNo == null ? "" : channelNo);
            dic.Add("StartTime", startTime == null ? "" : startTime);
            dic.Add("EndTime", endTime == null ? "" : endTime);
            dic.Add("SubjectTemplate", subjectTemplate);
            dic.Add("BU", bu);
            dic.Add("IsAudited", isaudited);
            dic.Add("RunStatus", runStatus);
            object obj = null;
            if (subjectNos != null && subjectNos.Count() > 0)
            {
                dic.Add("SubjectNoArray", subjectNos);
                obj = new { KeyWord = keyword.Trim(), ProductNo = productNo.Trim(), Status = status, ChannelSord = channelSord, ChannelNo = channelNo, StartTime = startTime, EndTime = endTime, SubjectTemplate = subjectTemplate, BU = bu, IsAudited = isaudited, BrandNo = brandNo, SubjectNoArray = subjectNos };
            }
            else
            {
                dic.Add("SubjectNoArray", "");
                obj = new { KeyWord = keyword.Trim(), ProductNo = productNo.Trim(), Status = status, ChannelSord = channelSord, ChannelNo = channelNo, StartTime = startTime, EndTime = endTime, SubjectTemplate = subjectTemplate, BU = bu, IsAudited = isaudited, BrandNo = brandNo, SubjectNoArray = "" };
            }
            IEnumerable<SubjectInfo> query = DapperUtil.QueryPaging<SubjectInfo>("ComBeziWfs_SWfsSubject_SubjectConsoleList_EP", pageIndex, pageSize, "DateCreate desc,DateBegin desc", dic, obj);
            Dictionary<string, List<SWfsSubjectChannelSordRef>> dicSordRef = GetSordBySubjectNoList(query.Select(x => x.SubjectNo).ToArray());

            foreach (var subject in query)
            {
                subject.ChannelNo = getChannel(subject.ChannelNo);
                subject.ChannelSordList = dicSordRef.Keys.Contains(subject.SubjectNo) ? dicSordRef[subject.SubjectNo] : null;
                subject.TotalHour = getTimeDiffer(subject.DateEnd, subject.DateBegin);
            }
            return PageConvertor.Convert(pageIndex, pageSize, query.ToList());
        }

        /// <summary>
        /// 活动管理-预热活动
        /// </summary>
        /// <param name="keyword">活动编号/名称</param>
        /// <param name="productNo">商品编号</param>
        /// <param name="brandNo">品牌编号</param>
        /// <param name="status">活动状态</param>
        /// <param name="channelSord">分类</param>
        /// <param name="channelNo"渠道></param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="pageIndex"当前页></param>
        /// <param name="pageSize">页码</param>
        /// <param name="subjectTemplate">模板类型</param>
        /// <param name="isaudited">发布状态</param>
        /// <param name="bu">BU部门</param>
        /// <returns></returns>
        public RecordPage<SubjectInfo> SelectSubjectPreheatList_EP(string keyword, string productNo, string brandNo, string status, string channelSord, string channelNo, string startTime, string endTime, int pageIndex, int pageSize, string subjectTemplate = "", string isaudited = "", string bu = "", string runStatus = "")
        {
            var dic = new Dictionary<string, object>();
            dic.Add("KeyWord", (keyword == null || keyword == "活动编号/名称") ? "" : keyword);
            dic.Add("ProductNo", (productNo == null || productNo == "商品编号") ? "" : productNo.Trim());
            dic.Add("BrandNo", (brandNo == null || brandNo == "品牌") ? "" : brandNo);
            dic.Add("Status", status == null ? "" : status);
            dic.Add("ChannelSord", channelSord == null ? "" : channelSord);
            dic.Add("ChannelNo", channelNo == null ? "" : channelNo);
            dic.Add("StartTime", startTime == null ? "" : startTime);
            dic.Add("EndTime", endTime == null ? "" : endTime);
            dic.Add("SubjectTemplate", subjectTemplate);
            dic.Add("BU", bu);
            dic.Add("IsAudited", isaudited);
            dic.Add("RunStatus", runStatus);
            IEnumerable<SubjectInfo> query = DapperUtil.QueryPaging<SubjectInfo>("ComBeziWfs_SWfsSubject_SubjectConsolePreheatList_EP", pageIndex, pageSize, "DateCreate desc,DateBegin desc", dic, new { KeyWord = keyword.Trim(), ProductNo = productNo.Trim(), Status = status, ChannelSord = channelSord, ChannelNo = channelNo, StartTime = startTime, EndTime = endTime, SubjectTemplate = subjectTemplate, BU = bu, IsAudited = isaudited, BrandNo = brandNo });
            Dictionary<string, List<SWfsSubjectChannelSordRef>> dicSordRef = GetSordBySubjectNoList(query.Select(x => x.SubjectNo).ToArray());

            foreach (var subject in query)
            {
                subject.ChannelNo = getChannel(subject.ChannelNo);
                subject.ChannelSordList = dicSordRef.Keys.Contains(subject.SubjectNo) ? dicSordRef[subject.SubjectNo] : null;
                subject.TotalHour = getTimeDiffer(subject.DateEnd, subject.DateBegin);
            }
            return PageConvertor.Convert(pageIndex, pageSize, query.ToList());
        }
        #endregion
        
        #region 活动排期
        /// <summary>
        /// 活动排期列表
        /// </summary>
        /// <param name="brandNo">品牌编号</param>
        /// <param name="channelSord">分类编号</param>
        /// <param name="startTime">查询开始时间</param>
        /// <param name="endTime">查询结束时间</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页码</param>
        /// <returns></returns>
        public List<SubjectInfo> GetSubjectWaitingListData_EP(string brandNo, string channelSord, string startTime, string endTime, int pageIndex, int pageSize)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("BrandNo", (brandNo == null || brandNo == "品牌") ? "" : brandNo);
            dic.Add("ChannelSord", channelSord == null ? "" : channelSord); 
            dic.Add("StartTime", startTime == null ? "" : startTime);
            dic.Add("EndTime", endTime == null ? "" : endTime);
            List<SubjectInfo> resultList = new List<SubjectInfo>();
            List<SubjectInfo> tempList = new List<SubjectInfo>();
            IEnumerable<SubjectInfo> query = DapperUtil.Query<SubjectInfo>("ComBeziWfs_SWfsSubject_SubjectWaitingList_EP", dic, new { BrandNo = brandNo, ChannelSord = channelSord, StartTime = startTime, EndTime = endTime });
            Dictionary<string, List<SWfsSubjectChannelSordRef>> dicSordRef = GetSordBySubjectNoList(query.Select(x => x.SubjectNo).ToArray());
            if (query != null && query.Count() > 0)
            {
                foreach (var subject in query)
                {
                    subject.ChannelSordList = dicSordRef.Keys.Contains(subject.SubjectNo) ? dicSordRef[subject.SubjectNo] : null;
                }
            }
            return query.ToList();
        }
        #endregion 
        #endregion
    }
}