using HomePageRemoveCacheService.DBUtility;
using log4net;
using Shangpin.Framework.Common.Cache;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomePageRemoveCacheService.Methods
{
    public class RemoveCacheMethod
    {
        private static ILog log = LogManager.GetLogger("RemoveCacheService_RollingLogFileAppender_Logger");
        private readonly string WfsConnString = AppConfig.ComBeziWfsConnString();
        //首页楼层缓存,删此缓存会自动级联删相关图片 链接缓存
        string floorKey = "ComBeziWfs_SWfsOperationPicture_GetCurrentSWfsOperationPictureByModuleIds_ALL";
        //上新缓存
        string NewArrivalKey = "ComBeziWfs_NewArrivalProductInfo_GetNewArrivalProductInfoByStartDate_GetNewArrivalInfo_index_INDEX_NA";
        //首页轮播图缓存
        string bannerKey = "ComBeziWfs_SWfsOperationPicture_GetSWfsOperationPicture_GetIndexTopSwitchPic_index_SwitchPicture";
        //找出楼层图片开始时间大于当前时间 中最小的开始时间
        string sql_foor = @"select top 1  pic.dateBegin from SWfsIndexModule (nolock) module 
                                 left join SWfsOperationPicture (nolock)   pic
                                on cast(module.moduleid as varchar(255) )=pic.PagePositionNo and pic.datastate=1
                               where 
                               pic.PageNo=module.PageNo
                                and pic.WebSiteNo=module.WebSiteNo and 
                                 module.dataState=1 and module.Stutas=1 and module.pageNo='index'  and module.WebSiteNo='shangpin.com'
	                            and pic.DateBegin<=getDate() order by pic.dateBegin desc";
        //找出首页轮播开始时间大于当前时间 中最小的开始时间
        string sql_banner = @"SELECT TOP 1 c.DateBegin
                        FROM   SWfsOperationPicture c
                        WHERE  c.[Status] = 1 and datastate=1
                        AND c.WebSiteNo='shangpin.com' AND c.PageNo='index' AND c.PagePositionNo='SwitchPicture'
                        AND c.PictureIndex = 1
                        AND c.DateBegin<=GETDATE() 
                        ORDER BY
                        c.DateBegin desc";
        //找出首页上新开始时间大于当前时间 中最小的开始时间
        string sql_NewArrival = @"   SELECT TOP 1 StartDate 
                        FROM SWfsIndexNewArrival
                        WHERE PageNo='index'
                        and PagePositionNo='INDEX_NA'
                        and Status=1 AND DataState=1
                        and StartDate<=GETDATE() ORDER BY StartDate desc";
        public void Run()
        {
            CheckRemoveCache(sql_foor, floorKey);
            CheckRemoveCache(sql_banner, bannerKey);
            CheckRemoveCache(sql_NewArrival, NewArrivalKey);
        }
        /// <summary>
        ///接近开始时间时清除缓存
        /// </summary>
        public void CheckRemoveCache(string sql, string key)
        {
            try
            {

                object result = null;//广告图时间 
                using (SqlConnection conn = new SqlConnection(WfsConnString))
                {
                    result = SqlHelper.ExecuteScalar(conn, CommandType.Text, sql);
                }
                DateTime now = DateTime.Now;
                DateTime destinationTime = Convert.ToDateTime("1900-01-01");
                if (result != null && DateTime.TryParse(result.ToString(), out destinationTime))
                {
                    CompareTimeForRemoveCache(now, destinationTime, key);
                }
              
            }
            catch (Exception exp)
            {
                log.Error("ERROR:清缓存服务异常，异常信息:" + exp.Message);
                throw;
            }
        }
        public void CompareTimeForRemoveCache(DateTime t1, DateTime t2, string key)
        {
            var cacheProvider = EnyimMemcachedClient.Instance;
            TimeSpan a = t2 - t1;
            if (a.TotalSeconds >= 0 && a.TotalSeconds <= 60)
            {
                cacheProvider.Remove(key);
                log.Debug("SUCCESS：首页清缓存服务，KEY:" + key + "");
            } 
           // cacheProvider.Remove(key);
        }
    }
}
