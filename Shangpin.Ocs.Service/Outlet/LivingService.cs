using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using Shangpin.Framework.Common;
using Shangpin.Framework.Common.Cache;

namespace Shangpin.Ocs.Service.Outlet
{
    public class LivingService
    {

        public string GetSate(string ip)
        {
            #region 数据库状态测试状态

            string sql1 = "ComBeziWfs_TestDB_LiveTesting";
            string sql2 = "ComBeziWfsSub_TestDB_LiveTesting";
            string sql3 = "ComBeziCommon_TestDB_LiveTesting";
            string sql5 = "ComBeziPic_TestDB_LiveTesting";
            //string sql6 = "ComShangPinMarketingAnalytics_TestDB_LiveTesting";
            //string sql7 = "ShangPinDWPlatform_TestDB_LiveTesting";

            var lst1 = DapperUtil.Query<int>(sql1).FirstOrDefault();
            var lst2 = DapperUtil.Query<int>(sql2).FirstOrDefault();
            var lst3 = DapperUtil.Query<int>(sql3).FirstOrDefault();

            var lst5 = DapperUtil.Query<int>(sql5).FirstOrDefault();
            //var lst6 = DapperUtil.Query<int>(sql6).FirstOrDefault();
            //var lst7 = DapperUtil.Query<int>(sql7).FirstOrDefault();

            #endregion

            #region 应用级缓存测试状态
            //cacheKey=IP地址+PID(进程号)
            string cacheKey = string.Format("{0}{1}", ip, System.Diagnostics.Process.GetCurrentProcess().Id);

            //cacheKey = HttpContext.Current.Request.ServerVariables["Remote_Addr"].ToString();

            #region Memcached

            var value = EnyimMemcachedClient.Instance.Get<string>(cacheKey);//获取缓存

            if (string.IsNullOrEmpty(value))
            {
                #region 设置缓存值
                value = "1";
                #endregion

                //缓存数据                
                EnyimMemcachedClient.Instance.Set(cacheKey, value, TimeSpan.FromDays(1));
            }

            #endregion

            #region RedisCache

            value = RedisCacheProvider.Instance.Get<string>(cacheKey);
            if (string.IsNullOrEmpty(value))
            {
                #region 设置缓存值
                value = "1";
                #endregion
                RedisCacheProvider.Instance.Set(cacheKey, value, TimeSpan.FromDays(1));
            }

            #endregion

            #endregion

            return "OK!";

        }
    }
}
