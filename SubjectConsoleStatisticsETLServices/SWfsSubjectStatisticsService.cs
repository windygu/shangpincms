using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SubjectConsoleStatisticsETLServices.DBUtility;
using Shangpin.Ocs.Entity.Extenstion.Outlet;
using System.Data;
using System.Data.SqlClient;
using Shangpin.Framework.Common.Cache;
using Shangpin.Entity.Wfs;
namespace SubjectConsoleStatisticsETLServices
{
    public class SWfsSubjectStatisticsService
    {
        private readonly string WfsConnString = AppConfig.ComBeziWfsConnString();
        private readonly string ReportConnString = AppConfig.ComBeziReportConnString();

        #region 奥莱活动列表
        /// <summary>
        /// 清除统计数据启动
        /// </summary>
        public void ClearDataRun()
        {
            DelSubjectStatisticData();
        }

        /// <summary>
        /// 获取奥莱活动列表
        /// <param name="type">1今日新开，2正在进行，3即将结束</param>
        /// </summary>
        /// <returns></returns>
        public void GetSubjectDataList(int type)
        {
            List<SubjectSaleVisitStatisticsDataModel> list = new List<SubjectSaleVisitStatisticsDataModel>();
            string caheKey = string.Empty;
            string subjectNo = string.Empty;
            string startDate = string.Empty;
            string stopDate = string.Empty;
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT sub.[SubjectNo] ,sub.[SubjectName] ");
            sql.Append("FROM [SWfsSubject] (NOLOCK) AS sub ");
            sql.Append("WHERE sub.SubjectType = 0 ");
            sql.Append("AND sub.IsRelated = 0 ");
            sql.Append("AND sub.[Status] = 1 ");
            sql.Append("AND sub.SubjectTemplate != 5 ");
            sql.Append("AND sub.IsAudited = 1 ");
            sql.Append("AND CHARINDEX('zsqd001', sub.ChannelNo) > 0 ");
            if (type == 1) //今天新开的活动
            {
                startDate = DateTime.Now.ToString("yyyy-MM-dd");
                sql.Append(" AND DATEDIFF(day, sub.DateBegin, @StartDate) = 0 AND sub.DateEnd > getdate() ");
            }
            else if (type == 2) //正在进行活动（包括今日新开活动、即将结束活动）
            {
                startDate = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
                stopDate = DateTime.Now.ToString();
                sql.Append(" AND sub.DateBegin < @StartDate AND sub.DateEnd > @StopDate ");
            }
            else if (type == 3) //已结束的活动
            {
                sql.Append(" AND sub.DateEnd < GETDATE() AND DATEDIFF(mm, DateEnd, GETDATE()) <= 2 ");
            }
            sql.Append("ORDER BY sub.DateBegin DESC ,sub.DateCreate DESC ");
            SqlParameter[] param = new SqlParameter[] { 
                new SqlParameter("@StartDate",startDate),
                new SqlParameter("@StopDate",stopDate)
            };
            SqlDataReader reader = SqlHelper.ExecuteReader(WfsConnString, CommandType.Text, sql.ToString(), param);
            if (reader != null)
            {
                while (reader.Read())
                {
                    subjectNo = reader["SubjectNo"] != null ? reader["SubjectNo"].ToString() : "";
                    SubjectSaleVisitStatisticsDataModel model = new SubjectSaleVisitStatisticsDataModel();
                    model.SubjectNo = subjectNo;
                    System.Threading.Thread.Sleep(300);
                    model.SaleStatistic = GetSubjectSaleStatisticsData(type, subjectNo); //销售统计
                    System.Threading.Thread.Sleep(300);
                    model.VisitStatistic = GetSubjectVisitStatisticsData(subjectNo); //访问统计
                    list.Add(model);
                }
                reader.Dispose();
                reader.Close();
                if (list != null && list.Count() > 0)
                {
                    #region 加入缓存
                    if (type == 1)
                    {
                        //caheKey = "AoLaiSubjectSaleVisitStatisticData_Today";
                        //SetRedisCache(caheKey, list);
                        SubjectStatisticDataMain(list, type);
                        Console.WriteLine("今日新开活动统计数据已加入缓存！");
                    }
                    else if (type == 2)
                    {
                        //caheKey = "AoLaiSubjectSaleVisitStatisticData_Runing";
                        //SetRedisCache(caheKey, list);
                        SubjectStatisticDataMain(list, type);
                        Console.WriteLine("进行中活动统计数据已加入缓存！");
                    }
                    else if (type == 3)
                    {
                        //caheKey = "AoLaiSubjectSaleVisitStatisticData_End";
                        //SetRedisCache(caheKey, list);
                        SubjectStatisticDataMain(list, type);
                        Console.WriteLine("已结束活动统计数据已加入缓存！");
                    }
                    #endregion
                }
            }
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="caheKey">Key值</param>
        /// <param name="list">统计数据</param>
        private void SetRedisCache(string caheKey, List<SubjectSaleVisitStatisticsDataM> list)
        {
            if (!String.IsNullOrEmpty(caheKey))
            {
                RedisCacheProvider.Instance.Set<List<SubjectSaleVisitStatisticsDataM>>(caheKey, list, new TimeSpan(0, 60, 0));
            }
        }
        #endregion

        #region 奥莱活动销售统计
        /// <summary>
        /// 奥莱活动销售统计
        /// </summary>
        /// <param name="type">1今日新开，2进行中，3已结束</param>
        /// <param name="subjectNo">活动编号</param>
        /// <returns></returns>
        public SubjectSaleStatistic GetSubjectSaleStatisticsData(int type, string subjectNo)
        {
            Console.WriteLine("正在读取编号为" + subjectNo + "活动的销售统计数据...");
            SubjectSaleStatistic model = new SubjectSaleStatistic();
            StringBuilder sql = new StringBuilder();
            if (type == 1 || type == 3)
            {
                sql.Append("SELECT Top 1 SubjectSaleStatisticsID,SubjectNo,DateStatistics,OrderNums,Amount,CostAmount,StockCount,SKUCount,SaleCount,SaledSKUCount ");
                sql.Append("FROM [OcsReport].[dbo].[SubjectSaleStatistics] (NOLOCK) ");
                sql.Append("WHERE SubjectNo = @SubjectNo AND ListingOutletFlag = 2 ");
                sql.Append("ORDER BY SubjectSaleStatisticsID DESC ");
            }
            else if (type == 2)
            {
                sql.Append("SELECT Top 2 * FROM  SubjectSaleStatistics t ");
                sql.Append("WHERE t.SubjectNo = @SubjectNo ");
                sql.Append("AND ListingOutletFlag = 2 ");
                sql.Append("AND NOT EXISTS ( SELECT 1 ");
                sql.Append("FROM SubjectSaleStatistics ");
                sql.Append("WHERE SubjectNo = @SubjectNo ");
                sql.Append("AND ListingOutletFlag = 2 ");
                sql.Append("AND DATEPART(yy, DateStatistics) = DATEPART(yy, t.DateStatistics) ");
                sql.Append("AND DATEPART(mm, DateStatistics) = DATEPART(mm, t.DateStatistics) ");
                sql.Append("AND DATEPART(dd, DateStatistics) = DATEPART(dd, t.DateStatistics) ");
                sql.Append("AND DateStatistics > t.DateStatistics) ");
                sql.Append("ORDER BY t.SubjectSaleStatisticsID DESC ");
            }
            SqlParameter[] param = new SqlParameter[] { 
                new SqlParameter("@SubjectNo",subjectNo)
            };
            SqlDataReader reader = SqlHelper.ExecuteReader(ReportConnString, CommandType.Text, sql.ToString(), param);
            int num = 0;
            while (reader.Read())
            {
                num++;
                if (num == 1)
                {
                    model.SubjectSaleStatisticsID = reader["SubjectSaleStatisticsID"] != null && !string.IsNullOrWhiteSpace(reader["SubjectSaleStatisticsID"].ToString()) ? Int32.Parse(reader["SubjectSaleStatisticsID"].ToString()) : 0;
                    model.SubjectNo = reader["SubjectNo"].ToString();
                    model.DateStatistics = reader["DateStatistics"] != null ? DateTime.Parse(reader["DateStatistics"].ToString()) : DateTime.Parse("1900-01-01 00:00:00");
                    model.OrderNums = reader["OrderNums"] != null && !string.IsNullOrWhiteSpace(reader["OrderNums"].ToString()) ? Decimal.Parse(reader["OrderNums"].ToString()) : 0;
                    model.Amount = reader["Amount"] != null && !string.IsNullOrWhiteSpace(reader["Amount"].ToString()) ? Int32.Parse(reader["Amount"].ToString()) : 0;
                    model.CostAmount = reader["CostAmount"] != null && !string.IsNullOrWhiteSpace(reader["CostAmount"].ToString()) ? Decimal.Parse(reader["CostAmount"].ToString()) : 0;
                    model.StockCount = reader["StockCount"] != null && !string.IsNullOrWhiteSpace(reader["StockCount"].ToString()) ? Int32.Parse(reader["StockCount"].ToString()) : 0;
                    model.SKUCount = reader["SKUCount"] != null && !string.IsNullOrWhiteSpace(reader["SKUCount"].ToString()) ? Int32.Parse(reader["SKUCount"].ToString()) : 0;
                    model.SaleCount = reader["SaleCount"] != null && !string.IsNullOrWhiteSpace(reader["SaleCount"].ToString()) ? Int32.Parse(reader["SaleCount"].ToString()) : 0;
                    model.SaledSKUCount = reader["SaledSKUCount"] != null && !string.IsNullOrWhiteSpace(reader["SaledSKUCount"].ToString()) ? Int32.Parse(reader["SaledSKUCount"].ToString()) : 0;
                }
                else if (num == 2)
                {
                    model.YesterDayAmount = reader["Amount"] != null && !string.IsNullOrWhiteSpace(reader["Amount"].ToString()) ? Int32.Parse(reader["Amount"].ToString()) : 0;
                }
            }
            reader.Dispose();
            reader.Close();
            Console.WriteLine("编号为" + subjectNo + "活动的销售统计数据加载完成");
            return model;
        }
        #endregion

        #region 奥莱活动访问统计
        /// <summary>
        /// 奥莱活动访问统计
        /// </summary>
        /// <returns></returns>
        public SubjectVisitStatistic GetSubjectVisitStatisticsData(string subjectNo)
        {
            Console.WriteLine("正在读取编号为" + subjectNo + "活动的访问统计数据...");
            SubjectVisitStatistic model = new SubjectVisitStatistic();
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT TOP 1 SubjectVisitStatisticsID,SubjectNo,DateStatistics,PV,UV,VIP,GoldenVIP,PlatinaVIP,DiamondVIP,OrderNums ");
            sql.Append("FROM [OcsReport].[dbo].[SubjectVisitStatistics] (NOLOCK) ");
            sql.Append("WHERE SubjectNo = @SubjectNo AND ListingOutletFlag = 2 ");
            sql.Append("ORDER BY SubjectVisitStatisticsID DESC ");
            SqlParameter[] param = new SqlParameter[] { 
                new SqlParameter("@SubjectNo",subjectNo)
            };
            SqlDataReader reader = SqlHelper.ExecuteReader(ReportConnString, CommandType.Text, sql.ToString(), param);
            while (reader.Read())
            {
                model.SubjectVisitStatisticsID = reader["SubjectVisitStatisticsID"] != null && !string.IsNullOrWhiteSpace(reader["SubjectVisitStatisticsID"].ToString()) ? Int32.Parse(reader["SubjectVisitStatisticsID"].ToString()) : 0;
                model.SubjectNo = reader["SubjectNo"].ToString();
                model.DateStatistics = reader["DateStatistics"] != null ? DateTime.Parse(reader["DateStatistics"].ToString()) : DateTime.Parse("1900-01-01 00:00:00");
                model.PV = reader["PV"] != null && !string.IsNullOrWhiteSpace(reader["PV"].ToString()) ? Int32.Parse(reader["PV"].ToString()) : 0;
                model.UV = reader["UV"] != null && !string.IsNullOrWhiteSpace(reader["UV"].ToString()) ? Int32.Parse(reader["UV"].ToString()) : 0;
                model.VIP = reader["VIP"] != null && !string.IsNullOrWhiteSpace(reader["VIP"].ToString()) ? Int32.Parse(reader["VIP"].ToString()) : 0;
                model.GoldenVIP = reader["GoldenVIP"] != null && !string.IsNullOrWhiteSpace(reader["GoldenVIP"].ToString()) ? Int32.Parse(reader["GoldenVIP"].ToString()) : 0;
                model.PlatinaVIP = reader["PlatinaVIP"] != null && !string.IsNullOrWhiteSpace(reader["PlatinaVIP"].ToString()) ? Int32.Parse(reader["PlatinaVIP"].ToString()) : 0;
                model.DiamondVIP = reader["DiamondVIP"] != null && !string.IsNullOrWhiteSpace(reader["DiamondVIP"].ToString()) ? Int32.Parse(reader["DiamondVIP"].ToString()) : 0;
                model.OrderNums = reader["OrderNums"] != null && !string.IsNullOrWhiteSpace(reader["OrderNums"].ToString()) ? Double.Parse(reader["OrderNums"].ToString()) : 0;
            }
            reader.Dispose();
            reader.Close();
            Console.WriteLine("编号为" + subjectNo + "活动的访问统计数据加载完成");
            return model;
        }
        #endregion

        #region 活动统计数据数据操作
        /// <summary>
        /// 导入活动统计数据
        /// </summary>
        /// <param name="list"></param>
        /// <param name="type">类型 1今日新开 2进行中 3已结束</param>
        public void SubjectStatisticDataMain(List<SubjectSaleVisitStatisticsDataModel> list, int type)
        {
            if (list != null && list.Count() > 0)
            {
                foreach (SubjectSaleVisitStatisticsDataModel item in list)
                {
                    if (item != null && !string.IsNullOrEmpty(item.SaleStatistic.SubjectNo))
                    {
                        SWfsSubjectStatisticsDataTemp model = new SWfsSubjectStatisticsDataTemp();
                        model.SubjectNo = item.SubjectNo;
                        model.StatisticsDataXML = StatisticsDataXMLAppend(item);
                        if (!IsExists(item.SaleStatistic.SubjectNo))
                        {
                            AddSubjectStatisticData(model); //添加到临时表
                        }
                        else
                        {
                            model.DateCreate = DateTime.Now;
                            EditSubjectStatisticData(model); //修改
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 拼接XML
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string StatisticsDataXMLAppend(SubjectSaleVisitStatisticsDataModel model)
        {
            StringBuilder sb = new StringBuilder();
            if (model != null && !string.IsNullOrEmpty(model.SubjectNo))
            {
                sb.Append("<XML>");
                sb.Append("<SubjectNo>" + model.SubjectNo + "</SubjectNo>");
                sb.Append("<SaleStatistic>");
                sb.Append("<SaleInfoID>" + model.SaleStatistic.SubjectSaleStatisticsID + "</SaleInfoID><SubjectNo>" + model.SaleStatistic.SubjectNo + "</SubjectNo>");
                sb.Append("<DateStatistics>" + model.SaleStatistic.DateStatistics.ToString("yyyy-MM-ddTHH:mm:ss") + "</DateStatistics><OrderNums>" + model.SaleStatistic.OrderNums + "</OrderNums>");
                sb.Append("<Amount>" + model.SaleStatistic.Amount + "</Amount><YesterDayAmount>" + model.SaleStatistic.YesterDayAmount + "</YesterDayAmount><CostAmount>" + model.SaleStatistic.CostAmount + "</CostAmount>");
                sb.Append("<StockCount>" + model.SaleStatistic.StockCount + "</StockCount><SKUCount>" + model.SaleStatistic.SKUCount + "</SKUCount><SaleCount>" + model.SaleStatistic.SaleCount + "</SaleCount>");
                sb.Append("<SaledSKUCount>" + model.SaleStatistic.SaledSKUCount + "</SaledSKUCount><StockTotalAmount>" + model.SaleStatistic.StockTotalAmount + "</StockTotalAmount><ListingOutletFlag>" + model.SaleStatistic.ListingOutletFlag + "</ListingOutletFlag>");
                sb.Append("</SaleStatistic>");
                sb.Append("<VisitStatistic>");
                sb.Append("<VisitinfoID>" + model.VisitStatistic.SubjectVisitStatisticsID + "</VisitinfoID><SubjectNo>" + model.VisitStatistic.SubjectNo + "</SubjectNo>");
                sb.Append("<DateStatistics>" + model.VisitStatistic.DateStatistics.ToString("yyyy-MM-ddTHH:mm:ss") + "</DateStatistics><PV>" + model.VisitStatistic.PV + "</PV><UV>" + model.VisitStatistic.UV + "</UV>");
                sb.Append("<VIP>" + model.VisitStatistic.VIP + "</VIP><GoldenVIP>" + model.VisitStatistic.GoldenVIP + "</GoldenVIP><PlatinaVIP>" + model.VisitStatistic.PlatinaVIP + "</PlatinaVIP><DiamondVIP>" + model.VisitStatistic.DiamondVIP + "</DiamondVIP>");
                sb.Append("<OrderNums>" + model.VisitStatistic.OrderNums + "</OrderNums><ListingOutletFlag>" + model.VisitStatistic.ListingOutletFlag + "</ListingOutletFlag>");
                sb.Append("</VisitStatistic>");
                sb.Append("</XML>");
            }
            else
            {
                sb.Append("");
            }
            return sb.ToString();
        }

        /// <summary>
        /// 添加活动统计数据
        /// </summary>
        /// <param name="model">统计数据</param>
        /// <returns></returns>
        private int AddSubjectStatisticData(SWfsSubjectStatisticsDataTemp model)
        {
            int result = 0;
            StringBuilder sql = new StringBuilder();
            sql.Append("INSERT INTO SWfsSubjectStatisticsDataTemp([SubjectNo],[StatisticsDataXML]) VALUES(@SubjectNo,@StatisticsDataXML)");
            SqlParameter[] param = new SqlParameter[] { 
                new SqlParameter("@SubjectNo",model.SubjectNo),
                new SqlParameter("@StatisticsDataXML",model.StatisticsDataXML)
            };
            try
            {
                using (SqlConnection conn = new SqlConnection(WfsConnString))
                {
                    result = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql.ToString(), param);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        /// <summary>
        /// 修改活动统计数据
        /// </summary>
        /// <param name="model">统计数据</param>
        /// <returns></returns>
        private int EditSubjectStatisticData(SWfsSubjectStatisticsDataTemp model)
        {
            int result = 0;
            StringBuilder sql = new StringBuilder();
            sql.Append("UPDATE SWfsSubjectStatisticsDataTemp SET StatisticsDataXML=@StatisticsDataXML,DateCreate=@DateCreate WHERE SubjectNo=@SubjectNo");
            SqlParameter[] param = new SqlParameter[] { 
                new SqlParameter("@SubjectNo",model.SubjectNo),
                new SqlParameter("@StatisticsDataXML",model.StatisticsDataXML),
                new SqlParameter("@DateCreate",DateTime.Now)
            };
            try
            {
                using (SqlConnection conn = new SqlConnection(WfsConnString))
                {
                    result = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql.ToString(), param);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        /// <summary>
        /// 查询活动是否存在
        /// </summary>
        /// <param name="subjectNo">活动编号</param>
        /// <returns></returns>
        private bool IsExists(string subjectNo)
        {
            int result = 0;
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT TOP 1 SDTID FROM SWfsSubjectStatisticsDataTemp WHERE SubjectNo=@SubjectNo ORDER BY DateCreate DESC");
            SqlParameter[] param = new SqlParameter[] { 
                new SqlParameter("@SubjectNo",subjectNo)
            };
            try
            {
                using (SqlConnection conn = new SqlConnection(WfsConnString))
                {
                    object obj = SqlHelper.ExecuteScalar(conn, CommandType.Text, sql.ToString(), param);
                    if (obj != null)
                    {
                        result = (int)obj;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result > 0 ? true : false;
        }

        /// <summary>
        /// 删除活动统计数据统计时间距离当前时间2个月以上的数据
        /// </summary>
        private void DelSubjectStatisticData()
        {
            int result = 0;
            int spaceLenMonth = AppConfig.SpaceLenMonthPeriod(); //间隔月
            StringBuilder sql = new StringBuilder();
            sql.Append("DELETE FROM SWfsSubjectStatisticsDataTemp WHERE DATEDIFF(mm, DateCreate, GETDATE()) > @SpaceLenMonth");
            SqlParameter[] param = new SqlParameter[] { 
                new SqlParameter("@SpaceLenMonth",spaceLenMonth)
            };
            try
            {
                using (SqlConnection conn = new SqlConnection(WfsConnString))
                {
                    result = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql.ToString(), param);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
