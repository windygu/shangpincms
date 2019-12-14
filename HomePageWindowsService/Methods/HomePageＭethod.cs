using Shangpin.Framework.Common.Cache;
using HomePageWindowsService.DBUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shangpin.Framework.Common; 

namespace HomePageWindowsService
{
    public class HomePageＭethod
    {
        private readonly string WfsConnString = AppConfig.ComBeziWfsConnString();
        public void Run()
        { ChangeHomePageFloorAdShowType(); }
        /// <summary>
        /// 首页楼层延迟切换单双图
        /// </summary>
        public void ChangeHomePageFloorAdShowType()
        {
            return;//暂时不用
            //  var cacheProvider = EnyimMemcachedClient.Instance;
            using (SqlConnection conn = new SqlConnection(WfsConnString))
            {
                string sql = @"update SWfsIndexModule 
                                  set ADSHOWTYPECHANGEDATE='2099-01-01',ADSHOWTYPE=case when ADShowType=1 then 2 else 1 end
                                   where pageNo='index' and webSiteNo='shangpin.com' and datastate=1 and ADShowType in (1,2)
                                  and getdate()> ADSHOWTYPECHANGEDATE";
                int result = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql.ToString());
                //if (result > 0)
                //{ //清除楼层缓存
                //    string key = "ComBeziWfs_SWfsIndexModule_GetSWfsIndexModuleIncludeTodayStyle_GetAllFloors";
                //    cacheProvider.Remove(key);
                //}
            }
        }
    }
}
