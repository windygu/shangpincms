﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomePageRemoveCacheService
{
    public class AppConfig
    {
        public static int AdShowTypeChangePeriod()
        {
            int time1 = 0;
            Int32.TryParse(ConfigurationManager.AppSettings["AdShowTypeChangePeriod"].ToString(), out time1);
            return time1;
        }
        /// <summary>
        /// 检测缓存清除间隔
        /// </summary>
        /// <returns></returns>
        public static int CheckRemoveCachePeriod()
        {
            int time1 = 0;
            Int32.TryParse(ConfigurationManager.AppSettings["CheckRemoveCachePeriod"].ToString(), out time1);
            return time1;
        }
        /// <summary>
        /// 主数据库连接
        /// </summary>
        /// <returns></returns>
        public static string ComBeziWfsConnString()
        {
            return ConfigurationManager.AppSettings["ComBeziWfsConnString"].ToString();
        }
    }


}
