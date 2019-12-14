using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
namespace SubjectConsoleStatisticsETLServices
{
    public static class AppConfig
    {
        /// <summary>
        /// 获取缓存设置
        /// </summary>
        /// <returns></returns>
        public static string RedisServer()
        {
            return ConfigurationManager.AppSettings["redisServer"].ToString();
        }

        /// <summary>
        /// 主数据库连接
        /// </summary>
        /// <returns></returns>
        public static string ComBeziWfsConnString()
        {
            return ConfigurationManager.AppSettings["ComBeziWfsConnString"].ToString();
        }

        /// <summary>
        /// 统计库数据库链接
        /// </summary>
        /// <returns></returns>
        public static string ComBeziReportConnString()
        {
            return ConfigurationManager.AppSettings["ComBeziReportConnString"].ToString();
        }

        /// <summary>
        /// 执行间隔时间
        /// </summary>
        /// <returns></returns>
        public static int Period()
        {
            int time1 = 0;
            Int32.TryParse(ConfigurationManager.AppSettings["Period"].ToString(), out time1);
            return time1;
        }

        /// <summary>
        /// 清除执行间隔时间
        /// </summary>
        /// <returns></returns>
        public static int ClearPeriod()
        {
            int time1 = 0;
            Int32.TryParse(ConfigurationManager.AppSettings["ClearPeriod"].ToString(), out time1);
            return time1;
        }

        /// <summary>
        /// 运行指令 0全部 1今日新开 2进行中 3已结束
        /// </summary>
        /// <returns></returns>
        public static string RunConfig()
        {
            return ConfigurationManager.AppSettings["RunConfig"].ToString();
        }

        /// <summary>
        /// 间隔月长度
        /// </summary>
        /// <returns></returns>
        public static int SpaceLenMonthPeriod()
        {
            int time1 = 0;
            Int32.TryParse(ConfigurationManager.AppSettings["SpaceLenMonthPeriod"].ToString(), out time1);
            return time1;
        }
    }
}
