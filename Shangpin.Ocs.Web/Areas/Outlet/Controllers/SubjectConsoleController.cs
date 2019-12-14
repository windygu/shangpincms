using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Shangpin.Ocs.Service.Outlet;
using Shangpin.Entity.Wfs;
using Shangpin.Ocs.Entity.Extenstion.Outlet;
using System.IO;
using Shangpin.Entity.ComBeziPic;
using Shangpin.Framework.Common;
using System.Configuration;
using Shangpin.Ocs.Service;
using System.Text;
using Shangpin.Ocs.Service.Common;
using Shangpin.Entity.Common;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq.Expressions;
using Shangpin.Framework.Common.Cache;
using System.Collections;
using System.Text.RegularExpressions;

namespace Shangpin.Ocs.Web.Areas.Outlet.Controllers
{
    [OCSAuthorization]
    public class SubjectConsoleController : Controller
    {
        private SWfsSubjectConsoleService subjectConsole;
        private SWfsSubjectService service;
        public SubjectConsoleController()
        {
            subjectConsole = new SWfsSubjectConsoleService();
            service = new SWfsSubjectService();
        }

        #region 活动监控
        /*
         * 活动监控
         * 今日新开(销售+访问)  AoLaiSubjectSaleVisitStatisticData_Today
         * 进行中(销售+访问)    AoLaiSubjectSaleVisitStatisticData_Runing
         * 已结束(销售)    AoLaiSubjectSaleVisitStatisticData_End
         * 
         * SubjectSaleVisitStatisticsDataM
         */
        /// <summary>
        /// 活动监控-今日新开
        /// </summary>
        /// <returns></returns>
        public ActionResult SubjectMonitorToday(SubjectMonitorSearchParm parm, int pageindex = 1, int pageSize = 10)
        {

            #region 今日新开数据读取
            //IEnumerable<SubjectInfo> todaySubjectList = service.GetSubjectsMonitorNew(parm);
            //EP项目用
            IEnumerable<SubjectInfo> todaySubjectList = service.GetSubjectsMonitorNew_EP(parm);

            List<SubjectInfo> newtodaySubjectList = new List<SubjectInfo>();

            if (todaySubjectList != null)
            {
                #region 排除重复今日新开广告活动

                /*今日新开广告图*/
                List<SWfsPictureManager> picADList = subjectConsole.GetIndexADPicListALL(3, new int[] { 10 });
                SWfsPictureManager todayAdModel = picADList.Where(r => r.Position.Equals(10)).OrderByDescending(r => r.DateCreate).FirstOrDefault();

                //增加一个规则20140604如果今日新开的广告图关联的活动与今日新开中的活动重复，则排除掉今日新开中的活动
                if (todayAdModel != null)
                {
                    string adSubjectNo = GetSubjectNoByLink(todayAdModel.LinkAddress);
                    todaySubjectList = todaySubjectList.Where(r => r.SubjectNo != adSubjectNo).ToList();
                }

                #endregion

                #region 排除专题重复活动

                List<SubjectTopicM> topPics = new List<SubjectTopicM>();
                //修改逻辑如下20140515：
                List<SubjectTopicM> tmptoplist = new List<SubjectTopicM>();
                List<SubjectTopicM> toplist = subjectConsole.GetFoucsAreaSubjectList();
                if (toplist == null || toplist.Count() < 3)
                {
                    List<string> subNos = new List<string>();
                    if (toplist != null) subNos = toplist.Select(t => t.SubjectNo).ToList();
                    int tmpNum = (toplist != null ? 3 - toplist.Count() : 3);
                    tmptoplist = subjectConsole.GetSubjectTopicListNewNormal(tmpNum, subNos);//20140515修改为读取正常开启专题类型的活动
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
                    todaySubjectList = (from today in todaySubjectList
                                        where !subNosNew.Contains(today.SubjectNo)
                                        select today).ToList();
                }
                #endregion

                newtodaySubjectList.AddRange(todaySubjectList);
            }

            #region 排序
            List<SubjectInfo> tmpList = newtodaySubjectList;
            List<SubjectInfo> tmpListII = new List<SubjectInfo>();
            IList<SWfsSubjectSort> sortList = service.GetSubjectSortListNew("3", 1, "1", DateTime.Now.ToString("yyyy-MM-dd"));
            if (sortList.Count > 0)
            {
                newtodaySubjectList = (from l in newtodaySubjectList
                                       join s in sortList on l.SubjectNo equals s.SubjectNo
                                       orderby s.Sort ascending
                                       select l).ToList();
            }
            Dictionary<string, int> dicSubjectDic = new Dictionary<string, int>();
            foreach (var item in sortList)
            {
                if (!dicSubjectDic.Keys.Contains(item.SubjectNo))
                {
                    dicSubjectDic.Add(item.SubjectNo, item.Sort);
                }
            }
            ViewBag.SortNum = dicSubjectDic;

            if (newtodaySubjectList.Count < tmpList.Count)
            {
                /*
                 * 排序规则
                 * 1首先读取已有排序的活动
                 * 2排除已有排序的活动
                 * 3在第2步活动中选择专题类型的活动添加到顶部显示
                 * 4其余的活动在底部显示
                 * 保证没有被手动排序的专题类型的活动默认显示在顶部
                 */
                tmpList = (from t in tmpList
                           where !(from b in newtodaySubjectList select b.SubjectNo).Contains(t.SubjectNo)
                           orderby t.DateBegin descending, t.DateCreate descending
                           select t).ToList();
                tmpListII = tmpList.Where(r => r.SubjectTemplate == 4).OrderByDescending(r => r.DateBegin).ToList();//专题类型的活动;
                tmpList = tmpList.Where(r => r.SubjectTemplate != 4).ToList();
                if (tmpList != null && tmpList.Count > 0)
                {
                    newtodaySubjectList.AddRange(tmpList);
                }
                if (tmpListII != null && tmpListII.Count > 0)
                {
                    newtodaySubjectList.InsertRange(0, tmpListII);
                }

            }
            #endregion

            newtodaySubjectList = newtodaySubjectList != null ? newtodaySubjectList.Distinct(new ComDifSubject()).ToList() : null;

            #region 获取活动的排序位置
            Dictionary<string, int> subjectSortNumDic = new Dictionary<string, int>();
            if (newtodaySubjectList != null && newtodaySubjectList.Count() > 0)
            {
                int num = 0;
                foreach (var item in newtodaySubjectList)
                {
                    if (!subjectSortNumDic.Keys.Contains(item.SubjectNo))
                    {
                        num++;
                        int sortNum = (dicSubjectDic.ContainsKey(item.SubjectNo) ? dicSubjectDic[item.SubjectNo] : num);
                        subjectSortNumDic.Add(item.SubjectNo, sortNum);
                    }
                }
            }
            ViewBag.SubjectSortNum = subjectSortNumDic;
            #endregion


            if (newtodaySubjectList != null && newtodaySubjectList.Count > 0)
            {
                int count = 0;
                Dictionary<string, List<SWfsSubjectChannelSordRef>> dicSordRef = service.GetSordBySubjectNoList(newtodaySubjectList.Select(x => x.SubjectNo).ToArray());
                foreach (var subject in newtodaySubjectList)
                {
                    count++;
                    //subject.ChannelNo = getChannel(subject.ChannelNo);
                    //subject.TotalHour = getTimeDiffer(subject.DateEnd, subject.DateBegin);

                    subject.ChannelSordList = dicSordRef.Keys.Contains(subject.SubjectNo) ? dicSordRef[subject.SubjectNo] : null;
                    subject.SortNum = (dicSubjectDic.ContainsKey(subject.SubjectNo) ? dicSubjectDic[subject.SubjectNo] : (pageindex - 1) * pageSize + count);
                }
                newtodaySubjectList = newtodaySubjectList.OrderBy(o => o.SortNum).ToList();
            }

            ViewBag.TotalCount = newtodaySubjectList.Count();

            newtodaySubjectList = newtodaySubjectList.Skip((pageindex - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.ToDaySubject = newtodaySubjectList;

            #endregion
            //ViewBag.Statistic = RedisCacheProvider.Instance.Get<List<SubjectSaleVisitStatisticsDataM>>("AoLaiSubjectSaleVisitStatisticData_Today");
            ViewBag.Statistic = new TempStatisticsService().GetStatisticsListBySubjectNoes(newtodaySubjectList.Select(x => x.SubjectNo).ToList());
            ViewBag.ChannelSordList = GetChannelSordList(2);
            ViewBag.PageIndex = pageindex;
            ViewBag.PageSize = pageSize;

            #region 统计活动数量
            int todayCount = 0;
            int runCount = 0;
            int inCount = 0;
            int outCount = 0;
            //subjectConsole.GetSubjectConsoleCount(1, out todayCount, out runCount, out inCount, out outCount);
            //EP项目用
            subjectConsole.GetSubjectConsoleCount_EP(1, out todayCount, out runCount, out inCount, out outCount);
            ViewBag.RunSubjectCount = runCount;
            ViewBag.InSubjectCount = inCount;
            ViewBag.OutSubjectCount = outCount;
            #endregion

            return View(parm);
        }

        /// <summary>
        /// 活动监控-进行中
        /// </summary>
        /// <returns></returns>
        public ActionResult SubjectMonitorRuning(SubjectMonitorSearchParm parm, int pageindex = 1, int pageSize = 10)
        {
            int channelNo = 3; //3全部,0女士频道,1男士频道
            //IEnumerable<SubjectInfo> subjectList = service.GetSubjectMonitorRuning(parm);
            //EP项目用
            IEnumerable<SubjectInfo> subjectList = service.GetSubjectMonitorRuning_EP(parm);
            List<SubjectInfo> todaySubjectList = new List<SubjectInfo>();
            if (subjectList != null)
            {

                #region 排序
                IEnumerable<SubjectInfo> saleingSubject = subjectList;
                IList<SWfsSubjectSort> saleingsortlist = service.GetSubjectSortListNew(channelNo.ToString(), 3, "1", DateTime.Now.ToString("yyyy-MM-dd"));
                List<SubjectInfo> tmpsaleingList = null == saleingSubject ? new List<SubjectInfo>() : saleingSubject.ToList();

                if (null != saleingsortlist && saleingsortlist.Count > 0)
                {
                    saleingSubject = from l in saleingSubject
                                     join s in saleingsortlist on l.SubjectNo equals s.SubjectNo
                                     orderby s.Sort ascending
                                     select l;
                    todaySubjectList = saleingSubject.ToList();
                }
                if (null != saleingSubject && saleingSubject.Count() < tmpsaleingList.Count)
                {
                    tmpsaleingList = (from t in tmpsaleingList
                                      where !(from b in saleingSubject select b.SubjectNo).Contains(t.SubjectNo)
                                      orderby t.DateBegin descending, t.DateCreate descending
                                      select t).ToList();
                    todaySubjectList = saleingSubject.ToList();
                    todaySubjectList.AddRange(tmpsaleingList);
                }
                #endregion

                #region 排重处理
                todaySubjectList = todaySubjectList != null && todaySubjectList.Count() > 0 ? todaySubjectList.Distinct(new ComparerSubject()).ToList() : saleingSubject.Distinct(new ComparerSubject()).ToList();
                #endregion

                ViewBag.TotalCount = todaySubjectList.Count();

                #region 分页
                if (todaySubjectList != null && todaySubjectList.Count() > 0)
                {
                    todaySubjectList = todaySubjectList.Skip((pageindex - 1) * pageSize).Take(pageSize).ToList();
                }
                #endregion
            }
            ViewBag.ToDaySubject = todaySubjectList;
            if (todaySubjectList != null && todaySubjectList.Count() > 0)
            {
                Dictionary<string, List<SWfsSubjectChannelSordRef>> dicSordRef = service.GetSordBySubjectNoList(todaySubjectList.Select(x => x.SubjectNo).ToArray());
                foreach (var subject in todaySubjectList)
                {
                    subject.ChannelSordList = dicSordRef.Keys.Contains(subject.SubjectNo) ? dicSordRef[subject.SubjectNo] : null;
                    subject.TotalHour = subject.DateEnd.Subtract(DateTime.Now).Duration().TotalHours;
                }
            }

            ViewBag.ChannelSordList = GetChannelSordList(2);
            ViewBag.PageIndex = pageindex;
            ViewBag.PageSize = pageSize;
            ViewBag.StartTime = parm.QueryStartTime;
            ViewBag.Endtime = parm.QueryEndTime;

            //ViewBag.Statistic = RedisCacheProvider.Instance.Get<List<SubjectSaleVisitStatisticsDataM>>("AoLaiSubjectSaleVisitStatisticData_Runing");
            ViewBag.Statistic = new TempStatisticsService().GetStatisticsListBySubjectNoes(todaySubjectList.Select(x => x.SubjectNo).ToList());
            #region 统计活动数量
            int todayCount = 0;
            int runCount = 0;
            int inCount = 0;
            int outCount = 0;
            //subjectConsole.GetSubjectConsoleCount(2, out todayCount, out runCount, out inCount, out outCount);
            //EP项目用
            subjectConsole.GetSubjectConsoleCount_EP(2, out todayCount, out runCount, out inCount, out outCount);
            ViewBag.TodaySubjectCount = todayCount;
            ViewBag.InSubjectCount = inCount;
            ViewBag.OutSubjectCount = outCount;
            #endregion

            return View(parm);
        }

        /// <summary>
        /// 活动监控-站内预热
        /// </summary>
        /// <param name="keyword">活动名称/编号</param>
        /// <param name="QuerybranchNo">品牌编号</param>
        /// <param name="QueryBrandName">品牌名称</param>
        /// <param name="channelSord">分类编号</param>
        /// <param name="bu">BU部门</param>
        /// <param name="pageindex">当前页</param>
        /// <param name="pageSize">页码</param>
        /// <param name="startTime">查询开始时间</param>
        /// <param name="endTime">查询结束时间</param>
        /// <returns></returns>
        public ActionResult SubjectMonitorInPreheat(string keyword, string QuerybranchNo, string QueryBrandName = "", string channelSord = "", string bu = "", int pageindex = 1, int pageSize = 10, string startTime = "", string endTime = "")
        {
            ViewBag.KeyWord = !string.IsNullOrEmpty(keyword) ? keyword.Trim() : "";
            ViewBag.ChannelSord = channelSord;
            ViewBag.BrandNo = QuerybranchNo;
            ViewBag.BrandName = QueryBrandName;
            ViewBag.ChannelSordList = GetChannelSordList(2);
            ViewBag.BU = bu;
            ViewBag.PageSize = pageSize;
            ViewBag.StartTime = startTime;
            ViewBag.Endtime = endTime;
            //RecordPage<SubjectInfo> list = subjectConsole.SelectSubjectMonitorInPreheatList(keyword, QuerybranchNo, channelSord, bu, pageindex, pageSize, startTime, endTime);
            //EP项目用
            RecordPage<SubjectInfo> list = subjectConsole.SelectSubjectMonitorInPreheatList_EP(keyword, QuerybranchNo, channelSord, bu, pageindex, pageSize, startTime, endTime);
            #region 统计活动数量
            int todayCount = 0;
            int runCount = 0;
            int inCount = 0;
            int outCount = 0;
            //subjectConsole.GetSubjectConsoleCount(3, out todayCount, out runCount, out inCount, out outCount);
            //EP项目用
            subjectConsole.GetSubjectConsoleCount_EP(3, out todayCount, out runCount, out inCount, out outCount);
            ViewBag.TodaySubjectCount = todayCount;
            ViewBag.RunSubjectCount = runCount;
            ViewBag.InSubjectCount = list.TotalItems;
            ViewBag.OutSubjectCount = outCount;
            #endregion
            return View(list);
        }

        /// <summary>
        /// 活动监控-站外推广
        /// </summary>
        /// <param name="keyword">活动名称/编号</param>
        /// <param name="QuerybranchNo">品牌编号</param>
        /// <param name="QueryBrandName">品牌名称</param>
        /// <param name="channelSord">分类编号</param>
        /// <param name="bu">BU部门</param>
        /// <param name="PromotionStartTime">推广确认开始时间</param>
        /// <param name="PromotionEndTime">推广确认结束时间</param>
        /// <param name="pageindex">当前页</param>
        /// <param name="pageSize">页码</param>
        /// <returns></returns>
        public ActionResult SubjectMonitorOutPromotion(string keyword, string QuerybranchNo, string QueryBrandName = "", string channelSord = "", string bu = "", string ConfirmStartTime = "", string ConfirmEndTime = "", int pageindex = 1, int pageSize = 10)
        {
            ViewBag.KeyWord = !string.IsNullOrEmpty(keyword) ? keyword.Trim() : "";
            ViewBag.ChannelSord = channelSord;
            ViewBag.BrandNo = QuerybranchNo;
            ViewBag.BrandName = QueryBrandName;
            ViewBag.ChannelSordList = GetChannelSordList(2);
            ViewBag.BU = bu;
            ViewBag.PageSize = pageSize;
            ViewBag.ConfirmStartTime = ConfirmStartTime;
            ViewBag.ConfirmEndtime = ConfirmEndTime;
            //RecordPage<SubjectInfo> list = subjectConsole.SelectSubjectMonitorOutPromotionList(keyword, QuerybranchNo, channelSord, bu, ConfirmStartTime, ConfirmEndTime, pageindex, pageSize);
            //EP项目用
            RecordPage<SubjectInfo> list = subjectConsole.SelectSubjectMonitorOutPromotionList_EP(keyword, QuerybranchNo, channelSord, bu, ConfirmStartTime, ConfirmEndTime, pageindex, pageSize);
            #region 统计活动数量
            int todayCount = 0;
            int runCount = 0;
            int inCount = 0;
            int outCount = 0;
            //subjectConsole.GetSubjectConsoleCount(4, out todayCount, out runCount, out inCount, out outCount);
            //EP项目用
            subjectConsole.GetSubjectConsoleCount_EP(4, out todayCount, out runCount, out inCount, out outCount);
            ViewBag.TodaySubjectCount = todayCount;
            ViewBag.RunSubjectCount = runCount;
            ViewBag.InSubjectCount = inCount;
            ViewBag.OutSubjectCount = list.TotalItems;
            #endregion
            return View(list);
        }

        /// <summary>
        /// 活动监控-已结束
        /// </summary>
        /// <param name="keyword">活动名称/编号</param>
        /// <param name="QuerybranchNo">品牌编号</param>
        /// <param name="QueryBrandName">品牌名称</param>
        /// <param name="bu">BU部门</param>
        /// <param name="channelSord">分类编号</param>
        /// <param name="pageindex">当前页</param>
        /// <param name="pageSize">页码</param>
        /// <returns></returns>
        public ActionResult SubjectMonitorEnd(string keyword, string QuerybranchNo, string QueryBrandName = "", string channelSord = "", string bu = "", int pageindex = 1, int pageSize = 10)
        {
            ViewBag.KeyWord = !string.IsNullOrEmpty(keyword) ? keyword.Trim() : "";
            ViewBag.ChannelSord = channelSord;
            ViewBag.BrandNo = QuerybranchNo;
            ViewBag.BrandName = QueryBrandName;
            ViewBag.ChannelSordList = GetChannelSordList(2);
            ViewBag.BU = bu;
            ViewBag.PageSize = pageSize;
            //RecordPage<SubjectInfo> list = subjectConsole.SelectSubjectMonitorEndList(keyword, QuerybranchNo, channelSord, bu, pageindex, pageSize);
            //EP项目用
            RecordPage<SubjectInfo> list = subjectConsole.SelectSubjectMonitorEndList_EP(keyword, QuerybranchNo, channelSord, bu, pageindex, pageSize);

            //ViewBag.Statistic = RedisCacheProvider.Instance.Get<List<SubjectSaleVisitStatisticsDataM>>("AoLaiSubjectSaleVisitStatisticData_End");
            ViewBag.Statistic = new TempStatisticsService().GetStatisticsListBySubjectNoes(list.Items.Select(x => x.SubjectNo).ToList());
            #region 统计活动数量
            int todayCount = 0;
            int runCount = 0;
            int inCount = 0;
            int outCount = 0;
            //subjectConsole.GetSubjectConsoleCount(0, out todayCount, out runCount, out inCount, out outCount);
            //EP项目用
            subjectConsole.GetSubjectConsoleCount_EP(0, out todayCount, out runCount, out inCount, out outCount);
            ViewBag.TodaySubjectCount = todayCount;
            ViewBag.RunSubjectCount = runCount;
            ViewBag.InSubjectCount = inCount;
            ViewBag.OutSubjectCount = outCount;
            #endregion

            return View(list);
        }
        #endregion

        #region 活动管理

        #region 活动管理列表
        /// <summary>
        /// 活动管理-全部活动
        /// </summary>
        /// <param name="keyword">活动编号/名称</param>
        /// <param name="productNo">商品编号</param>
        /// <param name="QuerybranchNo">品牌编号</param>
        /// <param name="status">活动状态</param>
        /// <param name="channelSord">分类</param>
        /// <param name="channelNo">展示渠道</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="pageindex">当前页</param>
        /// <param name="Template">活动类型</param>
        /// <param name="isaudited">发布状态</param>
        /// <param name="bu">BU部门</param>
        /// <param name="QueryBrandName">品牌名称</param>
        /// <param name="hidTimeFlag">固定时间查询</param>
        /// <param name="pageSize">页码</param>
        /// <param name="runStatus">运行状态 1预热 2正常 3结束</param>
        /// <returns></returns>
        public ActionResult SubjectManageList(string keyword = "", string productNo = "", string QuerybranchNo = "", string status = "", string channelSord = "", string channelNo = "", string startTime = "", string endTime = "", int pageindex = 1, string Template = "", string isaudited = "", string bu = "", string QueryBrandName = "", string hidTimeFlag = "", int pageSize = 10, string RunStatus = "")
        {
            ViewBag.KeyWord = !string.IsNullOrEmpty(keyword) ? keyword.Trim() : "";
            ViewBag.ProductNo = !string.IsNullOrEmpty(productNo) ? productNo.Trim() : "";
            ViewBag.Status = status;
            ViewBag.ChannelSord = channelSord;
            ViewBag.ChannelNo = channelNo;
            ViewBag.StartTime = startTime;
            ViewBag.EndTime = endTime;
            ViewBag.BrandNo = QuerybranchNo;
            ViewBag.BrandName = QueryBrandName;
            ViewBag.Template = Template;
            ViewBag.ChannelSordList = GetChannelSordList(2);
            ViewBag.BU = bu;
            ViewBag.IsAudited = isaudited;
            ViewBag.TimeFlag = !string.IsNullOrEmpty(hidTimeFlag) ? hidTimeFlag : "";
            ViewBag.PageSize = pageSize;
            ViewBag.PageIndex = pageindex;
            ViewBag.RunStatus = RunStatus;
            string subjectNos = Request.Form["BatchSubjectNo"] ?? "";
            subjectNos = !string.IsNullOrEmpty(subjectNos) && !subjectNos.Trim().Equals("多个活动编号用逗号或回车隔开") ? subjectNos.Trim() : "";
            string[] SubjectArrayNo = !string.IsNullOrEmpty(subjectNos) ? System.Web.HttpUtility.UrlDecode(subjectNos).Replace("\r\n", ",").Replace("\n\r", ",").Replace("\n", ",").Replace("\r", ",").TrimEnd(',').Trim().Split(',') : null;
            ViewBag.BatchSubjectNo = subjectNos;
            #region 开始时间计算
            if (!string.IsNullOrEmpty(hidTimeFlag))
            {
                if (hidTimeFlag.Equals("1")) //今日
                {
                    startTime = DateTime.Now.ToString("yyyy-MM-dd");
                    endTime = DateTime.Now.ToString("yyyy-MM-dd");
                }
                else if (hidTimeFlag.Equals("2")) //本周
                {
                    startTime = GetMondayDate(DateTime.Now).ToString("yyyy-MM-dd");
                    endTime = GetSundayDate(DateTime.Now).ToString("yyyy-MM-dd"); ;
                }
                else if (hidTimeFlag.Equals("3")) //本月
                {
                    DateTime now = DateTime.Now;
                    startTime = new DateTime(now.Year, now.Month, 1).ToString("yyyy-MM-dd");
                    endTime = new DateTime(now.Year, now.Month, 1).AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");
                }
            }
            #endregion
            //RecordPage<SubjectInfo> list = subjectConsole.SelectSubjectList(keyword, productNo, QuerybranchNo, status, channelSord, channelNo, startTime, endTime, pageindex, pageSize, Template, isaudited, bu, RunStatus, SubjectArrayNo);
            //EP项目用
            RecordPage<SubjectInfo> list = subjectConsole.SelectSubjectList_EP(keyword, productNo, QuerybranchNo, status, channelSord, channelNo, startTime, endTime, pageindex, pageSize, Template, isaudited, bu, RunStatus, SubjectArrayNo);
            ViewBag.ChannelList = GetChannelList(list.Items.Select(x => x.SubjectNo).ToArray());
            return View(list);
        }

        /// <summary>
        /// 活动管理-预热活动
        /// </summary>
        /// <param name="keyword">活动编号/名称</param>
        /// <param name="productNo">商品编号</param>
        /// <param name="QuerybranchNo">品牌编号</param>
        /// <param name="status">活动状态</param>
        /// <param name="channelSord">分类</param>
        /// <param name="channelNo">展示渠道</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="pageindex">当前页</param>
        /// <param name="Template">活动类型</param>
        /// <param name="isaudited">发布状态</param>
        /// <param name="bu">BU部门</param>
        /// <param name="QueryBrandName">品牌名称</param>
        /// <param name="hidTimeFlag">固定时间查询</param>
        /// <param name="pageSize">页码</param>
        /// <param name="preheatStatus">预热状态 1站内推广 2站外推广</param>
        /// <returns></returns>
        public ActionResult SubjectManagePreheatList(string keyword = "", string productNo = "", string QuerybranchNo = "", string status = "", string channelSord = "", string channelNo = "", string startTime = "", string endTime = "", int pageindex = 1, string Template = "", string isaudited = "", string bu = "", string QueryBrandName = "", string hidTimeFlag = "", int pageSize = 10, string preheatStatus = "")
        {
            ViewBag.KeyWord = !string.IsNullOrEmpty(keyword) ? keyword.Trim() : "";
            ViewBag.ProductNo = !string.IsNullOrEmpty(productNo) ? productNo.Trim() : "";
            ViewBag.Status = status;
            ViewBag.ChannelSord = channelSord;
            ViewBag.ChannelNo = channelNo;
            ViewBag.StartTime = startTime;
            ViewBag.EndTime = endTime;
            ViewBag.BrandNo = QuerybranchNo;
            ViewBag.BrandName = QueryBrandName;
            ViewBag.Template = Template;
            ViewBag.ChannelSordList = GetChannelSordList(2);
            ViewBag.BU = bu;
            ViewBag.IsAudited = isaudited;
            ViewBag.TimeFlag = !string.IsNullOrEmpty(hidTimeFlag) ? hidTimeFlag : "";
            ViewBag.PageSize = pageSize;
            ViewBag.PreheatStatus = preheatStatus;
            #region 开始时间计算
            if (!string.IsNullOrEmpty(hidTimeFlag))
            {
                if (hidTimeFlag.Equals("1")) //今日
                {
                    startTime = DateTime.Now.ToString("yyyy-MM-dd");
                    endTime = DateTime.Now.ToString("yyyy-MM-dd");
                }
                else if (hidTimeFlag.Equals("2")) //本周
                {
                    startTime = GetMondayDate(DateTime.Now).ToString("yyyy-MM-dd");
                    endTime = GetSundayDate(DateTime.Now).ToString("yyyy-MM-dd"); ;
                }
                else if (hidTimeFlag.Equals("3")) //本月
                {
                    DateTime now = DateTime.Now;
                    startTime = new DateTime(now.Year, now.Month, 1).ToString("yyyy-MM-dd");
                    endTime = new DateTime(now.Year, now.Month, 1).AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");
                }
            }
            #endregion
            //RecordPage<SubjectInfo> list = subjectConsole.SelectSubjectPreheatList(keyword, productNo, QuerybranchNo, status, channelSord, channelNo, startTime, endTime, pageindex, pageSize, Template, isaudited, bu, preheatStatus);
            //EP项目用
            RecordPage<SubjectInfo> list = subjectConsole.SelectSubjectPreheatList_EP(keyword, productNo, QuerybranchNo, status, channelSord, channelNo, startTime, endTime, pageindex, pageSize, Template, isaudited, bu, preheatStatus);
            ViewBag.ChannelList = GetChannelList(list.Items.Select(x => x.SubjectNo).ToArray());
            return View(list);
        }

        #endregion

        #region 编辑活动
        [HttpGet]
        public ActionResult ConsoleEdit(string subjectNo)
        {
            Task<SubjectInfo> task1 = Task.Factory.StartNew(() => service.GetSubjectInfo(subjectNo));
            SubjectInfo model = task1.Result;
            if (model.IsPreheat == 1)
            {
                SubjectPreheatInfoM predheatM = service.GetSubjectPreheatInfo(subjectNo);
                ViewBag.SubjectPredheatModel = predheatM;
            }
            Task<IList<SWfsSubjectChannelSordRef>> task2 = Task.Factory.StartNew(() => service.GetSordBySubjectNo(subjectNo));
            ViewBag.SubjectChannelSordList = task2.Result;
            Task<IList<SWfsChannelSord>> task3 = Task.Factory.StartNew(() => service.GetChannelSordList(2));
            ViewBag.ChannelSordList = task3.Result;
            Task<IList<SWfsSubjectCategoryRef>> task4 = Task.Factory.StartNew(() => service.GetErpCategoryListBySubjectNo(subjectNo));
            ViewBag.SubjectErpCategoryList = task4.Result;
            Task<IList<SpCategory>> task5 = Task.Factory.StartNew(() => service.GetERPCategoryListByParentNo("ROOT"));
            ViewBag.ErpCategoryList = task5.Result;

            Task<SpBrand> task6 = task1.ContinueWith<SpBrand>(p => service.GetBrandModelByBrandNo(model.BrandContent));
            SpBrand brandModel = task6.Result;
            model.BrandCnName = (brandModel == null) ? "" : brandModel.BrandCnName;
            model.BrandEnName = (brandModel == null) ? "" : brandModel.BrandEnName;
            return View(model);
        }
        #endregion

        #region 修改活动时间
        /// <summary>
        /// 修改活动时间
        /// </summary>
        /// <param name="subjectNo">活动编号</param>
        /// <returns></returns>
        public ActionResult EditSubjectTime(string subjectNo)
        {
            Task<SubjectInfo> task1 = Task.Factory.StartNew(() => service.GetSubjectInfo(subjectNo));
            SubjectInfo model = task1.Result;
            model.ChannelCategoryList = service.GetErpCategoryListBySubjectNo(subjectNo);
            if (model.IsPreheat == 1)
            {
                SubjectPreheatInfoM predheatM = service.GetSubjectPreheatInfo(subjectNo);
                ViewBag.SubjectPredheatModel = predheatM;
            }
            return View(model);
        }

        /// <summary>
        /// 保存修改活动时间
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveSubjectTime()
        {
            SubjectInfo subject = new SubjectInfo();
            string subjectNo = Request.Params["subjectNo"].ToString(); //活动编号
            string dateBegin = Request.Params["DateBegin"]; //活动开始时间
            string end = Request.Params["SubjectDuration"].ToString(); //持续时间
            string isPreheat = Request.Params["hidIsPreheat"]; //活动预热 0关闭 1开启
            string preheatTime = Request.Params["txtPreheatTime"]; //预热时间
            StringBuilder sb = new StringBuilder();

            #region 预热日期不可大于活动开始日期
            if (isPreheat.Equals("1"))
            {
                if (DateTime.Parse(preheatTime) >= DateTime.Parse(dateBegin))
                {
                    return Json(new { result = "-1", message = "预热日期不可大于活动开始日期！" });
                }
            }
            #endregion

            DateTime dateEnd = new DateTime();
            if (!string.IsNullOrEmpty(end) && end != "0")
            {
                dateEnd = Convert.ToDateTime(dateBegin).AddHours(Convert.ToInt32(end));
            }
            else
            {
                dateEnd = DateTime.MaxValue;
                dateEnd = Convert.ToDateTime(dateBegin).AddHours(72); //默认持续72小时
            }
            subject.DateBegin = Convert.ToDateTime(dateBegin);
            subject.DateEnd = dateEnd;
            subject.SubjectNo = subjectNo;
            if (isPreheat.Equals("1") && !string.IsNullOrEmpty(preheatTime))
            {
                sb.Append("预热" + DateTime.Parse(preheatTime).ToString("yyyy-MM-dd/HH:mm") + "<br/>");
            }
            sb.Append("<span>开始" + subject.DateBegin.ToString("yyyy-MM-dd/HH:mm") + "</span><br/>");
            sb.Append("结束" + subject.DateEnd.ToString("yyyy-MM-dd/HH:mm"));
            try
            {
                bool flag = subjectConsole.UpdateSubjectItem(1, subject);
                if (flag)
                {
                    #region 预热开启时编辑预热相关信息
                    if (isPreheat.Equals("1"))
                    {
                        SWfsSubjectTopExpand topExpandM = new SWfsSubjectTopExpand();
                        topExpandM.SubjectNo = subjectNo;
                        topExpandM.PreheatTime = DateTime.Parse(preheatTime);
                        subjectConsole.UpdateSubjectPreheatItem(topExpandM);
                    }
                    #endregion

                    #region 日志信息
                    SWfsSubjectOrChannelLog log = new SWfsSubjectOrChannelLog();
                    log.SubjectOrChannelNo = subjectNo;
                    log.TypeValue = 3; //3奥莱活动控制台
                    log.DateOperator = DateTime.Now;
                    log.OperatorContent = "奥莱控制台修改活动时间";
                    log.OperatorUserId = PresentationHelper.GetPassport().UserName;
                    log.OperatorActionType = LogActionType.Edit.GetHashCode();
                    service.InsertSubjectOrChannelLog(log);
                    #endregion

                    return Json(new { result = "1", returnStr = sb.ToString(), H = end, message = "修改成功！" });
                }
                else
                {
                    return Json(new { result = "0", returnStr = "", H = "72", message = "修改失败！" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { result = "0", message = ex.Message });
            }
        }
        #endregion

        #region 获取活动商品数量、UV流量、提醒人数
        /// <summary>
        /// 获取活动商品数量、UV流量、提醒人数
        /// </summary>
        /// <param name="type">0全部，1商品数量，2UV流量，3提醒人数</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateSubjectProductNum(int type = 0)
        {
            string subjectNo = Request.Form["subjectNo"].ToString();
            string[] arrayNo;
            List<SubjectConsoleProductM> productList = new List<SubjectConsoleProductM>(); //活动商品数量
            List<SubjectConsoleProductM> warnList = new List<SubjectConsoleProductM>(); //活动预热提醒人数
            List<SubjectUVCountM> uvList = new List<SubjectUVCountM>(); //活动UV流量
            if (!string.IsNullOrEmpty(subjectNo))
            {
                arrayNo = subjectNo.Split(',').ToArray().Where(a => !a.Equals("")).ToArray();
                if (arrayNo != null && arrayNo.Length > 0)
                {
                    if (type == 0 || type.ToString().IndexOf("1") > -1) //商品数量
                    {
                        productList = subjectConsole.GetSubjectConsoleProductCount(arrayNo);                        
                    }
                    if (type == 0 || type.ToString().IndexOf("2") > -1) //UV流量
                    {
                        uvList = subjectConsole.GetSubjectUVResultCount(arrayNo);
                    }
                    if (type == 0 || type.ToString().IndexOf("3") > -1) //提醒人数
                    {
                        warnList = subjectConsole.GetSubjectSaleWarnCount(arrayNo);
                    }
                }
            }
            return Json(new { jsonProduct = productList, jsonWarn = warnList, jsonUV = uvList });
        }
        #endregion

        #region 活动UV流量列表
        /// <summary>
        /// 活动UV流量列表
        /// </summary>
        /// <param name="subjectNo">活动编号</param>
        /// <returns></returns>
        public ActionResult SubjectUVFromList(string subjectNo, string startDateTime = "", string endDateTime = "")
        {
            List<SubjectUVFromM> list = new List<SubjectUVFromM>();
            int resultCount = 0;
            if (!string.IsNullOrEmpty(subjectNo))
            {
                list = subjectConsole.GetSubjectUVFromList(subjectNo, startDateTime, endDateTime);
                if (list != null && list.Count() > 0)
                {
                    foreach (var item in list)
                    {
                        resultCount += item.DataCount;
                    }
                }
            }
            ViewBag.SubjectNo = subjectNo;
            ViewBag.StartTime = startDateTime;
            ViewBag.EndTime = endDateTime;
            ViewBag.UVResultCount = resultCount;
            ViewBag.UVFromList = list;
            return View();
        }
        #endregion

        #region 更新活动发布审核状态
        /// <summary>
        /// 更新活动发布审核状态
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateSubjectAuditStatus()
        {
            string subjectNo = Request.Form["subjectNo"].ToString();
            List<SubjectCheckStatusM> dicMsg = new List<SubjectCheckStatusM>();
            List<SubjectCheckStatusM> successMsg = new List<SubjectCheckStatusM>();
            string[] arrayNo;
            bool flag = false;
            if (!string.IsNullOrEmpty(subjectNo))
            {
                arrayNo = subjectNo.Split(',').ToArray().Where(a => !a.Equals("")).ToArray();
                if (arrayNo != null && arrayNo.Length > 0)
                {
                    foreach (var item in arrayNo)
                    {
                        string[] arrayItem = item.Trim().Split('|');
                        if (arrayItem != null && arrayItem.Length > 0)
                        {
                            string status = arrayItem[1];
                            if (status.Equals("1")) //开启状态
                            {
                                SubjectInfo m = new SubjectInfo();
                                m.SubjectNo = arrayItem[0];
                                m.IsAudited = 1;
                                m.AuditedDateTime = DateTime.Now;
                                m.AuditedUserId = PresentationHelper.GetPassport().UserName;
                                flag = subjectConsole.UpdateSubjectItem(2, m);
                                if (!flag)
                                {
                                    dicMsg.Add(new SubjectCheckStatusM { key = arrayItem[0], value = "0" });
                                }
                                else
                                {
                                    #region 日志信息
                                    SWfsSubjectOrChannelLog log = new SWfsSubjectOrChannelLog();
                                    log.SubjectOrChannelNo = arrayItem[0];
                                    log.TypeValue = 3; //3奥莱活动控制台
                                    log.DateOperator = DateTime.Now;
                                    log.OperatorContent = "奥莱控制台审核活动";
                                    log.OperatorUserId = PresentationHelper.GetPassport().UserName;
                                    log.OperatorActionType = LogActionType.Audited.GetHashCode();
                                    service.InsertSubjectOrChannelLog(log);
                                    #endregion

                                    successMsg.Add(new SubjectCheckStatusM { key = arrayItem[0], value = "1" });
                                }
                            }
                            else
                            {
                                dicMsg.Add(new SubjectCheckStatusM { key = arrayItem[0], value = (status.Equals("0") ? "-1" : status.Equals("2") ? "-2" : "-3") }); //活动状态 -1 已关闭, -2 未开启, -3已删除
                            }
                        }
                    }
                }
                return Json(new { result = "1", errorMsg = dicMsg, successMsg = successMsg });
            }
            else
            {
                return Json(new { result = "0", errorMsg = dicMsg, successMsg = successMsg });
            }
        }
        #endregion

        #region 逻辑删除奥莱活动
        /// <summary>
        /// 逻辑删除奥莱活动
        /// </summary>
        /// <param name="subjectNo">活动编号</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateSubjectStatusToDel(string subjectNo)
        {
            string dicMsg = string.Empty;
            int flag = 0;
            if (!string.IsNullOrEmpty(subjectNo))
            {
                flag = subjectConsole.UpdateSubjectStatusToDel(subjectNo);
                if (flag == 1)
                {
                    #region 日志信息
                    SWfsSubjectOrChannelLog log = new SWfsSubjectOrChannelLog();
                    log.SubjectOrChannelNo = subjectNo;
                    log.TypeValue = 3; //3奥莱活动控制台
                    log.DateOperator = DateTime.Now;
                    log.OperatorContent = "奥莱控制台删除活动";
                    log.OperatorUserId = PresentationHelper.GetPassport().UserName;
                    log.OperatorActionType = LogActionType.Delete.GetHashCode();
                    service.InsertSubjectOrChannelLog(log);
                    #endregion

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

        #region 相关方法
        /// <summary>   
        /// 计算某日起始日期（礼拜一的日期）   
        /// </summary>   
        /// <param name="someDate">该周中任意一天</param>   
        /// <returns>返回礼拜一日期，后面的具体时、分、秒和传入值相等</returns>   
        public DateTime GetMondayDate(DateTime someDate)
        {
            int i = someDate.DayOfWeek - DayOfWeek.Monday;
            if (i == -1) i = 6;// i值 > = 0 ，因为枚举原因，Sunday排在最前，此时Sunday-Monday=-1，必须+7=6。   
            TimeSpan ts = new TimeSpan(i, 0, 0, 0);
            return someDate.Subtract(ts);
        }
        /// <summary>   
        /// 计算某日结束日期（礼拜日的日期）   
        /// </summary>   
        /// <param name="someDate">该周中任意一天</param>   
        /// <returns>返回礼拜日日期，后面的具体时、分、秒和传入值相等</returns>   
        public DateTime GetSundayDate(DateTime someDate)
        {
            int i = someDate.DayOfWeek - DayOfWeek.Sunday;
            if (i != 0) i = 7 - i;// 因为枚举原因，Sunday排在最前，相减间隔要被7减。   
            TimeSpan ts = new TimeSpan(i, 0, 0, 0);
            return someDate.Add(ts);
        }

        /// <summary>
        /// 获取所属频道
        /// </summary>
        /// <param name="SubjectNolist"></param>
        /// <returns></returns>
        private Dictionary<string, List<SWfsChannel>> GetChannelList(Array SubjectNolist)
        {
            var channelList = service.GetChannelBySubjectList(SubjectNolist);
            return channelList;

        }

        /// <summary>
        /// 获取所属分类
        /// </summary>
        /// <returns></returns>
        private IList<SWfsChannelSord> GetChannelSordList(int siteNo)
        {
            var channelSordList = service.GetChannelSordList(2);
            channelSordList.Add(new SWfsChannelSord() { SordName = "女士", SordNo = "0" });
            channelSordList.Add(new SWfsChannelSord() { SordName = "男士", SordNo = "1" });
            channelSordList.Add(new SWfsChannelSord() { SordName = "儿童", SordNo = "2" });
            return channelSordList.OrderBy(x => x.SordNo).ToList();
        }
        #endregion

        #region 判断是否存在结束时间在7天内的同品牌同品类活动
        /// <summary>
        /// 判断是否存在结束时间在7天内的同品牌同品类活动
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CheckDiffBrandCategory()
        {
            string fistTime = string.Empty;
            string subjectNo = Request.Form["subjectNo"] ?? "";
            string beginTime = Request.Form["beginTime"] ?? "";
            string brandNo = Request.Form["brandNo"] ?? "";
            string categoryNo = Request.Form["categoryNo"] ?? "";

            #region 根据开始时间7天内的活动结束时间是否有同品牌或同品类的活动
            if (!string.IsNullOrEmpty(brandNo) && !string.IsNullOrEmpty(categoryNo))
            {
                if (!string.IsNullOrEmpty(beginTime))
                {
                    DateTime dt = DateTime.Parse(beginTime);
                    fistTime = dt.AddDays(-7).ToString();
                }

                #region 品类
                List<SWfsSubjectCategoryRef> categoryNoList = new List<SWfsSubjectCategoryRef>();
                string[] categoryNoArray = categoryNo.Split(',');
                foreach (string item in categoryNoArray)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        SWfsSubjectCategoryRef categoryM = new SWfsSubjectCategoryRef();
                        categoryM.Category = item;
                        categoryM.SubjectNo = subjectNo;
                        categoryNoList.Add(categoryM);
                    }
                }
                #endregion

                bool IsHaveBCDiff = subjectConsole.GetSubjectDiffBrandClassListData(subjectNo, brandNo, categoryNoList, fistTime, beginTime);
                if (IsHaveBCDiff)
                {
                    return Json(new { result = "1", message = "结束时间7天内已经存在同品牌同分类的活动！" });
                }
            }
            #endregion

            return Json(new { result = "0", message = "" });
        }
        #endregion

        #endregion

        #region 活动排期
        /// <summary>
        /// 活动排期
        /// </summary>
        /// <param name="brandNo">品牌编号</param>
        /// <param name="channelSord">分类编号</param>
        /// <param name="startTime">查询开始时间</param>
        /// <param name="endTime">查询结束时间</param>
        /// <returns></returns>
        public ActionResult SubjectWaiting(string QuerybranchNo = "", string QueryBrandName = "", string channelSord = "", string startTime = "", string endTime = "", int pageIndex = 1)
        {
            int pageSize = 10;
            ViewBag.BrandNo = QuerybranchNo;
            ViewBag.BrandName = QueryBrandName;
            ViewBag.ChannelSordList = GetChannelSordList(2);
            ViewBag.StartTime = startTime;
            ViewBag.EndTime = endTime;
            ViewBag.ChannelSord = channelSord;
            ViewBag.pageIndex = pageIndex;
            ViewBag.PageSize = pageSize;
            List<SubjectInfo> resultList = new List<SubjectInfo>();
            List<SubjectInfo> tempList = new List<SubjectInfo>();
            //List<SubjectInfo> list = subjectConsole.GetSubjectWaitingListData(QuerybranchNo, channelSord, startTime, endTime, pageIndex, pageSize);
            //EP项目用
            List<SubjectInfo> list = subjectConsole.GetSubjectWaitingListData_EP(QuerybranchNo, channelSord, startTime, endTime, pageIndex, pageSize);
            if (list != null && list.Count() > 0)
            {
                List<SubjectInfo> itemList = new List<SubjectInfo>();
                #region 停用
                //查询品牌、分类、(预热|开始|结束)时间重复活动
                //foreach (var item in list)
                //{
                //    if (resultList.Where(a => a.SubjectNo.Equals(item.SubjectNo)).Count() == 0)
                //    {
                //        itemList = GroupSubject(list, item, resultList);
                //        resultList.AddRange(itemList);
                //    }
                //}
                #endregion
                #region 优化
                foreach (var item in list)
                {
                    if (resultList.Where(a => a.SubjectNo.Equals(item.SubjectNo)).Count() == 0)
                    {
                        itemList = GroupSubjectNew(list, item, resultList);
                        resultList.AddRange(itemList);
                    }
                }
                #endregion
            }
            if (resultList != null && resultList.Count() > 0)
            {
                ViewBag.TotalCount = resultList.Count();
                resultList = resultList.OrderByDescending(a => a.DateBegin).OrderByDescending(a => a.BrandContent).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
            ViewBag.SubjectList = resultList;
            return View();
        }

        #region 排重方法
        /// <summary>
        /// 获取品牌、分类、开始时间和结束时间相交时间活动
        /// </summary>
        /// <param name="list">活动数据集</param>
        /// <param name="item">主对比活动</param>
        /// <param name="tempResultList">比较相同的活动数据集</param>
        /// <returns></returns>
        private List<SubjectInfo> GroupSubjectNew(List<SubjectInfo> list, SubjectInfo item, List<SubjectInfo> tempResultList)
        {
            List<SubjectInfo> resultList = new List<SubjectInfo>();
            List<SubjectInfo> tempList = new List<SubjectInfo>();
            if (list != null && list.Count() > 0)
            {
                tempList.AddRange(list);
                tempList = tempList.Where(a => !a.SubjectNo.Equals(item.SubjectNo) && a.BrandContent.Equals(item.BrandContent)).ToList();
                if (tempList != null && tempList.Count() > 0)
                {
                    foreach (var m in tempList)
                    {
                        if (!m.SubjectNo.Equals(item.SubjectNo) && tempResultList.Where(a => a.SubjectNo.Equals(m.SubjectNo)).Count() == 0)
                        {
                            bool isClassExists = IsExistsSordNoNew(list, m.SubjectNo, m.BrandContent, m.ChannelSordList); //判断品牌相同的所有活动中是否有相同的分类
                            bool isDateExists = CheckDateTimeSame(m.DateBegin.ToString("yyyy-MM-dd"), m.DateEnd.ToString("yyyy-MM-dd"), item.DateBegin.ToString("yyyy-MM-dd"), item.DateEnd.ToString("yyyy-MM-dd")); //活动时间
                            if (isClassExists && isDateExists)
                            {
                                resultList.Add(m);
                            }
                        }
                    }
                }
                if (resultList != null && resultList.Count() > 0)
                {
                    resultList.Add(item);
                }
            }
            return resultList;
        }

        /// <summary>
        /// 检查判断是否存在相同分类
        /// </summary>
        /// <param name="list">活动数据集</param>
        /// <param name="subjectNo">活动编号</param>
        /// <param name="brandNo">品牌编号</param>
        /// <param name="sordList">活动所属分类</param>
        /// <returns></returns>
        private bool IsExistsSordNoNew(List<SubjectInfo> list, string subjectNo, string brandNo, IList<SWfsSubjectChannelSordRef> sordList)
        {
            bool IsHave = false;
            if (list != null && list.Count() > 0)
            {
                List<string> strList = GetSordNoListNew(list, subjectNo, brandNo);
                List<string> resultSordList = (from t in strList
                                               where (from b in sordList select b.SordNo).Contains(t)
                                               select t).ToList();
                if (resultSordList != null && resultSordList.Count() > 0)
                {
                    IsHave = true;
                }
            }
            return IsHave;
        }

        /// <summary>
        /// 根据品牌编号获取所有的分类，当前活动分类除外
        /// </summary>
        /// <param name="list">活动数据集</param>
        /// <param name="subjectNo">排除活动编号</param>
        /// <param name="brandNo">品牌编号</param>
        /// <returns></returns>
        private List<string> GetSordNoListNew(List<SubjectInfo> list, string subjectNo, string brandNo)
        {
            List<string> strList = new List<string>();
            if (list != null && list.Count() > 0)
            {
                List<SubjectInfo> sordList = new List<SubjectInfo>();
                if (!string.IsNullOrEmpty(subjectNo))
                {
                    sordList = list.Where(a => a.BrandContent.Equals(brandNo) && !a.SubjectNo.Equals(subjectNo)).ToList();
                }
                else
                {
                    sordList = list.Where(a => a.BrandContent.Equals(brandNo)).ToList();
                }
                if (sordList != null && sordList.Count() > 0)
                {
                    foreach (var item in sordList)
                    {
                        foreach (var val in item.ChannelSordList)
                        {
                            if (!strList.Contains(val.SordNo))
                            {
                                strList.Add(val.SordNo);
                            }
                        }
                    }
                }
            }
            return strList;
        }

        /// <summary>
        /// 判断两个时间段是否有相交的日期
        /// </summary>
        /// <param name="startTime1">开始时间1</param>
        /// <param name="endTime1">结束时间1</param>
        /// <param name="startTime2">开始时间2</param>
        /// <param name="endTime2">结束时间2</param>
        /// <returns></returns>
        private bool CheckDateTimeSame(string startTime1, string endTime1, string startTime2, string endTime2)
        {
            DateTime start;
            DateTime end;
            if (DateTime.Parse(startTime1) > DateTime.Parse(startTime2))
            {
                start = DateTime.Parse(startTime1);
            }
            else if (DateTime.Parse(startTime1) < DateTime.Parse(startTime2))
            {
                start = DateTime.Parse(startTime2);
            }
            else
            {
                start = DateTime.Parse(startTime1);
            }

            if (DateTime.Parse(endTime1) < DateTime.Parse(endTime2))
            {
                end = DateTime.Parse(endTime1);
            }
            else if (DateTime.Parse(endTime1) > DateTime.Parse(endTime2))
            {
                end = DateTime.Parse(endTime2);
            }
            else
            {
                end = DateTime.Parse(endTime1);
            }
            if (start <= end)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取品牌、分类、时间相同的活动
        /// </summary>
        /// <param name="list">活动数据集</param>
        /// <param name="item">主对比活动</param>
        /// <param name="tempResultList">比较相同的活动数据集</param>
        /// <returns></returns>
        private List<SubjectInfo> GroupSubject(List<SubjectInfo> list, SubjectInfo item, List<SubjectInfo> tempResultList)
        {
            List<SubjectInfo> resultList = new List<SubjectInfo>();
            if (list != null && list.Count() > 0)
            {
                foreach (var m in list)
                {
                    if (!m.SubjectNo.Equals(item.SubjectNo) && tempResultList.Where(a => a.SubjectNo.Equals(m.SubjectNo)).Count() == 0)
                    {
                        //IEnumerable<SWfsSubjectChannelSordRef> l = (from t in m.ChannelSordList
                        //                                            where (from b in item.ChannelSordList select b.SordNo).Contains(t.SordNo)
                        //                                            select t).ToList();
                        bool isExists = IsExistsSordNo(list, m.SubjectNo, m.BrandContent, m.ChannelSordList, m.DateBegin.ToString("yyyy-MM-dd"), m.DateEnd.ToString("yyyy-MM-dd"), m.PreheatTime.ToString("yyyy-MM-dd"));
                        if (isExists && m.BrandContent.Equals(item.BrandContent) && (m.DateBegin.ToString("yyyy-MM-dd").Equals(item.DateBegin.ToString("yyyy-MM-dd")) || m.DateEnd.ToString("yyyy-MM-dd").Equals(item.DateEnd.ToString("yyyy-MM-dd")) || m.PreheatTime.ToString("yyyy-MM-dd").Equals(item.PreheatTime.ToString("yyyy-MM-dd"))))
                        {
                            //m.IsStartTimeColor = m.DateBegin.ToString("yyyy-MM-dd").Equals(item.DateBegin.ToString("yyyy-MM-dd")) ? true : false;
                            //m.IsEndTimeColor = m.DateEnd.ToString("yyyy-MM-dd").Equals(item.DateEnd.ToString("yyyy-MM-dd")) ? true : false;
                            //m.IsPreheatTimeColor = m.PreheatTime.ToString("yyyy-MM-dd").Equals(item.PreheatTime.ToString("yyyy-MM-dd")) ? true : false;
                            resultList.Add(FirstSetColor(list, m));
                        }
                    }
                }
            }
            if (resultList != null && resultList.Count() > 0)
            {
                resultList.Add(FirstSetColor(list, item));
            }
            return resultList;
        }

        /// <summary>
        /// 获取活动标红时间
        /// </summary>
        /// <param name="list">活动数据集</param>
        /// <param name="model">需要标红的活动实体</param>
        /// <returns></returns>
        private SubjectInfo FirstSetColor(List<SubjectInfo> list, SubjectInfo model)
        {
            if (list != null && list.Count() > 0)
            {

                //开始时间
                foreach (var m in list)
                {
                    if (!m.SubjectNo.Equals(model.SubjectNo))
                    {
                        bool isExists = IsExistsSordNo(list, m.SubjectNo, m.BrandContent, m.ChannelSordList, m.DateBegin.ToString("yyyy-MM-dd"), m.DateEnd.ToString("yyyy-MM-dd"), m.PreheatTime.ToString("yyyy-MM-dd"));
                        if (isExists && m.BrandContent.Equals(model.BrandContent) && m.DateBegin.ToString("yyyy-MM-dd").Equals(model.DateBegin.ToString("yyyy-MM-dd")))
                        {
                            model.IsStartTimeColor = m.DateBegin.ToString("yyyy-MM-dd").Equals(model.DateBegin.ToString("yyyy-MM-dd")) ? true : false;
                            break;
                        }
                    }
                }
                //结束时间
                foreach (var m in list)
                {
                    if (!m.SubjectNo.Equals(model.SubjectNo))
                    {
                        bool isExistsEnd = IsExistsSordNo(list, m.SubjectNo, m.BrandContent, m.ChannelSordList, m.DateBegin.ToString("yyyy-MM-dd"), m.DateEnd.ToString("yyyy-MM-dd"), m.PreheatTime.ToString("yyyy-MM-dd"));
                        if (isExistsEnd && m.BrandContent.Equals(model.BrandContent) && m.DateEnd.ToString("yyyy-MM-dd").Equals(model.DateEnd.ToString("yyyy-MM-dd")))
                        {
                            model.IsEndTimeColor = m.DateEnd.ToString("yyyy-MM-dd").Equals(model.DateEnd.ToString("yyyy-MM-dd")) ? true : false;
                            break;
                        }
                    }
                }
                //预热时间
                if (model.IsPreheat == 1)
                {
                    foreach (var m in list)
                    {
                        if (!m.SubjectNo.Equals(model.SubjectNo))
                        {
                            bool isExistsPreheat = IsExistsSordNo(list, m.SubjectNo, m.BrandContent, m.ChannelSordList, m.DateBegin.ToString("yyyy-MM-dd"), m.DateEnd.ToString("yyyy-MM-dd"), m.PreheatTime.ToString("yyyy-MM-dd"));
                            if (isExistsPreheat && m.BrandContent.Equals(model.BrandContent) && m.PreheatTime.ToString("yyyy-MM-dd").Equals(model.PreheatTime.ToString("yyyy-MM-dd")))
                            {
                                model.IsPreheatTimeColor = m.PreheatTime.ToString("yyyy-MM-dd").Equals(model.PreheatTime.ToString("yyyy-MM-dd")) ? true : false;
                                break;
                            }
                        }
                    }
                }
            }
            return model;
        }

        /// <summary>
        /// 检查判断是否存在相同分类
        /// </summary>
        /// <param name="list">活动数据集</param>
        /// <param name="subjectNo">活动编号</param>
        /// <param name="brandNo">品牌编号</param>
        /// <param name="sordList">活动所属分类</param>
        /// <returns></returns>
        private bool IsExistsSordNo(List<SubjectInfo> list, string subjectNo, string brandNo, IList<SWfsSubjectChannelSordRef> sordList, string startTime, string endTime, string preheatTime)
        {
            bool IsHave = false;
            if (list != null && list.Count() > 0)
            {
                List<string> strList = GetSordNoList(list, subjectNo, brandNo, startTime, endTime, preheatTime);
                List<string> resultSordList = (from t in strList
                                               where (from b in sordList select b.SordNo).Contains(t)
                                               select t).ToList();
                if (resultSordList != null && resultSordList.Count() > 0)
                {
                    IsHave = true;
                }
            }
            return IsHave;
        }

        /// <summary>
        /// 根据品牌编号获取所有的分类，当前活动分类除外
        /// </summary>
        /// <param name="list">活动数据集</param>
        /// <param name="subjectNo">排除活动编号</param>
        /// <param name="brandNo">品牌编号</param>
        /// <returns></returns>
        private List<string> GetSordNoList(List<SubjectInfo> list, string subjectNo, string brandNo, string startTime, string endTime, string preheatTime)
        {
            List<string> strList = new List<string>();
            if (list != null && list.Count() > 0)
            {
                List<SubjectInfo> sordList = new List<SubjectInfo>();
                if (!string.IsNullOrEmpty(subjectNo))
                {
                    sordList = list.Where(a => a.BrandContent.Equals(brandNo) && !a.SubjectNo.Equals(subjectNo) && (a.DateBegin.ToString("yyyy-MM-dd").Equals(startTime) || a.DateEnd.ToString("yyyy-MM-dd").Equals(endTime) || a.PreheatTime.ToString("yyyy-MM-dd").Equals(preheatTime))).ToList();
                }
                else
                {
                    sordList = list.Where(a => a.BrandContent.Equals(brandNo)).ToList();
                }
                if (sordList != null && sordList.Count() > 0)
                {
                    foreach (var item in sordList)
                    {
                        foreach (var val in item.ChannelSordList)
                        {
                            if (!strList.Contains(val.SordNo))
                            {
                                strList.Add(val.SordNo);
                            }
                        }
                    }
                }
            }
            return strList;
        }
        #endregion

        #endregion

        #region 数据监控
        /// <summary>
        /// 数据监控
        /// </summary>
        /// <returns></returns>
        public ActionResult DataMonitor()
        {
            return View();
        }
        #endregion

        #region 活动排序保存

        /// <summary>
        ///  更新奥莱活动排序
        /// </summary>
        /// <param name="sno">活动编号</param>
        /// <param name="sord">排序顺序号</param>
        /// <param name="type">1 今日新开 2 正在进行 3 即将结束</param>
        /// <param name="channelNo">1男士 0 女士 3首页（目前只有首页有排序功能）</param>
        /// <returns></returns>
        public JsonResult UpdateSord(string sno, int sord, short type, string channelNo)
        {
            SWfsSubjectSort sort = service.GetSWfsSubjectSort(sno, type, channelNo);
            if (sort == null)
            {
                sort = new SWfsSubjectSort();
            }
            sort.SubjectNo = sno;
            sort.ChannelNo = channelNo;
            sort.Type = type;
            sort.Sort = sord;
            sort.ShowType = 1; //网站
            sort.ShowDateTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd"));
            try
            {
                if (sort.SubjectSortId <= 0)
                {
                    service.InsertSubjectSort(sort);
                }
                else
                {
                    service.UpdateSWfsSubjectSort(sort);
                }
                return Json(new { rs = "ok", msg = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { rs = "error", msg = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        ///  更新奥莱活动排序
        /// </summary>
        /// <param name="sno">活动编号</param>
        /// <param name="sord">排序顺序号</param>
        /// <param name="subjectSortNum">所有活动编号和顺序号数据集</param>
        /// <param name="type">1 今日新开 2 正在进行 3 即将结束</param>
        /// <param name="channelNo">3 首页</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UpdateSordNew(string sno, int sord, string subjectSortNum, short type, string channelNo)
        {
            string showType = "1"; //默认网站
            string showDateTime = DateTime.Now.ToString("yyyy-MM-dd"); //排序日期

            #region 装载所有活动排序号
            Dictionary<string, int> sortNumDic = new Dictionary<string, int>();
            if (!string.IsNullOrEmpty(subjectSortNum))
            {
                string[] arraySortNum = subjectSortNum.Split('|');
                if (arraySortNum != null && arraySortNum.Length > 0)
                {
                    foreach (var item in arraySortNum)
                    {
                        string[] sortArray = item.Split(',');
                        if (!sortNumDic.Keys.Contains(sortArray[0]))
                        {
                            sortNumDic.Add(sortArray[0], Int32.Parse(sortArray[1]));
                        }
                    }
                }
                if (sortNumDic != null && sortNumDic.Count() > 0)
                {
                    sortNumDic[sno] = sord;
                }
            }
            #endregion

            SWfsSubjectService subject = new SWfsSubjectService();
            int temp = subject.DeleteSubjectSortBySubjectNO(channelNo, type, showType, showDateTime);
            if (temp < 0)
            {
                return Json(new { rs = "error", msg = "保存失败！" });
            }
            else
            {
                foreach (var item in sortNumDic)
                {
                    SWfsSubjectSort sort = new SWfsSubjectSort();
                    sort.SubjectNo = item.Key;
                    sort.ChannelNo = channelNo;
                    sort.Type = type;
                    sort.Sort = item.Value;
                    sort.ShowType = !string.IsNullOrEmpty(showType) ? Int32.Parse(showType) : 0; //1网站，2手机端
                    sort.ShowDateTime = !string.IsNullOrEmpty(showDateTime) ? DateTime.Parse(showDateTime) : DateTime.Parse("1900-01-01 00:00:00");
                    try
                    {
                        subject.InsertSubjectSort(sort);
                    }
                    catch (Exception ex)
                    {
                        return Json(new { rs = "error", msg = ex.Message });
                    }
                }

                #region 操作日志
                SWfsSubjectOrChannelLog log = new SWfsSubjectOrChannelLog();
                log.SubjectOrChannelNo = channelNo;
                log.TypeValue = 0;
                log.DateOperator = DateTime.Now;
                log.OperatorContent = "修改活动排序";
                log.OperatorUserId = PresentationHelper.GetPassport().UserName;
                subject.InsertSubjectOrChannelLog(log);
                #endregion

                return Json(new { rs = "ok", msg = "保存成功！" });
            }
        }
        #endregion

        #region 比较
        public class ComparerSubject : IEqualityComparer<SubjectInfo>
        {
            public bool Equals(SubjectInfo t1, SubjectInfo t2)
            {
                return (t1.SubjectNo == t2.SubjectNo);
            }
            public int GetHashCode(SubjectInfo t)
            {
                return t.ToString().GetHashCode();
            }
        }
        /*20131021测试说出现商品重复问题，本地不能复现，添加强制排重*/
        /// <summary>
        /// 比较 用于排序
        /// </summary>
        public class ComparerSubjectProductRef : IEqualityComparer<SubjectProductRef>
        {
            public bool Equals(SubjectProductRef t1, SubjectProductRef t2)
            {
                return (t1.ProductNo == t2.ProductNo);
            }
            public int GetHashCode(SubjectProductRef t)
            {
                return t.ToString().GetHashCode();
            }
        }
        #endregion

        #region 方法
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
    }
}
