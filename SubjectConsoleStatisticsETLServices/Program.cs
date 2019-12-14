using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Shangpin.Framework.Common.Cache;
using System.Threading;
namespace SubjectConsoleStatisticsETLServices
{
    class Program
    {
        static void Main(string[] args)
        {
            #region 初始化配置
            RedisCacheProvider.SetRedisCluster(AppConfig.RedisServer()); //redis配置
            #endregion

            SWfsSubjectStatisticsService service = new SWfsSubjectStatisticsService();
            string RunConfig = AppConfig.RunConfig();

            Console.WriteLine("开始运行...");
            if (RunConfig.Equals("0") || RunConfig.Equals("1"))
            {
                service.GetSubjectDataList(1); //今日新开
                System.Threading.Thread.Sleep(3000);
                Console.WriteLine("=====================================================");
            }
            if (RunConfig.Equals("0") || RunConfig.Equals("2"))
            {
                service.GetSubjectDataList(2); //进行中
                System.Threading.Thread.Sleep(3000);
                Console.WriteLine("=====================================================");
            }
            if (RunConfig.Equals("0") || RunConfig.Equals("3"))
            {
                service.GetSubjectDataList(3); //已结束
            }
        }
       
    }
}
